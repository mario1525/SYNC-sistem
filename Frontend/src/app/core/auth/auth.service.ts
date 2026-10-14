import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { Observable, tap } from 'rxjs';
import { environment } from '@env/environment';
import { JwtToken, LoginRequest, LoginResponse, Mensaje, Rol, Usuario } from '../api/models';

const TOKEN_KEY = 'sync_token';
const ROLE_CLAIM = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';
const ID_CLAIM = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier';

/** Sesión del usuario. El token se guarda en sessionStorage (se borra al cerrar el navegador). */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly url = `${environment.apiUrl}/auth`;

  private readonly claims = signal<JwtToken | null>(this.leerToken());

  readonly autenticado = computed(() => {
    const c = this.claims();
    return !!c && c.exp * 1000 > Date.now();
  });
  readonly rol = computed<Rol | null>(() => this.claims()?.[ROLE_CLAIM] ?? null);
  readonly userId = computed(() => this.claims()?.[ID_CLAIM] ?? null);
  readonly idComp = computed(() => this.claims()?.IdComp ?? null);
  readonly nombre = computed(() => this.claims()?.sub ?? '');

  login(credenciales: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(this.url, credenciales).pipe(
      tap((r) => {
        sessionStorage.setItem(TOKEN_KEY, r.token);
        this.claims.set(this.decodificar(r.token));
      }),
    );
  }

  logout(): void {
    sessionStorage.removeItem(TOKEN_KEY);
    this.claims.set(null);
    void this.router.navigate(['/auth']);
  }

  token(): string | null {
    return this.autenticado() ? sessionStorage.getItem(TOKEN_KEY) : null;
  }

  tieneRol(...roles: Rol[]): boolean {
    const r = this.rol();
    return !!r && roles.includes(r);
  }

  me(): Observable<Usuario> {
    return this.http.get<Usuario>(`${this.url}/me`);
  }

  cambiarContrasenia(actual: string, nueva: string): Observable<Mensaje> {
    return this.http.post<Mensaje>(`${this.url}/me/password`, { actual, nueva });
  }

  private leerToken(): JwtToken | null {
    const token = sessionStorage.getItem(TOKEN_KEY);
    return token ? this.decodificar(token) : null;
  }

  private decodificar(token: string): JwtToken | null {
    try {
      return jwtDecode<JwtToken>(token);
    } catch {
      return null;
    }
  }
}
