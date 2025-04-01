import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LoginRequest, LoginResponse, JwtToken } from '../../../../Types/Auth';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'user_info';
  private currentUserSubject = new BehaviorSubject<JwtToken | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    const token = this.getToken();
    if (token) {
      this.setCurrentUser(token);
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>('/apigateway/Auth', credentials)
      .pipe(
        tap(response => {
          this.setToken(response.token);
          this.setCurrentUser(response.token);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSubject.next(null);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decodedToken = jwtDecode<JwtToken>(token);
      const currentTime = Date.now() / 1000;
      return decodedToken.exp > currentTime;
    } catch {
      return false;
    }
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  private setCurrentUser(token: string): void {
    try {
      const decodedToken = jwtDecode<JwtToken>(token);
      this.currentUserSubject.next(decodedToken);
      localStorage.setItem(this.USER_KEY, JSON.stringify(decodedToken));
    } catch (error) {
      console.error('Error al decodificar el token:', error);
      this.logout();
    }
  }

  getUserRole(): string | null {
    const user = this.currentUserSubject.value;
    return user ? user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] : null;
  }

  getUserId(): string | null {
    const user = this.currentUserSubject.value;
    return user ? user['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] : null;
  }

  getCompanyId(): string | null {
    const user = this.currentUserSubject.value;
    return user ? user.IdComp : null;
  }
} 