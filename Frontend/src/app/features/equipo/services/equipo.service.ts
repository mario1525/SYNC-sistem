import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Equipo, EquipoResponse } from '../../../../Types/Equipo';

@Injectable({
  providedIn: 'root',
})
export class EquipoService {
  private apiUrl = `${environment.apiUrl}/equipo`;

  constructor(private http: HttpClient) {}

  getEquipos(idComp: string): Observable<Equipo[]> {
    const params = new HttpParams().set('IdComp', idComp).set('Estado', true);
    return this.http.get<Equipo[]>(this.apiUrl, { params });
  }

  getEquipo(id: string): Observable<Equipo> {
    const params = new HttpParams().set('Id', id).set('Estado', true);
    return this.http
      .get<Equipo[]>(this.apiUrl, { params })
      .pipe(map((response) => response[0]));
  }

  createEquipo(equipo: Equipo): Observable<EquipoResponse> {
    return this.http.post<EquipoResponse>(this.apiUrl, equipo);
  }

  updateEquipo(id: string, equipo: Equipo): Observable<EquipoResponse> {
    return this.http.put<EquipoResponse>(`${this.apiUrl}/${id}`, equipo);
  }

  deleteEquipo(id: string): Observable<EquipoResponse> {
    return this.http.delete<EquipoResponse>(`${this.apiUrl}/${id}`);
  }
}
