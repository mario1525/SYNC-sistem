import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageProfileComponent } from './pages/page-profile/page-profile.component';
import { PageRegisterComponent } from './pages/page-register/page-register.component';

const routes: Routes = [
  { path: 'register', component: PageRegisterComponent },
  { path: 'profile', component: PageProfileComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UsersRoutingModule {}
