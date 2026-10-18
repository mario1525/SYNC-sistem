import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Rol } from '../core/api/models';
import { AuthService } from '../core/auth/auth.service';
import { iniciales, NOMBRE_ROL } from '../shared/formato';

interface ItemMenu {
  label: string;
  icon: string;
  route: string;
  roles: Rol[];
  /** Aparece en la barra inferior de tablets. */
  inferior?: boolean;
}

const TODOS: Rol[] = ['Root', 'Admin', 'Supervisor', 'Tecnico'];
const GESTION: Rol[] = ['Root', 'Admin'];
const OPERACION: Rol[] = ['Root', 'Admin', 'Supervisor'];

/** Layout principal: barra superior, menú lateral (PC) o barra inferior (tablet) según rol, y contenido. */
@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.less',
})
export class ShellComponent {
  protected readonly auth = inject(AuthService);
  protected readonly menuAbierto = signal(false);
  protected readonly iniciales = computed(() => iniciales(this.auth.nombre()));
  protected readonly nombreRol = computed(() => {
    const r = this.auth.rol();
    return r ? NOMBRE_ROL[r] : '';
  });

  private readonly items: ItemMenu[] = [
    { label: 'Inicio', icon: 'today', route: '/home', roles: TODOS, inferior: true },
    { label: 'Programación', icon: 'calendar_month', route: '/actividad', roles: TODOS, inferior: true },
    { label: 'Equipos', icon: 'precision_manufacturing', route: '/equipo', roles: TODOS, inferior: true },
    { label: 'Guías', icon: 'fact_check', route: '/guia', roles: TODOS, inferior: true },
    { label: 'Fallas', icon: 'report_problem', route: '/falla', roles: OPERACION },
    { label: 'Cuadrillas', icon: 'groups', route: '/cuadrilla', roles: OPERACION },
    { label: 'Usuarios', icon: 'badge', route: '/users', roles: OPERACION },
    { label: 'Compañías', icon: 'domain', route: '/comp', roles: ['Root'] },
    { label: 'Mi compañía', icon: 'factory', route: '/comp/mia', roles: ['Admin'] },
    { label: 'Catálogos', icon: 'tune', route: '/catalogos', roles: GESTION },
    { label: 'Mi perfil', icon: 'account_circle', route: '/perfil', roles: TODOS },
  ];

  protected readonly menu = computed(() => {
    const rol = this.auth.rol();
    return this.items.filter((i) => !!rol && i.roles.includes(rol));
  });
  protected readonly inferior = computed(() => this.menu().filter((i) => i.inferior));
}
