import { Routes } from '@angular/router';
import { authGuard, rolGuard } from './core/auth/auth.guard';
import { ShellComponent } from './layout/shell.component';

const GESTION = { roles: ['Root', 'Admin'] };
const OPERACION = { roles: ['Root', 'Admin', 'Supervisor'] };

export const routes: Routes = [
  { path: 'auth', loadComponent: () => import('./features/auth/login.page').then((m) => m.LoginPage) },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'home' },
      { path: 'home', loadComponent: () => import('./features/home/home.page').then((m) => m.HomePage) },
      { path: 'perfil', loadComponent: () => import('./features/usuarios/perfil.page').then((m) => m.PerfilPage) },

      // Organización
      {
        path: 'comp',
        canActivate: [rolGuard],
        data: { roles: ['Root'] },
        loadComponent: () => import('./features/companias/comp-list.page').then((m) => m.CompListPage),
      },
      {
        path: 'comp/:id',
        canActivate: [rolGuard],
        data: GESTION,
        loadComponent: () => import('./features/companias/comp-form.page').then((m) => m.CompFormPage),
      },
      {
        path: 'planta/:id',
        canActivate: [rolGuard],
        data: GESTION,
        loadComponent: () => import('./features/companias/planta.page').then((m) => m.PlantaPage),
      },
      {
        path: 'catalogos',
        canActivate: [rolGuard],
        data: GESTION,
        loadComponent: () => import('./features/catalogos/catalogos.page').then((m) => m.CatalogosPage),
      },

      // Activos y mantenimiento
      {
        path: 'equipo',
        loadComponent: () => import('./features/equipos/equipo-list.page').then((m) => m.EquipoListPage),
      },
      {
        path: 'equipo/:id',
        loadComponent: () => import('./features/equipos/equipo-form.page').then((m) => m.EquipoFormPage),
      },
      { path: 'guia', loadComponent: () => import('./features/guias/guia-list.page').then((m) => m.GuiaListPage) },
      { path: 'guia/:id', loadComponent: () => import('./features/guias/guia-form.page').then((m) => m.GuiaFormPage) },
      {
        path: 'actividad',
        loadComponent: () => import('./features/actividades/actividad-list.page').then((m) => m.ActividadListPage),
      },
      {
        path: 'actividad/:id',
        loadComponent: () => import('./features/actividades/actividad-form.page').then((m) => m.ActividadFormPage),
      },

      // Personas
      {
        path: 'cuadrilla',
        canActivate: [rolGuard],
        data: OPERACION,
        loadComponent: () => import('./features/cuadrillas/cuadrilla-list.page').then((m) => m.CuadrillaListPage),
      },
      {
        path: 'cuadrilla/:id',
        canActivate: [rolGuard],
        data: OPERACION,
        loadComponent: () => import('./features/cuadrillas/cuadrilla-form.page').then((m) => m.CuadrillaFormPage),
      },
      {
        path: 'users',
        canActivate: [rolGuard],
        data: OPERACION,
        loadComponent: () => import('./features/usuarios/usuario-list.page').then((m) => m.UsuarioListPage),
      },
      {
        path: 'users/:id',
        canActivate: [rolGuard],
        data: GESTION,
        loadComponent: () => import('./features/usuarios/usuario-form.page').then((m) => m.UsuarioFormPage),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
