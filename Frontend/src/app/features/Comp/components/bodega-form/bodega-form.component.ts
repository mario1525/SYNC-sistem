import { Component, Input, OnInit } from '@angular/core'; // <--- Input corregido aquí
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { BodegaService } from '../../services/bodega.service';

@Component({
  selector: 'app-bodega-form',
  templateUrl: './bodega-form.component.html',
  styleUrl: './bodega-form.component.less',
})
export class BodegaFormComponent implements OnInit {
  BodegaForm: FormGroup;
  isEditing = false;

  @Input() plantaId!: string; // <--- Input correcto
  @Input() BodegaId!: string; // <--- Input correcto

  constructor(
    private dialogRef: MatDialogRef<BodegaFormComponent>, // 👈 Aquí inyectas
    //@Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder,
    private BodegaService: BodegaService,
  ) {
    this.BodegaForm = this.fb.group({
      id: [''],
      idPlanta: [''],
      nombre: ['', Validators.required],
      estado: [true],
      fecha_log: ['2025-03-23T14:30:00Z'],
    });
  }

  ngOnInit(): void {
    if (this.BodegaId) {
      this.isEditing = true;
      this.loadComp(); // <--- Importante: cargar los datos al iniciar
    }
  }

  onSubmit(): void {
    if (this.BodegaForm.valid) {
      const comp = this.BodegaForm.value;
      if (this.isEditing && this.BodegaId) {
        this.BodegaService.updateBodega(this.BodegaId, comp).subscribe({
          next: () => this.closeModal(),
          error: (error) => console.error('Error al actualizar :', error),
        });
      } else {
        this.BodegaService.createBodega(comp).subscribe({
          next: () => this.closeModal(),
          error: (error) => console.error('Error al crear compañía:', error),
        });
      }
    }
  }

  get nombre() {
    return this.BodegaForm.get('nombre')!;
  }
  loadComp(): void {
    if (this.BodegaId) {
      this.BodegaService.getBodega(this.BodegaId).subscribe({
        next: (response) => {
          const comp = response;
          this.BodegaForm.patchValue(comp);
        },
        error: (error) => {
          console.error('Error al cargar:', error);
        },
      });
    } else {
      // Si no hay areaFunId, puedes inicializar el idPlanta
      this.BodegaForm.patchValue({ idPlanta: this.plantaId });
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }
}
