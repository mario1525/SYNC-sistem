import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth.guard';
import { GuiaListComponent } from './pages/guia-list/guia-list.component';
import { GuiaFormComponent } from './pages/guia-form/guia-form.component';

const routes: Routes = [
  {
    path: '',
    component: GuiaListComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'new',
    component: GuiaFormComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'edit/:id',
    component: GuiaFormComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GuiaRoutingModule {}
