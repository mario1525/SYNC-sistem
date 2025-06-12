import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ActividadRoutingModule } from './actividad-routing.module';
import { ActividadListComponent } from './pages/actividad-list/actividad-list.component';
import { ActividadFormComponent } from './pages/actividad-form/actividad-form.component';
import { MenuComponent } from '../../shared/Components/menu/menu.component';
import { ReactiveFormsModule } from '@angular/forms';
import { LogoHeaderComponent } from '../../shared/Components/logo-header/logo-header.component';

@NgModule({
  declarations: [ActividadListComponent, ActividadFormComponent],
  imports: [
    CommonModule,
    ActividadRoutingModule,
    MenuComponent,
    ReactiveFormsModule,
    LogoHeaderComponent,
  ],
})
export class ActividadModule {}
