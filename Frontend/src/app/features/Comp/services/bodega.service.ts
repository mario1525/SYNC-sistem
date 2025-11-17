import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Bodega, BodegaResponse } from '../../../../Types/bodega';

@Injectable({
  providedIn: 'root',
})
export class BodegaService {
  private apiUrl = `${environment.apiUrl}/bodega`;

  constructor(private http: HttpClient) {}

  getBodegas(idPlant: string): Observable<Bodega[]> {
    const params = new HttpParams()
      .set('IdPlanta', idPlant)
      .set('Estado', true);
    return this.http.get<Bodega[]>(this.apiUrl, { params });
  }

  getBodega(id: string): Observable<Bodega> {
    return this.http
      .get<Bodega[]>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => response[0])); // nos quedamos solo con el primer objeto del array
  }

  createBodega(Bodega: Partial<Bodega>): Observable<BodegaResponse> {
    console.log(Bodega);
    return this.http.post<BodegaResponse>(this.apiUrl, Bodega);
  }

  updateBodega(
    id: string,
    Bodega: Partial<Bodega>,
  ): Observable<BodegaResponse> {
    return this.http.put<BodegaResponse>(`${this.apiUrl}/${id}`, Bodega);
  }

  deleteBodega(id: string): Observable<BodegaResponse> {
    return this.http.delete<BodegaResponse>(`${this.apiUrl}/${id}`);
  }
}
