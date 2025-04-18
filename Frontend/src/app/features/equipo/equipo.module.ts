import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
//import { RouterModule, Routes } from '@angular/router';
import { EquipoListComponent } from './pages/equipo-list/equipo-list.component';
import { EquipoFormComponent } from './pages/equipo-form/equipo-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { EquipoRoutingModule } from './equipo-routing.module';
import { MenuComponent } from '../../shared/Components/menu/menu.component';
import { LogoHeaderComponent } from '../../shared/Components/logo-header/logo-header.component';

@NgModule({
  declarations: [EquipoListComponent, EquipoFormComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    SharedModule,
    EquipoRoutingModule,
    MenuComponent,
    LogoHeaderComponent,
  ],
})
export class EquipoModule {}
