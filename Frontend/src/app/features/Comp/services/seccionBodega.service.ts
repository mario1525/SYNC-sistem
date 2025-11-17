import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import {
  SeccionBodega,
  SeccionBodegaResponse,
} from '../../../../Types/seccionbodega';

@Injectable({
  providedIn: 'root',
})
export class seccionBodegaService {
  private apiUrl = `${environment.apiUrl}/seccionbodega`;

  constructor(private http: HttpClient) {}

  getseccionBodegas(idBodega: string): Observable<SeccionBodega[]> {
    const params = new HttpParams().set('IdArea', idBodega).set('Estado', true);
    return this.http.get<SeccionBodega[]>(this.apiUrl, { params });
  }

  getseccionBodega(id: string): Observable<SeccionBodega> {
    const params = new HttpParams().set('Id', id).set('Estado', true);
    return this.http
      .get<SeccionBodega[]>(this.apiUrl, { params })
      .pipe(map((response) => response[0]));
  }

  createseccionBodega(
    seccionBodega: Partial<SeccionBodega>,
  ): Observable<SeccionBodegaResponse> {
    console.log(seccionBodega);
    return this.http.post<SeccionBodegaResponse>(this.apiUrl, seccionBodega);
  }

  updateseccionBodega(
    id: string,
    seccionBodega: Partial<SeccionBodega>,
  ): Observable<SeccionBodegaResponse> {
    return this.http.put<SeccionBodegaResponse>(
      `${this.apiUrl}/${id}`,
      seccionBodega,
    );
  }

  deleteseccionBodega(id: string): Observable<SeccionBodegaResponse> {
    return this.http.delete<SeccionBodegaResponse>(`${this.apiUrl}/${id}`);
  }
}
