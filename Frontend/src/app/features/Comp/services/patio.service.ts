import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Patio, PatioResponse } from '../../../../Types/patio';
import { AuthService } from '../../auth/Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class PatioService {
  private apiUrl = `${environment.apiUrl}/Patio`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getPatios(): Observable<Patio[]> {
    const token = this.authService.getToken();
    return this.http.get<Patio[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  getPatio(id: string): Observable<Patio> {
    const token = this.authService.getToken();
    return this.http
      .get<Patio[]>(`${this.apiUrl}/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      .pipe(
        map((response) => response[0]), // nos quedamos solo con el primer objeto del array
      );
  }

  createPatio(Patio: Partial<Patio>): Observable<PatioResponse> {
    const token = this.authService.getToken();
    console.log(Patio);
    return this.http.post<PatioResponse>(this.apiUrl, Patio, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updatePatio(id: string, Patio: Partial<Patio>): Observable<PatioResponse> {
    const token = this.authService.getToken();
    return this.http.put<PatioResponse>(`${this.apiUrl}/${id}`, Patio, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deletePatio(id: string): Observable<PatioResponse> {
    const token = this.authService.getToken();
    return this.http.delete<PatioResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
