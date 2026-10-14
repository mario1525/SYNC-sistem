import { Component, computed, effect, inject, input, output, signal, untracked } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CrudService, Filtro } from '../core/api/crud.service';
import { Entidad } from '../core/api/models';
import { NotifyService } from '../core/ui/notify.service';

export interface CampoCatalogo {
  key: string;
  label: string;
  tipo?: 'text' | 'time' | 'textarea' | 'select';
  requerido?: boolean;
  /** Valor inicial para un registro nuevo. */
  defecto?: string;
  /** Opciones para tipo 'select'. */
  opciones?: { value: string; label: string }[];
  /** Columna a mostrar en la tabla (por defecto la misma key). */
  mostrar?: string;
  /** Solo se muestra en la tabla (no editable). */
  soloLectura?: boolean;
}

type Registro = Entidad & Record<string, unknown>;

/**
 * Tabla CRUD con edición en línea para catálogos simples
 * (especialidades, tipos de actividad, turnos, tipos de área...).
 */
@Component({
  selector: 'app-catalogo',
  imports: [FormsModule, RouterLink],
  template: `
    <div class="card">
      <h3>
        {{ titulo() }}
        @if (editable() && !editando()) {
          <button class="btn btn-primary" type="button" (click)="nuevo()">
            <span class="material-icons">add</span> Nuevo
          </button>
        }
      </h3>

      @if (editando(); as e) {
        <form class="form-grid editor" (ngSubmit)="guardar()" #f="ngForm">
          @for (c of editables(); track c.key) {
            <div class="form-group" [class.ancho]="c.tipo === 'textarea'">
              <label [for]="'c-' + c.key">{{ c.label }}{{ c.requerido ? ' *' : '' }}</label>
              @if (c.tipo === 'textarea') {
                <textarea
                  [id]="'c-' + c.key"
                  [name]="c.key"
                  [(ngModel)]="e[c.key]"
                  [required]="!!c.requerido"
                ></textarea>
              } @else if (c.tipo === 'select') {
                <select [id]="'c-' + c.key" [name]="c.key" [(ngModel)]="e[c.key]" [required]="!!c.requerido">
                  <option value="">— Seleccione —</option>
                  @for (o of c.opciones ?? []; track o.value) {
                    <option [value]="o.value">{{ o.label }}</option>
                  }
                </select>
              } @else {
                <input
                  [id]="'c-' + c.key"
                  [name]="c.key"
                  [type]="c.tipo ?? 'text'"
                  [(ngModel)]="e[c.key]"
                  [required]="!!c.requerido"
                />
              }
            </div>
          }
          <div class="form-actions ancho">
            <button class="btn btn-secondary" type="button" (click)="editando.set(null)">Cancelar</button>
            <button class="btn btn-primary" type="submit" [disabled]="f.invalid || guardando()">Guardar</button>
          </div>
        </form>
      }

      <div class="table-wrap">
        <table class="table">
          <thead>
            <tr>
              @for (c of columnas(); track c.key) {
                <th>{{ c.label }}</th>
              }
              <th>Estado</th>
              @if (editable()) {
                <th></th>
              }
            </tr>
          </thead>
          <tbody>
            @for (r of items(); track r.id) {
              <tr
                [class.seleccionado]="r.id === seleccionadoId()"
                [class.clicable]="seleccionable()"
                (click)="seleccionar.emit(r)"
              >
                @for (c of columnas(); track c.key; let primero = $first) {
                  <td>
                    @if (primero && enlace(); as fn) {
                      <a [routerLink]="fn(r)" (click)="$event.stopPropagation()">{{ r[c.mostrar ?? c.key] }}</a>
                    } @else {
                      {{ r[c.mostrar ?? c.key] }}
                    }
                  </td>
                }
                <td>
                  <span class="badge" [class.ok]="r.estado" [class.off]="!r.estado">{{
                    r.estado ? 'Activo' : 'Inactivo'
                  }}</span>
                </td>
                @if (editable()) {
                  <td class="acciones">
                    <button
                      class="btn btn-icon"
                      type="button"
                      title="Editar"
                      (click)="editar(r); $event.stopPropagation()"
                    >
                      <span class="material-icons">edit</span>
                    </button>
                    <button
                      class="btn btn-icon"
                      type="button"
                      [title]="r.estado ? 'Desactivar' : 'Activar'"
                      (click)="alternar(r); $event.stopPropagation()"
                    >
                      <span class="material-icons">{{ r.estado ? 'toggle_on' : 'toggle_off' }}</span>
                    </button>
                    <button
                      class="btn btn-icon peligro"
                      type="button"
                      title="Eliminar"
                      (click)="eliminar(r); $event.stopPropagation()"
                    >
                      <span class="material-icons">delete</span>
                    </button>
                  </td>
                }
              </tr>
            } @empty {
              <tr>
                <td [attr.colspan]="columnas().length + 2" class="vacio">
                  {{ cargando() ? 'Cargando…' : 'Sin registros.' }}
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: `
    .editor {
      margin-bottom: 16px;
      padding: 12px;
      background: #f7f9fb;
      border-radius: 6px;
    }
    tr.clicable {
      cursor: pointer;
    }
    tr.seleccionado td {
      background: #d7e7ff;
    }
  `,
})
export class CatalogoComponent {
  readonly titulo = input.required<string>();
  readonly servicio = input.required<CrudService<Registro>>();
  readonly campos = input.required<CampoCatalogo[]>();
  readonly filtro = input<Filtro>({});
  readonly editable = input(true);
  /** Valores que se fijan en todo registro nuevo (p. ej. { idPlanta }). */
  readonly fijos = input<Record<string, unknown>>({});
  /** Si se define, la primera columna enlaza a esta ruta. */
  readonly enlace = input<((r: Registro) => unknown[]) | null>(null);
  readonly seleccionable = input(false);
  readonly seleccionadoId = input<string | null | undefined>(null);
  readonly seleccionar = output<Registro>();
  /** Se emite tras cargar la lista (útil para el componente padre). */
  readonly cargado = output<Registro[]>();

  private readonly notify = inject(NotifyService);

  protected readonly items = signal<Registro[]>([]);
  protected readonly editando = signal<Registro | null>(null);
  protected readonly cargando = signal(true);
  protected readonly guardando = signal(false);
  protected readonly columnas = computed(() => this.campos().filter((c) => c.tipo !== 'textarea'));
  protected readonly editables = computed(() => this.campos().filter((c) => !c.soloLectura));

  constructor() {
    // Recarga cuando cambia el filtro (p. ej. al seleccionar otra área)
    effect(() => {
      this.filtro();
      untracked(() => this.cargar());
    });
  }

  cargar(): void {
    this.cargando.set(true);
    this.servicio()
      .list(this.filtro())
      .subscribe({
        next: (r) => {
          this.items.set(r);
          this.cargando.set(false);
          this.cargado.emit(r);
        },
        error: (e: unknown) => {
          this.cargando.set(false);
          this.notify.error(e);
        },
      });
  }

  nuevo(): void {
    const r: Registro = { estado: true };
    for (const c of this.editables()) r[c.key] = c.defecto ?? '';
    Object.assign(r, this.fijos());
    this.editando.set(r);
  }

  editar(r: Registro): void {
    this.editando.set({ ...r });
  }

  guardar(): void {
    const r = this.editando();
    if (!r) return;
    this.guardando.set(true);
    this.servicio()
      .save(r)
      .subscribe({
        next: (m) => {
          this.notify.ok(m.mensaje);
          this.editando.set(null);
          this.guardando.set(false);
          this.cargar();
        },
        error: (e: unknown) => {
          this.guardando.set(false);
          this.notify.error(e);
        },
      });
  }

  alternar(r: Registro): void {
    this.servicio()
      .setEstado(r.id!, !r.estado)
      .subscribe({ next: () => this.cargar(), error: (e: unknown) => this.notify.error(e) });
  }

  eliminar(r: Registro): void {
    const nombre = String(r['nombre'] ?? '');
    this.notify.confirmar('Eliminar registro', `¿Eliminar "${nombre}"?`).subscribe((ok) => {
      if (!ok) return;
      this.servicio()
        .delete(r.id!)
        .subscribe({
          next: (m) => {
            this.notify.ok(m.mensaje);
            this.cargar();
          },
          error: (e: unknown) => this.notify.error(e),
        });
    });
  }
}
