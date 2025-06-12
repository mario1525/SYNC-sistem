import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '@env/environment';
import { Usuario, UsuarioResponse } from '../../../../Types/usuario';
import { AuthService } from '../../auth/Services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class UsuarioService {
  private apiUrl = `${environment.apiUrl}/Users`;

  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getUsuarios(): Observable<Usuario[]> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('Estado', true);
    return this.http.get<Usuario[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      params: params,
    });
  }

  getUsuarioComp(idComp: string): Observable<Usuario[]> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('IdComp', idComp).set('Estado', true);
    return this.http.get<Usuario[]>(this.apiUrl, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      params: params,
    });
  }

  getUsuario(id: string): Observable<Usuario> {
    const token = this.authService.getToken();
    const params = new HttpParams().set('Id', id).set('Estado', true);
    return this.http
      .get<Usuario[]>(this.apiUrl, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
        params: params,
      })
      .pipe(
        map((response) => response[0]), // nos quedamos solo con el primer objeto del array
      );
  }

  createUsuario(Usuario: Partial<Usuario>): Observable<UsuarioResponse> {
    const token = this.authService.getToken();
    console.log(Usuario);
    return this.http.post<UsuarioResponse>(this.apiUrl, Usuario, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  updateUsuario(
    id: string,
    Usuario: Partial<Usuario>,
  ): Observable<UsuarioResponse> {
    const token = this.authService.getToken();
    return this.http.put<UsuarioResponse>(`${this.apiUrl}/${id}`, Usuario, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  deleteUsuario(id: string): Observable<UsuarioResponse> {
    const token = this.authService.getToken();
    return this.http.delete<UsuarioResponse>(`${this.apiUrl}/${id}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
  }
}
