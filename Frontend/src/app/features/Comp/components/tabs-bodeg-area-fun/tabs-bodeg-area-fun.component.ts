import { Component, OnInit } from '@angular/core';
import { AreaFuncional } from '../../../../../Types/areafuncional';
import { ActivatedRoute, Router } from '@angular/router';
import { Bodega } from '../../../../../Types/bodega';
import { MatDialog } from '@angular/material/dialog';
import { BodegaService } from '../../services/bodega.service';
import { AreaFuncionalService } from '../../services/areaFuncional.service';
import { AreaFuncionalFormComponent } from '../area-funcional-form/area-funcional-form.component';
import { BodegaFormComponent } from '../bodega-form/bodega-form.component';
//import { MatTabsModule } from '@angular/material/tabs';

@Component({
  selector: 'app-tabs-bodeg-area-fun',
  //standalone: true,
  //imports: [MatDialog],
  templateUrl: './tabs-bodeg-area-fun.component.html',
  styleUrl: './tabs-bodeg-area-fun.component.less',
})
export class TabsBodegAreaFunComponent implements OnInit {
  areasFuncionales: AreaFuncional[] = [];
  bodegas: Bodega[] = [];
  plantId: string | null = null;

  constructor(
    private bodegaService: BodegaService,
    private areaFunService: AreaFuncionalService,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog,
  ) {}

  ngOnInit(): void {
    this.plantId = this.route.snapshot.paramMap.get('plantaId');
    if (this.plantId) {
      this.loadAreafund();
      this.loadBodegas();
    }
  }
  //

  loadBodegas(): void {
    if (this.plantId) {
      this.bodegaService.getBodegas(this.plantId).subscribe({
        next: (response) => {
          this.bodegas = response as Bodega[];
        },
        error: (error) => {
          console.error('Error al cargar las bodegas:', error);
        },
      });
    }
  }

  loadAreafund(): void {
    if (this.plantId) {
      this.areaFunService.getAreaFuncionals(this.plantId).subscribe({
        next: (response) => {
          this.areasFuncionales = response as AreaFuncional[];
        },
        error: (error) => {
          console.error('Error al cargar las bodegas:', error);
        },
      });
    }
  }

  navigateToNewArea(): void {
    this.dialog.open(AreaFuncionalFormComponent, {
      width: '400px',
      data: { plantaId: this.plantId },
    });
  }
  editArea(id: string): void {
    this.dialog.open(AreaFuncionalFormComponent, {
      width: '400px',
      data: { areafunId: id, plantaId: this.plantId },
    });
  }
  deleteArea(id: string): void {
    if (confirm('¿Está seguro de que desea eliminar esta area funcional?')) {
      this.areaFunService.deleteAreaFuncional(id).subscribe({
        next: () => {
          //this.loadPlantas(this.compId);
        },
        error: (error) => {
          console.error('Error al eliminar el area funcional:', error);
        },
      });
    }
  }

  navigateToNewBodega(): void {
    this.dialog.open(BodegaFormComponent, {
      width: '400px',
      data: { plantaId: this.plantId },
    });
  }
  editBodega(id: string): void {
    this.dialog.open(BodegaFormComponent, {
      width: '400px',
      data: { BodegaId: id, plantaId: this.plantId },
    });
  }
  manageSecciones(id: string): void {
    this.router.navigate(['bodega', id], { relativeTo: this.route });
  }

  deleteBodega(id: string): void {
    if (confirm('¿Está seguro de que desea eliminar esta bodega?')) {
      this.bodegaService.deleteBodega(id).subscribe({
        next: () => {
          //this.loadPlantas(this.compId);
        },
        error: (error) => {
          console.error('Error al eliminar la bodega:', error);
        },
      });
    }
  }
}
