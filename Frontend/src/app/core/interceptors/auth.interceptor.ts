import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { environment } from '@env/environment';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

function getCookie(name: string): string | null {
  const value = document.cookie
    .split('; ')
    .find((row) => row.startsWith(name + '='))
    ?.split('=')[1];
  return value ? decodeURIComponent(value) : null;
}

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const token = getCookie('auth_token');

  let authReq = req;
  if (token && req.url.startsWith(environment.apiUrl)) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` },
    });
  }

  return next(authReq).pipe(
    catchError((err: any) => {
      if (
        err instanceof HttpErrorResponse &&
        (err.status === 401 || err.status === 403)
      ) {
        // Best-effort: limpiar token y redirigir a login
        document.cookie =
          'auth_token=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/';
        document.cookie =
          'user_info=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/';
        router.navigate(['/auth']);
      }
      return throwError(() => err);
    }),
  );
};
