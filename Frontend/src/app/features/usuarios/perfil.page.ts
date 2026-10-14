import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Usuario } from '../../core/api/models';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

/** Perfil del usuario autenticado y cambio de contraseña propia. */
@Component({
  selector: 'app-perfil',
  imports: [FormsModule],
  template: `
    <div class="page-header"><h2>Mi perfil</h2></div>

    @if (yo(); as u) {
      <div class="card">
        <h3>Datos</h3>
        <div class="form-grid datos">
          <div><span class="muted">Nombre</span><br />{{ u.nombre }} {{ u.apellido }}</div>
          <div><span class="muted">Identificación</span><br />{{ u.identificacion }}</div>
          <div><span class="muted">Correo</span><br />{{ u.correo || '—' }}</div>
          <div><span class="muted">Compañía</span><br />{{ u.compNombre }}</div>
          <div><span class="muted">Rol</span><br />{{ u.rol }}</div>
          <div><span class="muted">Cargo</span><br />{{ u.cargo || '—' }}</div>
          <div><span class="muted">Cuadrilla</span><br />{{ u.cuadNombre ?? '—' }}</div>
          <div><span class="muted">Especialidad</span><br />{{ u.espNombre ?? '—' }}</div>
          <div><span class="muted">Usuario</span><br />{{ u.usuario }}</div>
        </div>
      </div>
    }

    <form class="card" #f="ngForm" (ngSubmit)="cambiar(f.resetForm.bind(f))">
      <h3>Cambiar contraseña</h3>
      <div class="form-grid">
        <div class="form-group">
          <label for="actual">Contraseña actual</label>
          <input
            id="actual"
            name="actual"
            type="password"
            autocomplete="current-password"
            [(ngModel)]="actual"
            required
          />
        </div>
        <div class="form-group">
          <label for="nueva">Nueva contraseña (mín. 8)</label>
          <input
            id="nueva"
            name="nueva"
            type="password"
            autocomplete="new-password"
            [(ngModel)]="nueva"
            required
            minlength="8"
          />
        </div>
        <div class="form-group">
          <label for="confirmacion">Confirmar nueva contraseña</label>
          <input
            id="confirmacion"
            name="confirmacion"
            type="password"
            autocomplete="new-password"
            [(ngModel)]="confirmacion"
            required
          />
          @if (confirmacion && confirmacion !== nueva) {
            <span class="error">Las contraseñas no coinciden.</span>
          }
        </div>
      </div>
      <div class="form-actions">
        <button class="btn btn-primary" type="submit" [disabled]="f.invalid || confirmacion !== nueva || enviando()">
          Cambiar contraseña
        </button>
      </div>
    </form>
  `,
  styles: `
    .datos > div {
      font-size: 14px;
    }
  `,
})
export class PerfilPage {
  private readonly auth = inject(AuthService);
  private readonly notify = inject(NotifyService);
  protected readonly yo = signal<Usuario | null>(null);
  protected readonly enviando = signal(false);
  protected actual = '';
  protected nueva = '';
  protected confirmacion = '';

  constructor() {
    this.auth.me().subscribe({ next: (u) => this.yo.set(u), error: (e: unknown) => this.notify.error(e) });
  }

  cambiar(reset: () => void): void {
    this.enviando.set(true);
    this.auth.cambiarContrasenia(this.actual, this.nueva).subscribe({
      next: (m) => {
        this.enviando.set(false);
        this.notify.ok(m.mensaje);
        reset();
      },
      error: (e: unknown) => {
        this.enviando.set(false);
        this.notify.error(e);
      },
    });
  }
}
