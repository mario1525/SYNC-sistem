import { Component, inject } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { CrudService } from '../../core/api/crud.service';
import { Entidad } from '../../core/api/models';
import { EspService, TipoActividadService, TipoAreaService, TurnoService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { CampoCatalogo, CatalogoComponent } from '../../shared/catalogo.component';

type Generico = CrudService<Entidad & Record<string, unknown>>;

/** Catálogos de la compañía: especialidades, tipos de actividad, turnos y tipos de área. */
@Component({
  selector: 'app-catalogos',
  imports: [MatTabsModule, CatalogoComponent],
  template: `
    <div class="page-header"><h2>Catálogos</h2></div>
    <mat-tab-group animationDuration="0ms">
      <mat-tab label="Especialidades">
        <app-catalogo titulo="Especialidades" [servicio]="esp" [campos]="camposEsp" />
      </mat-tab>
      <mat-tab label="Tipos de actividad">
        <app-catalogo titulo="Tipos de actividad" [servicio]="tipoAct" [campos]="camposTipoAct" />
      </mat-tab>
      <mat-tab label="Turnos">
        <app-catalogo titulo="Turnos de trabajo" [servicio]="turno" [campos]="camposTurno" />
      </mat-tab>
      <mat-tab label="Tipos de área">
        <app-catalogo titulo="Tipos de área" [servicio]="tipoArea" [campos]="camposTipoArea" [editable]="esRoot" />
      </mat-tab>
    </mat-tab-group>
  `,
})
export class CatalogosPage {
  protected readonly esp = inject(EspService) as unknown as Generico;
  protected readonly tipoAct = inject(TipoActividadService) as unknown as Generico;
  protected readonly turno = inject(TurnoService) as unknown as Generico;
  protected readonly tipoArea = inject(TipoAreaService) as unknown as Generico;
  protected readonly esRoot = inject(AuthService).tieneRol('Root');

  protected readonly camposEsp: CampoCatalogo[] = [
    { key: 'nombre', label: 'Nombre', requerido: true },
    { key: 'descripcion', label: 'Descripción', tipo: 'textarea' },
  ];
  protected readonly camposTipoAct: CampoCatalogo[] = [
    { key: 'nombre', label: 'Nombre', requerido: true },
    { key: 'descripcion', label: 'Descripción', tipo: 'textarea' },
  ];
  protected readonly camposTurno: CampoCatalogo[] = [
    { key: 'nombre', label: 'Nombre', requerido: true },
    { key: 'horaInicio', label: 'Hora inicio', tipo: 'time', requerido: true, defecto: '06:00' },
    { key: 'horaFin', label: 'Hora fin', tipo: 'time', requerido: true, defecto: '14:00' },
  ];
  protected readonly camposTipoArea: CampoCatalogo[] = [
    { key: 'nombre', label: 'Nombre', requerido: true },
    { key: 'descripcion', label: 'Descripción' },
  ];
}
