import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { LoginRequest } from '../../../../../Types/Auth';
import { Router } from '@angular/router';
@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.less',
})
export class LoginFormComponent {
  loginRequest: LoginRequest = {
    usuario: '',
    contrasenia: '',
  };

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  onSubmit(): void {
    this.authService.login(this.loginRequest).subscribe({
      next: () => {
        const userRole = this.authService.getUserRole();
        if (userRole === 'Admin') {
          this.router.navigate(['/home']);
        }
        if (userRole === 'Root') {
          this.router.navigate(['/home']);
        }
        if (userRole === 'Supervisor') {
          this.router.navigate(['/home']);
        }
        if (userRole === 'Mecanico') {
          this.router.navigate(['/home']);
        }
      },
      error: (error) => {
        console.error('Error en login:', error);
      },
    });
  }
}
