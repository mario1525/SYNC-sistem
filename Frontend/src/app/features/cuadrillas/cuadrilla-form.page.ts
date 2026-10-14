import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Cuad, DIAS_SEMANA, Horario, Turno, Usuario } from '../../core/api/models';
import { CuadService, HorarioService, TurnoService, UsuarioService } from '../../core/api/services';
import { NotifyService } from '../../core/ui/notify.service';

/** Detalle de cuadrilla: integrantes y horario semanal (turno x día). */
@Component({
  selector: 'app-cuadrilla-form',
  imports: [RouterLink],
  template: `
    <div class="page-header">
      <div>
        <p class="subtitulo">Cuadrilla</p>
        <h2>{{ cuadrilla()?.nombre ?? '…' }}</h2>
      </div>
      <div class="acciones"><a class="btn btn-secondary" routerLink="/cuadrilla">Volver</a></div>
    </div>

    <div class="card">
      <h3>Horario semanal</h3>
      @if (turnos().length === 0) {
        <p class="muted">No hay turnos definidos. Créelos en <a routerLink="/catalogos">Catálogos › Turnos</a>.</p>
      } @else {
        <p class="muted">Marque los turnos que trabaja la cuadrilla cada día.</p>
        <div class="table-wrap">
          <table class="table horario">
            <thead>
              <tr>
                <th>Turno</th>
                @for (d of dias; track $index) {
                  <th>{{ d }}</th>
                }
              </tr>
            </thead>
            <tbody>
              @for (t of turnos(); track t.id) {
                <tr>
                  <td>
                    {{ t.nombre }} <span class="muted">{{ t.horaInicio }}–{{ t.horaFin }}</span>
                  </td>
                  @for (d of dias; track $index; let i = $index) {
                    <td class="celda">
                      <input
                        type="checkbox"
                        [checked]="!!asignado()[t.id + '|' + (i + 1)]"
                        [disabled]="ocupado()"
                        (change)="alternar(t, i + 1)"
                        [attr.aria-label]="t.nombre + ' ' + d"
                      />
                    </td>
                  }
                </tr>
              }
            </tbody>
          </table>
        </div>
      }
    </div>

    <div class="card">
      <h3>Integrantes ({{ integrantes().length }})</h3>
      <div class="table-wrap">
        <table class="table">
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Cargo</th>
              <th>Rol</th>
              <th>Especialidad</th>
            </tr>
          </thead>
          <tbody>
            @for (u of integrantes(); track u.id) {
              <tr>
                <td>{{ u.nombre }} {{ u.apellido }}</td>
                <td>{{ u.cargo }}</td>
                <td>{{ u.rol }}</td>
                <td>{{ u.espNombre ?? '—' }}</td>
              </tr>
            } @empty {
              <tr>
                <td colspan="4" class="vacio">Sin integrantes. Asigne la cuadrilla desde la ficha de cada usuario.</td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: `
    .horario td.celda {
      text-align: center;
    }
    .horario input {
      width: 18px;
      height: 18px;
      cursor: pointer;
    }
  `,
})
export class CuadrillaFormPage implements OnInit {
  readonly id = input.required<string>();

  private readonly cuadService = inject(CuadService);
  private readonly turnoService = inject(TurnoService);
  private readonly horarioService = inject(HorarioService);
  private readonly usuarioService = inject(UsuarioService);
  private readonly notify = inject(NotifyService);

  protected readonly dias = DIAS_SEMANA;
  protected readonly cuadrilla = signal<Cuad | null>(null);
  protected readonly turnos = signal<Turno[]>([]);
  protected readonly horarios = signal<Horario[]>([]);
  protected readonly integrantes = signal<Usuario[]>([]);
  protected readonly ocupado = signal(false);

  /** Mapa "idTurno|dia" → horario, para pintar la grilla. */
  protected readonly asignado = computed(() => {
    const m: Record<string, Horario> = {};
    for (const h of this.horarios()) m[`${h.idTurno}|${h.diaSemana}`] = h;
    return m;
  });

  ngOnInit(): void {
    forkJoin({
      cuad: this.cuadService.get(this.id()),
      turnos: this.turnoService.list({ Estado: true }),
      integrantes: this.usuarioService.list({ IdCuad: this.id() }),
    }).subscribe({
      next: (r) => {
        this.cuadrilla.set(r.cuad);
        this.turnos.set(r.turnos);
        this.integrantes.set(r.integrantes);
      },
      error: (e: unknown) => this.notify.error(e),
    });
    this.cargarHorarios();
  }

  alternar(t: Turno, dia: number): void {
    const actual = this.asignado()[`${t.id}|${dia}`];
    this.ocupado.set(true);
    const op = actual
      ? this.horarioService.delete(actual.id!)
      : this.horarioService.create({
          idTurno: t.id!,
          idCuad: this.id(),
          diaSemana: dia,
          esFestivo: false,
          estado: true,
        });
    op.subscribe({
      next: () => this.cargarHorarios(),
      error: (e: unknown) => {
        this.ocupado.set(false);
        this.notify.error(e);
        this.cargarHorarios();
      },
    });
  }

  private cargarHorarios(): void {
    this.horarioService.list({ IdCuad: this.id() }).subscribe((h) => {
      this.horarios.set(h);
      this.ocupado.set(false);
    });
  }
}
