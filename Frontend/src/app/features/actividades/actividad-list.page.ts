import { DatePipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Actividad, Cuad, TipoActividad } from '../../core/api/models';
import { ActividadService, CuadService, TipoActividadService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';
import { claseTipo, fechaIso, lunesDe } from '../../shared/formato';

const DIAS = ['Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb', 'Dom'];
const SIN_CUADRILLA = '';

interface Fila {
  idCuad: string;
  nombre: string;
  dias: Actividad[][];
}

/** Programación de actividades: semana por cuadrilla (vista principal) o lista filtrable. */
@Component({
  selector: 'app-actividad-list',
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './actividad-list.page.html',
  styleUrl: './actividad-list.page.less',
})
export class ActividadListPage implements OnInit {
  private readonly service = inject(ActividadService);
  private readonly notify = inject(NotifyService);
  protected readonly puedeEditar = inject(AuthService).tieneRol('Root', 'Admin', 'Supervisor');
  protected readonly claseTipo = claseTipo;

  protected readonly vista = signal<'semana' | 'lista'>('semana');
  protected readonly actividades = signal<Actividad[]>([]);
  protected readonly tipos = signal<TipoActividad[]>([]);
  protected readonly cuadrillas = signal<Cuad[]>([]);
  protected readonly cargando = signal(false);

  // Semana
  protected readonly lunes = signal(lunesDe(new Date()));
  protected readonly dias = computed(() => {
    const hoy = fechaIso(new Date());
    return DIAS.map((nombre, i) => {
      const d = new Date(this.lunes());
      d.setDate(d.getDate() + i);
      const iso = fechaIso(d);
      return { nombre, fecha: d, iso, hoy: iso === hoy, finde: i > 4 };
    });
  });
  protected readonly filas = computed<Fila[]>(() => {
    const dias = this.dias();
    const mapa = new Map<string, string>();
    for (const c of this.cuadrillas()) mapa.set(c.id!, c.nombre);
    for (const a of this.actividades()) mapa.set(a.idCuad ?? SIN_CUADRILLA, a.cuadNombre ?? 'Sin cuadrilla');
    if (!mapa.size) mapa.set(SIN_CUADRILLA, 'Sin cuadrilla');
    return [...mapa.entries()]
      .sort(([a, na], [b, nb]) => (a === SIN_CUADRILLA ? 1 : b === SIN_CUADRILLA ? -1 : na.localeCompare(nb)))
      .map(([idCuad, nombre]) => ({
        idCuad,
        nombre,
        dias: dias.map((d) =>
          this.actividades()
            .filter((a) => (a.idCuad ?? SIN_CUADRILLA) === idCuad && (a.fechaEjecucion ?? '').startsWith(d.iso))
            .sort((x, y) => (x.fechaEjecucion ?? '').localeCompare(y.fechaEjecucion ?? '')),
        ),
      }));
  });

  // Lista
  protected desde = fechaIso(new Date(Date.now() - 30 * 86_400_000));
  protected hasta = fechaIso(new Date(Date.now() + 60 * 86_400_000));
  protected idTipo = '';
  protected idCuad = '';

  constructor() {
    inject(TipoActividadService)
      .list({ Estado: true })
      .subscribe((t) => this.tipos.set(t));
    if (this.puedeEditar)
      inject(CuadService)
        .list({ Estado: true })
        .subscribe((c) => this.cuadrillas.set(c));
  }

  ngOnInit(): void {
    this.cargar();
  }

  cambiarVista(v: 'semana' | 'lista'): void {
    this.vista.set(v);
    this.cargar();
  }

  moverSemana(delta: number): void {
    const d = new Date(this.lunes());
    d.setDate(d.getDate() + delta * 7);
    this.lunes.set(delta === 0 ? lunesDe(new Date()) : d);
    this.cargar();
  }

  cargar(): void {
    const filtro =
      this.vista() === 'semana'
        ? { FechaDesde: this.dias()[0].iso, FechaHasta: this.dias()[6].iso }
        : { FechaDesde: this.desde, FechaHasta: this.hasta, IdTipoActividad: this.idTipo, IdCuad: this.idCuad };
    this.cargando.set(true);
    this.service.list(filtro).subscribe({
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
