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

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    const role = this.authService.getUserRole(); // Esto ya lo tienes implementado
    if (role && MENU_ITEMS[role]) {
      this.menuItems = MENU_ITEMS[role];
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth']);
  }
}

const MENU_ITEMS: Record<string, { label: string; route: string }[]> = {
  Admin: [
    { label: 'Inicio', route: '/home' },
    { label: 'Equipos', route: '/equipo/equipo' },
    { label: 'Compañías', route: '/comp' },
    { label: 'Usuarios', route: '/users/list' },
  ],
  Supervisor: [
    { label: 'Inicio', route: '/home' },
    { label: 'Equipos', route: '/equipo/list' },
    { label: 'Usuarios', route: '/users/list' },
  ],
  Mecanico: [
    { label: 'Inicio', route: '/home' },
    { label: 'Usuarios', route: '/users/profile' },
    { label: 'Equipos', route: '/equipos' },
  ],
};
