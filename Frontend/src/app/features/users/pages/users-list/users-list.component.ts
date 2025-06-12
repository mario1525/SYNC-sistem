import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../auth/Services/auth.service';
import { Usuario } from '../../../../../Types/usuario';
import { UsuarioService } from '../../Services/usuario.service';

@Component({
  selector: 'app-users-list',
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.less',
})
export class UsersListComponent implements OnInit {
  Usuarios: Usuario[] = [];
  idComp: string | null = null;
  role: string | null = null;

  constructor(
    private authService: AuthService,
    private usuarioService: UsuarioService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.role = this.authService.getUserRole();
    if (this.role == 'Root') {
      this.loadUsuarios();
    } else {
      this.idComp = this.authService.getCompanyId();
      if (this.idComp) {
        this.loadUsuariosComp(this.idComp);
      }
    }
  }

  loadUsuariosComp(idComp: string): void {
    this.usuarioService.getUsuarioComp(idComp).subscribe({
      next: (response) => {
        this.Usuarios = response as Usuario[];
      },
      error: (error) => {
        console.error('Error al cargar:', error);
      },
    });
  }

  loadUsuarios(): void {
    this.usuarioService.getUsuarios().subscribe({
      next: (response) => {
        this.Usuarios = response as Usuario[];
      },
      error: (error) => {
        console.error('Error al cargar:', error);
      },
    });
  }

  navigateToNew(): void {
    this.router.navigate(['/users/register']);
  }

  editGuia(id: string): void {
    this.router.navigate(['/users/register', id]);
  }

  deleteGuia(id: string): void {
    if (confirm('¿Está seguro de que desea eliminar este usuario?')) {
      this.usuarioService.deleteUsuario(id).subscribe({
        next: () => {
          if (this.idComp) {
            this.loadUsuariosComp(this.idComp);
          } else {
            this.loadUsuarios();
          }
        },
        error: (error) => {
          console.error('Error al eliminar el usuario:', error);
        },
      });
    }
  }
}
