import { Component } from '@angular/core';

@Component({
  selector: 'app-logo-header',
  standalone: true,
  template: `
    <div class="logo-container">
      <img
        src="assets/Img/Recurso 2.png"
        alt="Logo grande"
        class="logo-center"
      />
    </div>
  `,
  styles: [
    `
      .logo-container {
        display: flex;
        justify-content: left;
        align-items: left;
        background-color: #1a2b48;
        padding: 0;
        margin: 0;
        width: 100%;
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
      }
      .logo-center {
        height: 70px;
      }
    `,
  ],
})
export class LogoHeaderComponent {}
