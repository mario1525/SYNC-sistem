import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Esp, Guia, Proced } from '../../core/api/models';
import { EspService, GuiaService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

/**
 * Guía de mantenimiento con su checklist: Procedimientos (pasos) → Validaciones.
 * Al guardar se envía el árbol completo; el backend lo sincroniza en una transacción.
 */
@Component({
  selector: 'app-guia-form',
  imports: [FormsModule, RouterLink],
  templateUrl: './guia-form.page.html',
  styles: `
    .paso {
      border: 1px solid var(--sync-border);
      border-radius: 6px;
      padding: 12px;
      margin-bottom: 12px;
      background: #fbfcfd;
    }
    .paso-cab {
      display: flex;
      gap: 8px;
      align-items: flex-start;
    }
    .paso-cab .num {
      font-weight: 600;
      color: var(--sync-navy);
      padding-top: 8px;
      min-width: 24px;
    }
    .paso-cab .campos {
      flex: 1;
      display: grid;
      grid-template-columns: 1fr 2fr;
      gap: 8px;
    }
    .validaciones {
      margin: 8px 0 0 32px;
    }
    .valid {
      display: grid;
      grid-template-columns: 1fr 2fr auto;
      gap: 8px;
      margin-bottom: 6px;
      align-items: center;
    }
    .valid .material-icons.check {
      color: var(--sync-green);
      font-size: 18px;
    }
    @media (max-width: 800px) {
      .paso-cab .campos,
      .valid {
        grid-template-columns: 1fr;
      }
    }
  `,
})
export class GuiaFormPage implements OnInit {
  /** 'nueva' o el id de la guía. */
  readonly id = input.required<string>();

  private readonly service = inject(GuiaService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  protected readonly puedeEditar = inject(AuthService).tieneRol('Root', 'Admin');

  protected readonly guia = signal<Guia>({
    nombre: '',
    estado: true,
    intervalo: 30,
    personal: 1,
    duracion: 1,
    proced: [],
  });
  protected readonly especialidades = signal<Esp[]>([]);
  protected readonly esNueva = computed(() => this.id() === 'nueva');
  protected readonly guardando = signal(false);

  constructor() {
    inject(EspService)
      .list({ Estado: true })
      .subscribe((e) => this.especialidades.set(e));
  }

  ngOnInit(): void {
    if (this.esNueva()) return;
    this.service.get(this.id()).subscribe({
      next: (g) => this.guia.set({ ...g, proced: g.proced ?? [] }),
      error: (e: unknown) => {
        this.notify.error(e);
        void this.router.navigate(['/guia']);
      },
    });
  }

  get pasos(): Proced[] {
    return this.guia().proced ?? [];
  }

  agregarPaso(): void {
    this.mutar((g) => g.proced!.push({ nombre: '', descripcion: '', estado: true, valid: [] }));
  }

  quitarPaso(i: number): void {
    this.mutar((g) => g.proced!.splice(i, 1));
  }

  moverPaso(i: number, delta: number): void {
    this.mutar((g) => {
      const j = i + delta;
      if (j < 0 || j >= g.proced!.length) return;
      [g.proced![i], g.proced![j]] = [g.proced![j], g.proced![i]];
    });
  }

  agregarValidacion(p: Proced): void {
    this.mutar(() => p.valid.push({ nombre: '', descripcion: '', estado: true }));
  }

  quitarValidacion(p: Proced, i: number): void {
    this.mutar(() => p.valid.splice(i, 1));
  }

  guardar(): void {
    const g = this.guia();
    if (this.pasos.some((p) => !p.nombre.trim() || p.valid.some((v) => !v.nombre.trim()))) {
      this.notify.error(null, 'Todos los pasos y validaciones deben tener nombre.');
      return;
    }
    this.guardando.set(true);
    const op = this.esNueva() ? this.service.create(g) : this.service.update(this.id(), g);
    op.subscribe({
      next: (m) => {
        this.guardando.set(false);
        this.notify.ok(m.mensaje);
        if (this.esNueva() && m.id) void this.router.navigate(['/guia', m.id]);
        else this.ngOnInit();
      },
      error: (e: unknown) => {
        this.guardando.set(false);
        this.notify.error(e);
      },
    });
  }

  /** Aplica un cambio al árbol y notifica a la vista (zoneless). */
  private mutar(fn: (g: Guia) => void): void {
    const g = this.guia();
    fn(g);
    this.guia.set({ ...g, proced: [...(g.proced ?? [])] });
  }
}
