import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PageProfileComponent } from './pages/page-profile/page-profile.component';
import { PageRegisterComponent } from './pages/page-register/page-register.component';
import { UsersListComponent } from './pages/users-list/users-list.component';

const routes: Routes = [
  {
    path: 'register',
    component: PageRegisterComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'register/:id',
    component: PageRegisterComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'profile/:id',
    component: PageProfileComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'list/:id',
    component: UsersListComponent,
    canActivate: [AuthGuard],
  },
  {
    path: '',
    component: UsersListComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UsersRoutingModule {}
