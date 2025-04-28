import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import {
  AreaFuncional,
  AreaFuncionalResponse,
} from '../../../../Types/areafuncional';
import { AuthService } from '../../auth/Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class AreaFuncionalService {
  private apiUrl = `${environment.apiUrl}/AreaFuncional`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getAreaFuncionals(idPlant: string): Observable<AreaFuncional[]> {
    const token = this.authService.getToken();
    const params = new HttpParams()
      .set('IdPlanta', idPlant)
      .set('Estado', true);
    return this.http.get<AreaFuncional[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      params: params,
    });
  }

  getAreaFuncional(id: string): Observable<AreaFuncional> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('Id', id).set('Estado', true);
    return this.http
      .get<AreaFuncional[]>(this.apiUrl, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
        params: params,
      })
      .pipe(
        map((response) => response[0]), // nos quedamos solo con el primer objeto del array
      );
  }

  createAreaFuncional(
    AreaFuncional: Partial<AreaFuncional>,
  ): Observable<AreaFuncionalResponse> {
    const token = this.authService.getToken();
    console.log(AreaFuncional);
    return this.http.post<AreaFuncionalResponse>(this.apiUrl, AreaFuncional, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updateAreaFuncional(
    id: string,
    AreaFuncional: Partial<AreaFuncional>,
  ): Observable<AreaFuncionalResponse> {
    const token = this.authService.getToken();
    return this.http.put<AreaFuncionalResponse>(
      `${this.apiUrl}/${id}`,
      AreaFuncional,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      },
    );
  }

  deleteAreaFuncional(id: string): Observable<AreaFuncionalResponse> {
    const token = this.authService.getToken();
    return this.http.delete<AreaFuncionalResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
