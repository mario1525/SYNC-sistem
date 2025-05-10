import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EquipoService } from '../../services/equipo.service';
import { AuthService } from '../../../auth/Services/auth.service';

import { Planta } from '../../../../../Types/planta';
import { PlantaService } from '../../../Comp/services/planta.service';

import { Bodega } from '../../../../../Types/bodega';
import { BodegaService } from '../../../Comp/services/bodega.service';

import { AreaFuncional } from '../../../../../Types/areafuncional';
import { AreaFuncionalService } from '../../../Comp/services/areaFuncional.service';

import { Patio } from '../../../../../Types/patio';
import { PatioService } from '../../../Comp/services/patio.service';

import { SeccionBodega } from '../../../../../Types/seccionbodega';
import { seccionBodegaService } from '../../../Comp/services/seccionBodega.service';

@Component({
  selector: 'app-equipo-form',
  templateUrl: './equipo-form.component.html',
  styleUrls: ['./equipo-form.component.less'],
})
export class EquipoFormComponent implements OnInit {
  equipoForm: FormGroup;
  isEditing = false;
  equipoId: string | null = null;
  compId: string | null = null;
  role: string | null = null;
  Planta: Planta[] = [];
  Bodega: Bodega[] = [];
  AreaFuncional: AreaFuncional[] = [];
  Patio: Patio[] = [];
  SeccionBodega: SeccionBodega[] = [];
  tipoUbicacionOptions = ['Produccion', 'Bodega'];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private equipoService: EquipoService,
    private plantaService: PlantaService,
    private seccionBodegaService: seccionBodegaService,
    private areaFuncionalService: AreaFuncionalService,
    private patioService: PatioService,
    private bodegaService: BodegaService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.equipoForm = this.fb.group({
      id: [''],
      nombre: ['', Validators.required],
      descripcion: ['', Validators.required],
      idComp: [''],
      modelo: ['', Validators.required],
      nSerie: ['', Validators.required],
      ubicacion: this.fb.group({
        id: [''],
        tipoUbicacion: [''],
        idPlanta: [''],
        idAreaFuncional: [''],
        idBodega: [''],
        idSeccionBodega: [''],
        idPatio: [''],
        estado: [true],
        fecha_log: [''],
      }),
      fabricante: [''],
      marca: ['', Validators.required],
      funcion: [''],
      peso: [''],
      cilindraje: [''],
      potencia: [''],
      ancho: [''],
      alto: [''],
      largo: [''],
      capacidad: [''],
      anioFabricacion: ['', Validators.required],
      caracteristicas: [''],
      seccion: [''],
      estado: [true],
      fecha_log: ['2025-03-23T14:30:00Z'],
    });
  }

  ngOnInit(): void {
    this.role = this.authService.getUserRole();
    this.equipoId = this.route.snapshot.paramMap.get('id');
    this.compId = this.authService.getCompanyId();
    if (this.equipoId) {
      this.isEditing = true;
      this.loadEquipo();
      if (this.compId) {
        this.loadPlantas(this.compId);
      }
    }
  }

  get nombre() {
    return this.equipoForm.get('nombre')!;
  }
  get descripcion() {
    return this.equipoForm.get('descripcion')!;
  }
  get modelo() {
    return this.equipoForm.get('modelo')!;
  }
  get nSerie() {
    return this.equipoForm.get('nSerie')!;
  }
  get marca() {
    return this.equipoForm.get('marca')!;
  }
  get anioFabricacion() {
    return this.equipoForm.get('anioFabricacion')!;
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
        },
      });
    }
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

  loadBodegas(idPlanta: string): void {
    this.bodegaService.getBodegas(idPlanta).subscribe({
      next: (response) => {
        this.Bodega = response as Bodega[];
      },
      error: (error) => {
        console.error('Error al cargar las bodegas:', error);
      },
    });
  }

  loadseccionBodegas(idBodega: string): void {
    this.seccionBodegaService.getseccionBodegas(idBodega).subscribe({
      next: (response) => {
        this.SeccionBodega = response as SeccionBodega[];
      },
      error: (error) => {
        console.error('Error al cargar las seccionBodegas:', error);
      },
    });
  }

  loadareaFuncionals(idPlanta: string): void {
    this.areaFuncionalService.getAreaFuncionals(idPlanta).subscribe({
      next: (response) => {
        this.AreaFuncional = response as AreaFuncional[];
      },
      error: (error) => {
        console.error('Error al cargar las areaFuncionals:', error);
      },
    });
  }

  loadPatios(idBodega: string): void {
    this.patioService.getPatios(idBodega).subscribe({
      next: (response) => {
        this.Patio = response as Patio[];
      },
      error: (error) => {
        console.error('Error al cargar las Patios:', error);
      },
    });
  }

  onSubmit(): void {
    if (this.equipoForm.valid) {
      const equipo = this.equipoForm.value;
      if (this.isEditing && this.equipoId) {
        this.equipoService.updateEquipo(this.equipoId, equipo).subscribe({
          next: () => this.goBack(),
          error: (error) => console.error('Error al actualizar equipo:', error),
        });
      } else {
        this.equipoService.createEquipo(equipo).subscribe({
          next: () => this.goBack(),
          error: (error) => console.error('Error al crear equipo:', error),
        });
      }
    }
  }

  goBack(): void {
    this.router.navigate(['/equipo']);
  }
}
