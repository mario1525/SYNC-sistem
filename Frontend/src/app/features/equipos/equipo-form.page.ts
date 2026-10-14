import { DatePipe } from '@angular/common';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import {
  Area,
  Equipo,
  Guia,
  GuiaEquipo,
  Planta,
  SeccionArea,
  TipoUbicacion,
  UbicacionEquipo,
} from '../../core/api/models';
import {
  AreaService,
  EquipoService,
  GuiaEquipoService,
  GuiaService,
  PlantaService,
  SeccionAreaService,
} from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

const NUEVO: Equipo = {
  nombre: '',
  estado: true,
  peso: 0,
  cilindraje: 0,
  potencia: 0,
  ancho: 0,
  alto: 0,
  largo: 0,
  capacidad: 0,
  anioFabricacion: new Date().getFullYear(),
};

/** Ficha del equipo: datos técnicos, ubicación (planta → área → sección), guías e historial. */
@Component({
  selector: 'app-equipo-form',
  imports: [FormsModule, RouterLink, DatePipe],
  templateUrl: './equipo-form.page.html',
})
export class EquipoFormPage implements OnInit {
  /** 'nuevo' o el id del equipo. */
  readonly id = input.required<string>();

  private readonly service = inject(EquipoService);
  private readonly plantaService = inject(PlantaService);
  private readonly areaService = inject(AreaService);
  private readonly seccionService = inject(SeccionAreaService);
  private readonly guiaService = inject(GuiaService);
  private readonly guiaEquipoService = inject(GuiaEquipoService);
  private readonly notify = inject(NotifyService);
  private readonly router = inject(Router);
  protected readonly puedeEditar = inject(AuthService).tieneRol('Root', 'Admin');

  protected readonly equipo = signal<Equipo>({ ...NUEVO });
  protected readonly esNuevo = computed(() => this.id() === 'nuevo');
  protected readonly guardando = signal(false);

  // Ubicación en cascada
  protected readonly plantas = signal<Planta[]>([]);
  protected readonly areas = signal<Area[]>([]);
  protected readonly secciones = signal<SeccionArea[]>([]);
  protected tipoUbicacion: TipoUbicacion = 'Produccion';
  protected idPlanta = '';
  protected idArea = '';
  protected idSeccion = '';

  // Guías asignadas e historial
  protected readonly guias = signal<Guia[]>([]);
  protected readonly asignadas = signal<GuiaEquipo[]>([]);
  protected readonly historial = signal<UbicacionEquipo[]>([]);
  protected guiaNueva = '';
  protected readonly guiasDisponibles = computed(() => {
    const usadas = new Set(this.asignadas().map((a) => a.idGuia));
    return this.guias().filter((g) => !usadas.has(g.id!));
  });

  ngOnInit(): void {
    this.plantaService.list({ Estado: true }).subscribe((p) => this.plantas.set(p));
    if (this.esNuevo()) return;

    this.service.get(this.id()).subscribe({
      next: (e) => {
        this.equipo.set(e);
        const u = e.ubicacion;
        if (u?.idSeccionArea) {
          this.tipoUbicacion = u.tipoUbicacion;
          this.idPlanta = u.idPlanta ?? '';
          this.idArea = u.idArea ?? '';
          this.idSeccion = u.idSeccionArea;
          this.cargarAreas(false);
          this.cargarSecciones(false);
        }
      },
      error: (err: unknown) => {
        this.notify.error(err);
        this.volver();
      },
    });
    this.guiaService.list({ Estado: true }).subscribe((g) => this.guias.set(g));
    this.cargarAsignadas();
    this.service.ubicaciones(this.id()).subscribe((h) => this.historial.set(h));
  }

  cargarAreas(limpiar = true): void {
    if (limpiar) {
      this.idArea = '';
      this.idSeccion = '';
      this.secciones.set([]);
    }
    this.areas.set([]);
    if (this.idPlanta)
      this.areaService.list({ IdPlanta: this.idPlanta, Estado: true }).subscribe((a) => this.areas.set(a));
  }

  cargarSecciones(limpiar = true): void {
    if (limpiar) this.idSeccion = '';
    this.secciones.set([]);
    if (this.idArea)
      this.seccionService.list({ IdArea: this.idArea, Estado: true }).subscribe((s) => this.secciones.set(s));
  }

  guardar(): void {
    const e: Equipo = { ...this.equipo() };
    e.ubicacion = this.idSeccion ? { tipoUbicacion: this.tipoUbicacion, idSeccionArea: this.idSeccion } : null;
    this.guardando.set(true);
    const op = this.esNuevo() ? this.service.create(e) : this.service.update(this.id(), e);
    op.subscribe({
      next: (m) => {
        this.guardando.set(false);
        this.notify.ok(m.mensaje);
        if (this.esNuevo() && m.id) void this.router.navigate(['/equipo', m.id]);
        else this.service.ubicaciones(this.id()).subscribe((h) => this.historial.set(h));
      },
      error: (err: unknown) => {
        this.guardando.set(false);
        this.notify.error(err);
      },
    });
  }

  asignarGuia(): void {
    if (!this.guiaNueva) return;
    this.guiaEquipoService.create({ idGuia: this.guiaNueva, idEquipo: this.id(), estado: true }).subscribe({
      next: () => {
        this.guiaNueva = '';
        this.cargarAsignadas();
      },
      error: (err: unknown) => this.notify.error(err),
    });
  }

  quitarGuia(a: GuiaEquipo): void {
    this.guiaEquipoService.delete(a.id!).subscribe({
      next: () => this.cargarAsignadas(),
      error: (err: unknown) => this.notify.error(err),
    });
  }

  volver(): void {
    void this.router.navigate(['/equipo']);
  }

  private cargarAsignadas(): void {
    this.guiaEquipoService.list({ IdEquipo: this.id() }).subscribe((a) => this.asignadas.set(a));
  }
}
