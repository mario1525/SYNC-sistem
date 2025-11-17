import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Patio, PatioResponse } from '../../../../Types/patio';

@Injectable({
  providedIn: 'root',
})
export class PatioService {
  private apiUrl = `${environment.apiUrl}/patio`;

  constructor(private http: HttpClient) {}

  getPatios(idBodega: string): Observable<Patio[]> {
    const params = new HttpParams()
      .set('IdBodega', idBodega)
      .set('Estado', true);
    return this.http.get<Patio[]>(this.apiUrl, { params });
  }

  getPatio(id: string): Observable<Patio> {
    return this.http
      .get<Patio[]>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => response[0])); // nos quedamos solo con el primer objeto del array
  }

  createPatio(Patio: Partial<Patio>): Observable<PatioResponse> {
    console.log(Patio);
    return this.http.post<PatioResponse>(this.apiUrl, Patio);
  }

  updatePatio(id: string, Patio: Partial<Patio>): Observable<PatioResponse> {
    return this.http.put<PatioResponse>(`${this.apiUrl}/${id}`, Patio);
  }

  deletePatio(id: string): Observable<PatioResponse> {
    return this.http.delete<PatioResponse>(`${this.apiUrl}/${id}`);
  }
}
