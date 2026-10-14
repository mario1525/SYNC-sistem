import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Actividad, ActividadEquipo, Cuad, Equipo, Guia, TipoActividad } from '../../core/api/models';
import {
  ActividadEquipoService,
  ActividadService,
  CuadService,
  EquipoService,
  GuiaService,
  TipoActividadService,
} from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

/** Programación de una actividad y equipos que interviene (con la guía a aplicar). */
@Component({
  selector: 'app-actividad-form',
  imports: [FormsModule, RouterLink],
  template: `
    <div class="page-header">
      <h2>{{ esNueva() ? 'Programar actividad' : 'Actividad' }}</h2>
      <div class="acciones"><a class="btn btn-secondary" routerLink="/actividad">Volver</a></div>
    </div>

    <form class="card" #f="ngForm" (ngSubmit)="guardar()">
      <fieldset class="sin-borde" [disabled]="!puedeEditar">
        <div class="form-grid">
          <div class="form-group">
            <label for="tipo">Tipo de actividad *</label>
            <select id="tipo" name="idTipoActividad" [(ngModel)]="actividad().idTipoActividad" required>
              <option value="">— Seleccione —</option>
              @for (t of tipos(); track t.id) {
                <option [value]="t.id">{{ t.nombre }}</option>
              }
            </select>
            @if (tipos().length === 0) {
              <span class="error">Primero cree tipos de actividad en Catálogos.</span>
            }
          </div>
          <div class="form-group">
            <label for="fecha">Fecha y hora de ejecución *</label>
            <input id="fecha" name="fecha" type="datetime-local" [(ngModel)]="fecha" required />
          </div>
          <div class="form-group">
            <label for="cuad">Cuadrilla responsable</label>
            <select id="cuad" name="idCuad" [(ngModel)]="actividad().idCuad">
              <option [ngValue]="null">— Sin asignar —</option>
              @for (c of cuadrillas(); track c.id) {
                <option [ngValue]="c.id">{{ c.nombre }}</option>
              }
            </select>
          </div>
          <div class="form-group">
            <label for="intervalo">Repetir cada (días, 0 = única)</label>
            <input id="intervalo" name="intervalo" type="number" min="0" [(ngModel)]="actividad().intervalo" />
          </div>
          <div class="form-group">
            <label for="ubicacion">Lugar</label>
            <input id="ubicacion" name="ubicacion" [(ngModel)]="actividad().ubicacion" />
          </div>
          <div class="form-group ancho">
            <label for="descripcion">Descripción</label>
            <input id="descripcion" name="descripcion" [(ngModel)]="actividad().descripcion" />
          </div>
          <div class="form-group ancho">
            <label for="detalle">Detalle / instrucciones</label>
            <textarea id="detalle" name="detalle" [(ngModel)]="actividad().detalle"></textarea>
          </div>
        </div>
        @if (puedeEditar) {
          <div class="form-actions">
            <button class="btn btn-primary" type="submit" [disabled]="f.invalid || guardando()">
              <span class="material-icons">save</span> Guardar
            </button>
          </div>
        }
      </fieldset>
    </form>

    @if (!esNueva()) {
      <div class="card">
        <h3>Equipos a intervenir</h3>
        @if (puedeEditar) {
          <div class="filtros">
            <div class="form-group">
              <label for="equipo">Equipo</label>
              <select id="equipo" [(ngModel)]="nuevoEquipo">
                <option value="">— Seleccione —</option>
                @for (e of equiposDisponibles(); track e.id) {
                  <option [value]="e.id">{{ e.nombre }}</option>
                }
              </select>
            </div>
            <div class="form-group">
              <label for="guia">Guía a aplicar</label>
              <select id="guia" [(ngModel)]="nuevaGuia">
                <option value="">— Ninguna —</option>
                @for (g of guias(); track g.id) {
                  <option [value]="g.id">{{ g.nombre }}</option>
                }
              </select>
            </div>
            <button class="btn btn-info" type="button" [disabled]="!nuevoEquipo" (click)="agregarEquipo()">
              <span class="material-icons">add</span> Agregar
            </button>
          </div>
        }
        <div class="table-wrap">
          <table class="table">
            <thead>
              <tr>
                <th>Equipo</th>
                <th>Guía</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              @for (ae of equiposActividad(); track ae.id) {
                <tr>
                  <td>
                    <a [routerLink]="['/equipo', ae.idEquipo]">{{ ae.equipoNombre }}</a>
                  </td>
                  <td>
                    @if (ae.idGuia) {
                      <a [routerLink]="['/guia', ae.idGuia]">{{ ae.guiaNombre }}</a>
                    } @else {
                      <span class="muted">—</span>
                    }
                  </td>
                  <td class="acciones">
                    @if (puedeEditar) {
                      <button class="btn btn-icon peligro" type="button" title="Quitar" (click)="quitarEquipo(ae)">
                        <span class="material-icons">remove_circle</span>
                      </button>
                    }
                  </td>
                </tr>
              } @empty {
                <tr>
                  <td colspan="3" class="vacio">Aún no hay equipos en esta actividad.</td>
                </tr>
              }
            </tbody>
          </table>
        </div>
      </div>
    }
  `,
})
export class ActividadFormPage implements OnInit {
  /** 'nueva' o el id. */
  readonly id = input.required<string>();

  private readonly service = inject(ActividadService);
  private readonly aeService = inject(ActividadEquipoService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  protected readonly puedeEditar = inject(AuthService).tieneRol('Root', 'Admin', 'Supervisor');

  protected readonly actividad = signal<Actividad>({ idTipoActividad: '', intervalo: 0, estado: true, idCuad: null });
  protected readonly tipos = signal<TipoActividad[]>([]);
  protected readonly cuadrillas = signal<Cuad[]>([]);
  protected readonly equipos = signal<Equipo[]>([]);
  protected readonly guias = signal<Guia[]>([]);
  protected readonly equiposActividad = signal<ActividadEquipo[]>([]);
  protected readonly esNueva = computed(() => this.id() === 'nueva');
  protected readonly guardando = signal(false);
  protected readonly equiposDisponibles = computed(() => {
    const usados = new Set(this.equiposActividad().map((a) => a.idEquipo));
    return this.equipos().filter((e) => !usados.has(e.id!));
  });

  /** Fecha en formato del input datetime-local (yyyy-MM-ddTHH:mm). */
  protected fecha = localIso(new Date());
  protected nuevoEquipo = '';
  protected nuevaGuia = '';

  constructor() {
    inject(TipoActividadService)
      .list({ Estado: true })
      .subscribe((t) => this.tipos.set(t));
    if (this.puedeEditar)
      inject(CuadService)
        .list({ Estado: true })
        .subscribe((c) => this.cuadrillas.set(c));
    inject(EquipoService)
      .list({ Estado: true })
      .subscribe((e) => this.equipos.set(e));
    inject(GuiaService)
      .list({ Estado: true })
      .subscribe((g) => this.guias.set(g));
  }

  ngOnInit(): void {
    if (this.esNueva()) return;
    this.service.get(this.id()).subscribe({
      next: (a) => {
        if (a.fechaEjecucion) this.fecha = a.fechaEjecucion.substring(0, 16);
        this.actividad.set(a);
      },
      error: (e: unknown) => {
        this.notify.error(e);
        void this.router.navigate(['/actividad']);
      },
    });
    this.cargarEquipos();
  }

  guardar(): void {
    const a: Actividad = { ...this.actividad(), fechaEjecucion: this.fecha };
    this.guardando.set(true);
    const op = this.esNueva() ? this.service.create(a) : this.service.update(this.id(), a);
    op.subscribe({
      next: (m) => {
        this.guardando.set(false);
        this.notify.ok(m.mensaje);
        if (this.esNueva() && m.id) void this.router.navigate(['/actividad', m.id]);
      },
      error: (e: unknown) => {
        this.guardando.set(false);
        this.notify.error(e);
      },
    });
  }

  agregarEquipo(): void {
    this.aeService
      .create({ idActividad: this.id(), idEquipo: this.nuevoEquipo, idGuia: this.nuevaGuia || null, estado: true })
      .subscribe({
        next: () => {
          this.nuevoEquipo = '';
          this.nuevaGuia = '';
          this.cargarEquipos();
        },
        error: (e: unknown) => this.notify.error(e),
      });
  }

  quitarEquipo(ae: ActividadEquipo): void {
    this.aeService
      .delete(ae.id!)
      .subscribe({ next: () => this.cargarEquipos(), error: (e: unknown) => this.notify.error(e) });
  }

  private cargarEquipos(): void {
    this.aeService.list({ IdActividad: this.id() }).subscribe((r) => this.equiposActividad.set(r));
  }
}

function localIso(d: Date): string {
  const off = d.getTimezoneOffset() * 60_000;
  return new Date(d.getTime() - off).toISOString().substring(0, 16);
}
