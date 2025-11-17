import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Guia, GuiaResponse } from '../../../../Types/guia';

@Injectable({
  providedIn: 'root',
})
export class GuiaService {
  private apiUrl = `${environment.apiUrl}/guia`;

  constructor(private http: HttpClient) {}

  getGuias(idPlant: string): Observable<Guia[]> {
    const params = new HttpParams().set('IdComp', idPlant).set('Estado', true);
    return this.http.get<Guia[]>(this.apiUrl, { params });
  }

  getGuia(id: string): Observable<Guia> {
    return this.http
      .get<Guia[]>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => response[0]));
  }

  createGuia(Guia: Partial<Guia>): Observable<GuiaResponse> {
    console.log(Guia);
    return this.http.post<GuiaResponse>(this.apiUrl, Guia);
  }

  updateGuia(id: string, Guia: Partial<Guia>): Observable<GuiaResponse> {
    return this.http.put<GuiaResponse>(`${this.apiUrl}/${id}`, Guia);
  }

  deleteGuia(id: string): Observable<GuiaResponse> {
    return this.http.delete<GuiaResponse>(`${this.apiUrl}/${id}`);
  }
}
