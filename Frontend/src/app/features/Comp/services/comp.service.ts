import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { Comp, CompResponse } from '../../../../Types/Comp';
import { AuthService } from '../../auth/Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class CompService {
  private apiUrl = `${environment.apiUrl}/comp`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getComps(): Observable<Comp[]> {
    const token = this.authService.getToken();
    return this.http.get<Comp[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  getComp(id: string): Observable<Comp> {
    const token = this.authService.getToken();
    return this.http.get<Comp>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  createComp(comp: Partial<Comp>): Observable<CompResponse> {
    const token = this.authService.getToken();
    return this.http.post<CompResponse>(this.apiUrl, comp, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updateComp(id: string, comp: Partial<Comp>): Observable<CompResponse> {
    const token = this.authService.getToken();
    return this.http.put<CompResponse>(`${this.apiUrl}/${id}`, comp, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deleteComp(id: string): Observable<CompResponse> {
    const token = this.authService.getToken();
    return this.http.delete<CompResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
