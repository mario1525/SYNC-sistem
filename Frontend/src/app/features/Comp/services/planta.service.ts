import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Planta, PlantaResponse } from '../../../../Types/planta';

@Injectable({
  providedIn: 'root',
})
export class PlantaService {
  private apiUrl = `${environment.apiUrl}/planta`;

  constructor(private http: HttpClient) {}

  getPlantas(idComp: string): Observable<Planta[]> {
    const params = new HttpParams().set('IdComp', idComp).set('Estado', true);
    return this.http.get<Planta[]>(this.apiUrl, { params });
  }

  getPlanta(id: string): Observable<Planta> {
    const params = new HttpParams().set('Id', id).set('Estado', true);
    return this.http
      .get<Planta[]>(`${this.apiUrl}`, { params })
      .pipe(map((response) => response[0]));
  }

  createPlanta(Planta: Partial<Planta>): Observable<PlantaResponse> {
    console.log(Planta);
    return this.http.post<PlantaResponse>(this.apiUrl, Planta);
  }

  updatePlanta(
    id: string,
    Planta: Partial<Planta>,
  ): Observable<PlantaResponse> {
    return this.http.put<PlantaResponse>(`${this.apiUrl}/${id}`, Planta);
  }

  deletePlanta(id: string): Observable<PlantaResponse> {
    return this.http.delete<PlantaResponse>(`${this.apiUrl}/${id}`);
  }
}
