import { Component, computed, DestroyRef, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Location } from '@angular/common';
import { Router } from '@angular/router';
import { concatMap, from, last, of, switchMap } from 'rxjs';
import { Equipo, PRIORIDADES_FALLA, PrioridadFalla, TIPOS_FALLA, TipoFalla } from '../../core/api/models';
import { AdjuntoService, EquipoService, FallaService } from '../../core/api/services';
import { NotifyService } from '../../core/ui/notify.service';
import { FotoTomada, TomarFotoComponent } from '../../shared/tomar-foto.component';

interface FotoLocal extends FotoTomada {
  url: string;
}

const MAX_FOTOS = 4;

/** Reporte de falla desde campo en tres pasos: qué pasa, qué tan urgente, y descripción/fotos. */
@Component({
  selector: 'app-falla-reporte',
  imports: [FormsModule, TomarFotoComponent],
  templateUrl: './falla-reporte.page.html',
  styles: `
    .equipo-sel {
      display: flex;
      align-items: center;
      gap: 14px;
      padding: 14px 16px;
      background: #fff;
      border: 2px solid var(--ink);
      border-radius: var(--radius);
      .crece {
        flex: 1;
        min-width: 0;
      }
    }
    .resultados {
      display: flex;
      flex-direction: column;
      gap: 6px;
      margin-top: 8px;
    }
    .resultados .opcion {
      justify-content: flex-start;
      text-align: left;
    }
    h2.paso {
      margin: 0 0 10px;
      font-size: 18px;
      font-weight: 600;
      .mono {
        color: var(--muted);
        margin-right: 8px;
      }
    }
    .rejilla {
      display: grid;
      gap: 10px;
    }
    .tipos {
      grid-template-columns: repeat(3, minmax(0, 1fr));
      .opcion {
        min-height: 92px;
        flex-direction: column;
      }
    }
    .prioridades {
      grid-template-columns: repeat(4, minmax(0, 1fr));
      .opcion {
        min-height: 72px;
      }
    }
    .fotos {
      grid-template-columns: repeat(4, minmax(0, 1fr));
    }
    .foto {
      position: relative;
      height: 96px;
      border-radius: 14px;
      overflow: hidden;
      background: var(--ink-2);
      img {
        width: 100%;
        height: 100%;
        object-fit: cover;
      }
      button {
        position: absolute;
        top: 4px;
        right: 4px;
        width: 36px;
        height: 36px;
        border: 0;
        border-radius: 50%;
        background: rgba(21, 23, 26, 0.75);
        color: #fff;
        cursor: pointer;
      }
    }
    @media (max-width: 600px) {
      .tipos,
      .prioridades,
      .fotos {
        grid-template-columns: repeat(2, minmax(0, 1fr));
      }
    }
  `,
})
export class FallaReportePage implements OnInit {
  /** Equipo preseleccionado (?equipo=). */
  readonly equipo = input<string>();

  private readonly equipoService = inject(EquipoService);
  private readonly fallaService = inject(FallaService);
  private readonly adjuntoService = inject(AdjuntoService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  private readonly location = inject(Location);

  protected readonly tipos = TIPOS_FALLA;
  protected readonly prioridades = PRIORIDADES_FALLA;
  protected readonly maxFotos = MAX_FOTOS;

  protected readonly seleccionado = signal<Equipo | null>(null);
  protected readonly equipos = signal<Equipo[]>([]);
  protected readonly busqueda = signal('');
  protected readonly coincidencias = computed(() => {
    const q = this.busqueda().trim().toLowerCase();
    if (!q) return [];
    return this.equipos()
      .filter((e) => e.nombre.toLowerCase().includes(q) || (e.nSerie ?? '').toLowerCase().includes(q))
      .slice(0, 6);
  });

  protected readonly tipo = signal<TipoFalla | null>(null);
  protected readonly prioridad = signal<PrioridadFalla>('Media');
  protected readonly detenido = signal(false);
  protected descripcion = '';
  protected readonly fotos = signal<FotoLocal[]>([]);
  protected readonly enviando = signal(false);
  protected readonly urgente = computed(() => this.detenido() || this.prioridad() === 'Critica');
  protected readonly listo = computed(() => !!this.seleccionado() && !!this.tipo());

  // Dictado por voz (si el navegador lo soporta)
  protected readonly puedeDictar =
    typeof window !== 'undefined' && ('SpeechRecognition' in window || 'webkitSpeechRecognition' in window);
  protected readonly dictando = signal(false);
  private reconocimiento: { start(): void; stop(): void } | null = null;

  constructor() {
    inject(DestroyRef).onDestroy(() => {
      this.fotos().forEach((f) => URL.revokeObjectURL(f.url));
      this.reconocimiento?.stop();
    });
  }

  ngOnInit(): void {
    const id = this.equipo();
    if (id)
      this.equipoService
        .get(id)
        .subscribe({ next: (e) => this.seleccionado.set(e), error: () => this.cargarEquipos() });
    else this.cargarEquipos();
  }

  cambiarEquipo(): void {
    this.seleccionado.set(null);
    if (!this.equipos().length) this.cargarEquipos();
  }

  agregarFoto(foto: FotoTomada): void {
    if (this.fotos().length >= MAX_FOTOS) return;
    this.fotos.set([...this.fotos(), { ...foto, url: URL.createObjectURL(foto.blob) }]);
  }

  quitarFoto(i: number): void {
    const lista = [...this.fotos()];
    URL.revokeObjectURL(lista[i].url);
    lista.splice(i, 1);
    this.fotos.set(lista);
  }

  dictar(): void {
    if (this.dictando()) {
      this.reconocimiento?.stop();
      return;
    }
    const w = window as unknown as Record<string, new () => SpeechRecognitionLike>;
    const Ctor = w['SpeechRecognition'] ?? w['webkitSpeechRecognition'];
    const rec = new Ctor();
    rec.lang = 'es-CO';
    rec.interimResults = false;
    rec.onresult = (ev) => {
      const texto = Array.from(ev.results)
        .map((r) => r[0].transcript)
        .join(' ');
      this.descripcion = [this.descripcion.trim(), texto.trim()].filter(Boolean).join(' ');
    };
    rec.onend = () => this.dictando.set(false);
    rec.onerror = () => this.dictando.set(false);
    this.reconocimiento = rec;
    this.dictando.set(true);
    rec.start();
  }

  enviar(): void {
    const equipo = this.seleccionado();
    const tipo = this.tipo();
    if (!equipo || !tipo) return;
    this.enviando.set(true);
    this.fallaService
      .create({
        idEquipo: equipo.id!,
        tipo,
        prioridad: this.prioridad(),
        equipoDetenido: this.detenido(),
        descripcion: this.descripcion.trim() || null,
        estado: true,
      })
      .pipe(
        switchMap((m) =>
          this.fotos().length
            ? from(this.fotos()).pipe(
                concatMap((f) => this.adjuntoService.subir('Falla', m.id!, f.blob, f.nombre)),
                last(),
              )
            : of(m),
        ),
      )
      .subscribe({
        next: () => {
          this.enviando.set(false);
          this.notify.ok(this.urgente() ? 'Falla reportada como urgente.' : 'Falla reportada.');
          void this.router.navigate(['/equipo', equipo.id]);
        },
        error: (e: unknown) => {
          this.enviando.set(false);
          this.notify.error(e);
        },
      });
  }

  cancelar(): void {
    this.location.back();
  }

  private cargarEquipos(): void {
    this.equipoService.list({ Estado: true }).subscribe((e) => this.equipos.set(e));
  }
}

/** Subconjunto de la Web Speech API que se usa aquí. */
interface SpeechRecognitionLike {
  lang: string;
  interimResults: boolean;
  onresult: (ev: { results: ArrayLike<ArrayLike<{ transcript: string }>> }) => void;
  onend: () => void;
  onerror: () => void;
  start(): void;
  stop(): void;
}
