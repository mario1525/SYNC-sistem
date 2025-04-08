import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { EquipoListComponent } from './pages/equipo-list/equipo-list.component';
import { EquipoFormComponent } from './pages/equipo-form/equipo-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';

const routes: Routes = [
  { path: '', component: EquipoListComponent },
  { path: 'nuevo', component: EquipoFormComponent },
  { path: 'editar/:id', component: EquipoFormComponent }
];

@NgModule({
  declarations: [
    EquipoListComponent,
    EquipoFormComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class EquipoModule { } 