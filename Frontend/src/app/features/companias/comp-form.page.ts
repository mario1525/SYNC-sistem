import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { CrudService } from '../../core/api/crud.service';
import { Comp, Entidad } from '../../core/api/models';
import { CompService, PlantaService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';
import { CampoCatalogo, CatalogoComponent } from '../../shared/catalogo.component';

/**
 * Datos de una compañía y sus plantas.
 * Rutas: /comp/nueva (Root), /comp/:id, /comp/mia (Admin: su propia compañía).
 */
@Component({
  selector: 'app-comp-form',
  imports: [FormsModule, CatalogoComponent],
  template: `
    <div class="page-header">
      <h2>{{ esNueva() ? 'Nueva compañía' : comp().nombre }}</h2>
    </div>

    <form class="card" #f="ngForm" (ngSubmit)="guardar()">
      <h3>Datos generales</h3>
      <div class="form-grid">
        <div class="form-group">
          <label for="nombre">Nombre *</label>
          <input id="nombre" name="nombre" [(ngModel)]="comp().nombre" required />
        </div>
        <div class="form-group">
          <label for="nit">NIT</label>
          <input id="nit" name="nit" [(ngModel)]="comp().nit" />
        </div>
        <div class="form-group">
          <label for="ciudad">Ciudad</label>
          <input id="ciudad" name="ciudad" [(ngModel)]="comp().ciudad" />
        </div>
        <div class="form-group">
          <label for="direccion">Dirección</label>
          <input id="direccion" name="direccion" [(ngModel)]="comp().direccion" />
        </div>
        <div class="form-group">
          <label for="sector">Sector</label>
          <input id="sector" name="sector" [(ngModel)]="comp().sector" />
        </div>
      </div>
      <div class="form-actions">
        @if (auth.tieneRol('Root')) {
          <button class="btn btn-secondary" type="button" (click)="volver()">Volver</button>
        }
        <button class="btn btn-primary" type="submit" [disabled]="f.invalid || guardando()">Guardar</button>
      </div>
    </form>

    @if (!esNueva() && idComp()) {
      <app-catalogo
        titulo="Plantas"
        [servicio]="plantas"
        [campos]="camposPlanta"
        [filtro]="{ IdComp: idComp() }"
        [fijos]="{ idComp: idComp() }"
        [enlace]="enlacePlanta"
      />
      <p class="muted">Abra una planta para configurar sus áreas (producción, bodegas, patios) y secciones.</p>
    }
  `,
})
export class CompFormPage implements OnInit {
  /** Parámetro de ruta: 'nueva', 'mia' o el id. */
  readonly id = input.required<string>();

  protected readonly auth = inject(AuthService);
  private readonly service = inject(CompService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  protected readonly plantas = inject(PlantaService) as unknown as CrudService<Entidad & Record<string, unknown>>;

  protected readonly comp = signal<Comp>({ nombre: '', estado: true });
  protected readonly guardando = signal(false);
  protected readonly esNueva = computed(() => this.id() === 'nueva');
  protected readonly idComp = computed(() =>
    this.id() === 'mia' ? this.auth.idComp() : this.esNueva() ? null : this.id(),
  );

  protected readonly camposPlanta: CampoCatalogo[] = [
    { key: 'nombre', label: 'Nombre', requerido: true },
    { key: 'region', label: 'Región' },
  ];
  protected readonly enlacePlanta = (r: Entidad) => ['/planta', r.id];

  ngOnInit(): void {
    const id = this.idComp();
    if (id)
      this.service.get(id).subscribe({ next: (c) => this.comp.set(c), error: (e: unknown) => this.notify.error(e) });
  }

  guardar(): void {
    this.guardando.set(true);
    const c = this.comp();
    const op = this.esNueva() ? this.service.create(c) : this.service.update(this.idComp()!, c);
    op.subscribe({
      next: (m) => {
        this.guardando.set(false);
        this.notify.ok(m.mensaje);
        if (this.esNueva() && m.id) void this.router.navigate(['/comp', m.id]);
      },
      error: (e: unknown) => {
        this.guardando.set(false);
        this.notify.error(e);
      },
    });
  }

  volver(): void {
    void this.router.navigate(['/comp']);
  }
}
