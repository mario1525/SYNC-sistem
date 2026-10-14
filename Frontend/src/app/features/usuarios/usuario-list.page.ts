import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Cuad, ROLES, Usuario } from '../../core/api/models';
import { CuadService, UsuarioService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

@Component({
  selector: 'app-usuario-list',
  imports: [FormsModule, RouterLink],
  template: `
    <div class="page-header">
      <h2>Usuarios</h2>
      @if (puedeEditar) {
        <div class="acciones">
          <a class="btn btn-primary" routerLink="/users/nuevo"
            ><span class="material-icons">person_add</span> Nuevo usuario</a
          >
        </div>
      }
    </div>

    <div class="filtros">
      <div class="form-group">
        <label for="f-nombre">Buscar</label>
        <input id="f-nombre" [(ngModel)]="nombre" (keyup.enter)="cargar()" placeholder="Nombre o apellido" />
      </div>
      <div class="form-group">
        <label for="f-rol">Rol</label>
        <select id="f-rol" [(ngModel)]="rol" (ngModelChange)="cargar()">
          <option value="">Todos</option>
          @for (r of roles; track r) {
            <option [value]="r">{{ r }}</option>
          }
        </select>
      </div>
      <div class="form-group">
        <label for="f-cuad">Cuadrilla</label>
        <select id="f-cuad" [(ngModel)]="idCuad" (ngModelChange)="cargar()">
          <option value="">Todas</option>
          @for (c of cuadrillas(); track c.id) {
            <option [value]="c.id">{{ c.nombre }}</option>
          }
        </select>
      </div>
      <button class="btn btn-info" type="button" (click)="cargar()">
        <span class="material-icons">search</span> Buscar
      </button>
    </div>

    <div class="table-wrap">
      <table class="table">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Identificación</th>
            <th>Rol</th>
            <th>Cargo</th>
            <th>Cuadrilla</th>
            @if (esRoot) {
              <th>Compañía</th>
            }
            <th>Acceso</th>
            <th>Estado</th>
          </tr>
        </thead>
        <tbody>
          @for (u of usuarios(); track u.id) {
            <tr>
              <td>
                @if (puedeEditar) {
                  <a [routerLink]="['/users', u.id]">{{ u.nombre }} {{ u.apellido }}</a>
                } @else {
                  {{ u.nombre }} {{ u.apellido }}
                }
              </td>
              <td>{{ u.identificacion }}</td>
              <td>{{ u.rol }}</td>
              <td>{{ u.cargo }}</td>
              <td>{{ u.cuadNombre ?? '—' }}</td>
              @if (esRoot) {
                <td>{{ u.compNombre }}</td>
              }
              <td>
                @if (u.tieneCredencial) {
                  <span class="badge info">{{ u.usuario }}</span>
                } @else {
                  <span class="badge warn">Sin acceso</span>
                }
              </td>
              <td>
                <span class="badge" [class.ok]="u.estado" [class.off]="!u.estado">{{
                  u.estado ? 'Activo' : 'Inactivo'
                }}</span>
              </td>
            </tr>
          } @empty {
            <tr>
              <td [attr.colspan]="esRoot ? 8 : 7" class="vacio">{{ cargando() ? 'Cargando…' : 'No hay usuarios.' }}</td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  `,
})
export class UsuarioListPage implements OnInit {
  private readonly service = inject(UsuarioService);
  private readonly notify = inject(NotifyService);
  private readonly auth = inject(AuthService);
  protected readonly puedeEditar = this.auth.tieneRol('Root', 'Admin');
  protected readonly esRoot = this.auth.tieneRol('Root');
  protected readonly roles = ROLES;

  protected readonly usuarios = signal<Usuario[]>([]);
  protected readonly cuadrillas = signal<Cuad[]>([]);
  protected readonly cargando = signal(false);
  protected nombre = '';
  protected rol = '';
  protected idCuad = '';

  constructor() {
    inject(CuadService)
      .list({ Estado: true })
      .subscribe((c) => this.cuadrillas.set(c));
  }

  ngOnInit(): void {
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.service.list({ Nombre: this.nombre, Rol: this.rol, IdCuad: this.idCuad }).subscribe({
      next: (u) => {
        this.usuarios.set(u);
        this.cargando.set(false);
      },
      error: (e: unknown) => {
        this.cargando.set(false);
        this.notify.error(e);
      },
    });
  }
}
