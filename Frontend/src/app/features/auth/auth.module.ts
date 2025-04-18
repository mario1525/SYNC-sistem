import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthRoutingModule } from './auth-routing.module';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { LoginFormComponent } from './Components/login-form/login-form.component';

@NgModule({
  imports: [
    CommonModule,
    AuthRoutingModule,
    LoginPageComponent, // ✅ standalone component
    LoginFormComponent, // ✅ standalone component
  ],
})
export class AuthModule {}
