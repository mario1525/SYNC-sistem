import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormRegisterComponent } from './Components/form-register/form-register.component';
import { PageRegisterComponent } from './pages/page-register/page-register.component';
import { PageProfileComponent } from './pages/page-profile/page-profile.component';
import { UsersRoutingModule } from './users-routing.module';
import { UsersListComponent } from './pages/users-list/users-list.component';
import { MenuComponent } from '../../shared/Components/menu/menu.component';
import { ReactiveFormsModule } from '@angular/forms';
import { LogoHeaderComponent } from '../../shared/Components/logo-header/logo-header.component';

@NgModule({
  declarations: [UsersListComponent],
  imports: [
    CommonModule,
    FormRegisterComponent,
    PageRegisterComponent,
    PageProfileComponent,
    UsersRoutingModule,
    MenuComponent,
    ReactiveFormsModule,
    LogoHeaderComponent,
  ],
})
export class UsersModule {}
