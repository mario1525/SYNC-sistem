import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CompService } from '../../services/comp.service';
import { EspService } from '../../services/esp.service';
import { PlantaService } from '../../services/planta.service';
import { Esp } from '../../../../../Types/esp';
import { Planta } from '../../../../../Types/planta';

@Component({
  selector: 'app-comp-form',
  templateUrl: './comp-form.component.html',
  styleUrls: ['./comp-form.component.less'],
})
export class CompFormComponent implements OnInit {
  compForm: FormGroup;
  espForm: FormGroup;
  //plantForm: FormGroup;
  Planta: Planta[] = [];
  Esps: Esp[] = [];
  isEditing = false;
  compId: string | null = null;

  constructor(
    private fb: FormBuilder,
    private compService: CompService,
    private espService: EspService,
    private plantaService: PlantaService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.compForm = this.fb.group({
      id: [''],
      nombre: ['', Validators.required],
      ciudad: ['', Validators.required],
      nit: ['', Validators.required],
      direccion: ['', Validators.required],
      sector: ['', Validators.required],
      estado: [true],
      fecha_log: ['2025-03-23T14:30:00Z'],
    });

    this.espForm = this.fb.group({
      id: [''],
      nombre: ['', Validators.required],
      idComp: [''],
      estado: [true],
      fecha_log: ['2025-03-23T14:30:00Z'],
    });
  }

  ngOnInit(): void {
    this.compId = this.route.snapshot.paramMap.get('id');
    if (this.compId) {
      this.isEditing = true;
      this.loadComp();
      this.loadEsp();
      this.loadPlantas(this.compId);
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

  loadEsp(): void {
    // Este método carga los datos de las especialidades existentes de una compania
    if (this.compId) {
      // Si existe un ID de compañía, hace una petición al servicio
      this.espService.getesps(this.compId).subscribe({
        // Cuando la petición es exitosa
        next: (response) => {
          // Extrae los datos de la compañía de la respuesta
          const esp = response;
          this.Esps = esp;
          //console.log(esp);
          // Actualiza el formulario con los datos obtenidos
          //this.compForm.patchValue(esp);
        },
        // Si ocurre un error en la petición
        error: (error) => {
          // Muestra el error en consola
          console.error('Error al cargar las especialidades:', error);
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

  loadPlantas(idComp: string): void {
    this.plantaService.getPlantas(idComp).subscribe({
      next: (response) => {
        this.Planta = response as Planta[];
      },
      error: (error) => {
        console.error('Error al cargar compañías:', error);
      },
    });
  }

  navigateToNew(): void {
    this.router.navigate([`planta/new`], { relativeTo: this.route });
  }

  editPlanta(id: string): void {
    this.router.navigate(['planta', 'edit', id], { relativeTo: this.route });
  }

  deletePlanta(id: string): void {
    if (confirm('¿Está seguro de que desea eliminar esta planta?')) {
      this.plantaService.deletePlanta(id).subscribe({
        next: () => {
          //this.loadPlantas(this.compId);
        },
        error: (error) => {
          console.error('Error al eliminar la panta:', error);
        },
      });
    }
  }
}
