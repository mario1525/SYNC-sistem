import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Guia, GuiaResponse } from '../../../../Types/guia';
import { AuthService } from '../../auth/Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class GuiaService {
  private apiUrl = `${environment.apiUrl}/Guia`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getGuias(idPlant: string): Observable<Guia[]> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('IdComp', idPlant).set('Estado', true);
    return this.http.get<Guia[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      params: params,
    });
  }

  getGuia(id: string): Observable<Guia> {
    const token = this.authService.getToken();
    return this.http
      .get<Guia[]>(`${this.apiUrl}/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      .pipe(
        map((response) => response[0]), // nos quedamos solo con el primer objeto del array
      );
  }

  createGuia(Guia: Partial<Guia>): Observable<GuiaResponse> {
    const token = this.authService.getToken();
    console.log(Guia);
    return this.http.post<GuiaResponse>(this.apiUrl, Guia, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updateGuia(id: string, Guia: Partial<Guia>): Observable<GuiaResponse> {
    const token = this.authService.getToken();
    return this.http.put<GuiaResponse>(`${this.apiUrl}/${id}`, Guia, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deleteGuia(id: string): Observable<GuiaResponse> {
    const token = this.authService.getToken();
    return this.http.delete<GuiaResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
