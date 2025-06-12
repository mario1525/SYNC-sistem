import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CuadrillaRoutingModule } from './cuadrilla-routing.module';
import { CuadrillaFormComponent } from './pages/cuadrilla-form/cuadrilla-form.component';

@NgModule({
  declarations: [CuadrillaFormComponent],
  imports: [CommonModule, CuadrillaRoutingModule],
})
export class CuadrillaModule {}
