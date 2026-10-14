import { Component, inject } from '@angular/core';
import { CrudService } from '../../core/api/crud.service';
import { Entidad } from '../../core/api/models';
import { CuadService } from '../../core/api/services';
import { CampoCatalogo, CatalogoComponent } from '../../shared/catalogo.component';

@Component({
  selector: 'app-cuadrilla-list',
  imports: [CatalogoComponent],
  template: `
    <div class="page-header"><h2>Cuadrillas</h2></div>
    <app-catalogo titulo="Cuadrillas de trabajo" [servicio]="servicio" [campos]="campos" [enlace]="enlace" />
    <p class="muted">Abra una cuadrilla para ver sus integrantes y asignar sus turnos de la semana.</p>
  `,
})
export class CuadrillaListPage {
  protected readonly servicio = inject(CuadService) as unknown as CrudService<Entidad & Record<string, unknown>>;
  protected readonly campos: CampoCatalogo[] = [
    { key: 'nombre', label: 'Nombre', requerido: true },
    { key: 'numIntegrantes', label: 'Integrantes', soloLectura: true },
  ];
  protected readonly enlace = (r: Entidad) => ['/cuadrilla', r.id];
}
