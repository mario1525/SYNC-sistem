import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { AuthModule } from './features/auth/auth.module';
import { UsersModule } from './features/users/users.module';
import { EquipoModule } from './features/equipo/equipo.module';
import { ActividadModule } from './features/actividad/actividad.module';
@NgModule({
  declarations: [],
  imports: [
    BrowserModule,
    SharedModule,
    AppComponent,
    AuthModule,
    UsersModule,
    EquipoModule,
    ActividadModule,
  ],
  providers: [],
})
export class AppModule {}
