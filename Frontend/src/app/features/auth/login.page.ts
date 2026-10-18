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
        <div class="logo"><span class="cuadro" aria-hidden="true"></span><span>SYNC</span></div>
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
          <button class="btn btn-accent btn-xl" type="submit" [disabled]="f.invalid || enviando()">
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
      background: var(--ink);
      color: #c9cbc4;
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      gap: 16px;
      padding: 32px;
      text-align: center;
      font-size: 17px;
      .logo {
        display: flex;
        align-items: center;
        gap: 16px;
        color: #fff;
        font-size: 56px;
        font-weight: 700;
        letter-spacing: 0.08em;
      }
      .cuadro {
        width: 56px;
        height: 56px;
        border-radius: 12px;
        background: var(--accent);
      }
    }
    h2 {
      font-size: 28px;
      margin: 0 0 20px;
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
          font-size: 36px;
        }
        .cuadro {
          width: 36px;
          height: 36px;
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
