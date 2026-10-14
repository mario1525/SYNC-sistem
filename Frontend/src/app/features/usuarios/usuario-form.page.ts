import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Comp, Credencial, Cuad, Esp, Rol, ROLES, Usuario } from '../../core/api/models';
import { CompService, CredencialService, CuadService, EspService, UsuarioService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

/** Ficha de usuario: datos personales, rol, cuadrilla y credenciales de acceso. */
@Component({
  selector: 'app-usuario-form',
  imports: [FormsModule, RouterLink],
  templateUrl: './usuario-form.page.html',
})
export class UsuarioFormPage implements OnInit {
  /** 'nuevo' o el id. */
  readonly id = input.required<string>();

  private readonly service = inject(UsuarioService);
  private readonly credService = inject(CredencialService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  protected readonly auth = inject(AuthService);

  protected readonly usuario = signal<Usuario>({
    nombre: '',
    identificacion: 0,
    rol: 'Tecnico',
    estado: true,
    idCuad: null,
    idEsp: null,
  });
  protected readonly credencial = signal<Credencial | null>(null);
  protected readonly cuadrillas = signal<Cuad[]>([]);
  protected readonly especialidades = signal<Esp[]>([]);
  protected readonly companias = signal<Comp[]>([]);
  protected readonly esNuevo = computed(() => this.id() === 'nuevo');
  protected readonly esRoot = this.auth.tieneRol('Root');
  protected readonly esUnoMismo = computed(() => this.id() === this.auth.userId());
  /** Solo Root puede asignar el rol Root. */
  protected readonly roles = computed<Rol[]>(() => (this.esRoot ? ROLES : ROLES.filter((r) => r !== 'Root')));
  protected readonly guardando = signal(false);

  protected nuevoUsuario = '';
  protected nuevaClave = '';

  private readonly cuadService = inject(CuadService);

  constructor() {
    inject(EspService)
      .list({ Estado: true })
      .subscribe((e) => this.especialidades.set(e));
    if (this.esRoot)
      inject(CompService)
        .list({ Estado: true })
        .subscribe((c) => this.companias.set(c));
  }

  ngOnInit(): void {
    if (this.esNuevo()) {
      this.cargarCuadrillas();
      return;
    }
    this.service.get(this.id()).subscribe({
      next: (u) => {
        this.usuario.set(u);
        this.cargarCuadrillas();
      },
      error: (e: unknown) => {
        this.notify.error(e);
        void this.router.navigate(['/users']);
      },
    });
    this.cargarCredencial();
  }

  guardar(): void {
    this.guardando.set(true);
    const u = this.usuario();
    const op = this.esNuevo() ? this.service.create(u) : this.service.update(this.id(), u);
    op.subscribe({
      next: (m) => {
        this.guardando.set(false);
        this.notify.ok(m.mensaje);
        if (this.esNuevo() && m.id) void this.router.navigate(['/users', m.id]);
      },
      error: (e: unknown) => {
        this.guardando.set(false);
        this.notify.error(e);
      },
    });
  }

  crearAcceso(): void {
    this.credService
      .create({ usuario: this.nuevoUsuario.trim(), contrasenia: this.nuevaClave, idUser: this.id(), estado: true })
      .subscribe({
        next: (m) => {
          this.notify.ok(m.mensaje);
          this.nuevaClave = '';
          this.cargarCredencial();
        },
        error: (e: unknown) => this.notify.error(e),
      });
  }

  restablecerClave(): void {
    const c = this.credencial();
    if (!c) return;
    this.credService.update(c.id!, { usuario: c.usuario, contrasenia: this.nuevaClave, estado: c.estado }).subscribe({
      next: () => {
        this.notify.ok('Contraseña restablecida.');
        this.nuevaClave = '';
      },
      error: (e: unknown) => this.notify.error(e),
    });
  }

  alternarAcceso(): void {
    const c = this.credencial();
    if (!c) return;
    this.credService.setEstado(c.id!, !c.estado).subscribe({
      next: () => this.cargarCredencial(),
      error: (e: unknown) => this.notify.error(e),
    });
  }

  eliminar(): void {
    const u = this.usuario();
    this.notify
      .confirmar('Eliminar usuario', `¿Eliminar a ${u.nombre} ${u.apellido ?? ''}? También se revocará su acceso.`)
      .subscribe((ok) => {
        if (!ok) return;
        this.service.delete(this.id()).subscribe({
          next: (m) => {
            this.notify.ok(m.mensaje);
            void this.router.navigate(['/users']);
          },
          error: (e: unknown) => this.notify.error(e),
        });
      });
  }

  /** Cuadrillas de la compañía del usuario (Root puede estar editando otra compañía). */
  cargarCuadrillas(): void {
    this.cuadService.list({ Estado: true, IdComp: this.usuario().idComp }).subscribe((c) => this.cuadrillas.set(c));
  }

  private cargarCredencial(): void {
    this.credService.list({ IdUser: this.id() }).subscribe((c) => {
      this.credencial.set(c[0] ?? null);
      if (!c[0] && !this.nuevoUsuario) this.nuevoUsuario = (this.usuario().correo ?? '').split('@')[0];
    });
  }
}
