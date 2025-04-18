import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LogoHeaderComponent } from '../../shared/Components/logo-header/logo-header.component';
import { MenuComponent } from '../../shared/Components/menu/menu.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, LogoHeaderComponent, MenuComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less'],
})
export class HomeComponent {}
