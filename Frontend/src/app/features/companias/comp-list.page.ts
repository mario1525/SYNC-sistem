import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CrudService } from '../../core/api/crud.service';
import { Entidad } from '../../core/api/models';
import { CompService } from '../../core/api/services';
import { CampoCatalogo, CatalogoComponent } from '../../shared/catalogo.component';

/** Compañías (solo Root). El detalle de cada una se gestiona en /comp/:id. */
@Component({
  selector: 'app-comp-list',
  imports: [CatalogoComponent, RouterLink],
  template: `
    <div class="page-header">
      <h2>Compañías</h2>
      <div class="acciones">
        <a class="btn btn-primary" routerLink="/comp/nueva"><span class="material-icons">add</span> Nueva compañía</a>
      </div>
    </div>
    <app-catalogo
      titulo="Compañías registradas"
      [servicio]="servicio"
      [campos]="campos"
      [editable]="false"
      [enlace]="enlace"
    />
  `,
})
export class CompListPage {
  protected readonly servicio = inject(CompService) as unknown as CrudService<Entidad & Record<string, unknown>>;
  protected readonly campos: CampoCatalogo[] = [
    { key: 'nombre', label: 'Nombre' },
    { key: 'nit', label: 'NIT' },
    { key: 'ciudad', label: 'Ciudad' },
    { key: 'sector', label: 'Sector' },
  ];
  protected readonly enlace = (r: Entidad) => ['/comp', r.id];
}
