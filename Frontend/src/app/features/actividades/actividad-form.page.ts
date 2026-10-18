import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Actividad, ActividadEquipo, Cuad, Equipo, ESTADO_EJEC, Guia, TipoActividad } from '../../core/api/models';
import {
  ActividadEquipoService,
  ActividadService,
  CuadService,
  EquipoService,
  GuiaEquipoService,
  GuiaService,
  TipoActividadService,
} from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';
import { claseTipo } from '../../shared/formato';

/**
 * Programación de una actividad y de los equipos que interviene. Cada equipo lleva la guía a aplicar:
 * sus pasos y verificaciones son las actividades que se cargan al técnico en campo.
 */
@Component({
  selector: 'app-actividad-form',
  imports: [FormsModule, RouterLink],
  templateUrl: './actividad-form.page.html',
  styles: `
    .equipo-ae {
      border: 1px solid var(--line);
      border-radius: 12px;
      margin-bottom: 10px;
      overflow: hidden;
    }
    .ae-cab {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 14px;
      flex-wrap: wrap;
      .crece {
        flex: 1 1 220px;
        min-width: 0;
      }
    }
    .cargados {
      min-height: 40px;
      display: flex;
      align-items: center;
      gap: 8px;
      width: 100%;
      padding: 0 14px;
      border: 0;
      border-top: 1px solid var(--line-2);
      background: var(--bg-2);
      font: inherit;
      font-size: 14px;
      font-weight: 600;
      color: var(--ink);
      text-align: left;
      cursor: pointer;
      span.crece {
        flex: 1;
      }
    }
    ol {
      margin: 0;
      padding: 10px 14px 12px 40px;
      background: var(--bg-2);
      font-size: 14px;
      line-height: 1.7;
      color: var(--ink-3);
    }
    .previa {
      margin: 8px 0 0;
      font-size: 14px;
      color: var(--muted);
    }
  `,
})
export class ActividadFormPage implements OnInit {
  /** 'nueva' o el id. */
  readonly id = input.required<string>();
  /** Prellenado desde la vista semanal (?fecha=yyyy-MM-dd&cuad=id). */
  readonly fecha = input<string>();
  readonly cuad = input<string>();

  private readonly service = inject(ActividadService);
  private readonly aeService = inject(ActividadEquipoService);
  private readonly guiaService = inject(GuiaService);
  private readonly guiaEquipoService = inject(GuiaEquipoService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  protected readonly puedeEditar = inject(AuthService).tieneRol('Root', 'Admin', 'Supervisor');
  protected readonly estados = ESTADO_EJEC;
  protected readonly claseTipo = claseTipo;

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

  /** Detalle (árbol de pasos) de las guías ya consultadas. */
  protected readonly detalles = signal<Record<string, Guia>>({});
  protected readonly expandido = signal<string | null>(null);
  protected readonly guiasDelEquipo = signal<string[]>([]);

  /** Fecha en formato del input datetime-local (yyyy-MM-ddTHH:mm). */
  protected fechaHora = localIso(new Date());
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
    this.guiaService.list({ Estado: true }).subscribe((g) => this.guias.set(g));
  }

  ngOnInit(): void {
    if (this.esNueva()) {
      const f = this.fecha();
      if (f && /^\d{4}-\d{2}-\d{2}$/.test(f)) this.fechaHora = `${f}T08:00`;
      if (this.cuad()) this.actividad.set({ ...this.actividad(), idCuad: this.cuad()! });
      return;
    }
    this.service.get(this.id()).subscribe({
      next: (a) => {
        if (a.fechaEjecucion) this.fechaHora = a.fechaEjecucion.substring(0, 16);
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
    const a: Actividad = { ...this.actividad(), fechaEjecucion: this.fechaHora };
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

  /** Al elegir un equipo se sugiere la guía que tiene asignada. */
  elegirEquipo(): void {
    this.guiasDelEquipo.set([]);
    if (!this.nuevoEquipo) return;
    this.guiaEquipoService.list({ IdEquipo: this.nuevoEquipo }).subscribe((asignadas) => {
      const ids = asignadas.map((a) => a.idGuia);
      this.guiasDelEquipo.set(ids);
      if (ids.length) {
        this.nuevaGuia = ids[0];
        this.cargarDetalle(ids[0]);
      }
    });
  }

  elegirGuia(): void {
    if (this.nuevaGuia) this.cargarDetalle(this.nuevaGuia);
  }

  resumen(idGuia?: string | null): string {
    const g = idGuia ? this.detalles()[idGuia] : null;
    if (!g) return '';
    const pasos = g.proced ?? [];
    const verif = pasos.reduce((n, p) => n + p.valid.length, 0);
    const fotos = pasos.reduce((n, p) => n + p.valid.filter((v) => v.requiereFoto).length, 0);
    return `${pasos.length} pasos · ${verif} verificaciones${fotos ? ` · ${fotos} fotos` : ''} · ${g.duracion} h`;
  }

  alternar(ae: ActividadEquipo): void {
    if (this.expandido() === ae.id) {
      this.expandido.set(null);
      return;
    }
    this.expandido.set(ae.id!);
    if (ae.idGuia) this.cargarDetalle(ae.idGuia);
  }

  agregarEquipo(): void {
    this.aeService
      .create({ idActividad: this.id(), idEquipo: this.nuevoEquipo, idGuia: this.nuevaGuia || null, estado: true })
      .subscribe({
        next: () => {
          this.nuevoEquipo = '';
          this.nuevaGuia = '';
          this.guiasDelEquipo.set([]);
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

  private cargarDetalle(idGuia: string): void {
    if (this.detalles()[idGuia]) return;
    this.guiaService.get(idGuia).subscribe((g) => this.detalles.set({ ...this.detalles(), [idGuia]: g }));
  }

  private cargarEquipos(): void {
    this.aeService.list({ IdActividad: this.id() }).subscribe((r) => {
      this.equiposActividad.set(r);
      new Set(r.map((a) => a.idGuia).filter((g): g is string => !!g)).forEach((g) => this.cargarDetalle(g));
    });
  }
}

function localIso(d: Date): string {
  const off = d.getTimezoneOffset() * 60_000;
  return new Date(d.getTime() - off).toISOString().substring(0, 16);
}
