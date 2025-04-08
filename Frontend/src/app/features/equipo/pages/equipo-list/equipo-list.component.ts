import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { EquipoService } from '../../services/equipo.service';

@Component({
  selector: 'app-equipo-list',
  template: `
    <div class="container">
      <div class="header">
        <h2>Equipos</h2>
        <button class="btn btn-primary" (click)="navigateToNew()">
          <i class="fas fa-plus"></i> Nuevo Equipo
        </button>
      </div>

      <div class="table-responsive">
        <table class="table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Nombre</th>
              <th>Descripción</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr *ngFor="let equipo of equipos">
              <td>{{equipo.id}}</td>
              <td>{{equipo.nombre}}</td>
              <td>{{equipo.descripcion}}</td>
              <td>
                <span [class]="'badge ' + (equipo.estado ? 'bg-success' : 'bg-danger')">
                  {{equipo.estado ? 'Activo' : 'Inactivo'}}
                </span>
              </td>
              <td>
                <button class="btn btn-sm btn-info me-2" (click)="editEquipo(equipo.id)">
                  <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-sm btn-danger" (click)="deleteEquipo(equipo.id)">
                  <i class="fas fa-trash"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: [`
    .container {
      padding: 20px;
    }
    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 20px;
    }
    .table {
      width: 100%;
      margin-bottom: 1rem;
      background-color: transparent;
    }
    .table th,
    .table td {
      padding: 0.75rem;
      vertical-align: top;
      border-top: 1px solid #dee2e6;
    }
    .btn {
      padding: 0.375rem 0.75rem;
      border-radius: 0.25rem;
      cursor: pointer;
    }
    .btn-primary {
      background-color: #007bff;
      color: white;
      border: none;
    }
    .btn-info {
      background-color: #17a2b8;
      color: white;
      border: none;
    }
    .btn-danger {
      background-color: #dc3545;
      color: white;
      border: none;
    }
    .badge {
      padding: 0.25em 0.4em;
      border-radius: 0.25rem;
      font-size: 75%;
    }
    .bg-success {
      background-color: #28a745;
      color: white;
    }
    .bg-danger {
      background-color: #dc3545;
      color: white;
    }
  `]
})
export class EquipoListComponent implements OnInit {
  equipos: any[] = [];

  constructor(
    private equipoService: EquipoService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadEquipos();
  }

  loadEquipos(): void {
    this.equipoService.getEquipos().subscribe({
      next: (data) => {
        this.equipos = data;
      },
      error: (error) => {
        console.error('Error al cargar equipos:', error);
      }
    });
  }

  navigateToNew(): void {
    this.router.navigate(['/equipo/nuevo']);
  }

  editEquipo(id: number): void {
    this.router.navigate(['/equipo/editar', id]);
  }

  deleteEquipo(id: number): void {
    if (confirm('¿Está seguro de que desea eliminar este equipo?')) {
      this.equipoService.deleteEquipo(id).subscribe({
        next: () => {
          this.loadEquipos();
        },
        error: (error) => {
          console.error('Error al eliminar equipo:', error);
        }
      });
    }
  }
} 