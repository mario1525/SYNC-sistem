import { Routes } from '@angular/router';
import { LoginPageComponent } from './features/auth/pages/login-page/login-page.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'auth', pathMatch: 'full' },
  { path: 'auth', component: LoginPageComponent },
  { 
    path: 'equipo', 
    loadChildren: () => import('./features/equipo/equipo.module').then(m => m.EquipoModule),
    canActivate: [AuthGuard]
  },
  { path: 'users', loadChildren: () => import('./features/users/users.module').then(m => m.UsersModule) },
  { path: '**', redirectTo: 'auth' }
]; 