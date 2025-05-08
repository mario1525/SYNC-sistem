import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GuiaService } from '../../services/guia.service';
import {
  FormBuilder,
  FormGroup,
  FormArray,
  Validators,
  //FormControl,
} from '@angular/forms';
import { AuthService } from '../../../auth/Services/auth.service';

@Component({
  selector: 'app-guia-form',
  templateUrl: './guia-form.component.html',
  styleUrl: './guia-form.component.less',
})
export class GuiaFormComponent implements OnInit {
  guiaForm: FormGroup;
  guiaId: string | null = null;
  role: string | null = null;
  isEditing = false;

  constructor(
    private fb: FormBuilder,
    private guiaService: GuiaService,
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.guiaForm = this.fb.group({
      id: [''],
      nombre: ['', Validators.required],
      descripcion: [''],
      proceso: [''],
      inspeccion: [''],
      herramientas: [''],
      idComp: [''],
      idEsp: [''],
      seguridadInd: [''],
      seguridadAmb: [''],
      intervalo: [0],
      importante: [''],
      insumos: [''],
      personal: [0],
      duracion: [0],
      logistica: [''],
      situacion: [''],
      notas: [''],
      createdBy: [''],
      updatedBy: [''],
      fechaUpdate: [''],
      estado: [true],
      fecha_log: ['2025-03-23T14:30:00Z'],
      proced: this.fb.array([] as FormGroup[]),
    });
  }

  ngOnInit(): void {
    this.role = this.authService.getUserRole();
    this.guiaId = this.route.snapshot.paramMap.get('id');
    if (this.guiaId) {
      this.isEditing = true;
      this.loadGuia();
    }
  }

  get proced(): FormArray<FormGroup> {
    return this.guiaForm.get('proced') as FormArray<FormGroup>;
  }

  private createValidGroup(v: any = {}): FormGroup {
    return this.fb.group({
      id: [v.id || ''],
      idProced: [v.idProced || ''],
      nombre: [v.nombre || '', Validators.required],
      descripcion: [v.descripcion || ''],
      estado: [v.estado ?? true],
      fecha_log: [v.fecha_log || ''],
    });
  }

  private createProcedGroup(p: any = {}): FormGroup {
    const validArray = this.fb.array([] as FormGroup[]);

    if (Array.isArray(p.valid)) {
      p.valid.forEach((v: any) => validArray.push(this.createValidGroup(v)));
    }

    return this.fb.group({
      id: [p.id || ''],
      idGuia: [p.idGuia || ''],
      nombre: [p.nombre || '', Validators.required],
      descripcion: [p.descripcion || ''],
      estado: [p.estado ?? true],
      fecha_log: [p.fecha_log || ''],
      valid: validArray,
    });
  }

  addProced(): void {
    const procedGroup = this.fb.group({
      id: [''],
      idGuia: [''],
      nombre: ['', Validators.required],
      descripcion: [''],
      estado: [true],
      fecha_log: [''],
      valid: this.fb.array([]),
    });
    this.proced.push(procedGroup);
  }

  removeProced(index: number): void {
    this.proced.removeAt(index);
  }

  getValidControls(procedIndex: number): FormArray<FormGroup> {
    return this.proced.at(procedIndex).get('valid') as FormArray<FormGroup>;
  }

  addValid(procedIndex: number): void {
    const validGroup = this.fb.group({
      id: [''],
      idProced: [''],
      nombre: ['', Validators.required],
      descripcion: [''],
      estado: [true],
      fecha_log: [''],
    });

    this.getValidControls(procedIndex).push(validGroup);
  }

  removeValid(procedIndex: number, validIndex: number): void {
    this.getValidControls(procedIndex).removeAt(validIndex);
  }

  onSubmit(): void {
    if (this.guiaForm.valid) {
      const formData = this.guiaForm.value;

      // Asigna el usuario que actualiza o crea
      const currentUser = this.authService.getUserId(); // Asume que hay un método así

      if (this.isEditing) {
        formData.updatedBy = currentUser;
        formData.fechaUpdate = new Date().toISOString();

        this.guiaService.updateGuia(this.guiaId!, formData).subscribe({
          next: () => {
            console.log('Guía actualizada correctamente');
            this.goBack();
          },
          error: (error) => {
            console.error('Error al actualizar la guía:', error);
          },
        });
      } else {
        formData.createdBy = currentUser;
        formData.fecha_log = new Date().toISOString();

        this.guiaService.createGuia(formData).subscribe({
          next: () => {
            console.log('Guía creada correctamente');
            this.goBack();
          },
          error: (error) => {
            console.error('Error al crear la guía:', error);
          },
        });
      }
    } else {
      this.guiaForm.markAllAsTouched();
      console.warn('Formulario inválido. Verifica los campos requeridos.');
    }
  }

  loadGuia(): void {
    if (this.guiaId) {
      this.guiaService.getGuia(this.guiaId).subscribe({
        next: (response) => {
          const comp = response;
          this.guiaForm.patchValue(comp);
          this.proced.clear();

          comp.proced.forEach((p: any) => {
            this.proced.push(this.createProcedGroup(p));
          });
        },
        error: (error) => {
          console.error('Error al cargar la guía:', error);
          this.goBack();
        },
      });
    }
  }

  goBack(): void {
    this.router.navigate(['/guia']);
  }
}
