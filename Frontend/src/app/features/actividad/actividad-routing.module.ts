import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { ActividadListComponent } from './pages/actividad-list/actividad-list.component';
import { ActividadFormComponent } from './pages/actividad-form/actividad-form.component';

const routes: Routes = [
  {
    path: '',
    component: ActividadListComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'new',
    component: ActividadFormComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'edit/:id',
    component: ActividadFormComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ActividadRoutingModule {}
