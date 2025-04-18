import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormRegisterComponent } from './Components/form-register/form-register.component';
import { PageRegisterComponent } from './pages/page-register/page-register.component';
import { PageProfileComponent } from './pages/page-profile/page-profile.component';
import { UsersRoutingModule } from './users-routing.module';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    FormRegisterComponent,
    PageRegisterComponent,
    PageProfileComponent,
    UsersRoutingModule,
  ],
})
export class UsersModule {}
