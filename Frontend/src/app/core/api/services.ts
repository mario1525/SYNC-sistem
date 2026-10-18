import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { CrudService } from './crud.service';
import {
  Actividad,
  ActividadEquipo,
  Adjunto,
  Area,
  Comp,
  Credencial,
  Cuad,
  EjecucionValid,
  EntidadAdjunto,
  Equipo,
  Esp,
  Falla,
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

  // Ejecución en campo
  resultados(id: string): Observable<EjecucionValid[]> {
    return this.http.get<EjecucionValid[]>(`${this.url}/${encodeURIComponent(id)}/resultados`);
  }

  marcar(id: string, idValid: string, cumple: boolean, observacion?: string | null): Observable<Mensaje> {
    return this.http.put<Mensaje>(`${this.url}/${encodeURIComponent(id)}/resultados/${encodeURIComponent(idValid)}`, {
      cumple,
      observacion,
    });
  }

  finalizar(id: string, observacion?: string | null): Observable<Mensaje> {
    return this.http.post<Mensaje>(`${this.url}/${encodeURIComponent(id)}/finalizar`, { observacion });
  }

  reabrir(id: string): Observable<Mensaje> {
    return this.http.post<Mensaje>(`${this.url}/${encodeURIComponent(id)}/reabrir`, {});
  }
}

@Injectable({ providedIn: 'root' })
export class FallaService extends CrudService<Falla> {
  protected readonly recurso = 'falla';
}

/** Fotos de equipos, evidencias de ejecución y fotos de fallas. */
@Injectable({ providedIn: 'root' })
export class AdjuntoService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/adjunto`;

  list(entidad: EntidadAdjunto, idEntidad: string): Observable<Adjunto[]> {
    return this.http.get<Adjunto[]>(this.url, { params: { entidad, idEntidad } });
  }

  /** Bytes de la imagen (las imágenes requieren el token, por eso no se usan como src directo). */
  archivo(id: string): Observable<Blob> {
    return this.http.get(`${this.url}/${encodeURIComponent(id)}/archivo`, { responseType: 'blob' });
  }

  subir(entidad: EntidadAdjunto, idEntidad: string, archivo: Blob, nombre: string, ref?: string): Observable<Mensaje> {
    const form = new FormData();
    form.append('entidad', entidad);
    form.append('idEntidad', idEntidad);
    if (ref) form.append('ref', ref);
    form.append('archivo', archivo, nombre);
    return this.http.post<Mensaje>(this.url, form);
  }

  delete(id: string): Observable<Mensaje> {
    return this.http.delete<Mensaje>(`${this.url}/${encodeURIComponent(id)}`);
  }
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
