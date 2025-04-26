import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
//import { Planta } from '../../../../../Types/planta';
import { PlantaService } from '../../services/planta.service';
import { AreaFuncional } from '../../../../../Types/areafuncional';
import { Bodega } from '../../../../../Types/bodega';

@Component({
  selector: 'app-planta-form',
  templateUrl: './planta-form.component.html',
  styleUrl: './planta-form.component.less',
})
export class PlantaFormComponent implements OnInit {
  plantaForm: FormGroup;
  areasFuncionales: AreaFuncional[] = [];
  bodegas: Bodega[] = [];
  isEditing = false;
  plantId: string | null = null;
  CompId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private plantaService: PlantaService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.plantaForm = this.fb.group({
      id: [''],
      nombre: ['', Validators.required],
      region: ['', Validators.required],
      idComp: [''],
      estado: [true],
      fecha_log: ['2025-03-23T14:30:00Z'],
    });
  }

  ngOnInit(): void {
    this.plantId = this.route.snapshot.paramMap.get('plantaId');
    this.CompId = this.route.snapshot.paramMap.get('companiaId');
    if (this.plantId) {
      this.isEditing = true;
      this.loadPlant();
    }
  }

  get nombre() {
    return this.plantaForm.get('nombre')!;
  }
  get region() {
    return this.plantaForm.get('region')!;
  }

  loadPlant(): void {
    // Este método carga los datos de una compañía existente
    if (this.plantId) {
      // Si existe un ID de compañía, hace una petición al servicio
      this.plantaService.getPlanta(this.plantId).subscribe({
        // Cuando la petición es exitosa
        next: (response) => {
          // Extrae los datos de la compañía de la respuesta
          const plant = response;
          // Actualiza el formulario con los datos obtenidos
          this.plantaForm.patchValue(plant);
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
    if (this.plantaForm.valid) {
      const plant = this.plantaForm.value;
      if (this.isEditing && this.plantId) {
        this.plantaService.updatePlanta(this.plantId, plant).subscribe({
          next: () => this.goBack(),
          error: (error) =>
            console.error('Error al actualizar compañía:', error),
        });
      } else {
        this.plantaService.createPlanta(plant).subscribe({
          next: () => this.goBack(),
          error: (error) => console.error('Error al crear la planta:', error),
        });
      }
    }
  }

  goBack(): void {
    this.router.navigate(['/comp/edit', this.CompId]);
  }

  //
  navigateToNewArea(): void {}
  editArea(id: string): void {}
  deleteArea(id: string): void {}

  navigateToNewBodega(): void {}
  editBodega(id: string): void {}
  manageSecciones(id: string): void {}
  deleteBodega(id: string): void {}
}
