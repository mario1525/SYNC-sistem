import { Component, Input, OnInit } from '@angular/core'; // <--- Input corregido aquí
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { PatioService } from '../../services/patio.service';

@Component({
  selector: 'app-patio-form',
  templateUrl: './patio-form.component.html',
  styleUrl: './patio-form.component.less',
})
export class PatioFormComponent implements OnInit {
  PatioForm: FormGroup;
  isEditing = false;

  @Input() bodegaId!: string; // <--- Input correcto
  @Input() PatioId!: string; // <--- Input correcto

  constructor(
    private dialogRef: MatDialogRef<PatioFormComponent>, // 👈 Aquí inyectas
    //@Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder,
    private PatioService: PatioService,
  ) {
    this.PatioForm = this.fb.group({
      id: [''],
      idbodega: [''],
      nombre: ['', Validators.required],
      estado: [true],
      fecha_log: ['2025-03-23T14:30:00Z'],
    });
  }

  ngOnInit(): void {
    if (this.PatioId) {
      this.isEditing = true;
      this.loadComp(); // <--- Importante: cargar los datos al iniciar
    }
  }

  onSubmit(): void {
    if (this.PatioForm.valid) {
      const comp = this.PatioForm.value;
      if (this.isEditing && this.PatioId) {
        this.PatioService.updatePatio(this.PatioId, comp).subscribe({
          next: () => this.closeModal(),
          error: (error) => console.error('Error al actualizar :', error),
        });
      } else {
        this.PatioService.createPatio(comp).subscribe({
          next: () => this.closeModal(),
          error: (error) => console.error('Error al crear compañía:', error),
        });
      }
    }
  }

  get nombre() {
    return this.PatioForm.get('nombre')!;
  }
  loadComp(): void {
    if (this.PatioId) {
      this.PatioService.getPatio(this.PatioId).subscribe({
        next: (response) => {
          const comp = response;
          this.PatioForm.patchValue(comp);
        },
        error: (error) => {
          console.error('Error al cargar:', error);
        },
      });
    } else {
      // Si no hay areaFunId, puedes inicializar el idbodega
      this.PatioForm.patchValue({ idbodega: this.bodegaId });
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }
}
