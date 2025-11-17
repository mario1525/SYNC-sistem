import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@env/environment';
import { Comp, CompResponse } from '../../../../Types/Comp';

@Injectable({
  providedIn: 'root',
})
export class CompService {
  private apiUrl = `${environment.apiUrl}/comp`;

  constructor(private http: HttpClient) {}

  getComps(): Observable<Comp[]> {
    return this.http.get<Comp[]>(this.apiUrl);
  }

  getComp(id: string): Observable<Comp> {
    return this.http.get<Comp>(`${this.apiUrl}/${id}`);
  }

  createComp(comp: Partial<Comp>): Observable<CompResponse> {
    console.log(comp);
    return this.http.post<CompResponse>(this.apiUrl, comp);
  }

  updateComp(id: string, comp: Partial<Comp>): Observable<CompResponse> {
    return this.http.put<CompResponse>(`${this.apiUrl}/${id}`, comp);
  }

  deleteComp(id: string): Observable<CompResponse> {
    return this.http.delete<CompResponse>(`${this.apiUrl}/${id}`);
  }
}
