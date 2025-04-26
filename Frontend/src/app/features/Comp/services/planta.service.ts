import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Planta, PlantaResponse } from '../../../../Types/planta';
import { AuthService } from '../../auth/Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class PlantaService {
  private apiUrl = `${environment.apiUrl}/Planta`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getPlantas(idComp: string): Observable<Planta[]> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('IdComp', idComp).set('Estado', true);
    return this.http.get<Planta[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      params: params,
    });
  }

  getPlanta(id: string): Observable<Planta> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('id', id).set('Estado', true);
    return this.http
      .get<Planta[]>(`${this.apiUrl}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
        params: params,
      })
      .pipe(
        map((response) => response[0]), // nos quedamos solo con el primer objeto del array
      );
  }

  createPlanta(Planta: Partial<Planta>): Observable<PlantaResponse> {
    const token = this.authService.getToken();
    console.log(Planta);
    return this.http.post<PlantaResponse>(this.apiUrl, Planta, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updatePlanta(
    id: string,
    Planta: Partial<Planta>,
  ): Observable<PlantaResponse> {
    const token = this.authService.getToken();
    return this.http.put<PlantaResponse>(`${this.apiUrl}/${id}`, Planta, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deletePlanta(id: string): Observable<PlantaResponse> {
    const token = this.authService.getToken();
    return this.http.delete<PlantaResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
