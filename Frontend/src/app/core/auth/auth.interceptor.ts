import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { environment } from '@env/environment';
import { AuthService } from './auth.service';

/** Adjunta el JWT a las llamadas al gateway y cierra la sesión si el token expiró (401). */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.token();
  const esApi = req.url.startsWith(environment.apiUrl);
  const esLogin = req.url === `${environment.apiUrl}/auth`;

  const peticion = token && esApi ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

  return next(peticion).pipe(
    catchError((err: unknown) => {
      // 403 = sin permisos para esa acción: NO cierra la sesión
      if (err instanceof HttpErrorResponse && err.status === 401 && esApi && !esLogin) auth.logout();
      return throwError(() => err);
    }),
  );
};
