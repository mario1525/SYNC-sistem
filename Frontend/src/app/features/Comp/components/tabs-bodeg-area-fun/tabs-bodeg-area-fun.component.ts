import { Component, OnInit } from '@angular/core';
import { AreaFuncional } from '../../../../../Types/areafuncional';
import { ActivatedRoute, Router } from '@angular/router';
import { Bodega } from '../../../../../Types/bodega';
import { MatDialog } from '@angular/material/dialog';
import { BodegaService } from '../../services/bodega.service';
import { AreaFuncionalService } from '../../services/areaFuncional.service';
import { AreaFuncionalFormComponent } from '../area-funcional-form/area-funcional-form.component';
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

  navigateToNewArea(): void {}
  editArea(id: string): void {
    this.dialog.open(AreaFuncionalFormComponent, {
      width: '400px',
      data: { areafunId: id, plantaId: this.plantId },
    });
  }
  deleteArea(id: string): void {}

  navigateToNewBodega(): void {}
  editBodega(id: string): void {}
  manageSecciones(id: string): void {}
  deleteBodega(id: string): void {}
}
