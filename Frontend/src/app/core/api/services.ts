import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CrudService } from './crud.service';
import {
  Actividad,
  ActividadEquipo,
  Area,
  Comp,
  Credencial,
  Cuad,
  Equipo,
  Esp,
  Guia,
  GuiaEquipo,
  Horario,
  Mensaje,
  Planta,
  SeccionArea,
  TipoActividad,
  TipoArea,
  Turno,
  UbicacionEquipo,
  Usuario,
} from './models';

// Un servicio por recurso del gateway (/apigateway/{recurso}).

@Injectable({ providedIn: 'root' })
export class CompService extends CrudService<Comp> {
  protected readonly recurso = 'comp';
}

@Injectable({ providedIn: 'root' })
export class EspService extends CrudService<Esp> {
  protected readonly recurso = 'esp';
}

@Injectable({ providedIn: 'root' })
export class PlantaService extends CrudService<Planta> {
  protected readonly recurso = 'planta';
}

@Injectable({ providedIn: 'root' })
export class TipoAreaService extends CrudService<TipoArea> {
  protected readonly recurso = 'tipoarea';
}

@Injectable({ providedIn: 'root' })
export class AreaService extends CrudService<Area> {
  protected readonly recurso = 'area';
}

@Injectable({ providedIn: 'root' })
export class SeccionAreaService extends CrudService<SeccionArea> {
  protected readonly recurso = 'seccionarea';
}

@Injectable({ providedIn: 'root' })
export class EquipoService extends CrudService<Equipo> {
  protected readonly recurso = 'equipo';

  ubicaciones(id: string): Observable<UbicacionEquipo[]> {
    return this.http.get<UbicacionEquipo[]>(`${this.url}/${encodeURIComponent(id)}/ubicaciones`);
  }
}

@Injectable({ providedIn: 'root' })
export class GuiaService extends CrudService<Guia> {
  protected readonly recurso = 'guia';
}

@Injectable({ providedIn: 'root' })
export class GuiaEquipoService extends CrudService<GuiaEquipo> {
  protected readonly recurso = 'guiaequipo';
}

@Injectable({ providedIn: 'root' })
export class TipoActividadService extends CrudService<TipoActividad> {
  protected readonly recurso = 'tipoactividad';
}

@Injectable({ providedIn: 'root' })
export class ActividadService extends CrudService<Actividad> {
  protected readonly recurso = 'actividad';
}

@Injectable({ providedIn: 'root' })
export class ActividadEquipoService extends CrudService<ActividadEquipo> {
  protected readonly recurso = 'actividadequipo';
}

@Injectable({ providedIn: 'root' })
export class UsuarioService extends CrudService<Usuario> {
  protected readonly recurso = 'users';
}

@Injectable({ providedIn: 'root' })
export class CredencialService extends CrudService<Credencial> {
  protected readonly recurso = 'credential';
}

@Injectable({ providedIn: 'root' })
export class CuadService extends CrudService<Cuad> {
  protected readonly recurso = 'cuad';
}

@Injectable({ providedIn: 'root' })
export class TurnoService extends CrudService<Turno> {
  protected readonly recurso = 'turno';
}

@Injectable({ providedIn: 'root' })
export class HorarioService extends CrudService<Horario> {
  protected readonly recurso = 'horario';
}

export type { Mensaje };
