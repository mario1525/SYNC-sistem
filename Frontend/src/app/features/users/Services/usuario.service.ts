import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Usuario, UsuarioResponse } from '../../../../Types/usuario';

@Injectable({
  providedIn: 'root',
})
export class UsuarioService {
  private apiUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  getUsuarios(): Observable<Usuario[]> {
    const params = new HttpParams().set('Estado', true);
    return this.http.get<Usuario[]>(this.apiUrl, { params });
  }

  getUsuarioComp(idComp: string): Observable<Usuario[]> {
    const params = new HttpParams().set('IdComp', idComp).set('Estado', true);
    return this.http.get<Usuario[]>(this.apiUrl, { params });
  }

  getUsuario(id: string): Observable<Usuario> {
    const params = new HttpParams().set('Id', id).set('Estado', true);
    return this.http
      .get<Usuario[]>(this.apiUrl, { params })
      .pipe(map((response) => response[0]));
  }

  createUsuario(Usuario: Partial<Usuario>): Observable<UsuarioResponse> {
    console.log(Usuario);
    return this.http.post<UsuarioResponse>(this.apiUrl, Usuario);
  }

  updateUsuario(
    id: string,
    Usuario: Partial<Usuario>,
  ): Observable<UsuarioResponse> {
    return this.http.put<UsuarioResponse>(`${this.apiUrl}/${id}`, Usuario);
  }

  deleteUsuario(id: string): Observable<UsuarioResponse> {
    return this.http.delete<UsuarioResponse>(`${this.apiUrl}/${id}`);
  }
}
