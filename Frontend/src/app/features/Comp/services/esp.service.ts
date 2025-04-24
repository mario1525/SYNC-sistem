import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Esp, EspResponse } from '../../../../Types/esp';
import { AuthService } from '../../auth/Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class EspService {
  private apiUrl = `${environment.apiUrl}/esp`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getesps(id: string): Observable<Esp[]> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('IdComp', id).set('Estado', true);
    return this.http.get<Esp[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      params: params,
    });
  }

  getEsp(id: string): Observable<Esp> {
    const token = this.authService.getToken();
    return this.http
      .get<Esp[]>(`${this.apiUrl}/${id}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      })
      .pipe(
        map((response) => response[0]), // nos quedamos solo con el primer objeto del array
      );
  }

  createEsp(Esp: Partial<Esp>): Observable<EspResponse> {
    const token = this.authService.getToken();
    console.log(Esp);
    return this.http.post<EspResponse>(this.apiUrl, Esp, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updateEsp(id: string, Esp: Partial<Esp>): Observable<EspResponse> {
    const token = this.authService.getToken();
    return this.http.put<EspResponse>(`${this.apiUrl}/${id}`, Esp, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deleteEsp(id: string): Observable<EspResponse> {
    const token = this.authService.getToken();
    return this.http.delete<EspResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
