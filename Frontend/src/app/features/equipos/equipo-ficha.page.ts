import { DatePipe } from '@angular/common';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  ActividadEquipo,
  Adjunto,
  Equipo,
  ESTADO_EJEC,
  ESTADOS_FALLA,
  Falla,
  GuiaEquipo,
  TIPOS_FALLA,
  UbicacionEquipo,
} from '../../core/api/models';
import {
  ActividadEquipoService,
  AdjuntoService,
  EquipoService,
  FallaService,
  GuiaEquipoService,
} from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';
import { FotoComponent } from '../../shared/foto.component';
import { claseTipo } from '../../shared/formato';
import { FotoTomada, TomarFotoComponent } from '../../shared/tomar-foto.component';

/** Ficha del equipo para consulta en campo: fotos, datos técnicos, guías, mantenimientos, fallas y ubicaciones. */
@Component({
  selector: 'app-equipo-ficha',
  imports: [RouterLink, DatePipe, FotoComponent, TomarFotoComponent],
  templateUrl: './equipo-ficha.page.html',
  styles: `
    .carrusel {
      position: relative;
      height: 340px;
      border-radius: 12px;
      overflow: hidden;
      background: var(--ink-2);
    }
    .carrusel app-foto {
      width: 100%;
      height: 100%;
      --foto-ajuste: contain;
    }
    .nav-foto {
      position: absolute;
      top: calc(50% - 28px);
      width: 56px;
      height: 56px;
      border-radius: 50%;
      border: 0;
      background: #fff;
      color: var(--ink);
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.25);
      .material-icons {
        font-size: 30px;
      }
    }
    .ant {
      left: 12px;
    }
    .sig {
      right: 12px;
    }
    .rotulo {
      position: absolute;
      bottom: 12px;
      padding: 4px 10px;
      border-radius: 8px;
      background: var(--ink);
      color: #fff;
      font-size: 14px;
    }
    .miniaturas {
      display: flex;
      gap: 8px;
      margin-top: 12px;
      overflow-x: auto;
      padding-bottom: 2px;
    }
    .mini {
      width: 76px;
      height: 60px;
      flex-shrink: 0;
      padding: 0;
      border: 3px solid transparent;
      border-radius: 10px;
      overflow: hidden;
      cursor: pointer;
      background: var(--line-2);
      app-foto {
        width: 100%;
        height: 100%;
      }
      &[aria-current='true'] {
        border-color: var(--ink);
      }
    }
    .linea {
      display: flex;
      gap: 14px;
      padding: 8px 0;
      .marca {
        width: 12px;
        height: 12px;
        margin-top: 5px;
        flex-shrink: 0;
        border-radius: 50%;
        background: var(--line);
        &.vigente {
          background: var(--accent);
          box-shadow: 0 0 0 3px var(--ink);
        }
      }
    }
    .fila {
      display: flex;
      align-items: center;
      gap: 12px;
      min-height: 56px;
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
export class EquipoFichaPage implements OnInit {
  readonly id = input.required<string>();

  private readonly service = inject(EquipoService);
  private readonly adjuntos = inject(AdjuntoService);
  private readonly aeService = inject(ActividadEquipoService);
  private readonly guiaEquipo = inject(GuiaEquipoService);
  private readonly fallaService = inject(FallaService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);
  protected readonly puedeEditar = this.auth.tieneRol('Root', 'Admin');
  protected readonly esSupervision = this.auth.tieneRol('Root', 'Admin', 'Supervisor');
  protected readonly estados = ESTADO_EJEC;
  protected readonly estadosFalla = ESTADOS_FALLA;
  protected readonly claseTipo = claseTipo;

  protected readonly equipo = signal<Equipo | null>(null);
  protected readonly fotos = signal<Adjunto[]>([]);
  protected readonly indice = signal(0);
  protected readonly actual = computed(() => this.fotos()[this.indice()] ?? null);
  protected readonly subiendo = signal(false);
  protected readonly guias = signal<GuiaEquipo[]>([]);
  protected readonly intervenciones = signal<ActividadEquipo[]>([]);
  protected readonly fallas = signal<Falla[]>([]);
  protected readonly historial = signal<UbicacionEquipo[]>([]);

  /** Próxima intervención no completada (la más cercana). */
  protected readonly proxima = computed(
    () =>
      [...this.intervenciones()]
        .filter((i) => i.estadoEjec !== 'Completada')
        .sort((a, b) => (a.fechaEjecucion ?? '').localeCompare(b.fechaEjecucion ?? ''))[0] ?? null,
  );
  protected readonly realizadas = computed(() => this.intervenciones().filter((i) => i.estadoEjec === 'Completada'));

  protected readonly datos = computed(() => {
    const e = this.equipo();
    if (!e) return [];
    const n = (v: number, u: string) => (v ? `${v} ${u}` : '—');
    const dims = [e.largo, e.ancho, e.alto].some((x) => x) ? `${e.largo} × ${e.ancho} × ${e.alto} cm` : '—';
    return [
      { k: 'Marca', v: e.marca || '—' },
      { k: 'Modelo', v: e.modelo || '—' },
      { k: 'N.º de serie', v: e.nSerie || '—' },
      { k: 'Fabricante', v: e.fabricante || '—' },
      { k: 'Año', v: e.anioFabricacion ? String(e.anioFabricacion) : '—' },
      { k: 'Potencia', v: n(e.potencia, 'kW') },
      { k: 'Capacidad', v: e.capacidad ? String(e.capacidad) : '—' },
      { k: 'Cilindraje', v: n(e.cilindraje, 'cc') },
      { k: 'Peso', v: n(e.peso, 'kg') },
      { k: 'Dimensiones (L×A×H)', v: dims },
    ];
  });

  ngOnInit(): void {
    const id = this.id();
    this.service.get(id).subscribe({
      next: (e) => this.equipo.set(e),
      error: (err: unknown) => {
        this.notify.error(err);
        void this.router.navigate(['/equipo']);
      },
    });
    this.cargarFotos();
    this.service.ubicaciones(id).subscribe((h) => this.historial.set(h));
    this.guiaEquipo.list({ IdEquipo: id }).subscribe((g) => this.guias.set(g));
    this.aeService.list({ IdEquipo: id }).subscribe((a) => this.intervenciones.set(a));
    this.fallaService.list({ IdEquipo: id }).subscribe((f) => this.fallas.set(f));
  }

  mover(delta: number): void {
    const n = this.fotos().length;
    if (n) this.indice.set((this.indice() + delta + n) % n);
  }

  subir(foto: FotoTomada): void {
    this.subiendo.set(true);
    this.adjuntos.subir('Equipo', this.id(), foto.blob, foto.nombre).subscribe({
      next: (m) => {
        this.subiendo.set(false);
        this.notify.ok(m.mensaje);
        this.cargarFotos(m.id);
      },
      error: (e: unknown) => {
        this.subiendo.set(false);
        this.notify.error(e);
      },
    });
  }

  puedeBorrar(f: Adjunto): boolean {
    return this.esSupervision || f.idUser === this.auth.userId();
  }

  borrar(f: Adjunto): void {
    this.notify.confirmar('Eliminar foto', '¿Eliminar esta foto del equipo?').subscribe((ok) => {
      if (!ok) return;
      this.adjuntos.delete(f.id).subscribe({
        next: () => this.cargarFotos(),
        error: (e: unknown) => this.notify.error(e),
      });
    });
  }

  tipoFalla(id: string): string {
    return TIPOS_FALLA.find((t) => t.id === id)?.nombre ?? id;
  }

  private cargarFotos(seleccionar?: string): void {
    this.adjuntos.list('Equipo', this.id()).subscribe((f) => {
      this.fotos.set(f);
      const i = seleccionar ? f.findIndex((x) => x.id === seleccionar) : -1;
      this.indice.set(i >= 0 ? i : Math.min(this.indice(), Math.max(0, f.length - 1)));
    });
  }
}
