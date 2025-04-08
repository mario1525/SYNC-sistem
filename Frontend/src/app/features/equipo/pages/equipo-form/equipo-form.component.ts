import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EquipoService } from '../../services/equipo.service';

@Component({
  selector: 'app-equipo-form',
  template: `
    <div class="container">
      <div class="header">
        <h2>{{isEditing ? 'Editar' : 'Nuevo'}} Equipo</h2>
      </div>

      <form [formGroup]="equipoForm" (ngSubmit)="onSubmit()" class="form">
        <div class="form-group">
          <label for="nombre">Nombre</label>
          <input 
            type="text" 
            id="nombre" 
            formControlName="nombre" 
            class="form-control"
            [class.is-invalid]="nombre.invalid && nombre.touched"
          >
          <div class="invalid-feedback" *ngIf="nombre.invalid && nombre.touched">
            El nombre es requerido
          </div>
        </div>

        <div class="form-group">
          <label for="descripcion">Descripción</label>
          <textarea 
            id="descripcion" 
            formControlName="descripcion" 
            class="form-control"
            rows="3"
          ></textarea>
        </div>

        <div class="form-group">
          <label class="checkbox-label">
            <input type="checkbox" formControlName="estado">
            Activo
          </label>
        </div>

        <div class="form-actions">
          <button type="button" class="btn btn-secondary" (click)="goBack()">
            Cancelar
          </button>
          <button type="submit" class="btn btn-primary" [disabled]="equipoForm.invalid">
            {{isEditing ? 'Actualizar' : 'Crear'}}
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .container {
      padding: 20px;
      max-width: 600px;
      margin: 0 auto;
    }
    .header {
      margin-bottom: 20px;
    }
    .form {
      background: white;
      padding: 20px;
      border-radius: 8px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    .form-group {
      margin-bottom: 1rem;
    }
    .form-control {
      width: 100%;
      padding: 0.375rem 0.75rem;
      border: 1px solid #ced4da;
      border-radius: 0.25rem;
    }
    .is-invalid {
      border-color: #dc3545;
    }
    .invalid-feedback {
      color: #dc3545;
      font-size: 80%;
      margin-top: 0.25rem;
    }
    .checkbox-label {
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
    .form-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      margin-top: 1rem;
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
    .btn-secondary {
      background-color: #6c757d;
      color: white;
      border: none;
    }
    .btn:disabled {
      opacity: 0.65;
      cursor: not-allowed;
    }
  `]
})
export class EquipoFormComponent implements OnInit {
  equipoForm: FormGroup;
  isEditing = false;
  equipoId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private equipoService: EquipoService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.equipoForm = this.fb.group({
      nombre: ['', Validators.required],
      descripcion: [''],
      estado: [true]
    });
  }

  ngOnInit(): void {
    this.equipoId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.equipoId) {
      this.isEditing = true;
      this.loadEquipo();
    }
  }

  get nombre() {
    return this.equipoForm.get('nombre')!;
  }

  loadEquipo(): void {
    if (this.equipoId) {
      this.equipoService.getEquipo(this.equipoId).subscribe({
        next: (equipo) => {
          this.equipoForm.patchValue(equipo);
        },
        error: (error) => {
          console.error('Error al cargar equipo:', error);
          this.goBack();
        }
      });
    }
  }

  onSubmit(): void {
    if (this.equipoForm.valid) {
      const equipo = this.equipoForm.value;
      if (this.isEditing && this.equipoId) {
        this.equipoService.updateEquipo(this.equipoId, equipo).subscribe({
          next: () => this.goBack(),
          error: (error) => console.error('Error al actualizar equipo:', error)
        });
      } else {
        this.equipoService.createEquipo(equipo).subscribe({
          next: () => this.goBack(),
          error: (error) => console.error('Error al crear equipo:', error)
        });
      }
    }
  }

  goBack(): void {
    this.router.navigate(['/equipo']);
  }
} 