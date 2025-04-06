import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { AuthModule } from './features/auth/auth.module';
import { UsersModule } from './features/users/users.module';
@NgModule({
  declarations: [      
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    SharedModule,
    AppComponent,
    AuthModule,
    UsersModule
  ],
  providers: [],
 })
export class AppModule { }
