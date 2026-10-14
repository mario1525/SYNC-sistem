import { HttpClient, HttpParams } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { Entidad, Mensaje } from './models';

export type Filtro = Record<string, string | number | boolean | null | undefined>;

/**
 * Cliente REST genérico para los recursos del gateway:
 *   GET /{recurso}?filtros · GET /{recurso}/{id} · POST · PUT /{id} · DELETE /{id} · PATCH /{id}/estado
 * La compañía la impone el backend a partir del token: no hace falta enviar IdComp.
 */
export abstract class CrudService<T extends Entidad> {
  protected readonly http = inject(HttpClient);
  protected abstract readonly recurso: string;

  protected get url(): string {
    return `${environment.apiUrl}/${this.recurso}`;
  }

  list(filtro: Filtro = {}): Observable<T[]> {
    return this.http.get<T[]>(this.url, { params: toParams(filtro) });
  }

  get(id: string): Observable<T> {
    return this.http.get<T>(`${this.url}/${encodeURIComponent(id)}`);
  }

  create(entidad: Partial<T>): Observable<Mensaje> {
    return this.http.post<Mensaje>(this.url, entidad);
  }

  update(id: string, entidad: Partial<T>): Observable<Mensaje> {
    return this.http.put<Mensaje>(`${this.url}/${encodeURIComponent(id)}`, entidad);
  }

  /** Crea o actualiza según exista id. */
  save(entidad: Partial<T>): Observable<Mensaje> {
    return entidad.id ? this.update(entidad.id, entidad) : this.create(entidad);
  }

  delete(id: string): Observable<Mensaje> {
    return this.http.delete<Mensaje>(`${this.url}/${encodeURIComponent(id)}`);
  }

  setEstado(id: string, estado: boolean): Observable<Mensaje> {
    return this.http.patch<Mensaje>(`${this.url}/${encodeURIComponent(id)}/estado`, estado);
  }
}

export function toParams(filtro: Filtro): HttpParams {
  let params = new HttpParams();
  for (const [k, v] of Object.entries(filtro)) {
    if (v !== null && v !== undefined && v !== '') params = params.set(k, String(v));
  }
  return params;
}
