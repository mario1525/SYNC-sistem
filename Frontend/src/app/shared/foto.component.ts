import { Component, DestroyRef, effect, inject, input, signal, untracked } from '@angular/core';
import { Subscription } from 'rxjs';
import { AdjuntoService } from '../core/api/services';

/**
 * Muestra una foto guardada en el backend. Las imágenes requieren el token,
 * así que se descargan con HttpClient y se muestran como object URL.
 */
@Component({
  selector: 'app-foto',
  template: `
    @if (url(); as src) {
      <img [src]="src" [alt]="alt()" />
    } @else {
      <span class="material-icons" aria-hidden="true">{{ error() ? 'broken_image' : 'image' }}</span>
    }
  `,
  styles: `
    :host {
      display: flex;
      align-items: center;
      justify-content: center;
      overflow: hidden;
      background: var(--ink-2);
      color: var(--faint);
    }
    img {
      width: 100%;
      height: 100%;
      object-fit: var(--foto-ajuste, cover);
      display: block;
    }
  `,
})
export class FotoComponent {
  readonly id = input.required<string>();
  readonly alt = input('');

  private readonly service = inject(AdjuntoService);
  protected readonly url = signal<string | null>(null);
  protected readonly error = signal(false);
  private sub?: Subscription;

  constructor() {
    effect(() => {
      const id = this.id();
      // Solo depende del id: leer `url` aquí volvería a disparar la descarga al terminar
      untracked(() => {
        this.sub?.unsubscribe();
        this.liberar();
        this.error.set(false);
        this.sub = this.service.archivo(id).subscribe({
          next: (blob) => this.url.set(URL.createObjectURL(blob)),
          error: () => this.error.set(true),
        });
      });
    });
    inject(DestroyRef).onDestroy(() => {
      this.sub?.unsubscribe();
      this.liberar();
    });
  }

  private liberar(): void {
    const actual = this.url();
    if (actual) URL.revokeObjectURL(actual);
    this.url.set(null);
  }
}
