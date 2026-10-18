import { DatePipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Adjunto, EstadoFalla, ESTADOS_FALLA, Falla, PRIORIDADES_FALLA, TIPOS_FALLA } from '../../core/api/models';
import { AdjuntoService, FallaService } from '../../core/api/services';
import { NotifyService } from '../../core/ui/notify.service';
import { FotoComponent } from '../../shared/foto.component';

type Filtro = 'activas' | EstadoFalla | 'todas';

/** Gestión de los reportes de falla: atender, cerrar o reabrir. */
@Component({
  selector: 'app-falla-list',
  imports: [RouterLink, DatePipe, FotoComponent],
  template: `
    <div class="page-header">
      <div>
        <p class="subtitulo">Reportes hechos desde campo</p>
        <h2>Fallas</h2>
      </div>
      <div class="acciones">
        <a class="btn btn-primary btn-lg" routerLink="/falla/nueva">
          <span class="material-icons" aria-hidden="true">add</span> Reportar falla
        </a>
      </div>
    </div>

    <div class="seg" role="group" aria-label="Filtrar por estado" style="margin-bottom: 16px">
      @for (f of filtros; track f.id) {
        <button type="button" [attr.aria-pressed]="filtro() === f.id" (click)="filtro.set(f.id)">
          {{ f.nombre }} <span class="mono">{{ contar(f.id) }}</span>
        </button>
      }
    </div>

    <div class="lista">
      @for (f of visibles(); track f.id) {
        <article
          class="card falla"
          [class.urgente]="f.estadoFalla !== 'Cerrada' && (f.prioridad === 'Critica' || f.equipoDetenido)"
        >
          <button class="cab" type="button" [attr.aria-expanded]="abierta() === f.id" (click)="alternar(f)">
            <span
              class="punto"
              [class.alerta]="f.estadoFalla === 'Abierta'"
              [class.encurso]="f.estadoFalla === 'EnAtencion'"
              [class.completada]="f.estadoFalla === 'Cerrada'"
            ></span>
            <span class="crece">
              <strong>{{ f.equipoNombre }}</strong>
              @if (f.equipoSerie) {
                <span class="mono muted"> {{ f.equipoSerie }}</span>
              }
              <span class="muted linea2">
                {{ tipo(f.tipo) }} · {{ f.reportaNombre || 'Sin nombre' }} · {{ f.fechaReporte | date: 'd MMM, HH:mm' }}
                @if (f.numFotos) {
                  · {{ f.numFotos }} {{ f.numFotos === 1 ? 'foto' : 'fotos' }}
                }
              </span>
            </span>
            @if (f.equipoDetenido) {
              <span class="badge off">Equipo detenido</span>
            }
            <span class="badge" [class.warn]="f.prioridad === 'Critica' || f.prioridad === 'Alta'">{{
              prioridad(f.prioridad)
            }}</span>
            <span class="badge">{{ estados[f.estadoFalla ?? 'Abierta'] }}</span>
            <span class="material-icons muted" aria-hidden="true">{{
              abierta() === f.id ? 'expand_less' : 'expand_more'
            }}</span>
          </button>

          @if (abierta() === f.id) {
            <div class="detalle">
              <p>{{ f.descripcion || 'Sin descripción.' }}</p>
              @if (fotos().length) {
                <div class="fotos">
                  @for (a of fotos(); track a.id) {
                    <app-foto [id]="a.id" [alt]="'Foto de la falla en ' + f.equipoNombre" />
                  }
                </div>
              }
              <div class="form-actions" style="justify-content: flex-start">
                <a class="btn btn-soft" [routerLink]="['/equipo', f.idEquipo]">Ver equipo</a>
                @if (f.estadoFalla === 'Abierta') {
                  <button class="btn btn-accent" type="button" (click)="cambiar(f, 'EnAtencion')">Atender</button>
                }
                @if (f.estadoFalla !== 'Cerrada') {
                  <button class="btn btn-primary" type="button" (click)="cambiar(f, 'Cerrada')">Cerrar falla</button>
                } @else {
                  <button class="btn btn-secondary" type="button" (click)="cambiar(f, 'Abierta')">Reabrir</button>
                }
                <a class="btn btn-secondary" routerLink="/actividad/nueva">Programar correctiva</a>
              </div>
            </div>
          }
        </article>
      } @empty {
        <div class="vacio-bloque">
          <span class="material-icons" aria-hidden="true">verified</span>
          {{ cargando() ? 'Cargando…' : 'No hay fallas en este estado.' }}
        </div>
      }
    </div>
  `,
  styles: `
    .lista {
      display: flex;
      flex-direction: column;
      gap: 10px;
    }
    .falla {
      padding: 0;
      margin: 0;
      overflow: hidden;
      &.urgente {
        box-shadow: inset 4px 0 0 var(--alert);
      }
    }
    .cab {
      width: 100%;
      min-height: 72px;
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 18px;
      border: 0;
      background: none;
      font: inherit;
      color: inherit;
      text-align: left;
      cursor: pointer;
      flex-wrap: wrap;
    }
    .crece {
      flex: 1 1 260px;
      min-width: 0;
    }
    .linea2 {
      display: block;
      font-size: 14px;
    }
    .detalle {
      padding: 0 18px 18px 40px;
      p {
        margin: 0 0 12px;
        line-height: 1.5;
      }
    }
    .fotos {
      display: flex;
      gap: 8px;
      flex-wrap: wrap;
      app-foto {
        width: 160px;
        height: 120px;
        border-radius: 10px;
      }
    }
  `,
})
export class FallaListPage implements OnInit {
  private readonly service = inject(FallaService);
  private readonly adjuntos = inject(AdjuntoService);
  private readonly notify = inject(NotifyService);

  protected readonly estados = ESTADOS_FALLA;
  protected readonly filtros: { id: Filtro; nombre: string }[] = [
    { id: 'activas', nombre: 'Activas' },
    { id: 'Abierta', nombre: 'Abiertas' },
    { id: 'EnAtencion', nombre: 'En atención' },
    { id: 'Cerrada', nombre: 'Cerradas' },
    { id: 'todas', nombre: 'Todas' },
  ];
  protected readonly filtro = signal<Filtro>('activas');
  protected readonly fallas = signal<Falla[]>([]);
  protected readonly cargando = signal(true);
  protected readonly abierta = signal<string | null>(null);
  protected readonly fotos = signal<Adjunto[]>([]);
  protected readonly visibles = computed(() => this.fallas().filter((f) => this.coincide(f, this.filtro())));

  ngOnInit(): void {
    this.cargar();
  }

  contar(filtro: Filtro): number {
    return this.fallas().filter((f) => this.coincide(f, filtro)).length;
  }

  tipo(id: string): string {
    return TIPOS_FALLA.find((t) => t.id === id)?.nombre ?? id;
  }

  prioridad(id: string): string {
    return PRIORIDADES_FALLA.find((p) => p.id === id)?.nombre ?? id;
  }

  alternar(f: Falla): void {
    if (this.abierta() === f.id) {
      this.abierta.set(null);
      return;
    }
    this.abierta.set(f.id!);
    this.fotos.set([]);
    if (f.numFotos) this.adjuntos.list('Falla', f.id!).subscribe((a) => this.fotos.set(a));
  }

  cambiar(f: Falla, estado: EstadoFalla): void {
    this.service.update(f.id!, { ...f, estadoFalla: estado }).subscribe({
      next: () => {
        this.notify.ok(`Falla: ${ESTADOS_FALLA[estado].toLowerCase()}.`);
        this.cargar();
      },
      error: (e: unknown) => this.notify.error(e),
    });
  }

  private coincide(f: Falla, filtro: Filtro): boolean {
    if (filtro === 'todas') return true;
    if (filtro === 'activas') return f.estadoFalla !== 'Cerrada';
    return f.estadoFalla === filtro;
  }

  private cargar(): void {
    this.service.list({ Estado: true }).subscribe({
      next: (f) => {
        this.fallas.set(f);
        this.cargando.set(false);
      },
      error: (e: unknown) => {
        this.cargando.set(false);
        this.notify.error(e);
      },
    });
  }
}
