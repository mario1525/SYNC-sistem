import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { EquipoFormComponent } from './pages/equipo-form/equipo-form.component';

const routes: Routes = [
  { path: 'equipo', component: EquipoFormComponent }, // /auth → LoginPage
  { path: 'editar/:id', component: EquipoFormComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class EquipoRoutingModule {}
