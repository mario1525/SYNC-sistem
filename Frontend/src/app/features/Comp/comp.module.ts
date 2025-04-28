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
import { TabsBodegAreaFunComponent } from './components/tabs-bodeg-area-fun/tabs-bodeg-area-fun.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@NgModule({
  declarations: [
    CompListComponent,
    CompFormComponent,
    PlantaFormComponent,
    BodegaFormComponent,
    AreaFuncionalFormComponent,
    TabsBodegAreaFunComponent,
  ],
  imports: [
    CommonModule,
    MatTabsModule,
    CompRoutingModule,
    RouterModule,
    ReactiveFormsModule,
    MenuComponent,
    LogoHeaderComponent,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogModule,
  ],
})
export class CompModule {}
