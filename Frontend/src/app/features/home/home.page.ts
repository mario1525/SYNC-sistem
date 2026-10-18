import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Actividad, ActividadEquipo, Equipo, ESTADO_EJEC, Falla, TIPOS_FALLA } from '../../core/api/models';
import {
  ActividadEquipoService,
  ActividadService,
  EquipoService,
  FallaService,
  GuiaService,
} from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { claseTipo, fechaIso } from '../../shared/formato';

/**
 * Inicio.
 *  - Técnico: "Hoy", las intervenciones de su cuadrilla con un botón grande para ejecutarlas.
 *  - Supervisión y administración: indicadores, lo que pasa hoy en planta, fallas abiertas y equipos sin ubicación.
 */
@Component({
  selector: 'app-home',
  imports: [RouterLink, DatePipe, FormsModule],
  templateUrl: './home.page.html',
  styles: `
    .buscar {
      display: flex;
      gap: 12px;
      flex-wrap: wrap;
      margin-bottom: 20px;
    }
    .tareas {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    .tarea {
      display: flex;
      align-items: center;
      gap: 20px;
      padding: 16px 16px 16px 20px;
      background: #fff;
      border: 1px solid var(--line);
      border-radius: var(--radius);
      flex-wrap: wrap;
    }
    .hora {
      width: 72px;
      font-family: var(--mono);
      font-size: 22px;
      font-weight: 600;
      small {
        display: block;
        font-family: var(--sans);
        font-size: 13px;
        font-weight: 400;
        color: var(--muted);
      }
    }
    .info {
      flex: 1 1 200px;
      min-width: 0;
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .info .nombre {
      font-size: 20px;
      font-weight: 600;
    }
    .info .lugar {
      display: flex;
      align-items: center;
      gap: 6px;
      font-size: 15px;
      color: var(--muted);
      .material-icons {
        font-size: 18px;
      }
    }
    .avance {
      width: 130px;
      font-size: 13px;
      color: var(--muted);
      display: flex;
      flex-direction: column;
      gap: 6px;
    }
    .tarea .btn {
      min-width: 150px;
    }
    .resumen .valor {
      font-family: var(--mono);
      font-size: 44px;
      font-weight: 600;
    }
    .barras {
      display: grid;
      grid-auto-flow: column;
      grid-auto-columns: minmax(0, 1fr);
      gap: 6px;
      margin-top: 12px;
      span {
        height: 10px;
        border-radius: 5px;
        background: var(--line-2);
        &.encurso {
          background: var(--accent);
        }
        &.completada {
          background: var(--ink);
        }
      }
    }
    .fila-lista {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 10px 0;
      border-top: 1px solid var(--line-2);
      text-decoration: none;
      color: inherit;
      .crece {
        flex: 1;
        min-width: 0;
      }
    }
  `,
})
export class HomePage {
  protected readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly aeService = inject(ActividadEquipoService);

  protected readonly esTecnico = this.auth.tieneRol('Tecnico');
  protected readonly puedeUbicar = this.auth.tieneRol('Root', 'Admin');
  protected readonly estados = ESTADO_EJEC;
  protected readonly claseTipo = claseTipo;
  protected readonly hoy = new Date();

  // Vista técnico
  protected readonly dia = signal<'hoy' | 'manana'>('hoy');
  protected readonly cuadrilla = signal<string | null>(null);
  protected readonly tareas = signal<ActividadEquipo[] | null>(null);
  protected readonly completadas = computed(
    () => (this.tareas() ?? []).filter((t) => t.estadoEjec === 'Completada').length,
  );
  protected busqueda = '';

  // Vista supervisión
  protected readonly equipos = signal<Equipo[] | null>(null);
  protected readonly sinUbicacion = computed(() => (this.equipos() ?? []).filter((e) => !e.ubicacion?.idSeccionArea));
  protected readonly guias = signal<number | null>(null);
  protected readonly fallas = signal<Falla[]>([]);
  protected readonly enPlantaHoy = signal<ActividadEquipo[]>([]);
  protected readonly proximas = signal<Actividad[]>([]);
  private idCuad: string | null = null;

  constructor() {
    if (this.esTecnico) {
      this.auth.me().subscribe({
        next: (u) => {
          this.idCuad = u.idCuad ?? null;
          this.cuadrilla.set(u.cuadNombre ?? null);
          this.cargarTareas();
        },
        error: () => this.cargarTareas(),
      });
      return;
    }

    const hoy = fechaIso(new Date());
    const en7 = fechaIso(new Date(Date.now() + 7 * 86_400_000));
    forkJoin({
      equipos: inject(EquipoService).list({ Estado: true }),
      guias: inject(GuiaService).list({ Estado: true }),
      fallas: inject(FallaService).list({ Estado: true }),
      hoy: this.aeService.list({ FechaDesde: hoy, FechaHasta: hoy }),
      actividades: inject(ActividadService).list({ FechaDesde: hoy, FechaHasta: en7 }),
    }).subscribe(({ equipos, guias, fallas, hoy: aeHoy, actividades }) => {
      this.equipos.set(equipos);
      this.guias.set(guias.length);
      this.fallas.set(fallas.filter((f) => f.estadoFalla !== 'Cerrada'));
      this.enPlantaHoy.set([...aeHoy].sort(porFecha));
      this.proximas.set([...actividades].reverse());
    });
  }

  cambiarDia(dia: 'hoy' | 'manana'): void {
    this.dia.set(dia);
    this.cargarTareas();
  }

  buscarEquipo(): void {
    void this.router.navigate(['/equipo'], { queryParams: { q: this.busqueda.trim() || null } });
  }

  accion(t: ActividadEquipo): { texto: string; clase: string } {
    switch (t.estadoEjec) {
      case 'EnCurso':
        return { texto: 'Continuar', clase: 'btn btn-accent btn-lg' };
      case 'Completada':
        return { texto: 'Ver', clase: 'btn btn-soft btn-lg' };
      default:
        return { texto: 'Iniciar', clase: 'btn btn-primary btn-lg' };
    }
  }

  porcentaje(t: ActividadEquipo): number {
    return t.totalValid ? Math.round(((t.validCumplidas ?? 0) * 100) / t.totalValid) : 0;
  }

  tipoFalla(id: string): string {
    return TIPOS_FALLA.find((t) => t.id === id)?.nombre ?? id;
  }

  private cargarTareas(): void {
    const d = new Date();
    if (this.dia() === 'manana') d.setDate(d.getDate() + 1);
    const f = fechaIso(d);
    this.tareas.set(null);
    this.aeService
      .list({ FechaDesde: f, FechaHasta: f, IdCuad: this.idCuad })
      .subscribe((r) => this.tareas.set([...r].sort(porFecha)));
  }
}

function porFecha(a: ActividadEquipo, b: ActividadEquipo): number {
  return (
    (a.fechaEjecucion ?? '').localeCompare(b.fechaEjecucion ?? '') ||
    (a.equipoNombre ?? '').localeCompare(b.equipoNombre ?? '')
  );
}
