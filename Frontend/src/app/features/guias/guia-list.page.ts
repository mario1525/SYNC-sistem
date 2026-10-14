import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Esp, Guia } from '../../core/api/models';
import { EspService, GuiaService } from '../../core/api/services';
import { AuthService } from '../../core/auth/auth.service';
import { NotifyService } from '../../core/ui/notify.service';

@Component({
  selector: 'app-guia-list',
  imports: [FormsModule, RouterLink],
  template: `
    <div class="page-header">
      <h2>Guías de mantenimiento</h2>
      @if (puedeEditar) {
        <div class="acciones">
          <a class="btn btn-primary" routerLink="/guia/nueva"><span class="material-icons">add</span> Nueva guía</a>
        </div>
      }
    </div>

    <div class="filtros">
      <div class="form-group">
        <label for="f-nombre">Buscar</label>
        <input id="f-nombre" [(ngModel)]="nombre" (keyup.enter)="cargar()" placeholder="Nombre de la guía" />
      </div>
      <div class="form-group">
        <label for="f-esp">Especialidad</label>
        <select id="f-esp" [(ngModel)]="idEsp" (ngModelChange)="cargar()">
          <option value="">Todas</option>
          @for (e of especialidades(); track e.id) {
            <option [value]="e.id">{{ e.nombre }}</option>
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
            <th>Especialidad</th>
            <th>Frecuencia</th>
            <th>Duración</th>
            <th>Personal</th>
            <th>Estado</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          @for (g of guias(); track g.id) {
            <tr>
              <td>
                <a [routerLink]="['/guia', g.id]">{{ g.nombre }}</a>
              </td>
              <td>{{ g.espNombre ?? '—' }}</td>
              <td>{{ g.intervalo ? 'Cada ' + g.intervalo + ' días' : '—' }}</td>
              <td>{{ g.duracion }} h</td>
              <td>{{ g.personal }}</td>
              <td>
                <span class="badge" [class.ok]="g.estado" [class.off]="!g.estado">{{
                  g.estado ? 'Activa' : 'Inactiva'
                }}</span>
              </td>
              <td class="acciones">
                @if (puedeEditar) {
                  <button class="btn btn-icon peligro" type="button" title="Eliminar" (click)="eliminar(g)">
                    <span class="material-icons">delete</span>
                  </button>
                }
              </td>
            </tr>
          } @empty {
            <tr>
              <td colspan="7" class="vacio">{{ cargando() ? 'Cargando…' : 'No hay guías registradas.' }}</td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  `,
})
export class GuiaListPage implements OnInit {
  private readonly service = inject(GuiaService);
  private readonly espService = inject(EspService);
  private readonly notify = inject(NotifyService);
  protected readonly puedeEditar = inject(AuthService).tieneRol('Root', 'Admin');

  protected readonly guias = signal<Guia[]>([]);
  protected readonly especialidades = signal<Esp[]>([]);
  protected readonly cargando = signal(false);
  protected nombre = '';
  protected idEsp = '';

  ngOnInit(): void {
    this.espService.list({ Estado: true }).subscribe((e) => this.especialidades.set(e));
    this.cargar();
  }

  cargar(): void {
    this.cargando.set(true);
    this.service.list({ Nombre: this.nombre, IdEsp: this.idEsp }).subscribe({
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

  eliminar(g: Guia): void {
    this.notify.confirmar('Eliminar guía', `¿Eliminar la guía "${g.nombre}"?`).subscribe((ok) => {
      if (!ok) return;
      this.service.delete(g.id!).subscribe({
        next: (m) => {
          this.notify.ok(m.mensaje);
          this.cargar();
        },
        error: (e: unknown) => this.notify.error(e),
      });
    });
  }
}
