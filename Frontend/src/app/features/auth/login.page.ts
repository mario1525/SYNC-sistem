import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { mensajeDeError } from '../../core/ui/notify.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule],
  template: `
    <div class="login">
      <section class="marca">
        <img src="assets/Img/Recurso 2.png" alt="Logo SYNC" class="logo" />
        <img src="assets/Img/Recurso 3.png" alt="SYNC" class="titulo" />
        <p>Sistema de Información y Coordinación de Mantenimiento</p>
      </section>

      <section class="formulario">
        <h2>Iniciar sesión</h2>
        <form #f="ngForm" (ngSubmit)="entrar()">
          <div class="form-group">
            <label for="usuario">Usuario</label>
            <input id="usuario" name="usuario" [(ngModel)]="usuario" required autocomplete="username" />
          </div>
          <div class="form-group">
            <label for="contrasenia">Contraseña</label>
            <input
              id="contrasenia"
              name="contrasenia"
              type="password"
              [(ngModel)]="contrasenia"
              required
              autocomplete="current-password"
            />
          </div>
          @if (error()) {
            <p class="error" role="alert">{{ error() }}</p>
          }
          <button class="btn btn-primary" type="submit" [disabled]="f.invalid || enviando()">
            {{ enviando() ? 'Ingresando…' : 'Entrar' }}
          </button>
        </form>
      </section>
    </div>
  `,
  styles: `
    .login {
      min-height: 100vh;
      display: grid;
      grid-template-columns: 1fr 1fr;
    }
    .marca {
      background: var(--sync-navy);
      color: #cfd8e6;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 16px;
      padding: 32px;
      text-align: center;
      .logo {
        width: min(260px, 60%);
      }
      .titulo {
        width: min(320px, 70%);
      }
    }
    .formulario {
      display: flex;
      flex-direction: column;
      justify-content: center;
      padding: 48px;
      max-width: 380px;
      width: 100%;
      margin: 0 auto;
      box-sizing: border-box;
      form {
        display: flex;
        flex-direction: column;
        gap: 16px;
      }
      button {
        justify-content: center;
        padding: 10px;
        font-size: 16px;
      }
    }
    .error {
      color: var(--sync-danger);
      margin: 0;
      font-size: 14px;
    }
    @media (max-width: 800px) {
      .login {
        grid-template-columns: 1fr;
      }
      .marca {
        padding: 24px;
        .logo {
          width: 120px;
        }
      }
    }
  `,
})
export class LoginPage {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  protected usuario = '';
  protected contrasenia = '';
  protected readonly enviando = signal(false);
  protected readonly error = signal('');

  constructor() {
    if (this.auth.autenticado()) void this.router.navigate(['/home']);
  }

  entrar(): void {
    this.enviando.set(true);
    this.error.set('');
    this.auth.login({ usuario: this.usuario.trim(), contrasenia: this.contrasenia }).subscribe({
      next: () => void this.router.navigate(['/home']),
      error: (e: unknown) => {
        this.enviando.set(false);
        this.error.set(mensajeDeError(e, 'No fue posible iniciar sesión.'));
      },
    });
  }
}
