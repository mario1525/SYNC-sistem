import { Routes } from '@angular/router';
import { LoginPageComponent } from './features/auth/pages/login-page/login-page.component';
import { AuthGuard } from './core/guards/auth.guard';
import { HomeComponent } from './pages/home/home.component';


export const routes: Routes = [
  { path: '', redirectTo: 'auth', pathMatch: 'full' },
  { path: 'auth', component: LoginPageComponent },
  { path: 'home', component: HomeComponent ,canActivate: [AuthGuard]},  
  { path: 'users', loadChildren: () => import('./features/users/users.module').then(m => m.UsersModule), canActivate: [AuthGuard]},
  { path: 'equipo', loadChildren: () => import('./features/equipo/equipo.module').then(m => m.EquipoModule)/*, canActivate: [AuthGuard]*/},
  { path: '**', redirectTo: 'auth' }
]; 