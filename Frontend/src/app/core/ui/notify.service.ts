import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Observable, map } from 'rxjs';
import { ProblemDetails } from '../api/models';
import { ConfirmDialogComponent } from './confirm-dialog.component';

/** Mensajes al usuario: éxito, error (a partir de ProblemDetails) y confirmaciones. */
@Injectable({ providedIn: 'root' })
export class NotifyService {
  private readonly snack = inject(MatSnackBar);
  private readonly dialog = inject(MatDialog);

  ok(mensaje: string): void {
    this.snack.open(mensaje, 'OK', { duration: 3000, panelClass: 'snack-ok' });
  }

  error(err: unknown, porDefecto = 'Ocurrió un error inesperado.'): void {
    this.snack.open(mensajeDeError(err, porDefecto), 'Cerrar', { duration: 6000, panelClass: 'snack-error' });
  }

  confirmar(titulo: string, mensaje: string, accion = 'Eliminar'): Observable<boolean> {
    return this.dialog
      .open(ConfirmDialogComponent, { data: { titulo, mensaje, accion }, width: '420px' })
      .afterClosed()
      .pipe(map((r) => r === true));
  }
}

export function mensajeDeError(err: unknown, porDefecto = 'Ocurrió un error inesperado.'): string {
  if (err instanceof HttpErrorResponse) {
    if (err.status === 0) return 'No hay conexión con el servidor.';
    const body = err.error as (ProblemDetails & { mensaje?: string; errors?: Record<string, string[]> }) | null;
    if (body?.errors) return Object.values(body.errors).flat().join(' ');
    return body?.title ?? body?.mensaje ?? body?.detail ?? porDefecto;
  }
  return porDefecto;
}
