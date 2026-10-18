import { Component, computed, effect, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Esp, Guia, GuiaEquipo } from '../../core/api/models';
import { EspService, GuiaEquipoService, GuiaService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

/** Guías de mantenimiento: lista a la izquierda y detalle de la guía elegida a la derecha (/guia/:id). */
@Component({
  selector: 'app-guia-list',
  imports: [FormsModule, RouterLink],
  templateUrl: './guia-list.page.html',
  styleUrl: './guia-list.page.less',
})
export class GuiaListPage implements OnInit {
  /** Guía seleccionada (ruta /guia/:id). */
  readonly id = input<string>();

  private readonly service = inject(GuiaService);
  private readonly espService = inject(EspService);
  private readonly guiaEquipoService = inject(GuiaEquipoService);
  private readonly notify = inject(NotifyService);
  protected readonly puedeEditar = inject(AuthService).tieneRol('Root', 'Admin');

  protected readonly guias = signal<Guia[]>([]);
  protected readonly especialidades = signal<Esp[]>([]);
  protected readonly cargando = signal(false);
  protected nombre = '';
  protected idEsp = '';

  protected readonly guia = signal<Guia | null>(null);
  protected readonly equipos = signal<GuiaEquipo[]>([]);
  protected readonly abiertos = signal<Set<number>>(new Set([0]));
  protected readonly totales = computed(() => {
    const pasos = this.guia()?.proced ?? [];
    return {
      verificaciones: pasos.reduce((n, p) => n + p.valid.length, 0),
      fotos: pasos.reduce((n, p) => n + p.valid.filter((v) => v.requiereFoto).length, 0),
    };
  });
  protected readonly ficha = computed(() => {
    const g = this.guia();
    if (!g) return [];
    return [
      { titulo: 'Herramientas', texto: g.herramientas, alerta: false },
      { titulo: 'Insumos y repuestos', texto: g.insumos, alerta: false },
      { titulo: 'Seguridad industrial', texto: g.seguridadInd, alerta: true },
      { titulo: 'Medidas ambientales', texto: g.seguridadAmb, alerta: false },
      { titulo: 'Logística', texto: g.logistica, alerta: false },
      { titulo: 'Importante', texto: g.importante, alerta: false },
      { titulo: 'Notas', texto: g.notas, alerta: false },
    ].filter((b) => !!b.texto?.trim());
  });

  constructor() {
    effect(() => {
      const id = this.id();
      this.guia.set(null);
      this.equipos.set([]);
      this.abiertos.set(new Set([0]));
      if (!id) return;
      this.service.get(id).subscribe({
        next: (g) => this.guia.set({ ...g, proced: g.proced ?? [] }),
        error: (e: unknown) => this.notify.error(e),
      });
      this.guiaEquipoService.list({ IdGuia: id }).subscribe((e) => this.equipos.set(e));
    });
  }

  ngOnInit(): void {
    this.espService.list({ Estado: true }).subscribe((e) => this.especialidades.set(e));
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.service.list({ Nombre: this.nombre.trim(), IdEsp: this.idEsp }).subscribe({
      next: (g) => {
        this.guias.set(g);
        this.cargando.set(false);
      },
      error: (e: unknown) => {
        this.cargando.set(false);
        this.notify.error(e);
      },
    });
  }

  alternarPaso(i: number): void {
    const s = new Set(this.abiertos());
    if (s.has(i)) s.delete(i);
    else s.add(i);
    this.abiertos.set(s);
  }

  fotosDe(i: number): number {
    return this.guia()?.proced?.[i]?.valid.filter((v) => v.requiereFoto).length ?? 0;
  }

  eliminar(g: Guia): void {
    this.notify.confirmar('Eliminar guía', `¿Eliminar la guía "${g.nombre}"?`).subscribe((ok) => {
      if (!ok) return;
      this.service.delete(g.id!).subscribe({
        next: (m) => {
          this.notify.ok(m.mensaje);
          this.guia.set(null);
          this.cargar();
        },
        error: (e: unknown) => this.notify.error(e),
      });
    });
  }
}
