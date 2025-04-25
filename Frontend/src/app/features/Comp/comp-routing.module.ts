import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CompListComponent } from './pages/comp-list/comp-list.component';
import { CompFormComponent } from './pages/comp-form/comp-form.component';
import { AuthGuard } from '../../core/guards/auth.guard';
import { PlantaFormComponent } from './pages/planta-form/planta-form.component';

const routes: Routes = [
  {
    path: '',
    component: CompListComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'new',
    component: CompFormComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'edit/:id',
    component: CompFormComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'edit/:id/planta/new',
    component: PlantaFormComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'edit/:companiaId/planta/edit/:plantaId',
    component: PlantaFormComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class CompRoutingModule {}
