import { DatePipe } from '@angular/common';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { ActividadEquipo, Adjunto, EjecucionValid, Guia, Proced, Valid } from '../../core/api/models';
import { ActividadEquipoService, AdjuntoService, GuiaService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';
import { FotoComponent } from '../../shared/foto.component';
import { FotoTomada, TomarFotoComponent } from '../../shared/tomar-foto.component';

/**
 * Ejecución en campo de la guía sobre un equipo: un paso por pantalla, verificaciones grandes
 * y las fotos que exige la guía. "Siguiente" solo se habilita con el paso completo.
 * Paso 0 = "Antes de empezar" (herramientas, insumos y seguridad de la guía).
 */
@Component({
  selector: 'app-ejecucion',
  imports: [RouterLink, DatePipe, FormsModule, FotoComponent, TomarFotoComponent],
  templateUrl: './ejecucion.page.html',
  styleUrl: './ejecucion.page.less',
})
export class EjecucionPage implements OnInit {
  readonly id = input.required<string>();

  private readonly service = inject(ActividadEquipoService);
  private readonly guiaService = inject(GuiaService);
  private readonly adjuntoService = inject(AdjuntoService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  private readonly auth = inject(AuthService);
  protected readonly esSupervision = this.auth.tieneRol('Root', 'Admin', 'Supervisor');

  protected readonly ae = signal<ActividadEquipo | null>(null);
  protected readonly guia = signal<Guia | null>(null);
  protected readonly resultados = signal<Map<string, EjecucionValid>>(new Map());
  protected readonly fotos = signal<Adjunto[]>([]);
  protected readonly cargando = signal(true);
  /** 0 = preparación; 1..n = pasos de la guía. */
  protected readonly paso = signal(0);
  protected readonly ocupado = signal<string | null>(null);
  protected observacion = '';

  protected readonly pasos = computed<Proced[]>(() => this.guia()?.proced ?? []);
  protected readonly actual = computed<Proced | null>(() => this.pasos()[this.paso() - 1] ?? null);
  protected readonly esUltimo = computed(() => this.paso() === this.pasos().length);
  protected readonly finalizada = computed(() => this.ae()?.estadoEjec === 'Completada');
  protected readonly totalVerificaciones = computed(() => this.pasos().reduce((n, p) => n + p.valid.length, 0));
  protected readonly cumplidas = computed(() => {
    const r = this.resultados();
    return this.pasos().reduce((n, p) => n + p.valid.filter((v) => r.get(v.id!)?.cumple).length, 0);
  });
  protected readonly avance = computed(() => {
    const total = this.totalVerificaciones();
    return total ? Math.round((this.cumplidas() * 100) / total) : 0;
  });
  protected readonly fotosPaso = computed(() => (this.actual()?.valid ?? []).filter((v) => v.requiereFoto));
  protected readonly pasoListo = computed(() => {
    const p = this.actual();
    return !p || this.completo(p);
  });
  /** Qué le falta al paso actual para poder avanzar. */
  protected readonly mensajePaso = computed(() => {
    const p = this.actual();
    if (!p?.valid.length) return '';
    if (this.pasoListo()) return 'Paso completo';
    return p.valid.every((v) => this.cumple(v)) ? 'Falta la foto de evidencia' : 'Marca todas las verificaciones';
  });

  ngOnInit(): void {
    this.service.get(this.id()).subscribe({
      next: (ae) => {
        this.ae.set(ae);
        forkJoin({
          guia: ae.idGuia ? this.guiaService.get(ae.idGuia) : of(null),
          resultados: this.service.resultados(this.id()),
          fotos: this.adjuntoService.list('Ejecucion', this.id()),
        }).subscribe({
          next: ({ guia, resultados, fotos }) => {
            this.guia.set(guia);
            this.resultados.set(new Map(resultados.map((r) => [r.idValid, r])));
            this.fotos.set(fotos);
            this.observacion = ae.observacion ?? '';
            // Retoma en el primer paso incompleto
            const pendiente = (guia?.proced ?? []).findIndex((p) => !this.completo(p));
            if (ae.estadoEjec === 'EnCurso')
              this.paso.set(pendiente >= 0 ? pendiente + 1 : (guia?.proced?.length ?? 0));
            this.cargando.set(false);
          },
          error: (e: unknown) => this.salir(e),
        });
      },
      error: (e: unknown) => this.salir(e),
    });
  }

  cumple(v: Valid): boolean {
    return !!this.resultados().get(v.id!)?.cumple;
  }

  fotoDe(v: Valid): Adjunto | undefined {
    return this.fotos().find((f) => f.ref === v.id);
  }

  completo(p: Proced): boolean {
    return p.valid.every((v) => this.cumple(v) && (!v.requiereFoto || !!this.fotoDe(v)));
  }

  marcar(v: Valid): void {
    if (this.finalizada() || this.ocupado()) return;
    const nuevo = !this.cumple(v);
    const previo = this.resultados();
    const siguiente = new Map(previo);
    siguiente.set(v.id!, { idActividadEquipo: this.id(), idValid: v.id!, cumple: nuevo });
    this.resultados.set(siguiente);
    this.service.marcar(this.id(), v.id!, nuevo).subscribe({
      next: () => {
        const ae = this.ae();
        if (ae && ae.estadoEjec === 'Pendiente') this.ae.set({ ...ae, estadoEjec: 'EnCurso' });
      },
      error: (e: unknown) => {
        this.resultados.set(previo);
        this.notify.error(e);
      },
    });
  }

  subirFoto(v: Valid, foto: FotoTomada): void {
    this.ocupado.set(v.id!);
    const anterior = this.fotoDe(v);
    this.adjuntoService.subir('Ejecucion', this.id(), foto.blob, foto.nombre, v.id!).subscribe({
      next: () => {
        // Al repetir una foto, la anterior se reemplaza
        if (anterior) this.adjuntoService.delete(anterior.id).subscribe({ complete: () => this.recargarFotos() });
        else this.recargarFotos();
      },
      error: (e: unknown) => {
        this.ocupado.set(null);
        this.notify.error(e);
      },
    });
  }

  ir(paso: number): void {
    this.paso.set(Math.max(0, Math.min(paso, this.pasos().length)));
    window.scrollTo({ top: 0 });
  }

  siguiente(): void {
    if (this.pasoListo()) this.ir(this.paso() + 1);
  }

  finalizar(): void {
    this.ocupado.set('finalizar');
    this.service.finalizar(this.id(), this.observacion.trim() || null).subscribe({
      next: (m) => {
        this.ocupado.set(null);
        this.notify.ok(m.mensaje);
        void this.router.navigate(['/home']);
      },
      error: (e: unknown) => {
        this.ocupado.set(null);
        this.notify.error(e);
      },
    });
  }

  reabrir(): void {
    this.service.reabrir(this.id()).subscribe({
      next: (m) => {
        this.notify.ok(m.mensaje);
        const ae = this.ae();
        if (ae) this.ae.set({ ...ae, estadoEjec: 'EnCurso' });
      },
      error: (e: unknown) => this.notify.error(e),
    });
  }

  private recargarFotos(): void {
    this.adjuntoService.list('Ejecucion', this.id()).subscribe((f) => {
      this.fotos.set(f);
      this.ocupado.set(null);
    });
  }

  private salir(e: unknown): void {
    this.notify.error(e);
    void this.router.navigate(['/home']);
  }
}
