import { DatePipe } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Actividad, Cuad, TipoActividad } from '../../core/api/models';
import { ActividadService, CuadService, TipoActividadService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

const iso = (d: Date) => d.toISOString().substring(0, 10);

@Component({
  selector: 'app-actividad-list',
  imports: [FormsModule, RouterLink, DatePipe],
  template: `
    <div class="page-header">
      <h2>Actividades de mantenimiento</h2>
      @if (puedeEditar) {
        <div class="acciones">
          <a class="btn btn-primary" routerLink="/actividad/nueva"
            ><span class="material-icons">add</span> Programar actividad</a
          >
        </div>
      }
    </div>

    <div class="filtros">
      <div class="form-group">
        <label for="desde">Desde</label>
        <input id="desde" type="date" [(ngModel)]="desde" (change)="cargar()" />
      </div>
      <div class="form-group">
        <label for="hasta">Hasta</label>
        <input id="hasta" type="date" [(ngModel)]="hasta" (change)="cargar()" />
      </div>
      <div class="form-group">
        <label for="tipo">Tipo</label>
        <select id="tipo" [(ngModel)]="idTipo" (ngModelChange)="cargar()">
          <option value="">Todos</option>
          @for (t of tipos(); track t.id) {
            <option [value]="t.id">{{ t.nombre }}</option>
          }
        </select>
      </div>
      <div class="form-group">
        <label for="cuad">Cuadrilla</label>
        <select id="cuad" [(ngModel)]="idCuad" (ngModelChange)="cargar()">
          <option value="">Todas</option>
          @for (c of cuadrillas(); track c.id) {
            <option [value]="c.id">{{ c.nombre }}</option>
          }
        </select>
      </div>
    </div>

    <div class="table-wrap">
      <table class="table">
        <thead>
          <tr>
            <th>Fecha</th>
            <th>Tipo</th>
            <th>Descripción</th>
            <th>Cuadrilla</th>
            <th>Equipos</th>
            <th>Frecuencia</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          @for (a of actividades(); track a.id) {
            <tr>
              <td>
                <a [routerLink]="['/actividad', a.id]">{{ a.fechaEjecucion | date: 'dd/MM/yyyy HH:mm' }}</a>
              </td>
              <td>{{ a.tipoActividadNombre }}</td>
              <td>{{ a.descripcion }}</td>
              <td>{{ a.cuadNombre ?? '—' }}</td>
              <td>{{ a.numEquipos }}</td>
              <td>{{ a.intervalo ? 'Cada ' + a.intervalo + ' días' : 'Única' }}</td>
              <td class="acciones">
                @if (puedeEditar) {
                  <button class="btn btn-icon peligro" type="button" title="Eliminar" (click)="eliminar(a)">
                    <span class="material-icons">delete</span>
                  </button>
                }
              </td>
            </tr>
          } @empty {
            <tr>
              <td colspan="7" class="vacio">{{ cargando() ? 'Cargando…' : 'No hay actividades en el período.' }}</td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  `,
})
export class ActividadListPage implements OnInit {
  private readonly service = inject(ActividadService);
  private readonly notify = inject(NotifyService);
  protected readonly puedeEditar = inject(AuthService).tieneRol('Root', 'Admin', 'Supervisor');

  protected readonly actividades = signal<Actividad[]>([]);
  protected readonly tipos = signal<TipoActividad[]>([]);
  protected readonly cuadrillas = signal<Cuad[]>([]);
  protected readonly cargando = signal(false);

  protected desde = iso(new Date(Date.now() - 30 * 86_400_000));
  protected hasta = iso(new Date(Date.now() + 60 * 86_400_000));
  protected idTipo = '';
  protected idCuad = '';

  constructor() {
    inject(TipoActividadService)
      .list({ Estado: true })
      .subscribe((t) => this.tipos.set(t));
    if (inject(AuthService).tieneRol('Root', 'Admin', 'Supervisor'))
      inject(CuadService)
        .list({ Estado: true })
        .subscribe((c) => this.cuadrillas.set(c));
  }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.service
      .list({ FechaDesde: this.desde, FechaHasta: this.hasta, IdTipoActividad: this.idTipo, IdCuad: this.idCuad })
      .subscribe({
        next: (a) => {
          this.actividades.set(a);
          this.cargando.set(false);
        },
        error: (e: unknown) => {
          this.cargando.set(false);
          this.notify.error(e);
        },
      });
  }

  eliminar(a: Actividad): void {
    this.notify.confirmar('Eliminar actividad', `¿Eliminar la actividad "${a.descripcion ?? ''}"?`).subscribe((ok) => {
      if (!ok) return;
      this.service.delete(a.id!).subscribe({
        next: (m) => {
          this.notify.ok(m.mensaje);
          this.cargar();
        },
        error: (e: unknown) => this.notify.error(e),
      });
    });
  }
}
