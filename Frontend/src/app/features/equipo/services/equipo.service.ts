import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { AuthService } from '../../auth/Services/auth.service';
import { Equipo, EquipoResponse } from '../../../../Types/Equipo';

@Injectable({
  providedIn: 'root',
})
export class EquipoService {
  private apiUrl = `${environment.apiUrl}/equipo`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getEquipos(idComp: string): Observable<Equipo[]> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('IdComp', idComp).set('Estado', true);
    return this.http.get<Equipo[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      params: params,
    });
  }

  getEquipo(id: string): Observable<Equipo> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('Id', id).set('Estado', true);
    return this.http
      .get<Equipo[]>(this.apiUrl, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
        params: params,
      })
      .pipe(
        map((response) => response[0]), // nos quedamos solo con el primer objeto del array
      );
  }

  createEquipo(equipo: Equipo): Observable<EquipoResponse> {
    const token = this.authService.getToken();
    return this.http.post<EquipoResponse>(this.apiUrl, equipo, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updateEquipo(id: string, equipo: Equipo): Observable<EquipoResponse> {
    const token = this.authService.getToken();
    return this.http.put<EquipoResponse>(`${this.apiUrl}/${id}`, equipo, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deleteEquipo(id: string): Observable<EquipoResponse> {
    const token = this.authService.getToken();
    return this.http.delete<EquipoResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
