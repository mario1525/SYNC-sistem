import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Area, Equipo, Planta } from '../../core/api/models';
import { AreaService, EquipoService, PlantaService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

@Component({
  selector: 'app-equipo-list',
  imports: [FormsModule, RouterLink],
  template: `
    <div class="page-header">
      <h2>Equipos</h2>
      @if (puedeEditar) {
        <div class="acciones">
          <a class="btn btn-primary" routerLink="/equipo/nuevo"><span class="material-icons">add</span> Nuevo equipo</a>
        </div>
      }
    </div>

    <div class="filtros">
      <div class="form-group">
        <label for="f-nombre">Buscar</label>
        <input id="f-nombre" [(ngModel)]="nombre" (keyup.enter)="cargar()" placeholder="Nombre del equipo" />
      </div>
      <div class="form-group">
        <label for="f-planta">Planta</label>
        <select id="f-planta" [(ngModel)]="idPlanta" (ngModelChange)="cambiarPlanta()">
          <option value="">Todas</option>
          @for (p of plantas(); track p.id) {
            <option [value]="p.id">{{ p.nombre }}</option>
          }
        </select>
      </div>
      <div class="form-group">
        <label for="f-area">Área</label>
        <select id="f-area" [(ngModel)]="idArea" (ngModelChange)="cargar()" [disabled]="!idPlanta">
          <option value="">Todas</option>
          @for (a of areas(); track a.id) {
            <option [value]="a.id">{{ a.nombre }} ({{ a.tipoAreaNombre }})</option>
          }
        </select>
      </div>
      <button class="btn btn-info" type="button" (click)="cargar()">
        <span class="material-icons">search</span> Buscar
      </button>
    </div>

    <div class="table-wrap">
      <table class="table">
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Marca / Modelo</th>
            <th>N.º serie</th>
            <th>Ubicación</th>
            <th>Estado</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          @for (e of equipos(); track e.id) {
            <tr>
              <td>
                <a [routerLink]="['/equipo', e.id]">{{ e.nombre }}</a>
              </td>
              <td>{{ e.marca }} {{ e.modelo }}</td>
              <td>{{ e.nSerie }}</td>
              <td>
                @let u = e.ubicacion;
                @if (u && u.idSeccionArea) {
                  <span
                    class="badge"
                    [class.ok]="u.tipoUbicacion === 'Produccion'"
                    [class.info]="u.tipoUbicacion === 'Almacen'"
                  >
                    {{ u.tipoUbicacion === 'Produccion' ? 'Producción' : 'Almacén' }}
                  </span>
                  {{ u.plantaNombre }} › {{ u.areaNombre }} › {{ u.seccionNombre }}
                } @else {
                  <span class="badge warn">Sin ubicación</span>
                }
              </td>
              <td>
                <span class="badge" [class.ok]="e.estado" [class.off]="!e.estado">{{
                  e.estado ? 'Activo' : 'Inactivo'
                }}</span>
              </td>
              <td class="acciones">
                @if (puedeEditar) {
                  <button class="btn btn-icon peligro" type="button" title="Eliminar" (click)="eliminar(e)">
                    <span class="material-icons">delete</span>
                  </button>
                }
              </td>
            </tr>
          } @empty {
            <tr>
              <td colspan="6" class="vacio">{{ cargando() ? 'Cargando…' : 'No hay equipos con estos filtros.' }}</td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  `,
})
export class EquipoListPage implements OnInit {
  private readonly service = inject(EquipoService);
  private readonly plantaService = inject(PlantaService);
  private readonly areaService = inject(AreaService);
  private readonly notify = inject(NotifyService);
  protected readonly puedeEditar = inject(AuthService).tieneRol('Root', 'Admin');

  protected readonly equipos = signal<Equipo[]>([]);
  protected readonly plantas = signal<Planta[]>([]);
  protected readonly areas = signal<Area[]>([]);
  protected readonly cargando = signal(false);
  protected nombre = '';
  protected idPlanta = '';
  protected idArea = '';

  ngOnInit(): void {
    this.plantaService.list({ Estado: true }).subscribe((p) => this.plantas.set(p));
    this.cargar();
  }

  cambiarPlanta(): void {
    this.idArea = '';
    this.areas.set([]);
    if (this.idPlanta) this.areaService.list({ IdPlanta: this.idPlanta }).subscribe((a) => this.areas.set(a));
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.service.list({ Nombre: this.nombre, IdPlanta: this.idPlanta, IdArea: this.idArea }).subscribe({
      next: (r) => {
        this.equipos.set(r);
        this.cargando.set(false);
      },
      error: (e: unknown) => {
        this.cargando.set(false);
        this.notify.error(e);
      },
    });
  }

  eliminar(e: Equipo): void {
    this.notify.confirmar('Eliminar equipo', `¿Eliminar el equipo "${e.nombre}"?`).subscribe((ok) => {
      if (!ok) return;
      this.service.delete(e.id!).subscribe({
        next: (m) => {
          this.notify.ok(m.mensaje);
          this.cargar();
        },
        error: (err: unknown) => this.notify.error(err),
      });
    });
  }
}
