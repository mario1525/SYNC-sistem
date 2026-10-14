import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Rol } from '../core/api/models';
import { AuthService } from '../core/auth/auth.service';

interface ItemMenu {
  label: string;
  icon: string;
  route: string;
  roles: Rol[];
}

const TODOS: Rol[] = ['Root', 'Admin', 'Supervisor', 'Tecnico'];
const GESTION: Rol[] = ['Root', 'Admin'];
const OPERACION: Rol[] = ['Root', 'Admin', 'Supervisor'];

/** Layout principal: cabecera, menú lateral según rol y contenido. */
@Component({
  selector: 'app-shell',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.less',
})
export class ShellComponent {
  protected readonly auth = inject(AuthService);
  protected readonly menuAbierto = signal(false);

  private readonly items: ItemMenu[] = [
    { label: 'Inicio', icon: 'home', route: '/home', roles: TODOS },
    { label: 'Compañías', icon: 'domain', route: '/comp', roles: ['Root'] },
    { label: 'Mi compañía', icon: 'factory', route: '/comp/mia', roles: ['Admin'] },
    { label: 'Equipos', icon: 'precision_manufacturing', route: '/equipo', roles: TODOS },
    { label: 'Guías', icon: 'menu_book', route: '/guia', roles: TODOS },
    { label: 'Actividades', icon: 'event_note', route: '/actividad', roles: TODOS },
    { label: 'Cuadrillas', icon: 'groups', route: '/cuadrilla', roles: OPERACION },
    { label: 'Usuarios', icon: 'badge', route: '/users', roles: OPERACION },
    { label: 'Catálogos', icon: 'tune', route: '/catalogos', roles: GESTION },
    { label: 'Mi perfil', icon: 'account_circle', route: '/perfil', roles: TODOS },
  ];

  protected readonly menu = computed(() => {
    const rol = this.auth.rol();
    return this.items.filter((i) => !!rol && i.roles.includes(rol));
  });
}
