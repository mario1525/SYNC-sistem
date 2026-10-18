import { Component, input, output, signal } from '@angular/core';

export interface FotoTomada {
  blob: Blob;
  nombre: string;
}

/**
 * Botón que abre la cámara (en tablets/teléfonos) o el selector de archivos (en PC).
 * Antes de emitir, reduce la foto a máx. 1600 px en JPEG: en planta la señal suele ser mala.
 */
@Component({
  selector: 'app-tomar-foto',
  template: `
    <label [class]="clase()" [attr.aria-disabled]="deshabilitado() || procesando()">
      <input
        type="file"
        accept="image/jpeg,image/png,image/webp"
        capture="environment"
        class="sr-only"
        [disabled]="deshabilitado() || procesando()"
        (change)="elegir($event)"
      />
      <ng-content />
    </label>
  `,
  styles: `
    label {
      cursor: pointer;
    }
    label[aria-disabled='true'] {
      opacity: 0.45;
      cursor: not-allowed;
    }
    label:focus-within {
      outline: 3px solid var(--blue);
      outline-offset: 2px;
    }
  `,
})
export class TomarFotoComponent {
  readonly clase = input('btn btn-secondary');
  readonly deshabilitado = input(false);
  readonly foto = output<FotoTomada>();
  protected readonly procesando = signal(false);

  async elegir(evento: Event): Promise<void> {
    const campo = evento.target as HTMLInputElement;
    const archivo = campo.files?.[0];
    campo.value = '';
    if (!archivo) return;
    this.procesando.set(true);
    try {
      const blob = await comprimirImagen(archivo);
      const base = archivo.name.replace(/\.[^.]+$/, '') || 'foto';
      this.foto.emit({ blob, nombre: blob === archivo ? archivo.name : `${base}.jpg` });
    } finally {
      this.procesando.set(false);
    }
  }
}

/** Reduce la imagen a `max` px en su lado mayor (JPEG). Si el navegador no puede, devuelve el original. */
export async function comprimirImagen(archivo: File, max = 1600, calidad = 0.82): Promise<Blob> {
  try {
    const bitmap = await createImageBitmap(archivo, { imageOrientation: 'from-image' });
    const escala = Math.min(1, max / Math.max(bitmap.width, bitmap.height));
    if (escala === 1 && archivo.size < 1_000_000) {
      bitmap.close();
      return archivo;
    }
    const canvas = document.createElement('canvas');
    canvas.width = Math.round(bitmap.width * escala);
    canvas.height = Math.round(bitmap.height * escala);
    canvas.getContext('2d')!.drawImage(bitmap, 0, 0, canvas.width, canvas.height);
    bitmap.close();
    const blob = await new Promise<Blob | null>((ok) => canvas.toBlob(ok, 'image/jpeg', calidad));
    return blob ?? archivo;
  } catch {
    return archivo;
  }
}
