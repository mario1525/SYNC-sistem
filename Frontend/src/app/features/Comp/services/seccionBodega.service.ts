import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import {
  SeccionBodega,
  SeccionBodegaResponse,
} from '../../../../Types/seccionbodega';
import { AuthService } from '../../auth/Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class seccionBodegaService {
  private apiUrl = `${environment.apiUrl}/seccionBodega`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getseccionBodegas(idBodega: string): Observable<SeccionBodega[]> {
    const token = this.authService.getToken();
    const params = new HttpParams()
      .set('IdBodega', idBodega)
      .set('Estado', true);
    return this.http.get<SeccionBodega[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      params: params,
    });
  }

  getseccionBodega(id: string): Observable<SeccionBodega> {
    const token = this.authService.getToken();
    return this.http
      .get<SeccionBodega[]>(`${this.apiUrl}/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      .pipe(
        map((response) => response[0]), // nos quedamos solo con el primer objeto del array
      );
  }

  createseccionBodega(
    seccionBodega: Partial<SeccionBodega>,
  ): Observable<SeccionBodegaResponse> {
    const token = this.authService.getToken();
    console.log(seccionBodega);
    return this.http.post<SeccionBodegaResponse>(this.apiUrl, seccionBodega, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updateseccionBodega(
    id: string,
    seccionBodega: Partial<SeccionBodega>,
  ): Observable<SeccionBodegaResponse> {
    const token = this.authService.getToken();
    return this.http.put<SeccionBodegaResponse>(
      `${this.apiUrl}/${id}`,
      seccionBodega,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      },
    );
  }

  deleteseccionBodega(id: string): Observable<SeccionBodegaResponse> {
    const token = this.authService.getToken();
    return this.http.delete<SeccionBodegaResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
