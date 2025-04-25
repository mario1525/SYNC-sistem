import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Bodega, BodegaResponse } from '../../../../Types/bodega';
import { AuthService } from '../../auth/Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class BodegaService {
  private apiUrl = `${environment.apiUrl}/Bodega`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getBodegas(): Observable<Bodega[]> {
    const token = this.authService.getToken();
    return this.http.get<Bodega[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  getBodega(id: string): Observable<Bodega> {
    const token = this.authService.getToken();
    return this.http
      .get<Bodega[]>(`${this.apiUrl}/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      .pipe(
        map((response) => response[0]), // nos quedamos solo con el primer objeto del array
      );
  }

  createBodega(Bodega: Partial<Bodega>): Observable<BodegaResponse> {
    const token = this.authService.getToken();
    console.log(Bodega);
    return this.http.post<BodegaResponse>(this.apiUrl, Bodega, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updateBodega(
    id: string,
    Bodega: Partial<Bodega>,
  ): Observable<BodegaResponse> {
    const token = this.authService.getToken();
    return this.http.put<BodegaResponse>(`${this.apiUrl}/${id}`, Bodega, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deleteBodega(id: string): Observable<BodegaResponse> {
    const token = this.authService.getToken();
    return this.http.delete<BodegaResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
