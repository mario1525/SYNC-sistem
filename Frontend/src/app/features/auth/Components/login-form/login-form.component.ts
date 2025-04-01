import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../Services/auth.service';
import { LoginRequest } from '../../../../../app/Types/Auth';

@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.less'
})
export class LoginFormComponent {
  loginRequest: LoginRequest = {
    username: '',
    password: ''
  };

  constructor(private authService: AuthService) {}

  onSubmit(): void {
    this.authService.login(this.loginRequest).subscribe({
      next: () => {
        // Manejar login exitoso
      },
      error: (error) => {
        console.error('Error en login:', error);
      }
    });
  }
}