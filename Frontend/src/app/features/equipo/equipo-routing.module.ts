import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EquipoFormComponent } from './pages/equipo-form/equipo-form.component';
import { EquipoListComponent } from './pages/equipo-list/equipo-list.component';
import { AuthGuard } from '../../core/guards/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: EquipoListComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'new',
    component: EquipoFormComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'edit/:id',
    component: EquipoFormComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class EquipoRoutingModule {}
