import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Esp, EspResponse } from '../../../../Types/esp';

@Injectable({
  providedIn: 'root',
})
export class EspService {
  private apiUrl = `${environment.apiUrl}/esp`;

  constructor(private http: HttpClient) {}

  getesps(id: string): Observable<Esp[]> {
    const params = new HttpParams().set('IdComp', id).set('Estado', true);
    return this.http.get<Esp[]>(this.apiUrl, { params });
  }

  getEsp(id: string): Observable<Esp> {
    return this.http
      .get<Esp[]>(`${this.apiUrl}/${id}`)
      .pipe(map((response) => response[0]));
  }

  createEsp(Esp: Partial<Esp>): Observable<EspResponse> {
    console.log(Esp);
    return this.http.post<EspResponse>(this.apiUrl, Esp);
  }

  updateEsp(id: string, Esp: Partial<Esp>): Observable<EspResponse> {
    return this.http.put<EspResponse>(`${this.apiUrl}/${id}`, Esp);
  }

  deleteEsp(id: string): Observable<EspResponse> {
    return this.http.delete<EspResponse>(`${this.apiUrl}/${id}`);
  }
}
