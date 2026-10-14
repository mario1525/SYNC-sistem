import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CrudService } from '../../core/api/crud.service';
import { Entidad, Planta, TipoArea } from '../../core/api/models';
import { AreaService, PlantaService, SeccionAreaService, TipoAreaService } from '../../core/api/services';
import { NotifyService } from '../../core/ui/notify.service';
import { CampoCatalogo, CatalogoComponent } from '../../shared/catalogo.component';

type Registro = Entidad & Record<string, unknown>;

/**
 * Estructura física de una planta (ADR-03):
 *   Planta → Áreas (Producción / Bodega / Patio) → Secciones (donde se ubican los equipos).
 */
@Component({
  selector: 'app-planta',
  imports: [CatalogoComponent, RouterLink],
  template: `
    <div class="page-header">
      <div>
        <p class="subtitulo">Planta</p>
        <h2>
          {{ planta()?.nombre ?? '…' }} <span class="muted region">{{ planta()?.region }}</span>
        </h2>
      </div>
      <div class="acciones">
        @if (planta(); as p) {
          <a class="btn btn-secondary" [routerLink]="['/comp', p.idComp]">Volver a la compañía</a>
        }
      </div>
    </div>

    <div class="dos-columnas">
      <app-catalogo
        titulo="Áreas"
        [servicio]="areas"
        [campos]="camposArea()"
        [filtro]="{ IdPlanta: id() }"
        [fijos]="{ idPlanta: id() }"
        [seleccionable]="true"
        [seleccionadoId]="area()?.id"
        (seleccionar)="area.set($event)"
      />

      @if (area(); as a) {
        <app-catalogo
          [titulo]="'Secciones de ' + a['nombre']"
          [servicio]="secciones"
          [campos]="camposSeccion"
          [filtro]="{ IdArea: a.id }"
          [fijos]="{ idArea: a.id }"
        />
      } @else {
        <div class="card muted">Seleccione un área para ver y editar sus secciones.</div>
      }
    </div>
  `,
  styles: `
    .dos-columnas {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      align-items: start;
    }
    .region {
      font-size: 14px;
      margin-left: 8px;
    }
    @media (max-width: 1100px) {
      .dos-columnas {
        grid-template-columns: 1fr;
      }
    }
  `,
})
export class PlantaPage implements OnInit {
  readonly id = input.required<string>();

  private readonly notify = inject(NotifyService);
  private readonly plantaService = inject(PlantaService);
  private readonly tipoAreaService = inject(TipoAreaService);
  protected readonly areas = inject(AreaService) as unknown as CrudService<Registro>;
  protected readonly secciones = inject(SeccionAreaService) as unknown as CrudService<Registro>;

  protected readonly planta = signal<Planta | null>(null);
  protected readonly tipos = signal<TipoArea[]>([]);
  protected readonly area = signal<Registro | null>(null);

  protected readonly camposArea = computed<CampoCatalogo[]>(() => [
    { key: 'nombre', label: 'Nombre', requerido: true },
    {
      key: 'tipoArea',
      label: 'Tipo',
      tipo: 'select',
      requerido: true,
      mostrar: 'tipoAreaNombre',
      opciones: this.tipos().map((t) => ({ value: t.id!, label: t.nombre })),
    },
  ]);
  protected readonly camposSeccion: CampoCatalogo[] = [{ key: 'nombre', label: 'Nombre', requerido: true }];

  ngOnInit(): void {
    this.plantaService
      .get(this.id())
      .subscribe({ next: (p) => this.planta.set(p), error: (e: unknown) => this.notify.error(e) });
    this.tipoAreaService.list({ Estado: true }).subscribe((t) => this.tipos.set(t));
  }
}
