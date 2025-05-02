import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GuiaRoutingModule } from './guia-routing.module';
import { GuiaListComponent } from './pages/guia-list/guia-list.component';
import { GuiaFormComponent } from './pages/guia-form/guia-form.component';
import { MenuComponent } from '../../shared/Components/menu/menu.component';
import { LogoHeaderComponent } from '../../shared/Components/logo-header/logo-header.component';

@NgModule({
  declarations: [GuiaListComponent, GuiaFormComponent],
  imports: [
    CommonModule,
    GuiaRoutingModule,
    MenuComponent,
    LogoHeaderComponent,
  ],
})
export class GuiaModule {}
