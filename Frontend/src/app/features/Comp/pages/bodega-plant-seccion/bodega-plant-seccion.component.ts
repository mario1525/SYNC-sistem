import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PatioService } from '../../services/patio.service';
import { seccionBodegaService } from '../../services/seccionBodega.service';
import { Patio } from '../../../../../Types/patio';
import { SeccionBodega } from '../../../../../Types/seccionbodega';

@Component({
  selector: 'app-bodega-plant-seccion',
  templateUrl: './bodega-plant-seccion.component.html',
  styleUrl: './bodega-plant-seccion.component.less',
})
export class BodegaPlantSeccionComponent implements OnInit {
  patio: Patio[] = [];
  seccionBodega: SeccionBodega[] = [];
  bodegaId: string | null = null;

  constructor(
    private seccionBodegaService: seccionBodegaService,
    private patioService: PatioService,
    private router: Router,
    private route: ActivatedRoute,
  ) {}

  ngOnInit(): void {
    this.bodegaId = this.route.snapshot.paramMap.get('bodegaId');
    if (this.bodegaId) {
      this.loadSeccionB();
      this.loadPatio();
    }
  }

  loadPatio(): void {
    // Este método carga los datos de una compañía existente
    if (this.bodegaId) {
      // Si existe un ID de compañía, hace una petición al servicio
      this.patioService.getPatios(this.bodegaId).subscribe({
        // Cuando la petición es exitosa
        next: (response) => {
          // Extrae los datos de la compañía de la respuesta
          const value = response;
          this.patio = value;
        },
        // Si ocurre un error en la petición
        error: (error) => {
          // Muestra el error en consola
          console.error('Error al cargar:', error);
          // Regresa a la página anterior
          //this.goBack();
        },
      });
    }
  }

  loadSeccionB(): void {
    // Este método carga los datos de una compañía existente
    if (this.bodegaId) {
      // Si existe un ID de compañía, hace una petición al servicio
      this.seccionBodegaService.getseccionBodegas(this.bodegaId).subscribe({
        // Cuando la petición es exitosa
        next: (response) => {
          // Extrae los datos de la compañía de la respuesta
          const value = response;
          this.seccionBodega = value;
        },
        // Si ocurre un error en la petición
        error: (error) => {
          // Muestra el error en consola
          console.error('Error al cargar:', error);
          // Regresa a la página anterior
          //this.goBack();
        },
      });
    }
  }
}
