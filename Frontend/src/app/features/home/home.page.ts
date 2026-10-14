import { DatePipe } from '@angular/common';
import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Actividad } from '../../core/api/models';
import { ActividadService, EquipoService, GuiaService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';

/** Inicio: resumen de equipos, guías y próximas actividades de la compañía. */
@Component({
  selector: 'app-home',
  imports: [RouterLink, DatePipe],
  template: `
    <div class="page-header">
      <div>
        <p class="subtitulo">Bienvenido</p>
        <h2>{{ auth.nombre() }}</h2>
      </div>
    </div>

    <div class="grid-cards">
      <a class="card kpi enlace" routerLink="/equipo">
        <div class="valor">{{ equipos() ?? '—' }}</div>
        <div class="etiqueta">Equipos activos</div>
      </a>
      <a class="card kpi enlace" routerLink="/equipo">
        <div class="valor">{{ sinUbicacion() ?? '—' }}</div>
        <div class="etiqueta">Equipos sin ubicación</div>
      </a>
      <a class="card kpi enlace" routerLink="/guia">
        <div class="valor">{{ guias() ?? '—' }}</div>
        <div class="etiqueta">Guías de mantenimiento</div>
      </a>
      <a class="card kpi enlace" routerLink="/actividad">
        <div class="valor">{{ proximas().length }}</div>
        <div class="etiqueta">Actividades próximos 30 días</div>
      </a>
    </div>

    <div class="card">
      <h3>Próximas actividades</h3>
      <div class="table-wrap">
        <table class="table">
          <thead>
            <tr>
              <th>Fecha</th>
              <th>Tipo</th>
              <th>Descripción</th>
              <th>Cuadrilla</th>
              <th>Equipos</th>
            </tr>
          </thead>
          <tbody>
            @for (a of proximas(); track a.id) {
              <tr>
                <td>
                  <a [routerLink]="['/actividad', a.id]">{{ a.fechaEjecucion | date: 'EEE d MMM, HH:mm' }}</a>
                </td>
                <td>{{ a.tipoActividadNombre }}</td>
                <td>{{ a.descripcion }}</td>
                <td>{{ a.cuadNombre ?? '—' }}</td>
                <td>{{ a.numEquipos }}</td>
              </tr>
            } @empty {
              <tr>
                <td colspan="5" class="vacio">No hay actividades programadas en los próximos 30 días.</td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: `
    .enlace {
      text-decoration: none;
      color: inherit;
      transition: transform 0.1s;
    }
    .enlace:hover {
      transform: translateY(-2px);
    }
  `,
})
export class HomePage {
  protected readonly auth = inject(AuthService);
  protected readonly equipos = signal<number | null>(null);
  protected readonly sinUbicacion = signal<number | null>(null);
  protected readonly guias = signal<number | null>(null);
  protected readonly proximas = signal<Actividad[]>([]);

  constructor() {
    const hoy = new Date();
    const en30 = new Date(hoy.getTime() + 30 * 86_400_000);
    const iso = (d: Date) => d.toISOString().substring(0, 10);

    forkJoin({
      equipos: inject(EquipoService).list({ Estado: true }),
      guias: inject(GuiaService).list({ Estado: true }),
      actividades: inject(ActividadService).list({ FechaDesde: iso(hoy), FechaHasta: iso(en30) }),
    }).subscribe(({ equipos, guias, actividades }) => {
      this.equipos.set(equipos.length);
      this.sinUbicacion.set(equipos.filter((e) => !e.ubicacion?.idSeccionArea).length);
      this.guias.set(guias.length);
      this.proximas.set([...actividades].reverse().slice(0, 10));
    });
  }
}
