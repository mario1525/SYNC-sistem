import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CompService } from '../../services/comp.service';

@Component({
  selector: 'app-comp-form',
  templateUrl: './comp-form.component.html',
  styleUrls: ['./comp-form.component.less'],
})
export class CompFormComponent implements OnInit {
  compForm: FormGroup;
  isEditing = false;
  compId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private compService: CompService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.compForm = this.fb.group({
      nombre: ['', Validators.required],
      ciudad: ['', Validators.required],
      nit: ['', Validators.required],
      direccion: ['', Validators.required],
      sector: ['', Validators.required],
      estado: [true],
    });
  }

  ngOnInit(): void {
    this.compId = this.route.snapshot.paramMap.get('id');
    if (this.compId) {
      this.isEditing = true;
      this.loadComp();
    }
  }

  get nombre() {
    return this.compForm.get('nombre')!;
  }
  get sector() {
    return this.compForm.get('sector');
  }
  get ciudad() {
    return this.compForm.get('ciudad');
  }
  get nit() {
    return this.compForm.get('nit');
  }
  get direccion() {
    return this.compForm.get('direccion');
  }

  loadComp(): void {
    // Este método carga los datos de una compañía existente
    if (this.compId) {
      // Si existe un ID de compañía, hace una petición al servicio
      this.compService.getComp(this.compId).subscribe({
        // Cuando la petición es exitosa
        next: (response) => {
          // Extrae los datos de la compañía de la respuesta
          const comp = response;
          // Actualiza el formulario con los datos obtenidos
          this.compForm.patchValue(comp);
        },
        // Si ocurre un error en la petición
        error: (error) => {
          // Muestra el error en consola
          console.error('Error al cargar compañía:', error);
          // Regresa a la página anterior
          this.goBack();
        },
      });
    }
  }

  onSubmit(): void {
    if (this.compForm.valid) {
      const comp = this.compForm.value;
      if (this.isEditing && this.compId) {
        this.compService.updateComp(this.compId, comp).subscribe({
          next: () => this.goBack(),
          error: (error) =>
            console.error('Error al actualizar compañía:', error),
        });
      } else {
        this.compService.createComp(comp).subscribe({
          next: () => this.goBack(),
          error: (error) => console.error('Error al crear compañía:', error),
        });
      }
    }
  }

  goBack(): void {
    this.router.navigate(['/comp']);
  }
}
