import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';

interface ConfirmData {
  titulo: string;
  mensaje: string;
  accion: string;
}

@Component({
  selector: 'app-confirm-dialog',
  imports: [MatDialogModule],
  template: `
    <h2 mat-dialog-title>{{ data.titulo }}</h2>
    <mat-dialog-content>{{ data.mensaje }}</mat-dialog-content>
    <mat-dialog-actions align="end">
      <button class="btn btn-secondary" [mat-dialog-close]="false">Cancelar</button>
      <button class="btn btn-danger" [mat-dialog-close]="true">{{ data.accion }}</button>
    </mat-dialog-actions>
  `,
})
export class ConfirmDialogComponent {
  readonly data = inject<ConfirmData>(MAT_DIALOG_DATA);
}
