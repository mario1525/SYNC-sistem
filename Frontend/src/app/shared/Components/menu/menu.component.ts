import { Component, OnInit } from '@angular/core';
import { RouterModule, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../features/auth/Services/auth.service'; // Ajusta la ruta según tu estructura

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.less'],
})
export class MenuComponent implements OnInit {
  menuItems: { label: string; route: string }[] = [];
  idComp: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    const role = this.authService.getUserRole();
    this.idComp = this.authService.getCompanyId();
    if (role) {
      this.menuItems = getMenuItems(role, this.idComp);
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth']);
  }
}

function getMenuItems(role: string, idComp: string | null) {
  const routes: Record<string, { label: string; route: string }[]> = {
    Root: [
      { label: 'Inicio', route: '/home' },
      { label: 'Compañías', route: '/comp' },
      { label: 'Usuarios', route: '/user' },
    ],
    Admin: [
      { label: 'Inicio', route: '/home' },
      { label: 'Equipos', route: '/equipo/equipo' },
      { label: 'Compañía', route: `/comp/edit/${idComp ?? ''}` },
      { label: 'Guias', route: '/guia' },
      { label: 'Usuarios', route: '/users/list' },
    ],
    Supervisor: [
      { label: 'Inicio', route: '/home' },
      { label: 'Equipos', route: '/equipo/list' },
      { label: 'Usuarios', route: '/users/list' },
    ],
    Mecanico: [
      { label: 'Inicio', route: '/home' },
      { label: 'Usuario', route: '/users/profile' },
      { label: 'Equipos', route: '/equipos' },
    ],
  };

  return routes[role] || [];
}
