import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CompRoutingModule } from './comp-routing.module';
import { CompListComponent } from './pages/comp-list/comp-list.component';
import { CompFormComponent } from './pages/comp-form/comp-form.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MenuComponent } from '../../shared/Components/menu/menu.component';
import { LogoHeaderComponent } from '../../shared/Components/logo-header/logo-header.component';
import { PlantaFormComponent } from './pages/planta-form/planta-form.component';
import { BodegaFormComponent } from './pages/bodega-form/bodega-form.component';
import { AreaFuncionalFormComponent } from './pages/area-funcional-form/area-funcional-form.component';

@NgModule({
  declarations: [CompListComponent, CompFormComponent, PlantaFormComponent, BodegaFormComponent, AreaFuncionalFormComponent],
  imports: [
    CommonModule,
    MatTabsModule,
    CompRoutingModule,
    RouterModule,
    ReactiveFormsModule,
    MenuComponent,
    LogoHeaderComponent,
  ],
})
export class CompModule {}
