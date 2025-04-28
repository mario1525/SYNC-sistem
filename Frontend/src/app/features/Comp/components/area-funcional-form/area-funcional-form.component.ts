import { Component, Input, OnInit } from '@angular/core'; // <--- Input corregido aquí
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { AreaFuncionalService } from '../../services/areaFuncional.service';

@Component({
  selector: 'app-area-funcional-form',
  templateUrl: './area-funcional-form.component.html',
  styleUrl: './area-funcional-form.component.less',
})
export class AreaFuncionalFormComponent implements OnInit {
  areaFunForm: FormGroup;
  isEditing = false;

  @Input() plantaId!: string; // <--- Input correcto
  @Input() areafunId!: string; // <--- Input correcto

  constructor(
    private dialogRef: MatDialogRef<AreaFuncionalFormComponent>, // 👈 Aquí inyectas
    //@Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder,
    private areaFuncionalService: AreaFuncionalService,
  ) {
    this.areaFunForm = this.fb.group({
      id: [''],
      idPlanta: [''],
      nombre: ['', Validators.required],
      estado: [true],
      fecha_log: ['2025-03-23T14:30:00Z'],
    });
  }

  ngOnInit(): void {
    if (this.areafunId) {
      this.isEditing = true;
      this.loadComp(); // <--- Importante: cargar los datos al iniciar
    }
  }

  onSubmit(): void {
    if (this.areaFunForm.valid) {
      const comp = this.areaFunForm.value;
      if (this.isEditing && this.areafunId) {
        this.areaFuncionalService
          .updateAreaFuncional(this.areafunId, comp)
          .subscribe({
            next: () => this.closeModal(),
            error: (error) => console.error('Error al actualizar :', error),
          });
      } else {
        this.areaFuncionalService.createAreaFuncional(comp).subscribe({
          next: () => this.closeModal(),
          error: (error) => console.error('Error al crear compañía:', error),
        });
      }
    }
  }

  get nombre() {
    return this.areaFunForm.get('nombre')!;
  }
  loadComp(): void {
    if (this.areafunId) {
      this.areaFuncionalService.getAreaFuncional(this.areafunId).subscribe({
        next: (response) => {
          const comp = response;
          this.areaFunForm.patchValue(comp);
        },
        error: (error) => {
          console.error('Error al cargar:', error);
        },
      });
    } else {
      // Si no hay areaFunId, puedes inicializar el idPlanta
      this.areaFunForm.patchValue({ idPlanta: this.plantaId });
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }
}
