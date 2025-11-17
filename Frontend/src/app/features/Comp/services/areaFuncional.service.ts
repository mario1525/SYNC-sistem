import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import {
  AreaFuncional,
  AreaFuncionalResponse,
} from '../../../../Types/areafuncional';

@Injectable({
  providedIn: 'root',
})
export class AreaFuncionalService {
  private apiUrl = `${environment.apiUrl}/areafuncional`;

  constructor(private http: HttpClient) {}

  getAreaFuncionals(idPlant: string): Observable<AreaFuncional[]> {
    const params = new HttpParams()
      .set('IdPlanta', idPlant)
      .set('Estado', true);
    return this.http.get<AreaFuncional[]>(this.apiUrl, { params });
  }

  getAreaFuncional(id: string): Observable<AreaFuncional> {
    const params = new HttpParams().set('Id', id).set('Estado', true);
    return this.http
      .get<AreaFuncional[]>(this.apiUrl, { params })
      .pipe(map((response) => response[0]));
  }

  createAreaFuncional(
    AreaFuncional: Partial<AreaFuncional>,
  ): Observable<AreaFuncionalResponse> {
    console.log(AreaFuncional);
    return this.http.post<AreaFuncionalResponse>(this.apiUrl, AreaFuncional);
  }

  updateAreaFuncional(
    id: string,
    AreaFuncional: Partial<AreaFuncional>,
  ): Observable<AreaFuncionalResponse> {
    return this.http.put<AreaFuncionalResponse>(
      `${this.apiUrl}/${id}`,
      AreaFuncional,
    );
  }

  deleteAreaFuncional(id: string): Observable<AreaFuncionalResponse> {
    return this.http.delete<AreaFuncionalResponse>(`${this.apiUrl}/${id}`);
  }
}
