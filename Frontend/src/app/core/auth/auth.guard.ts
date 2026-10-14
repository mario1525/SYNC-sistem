import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Rol } from '../api/models';
import { AuthService } from './auth.service';

/** Requiere sesión válida. */
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.autenticado() || inject(Router).createUrlTree(['/auth']);
};

/** Requiere uno de los roles indicados en `data.roles` de la ruta. */
export const rolGuard: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const roles = (route.data['roles'] as Rol[] | undefined) ?? [];
  if (!auth.autenticado()) return inject(Router).createUrlTree(['/auth']);
  return roles.length === 0 || auth.tieneRol(...roles) || inject(Router).createUrlTree(['/home']);
};
