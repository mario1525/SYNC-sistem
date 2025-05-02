import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GuiaService } from '../../services/guia.service';
import { Guia } from '../../../../../Types/guia';
import { AuthService } from '../../../auth/Services/auth.service';

@Component({
  selector: 'app-guia-list',
  templateUrl: './guia-list.component.html',
  styleUrl: './guia-list.component.less',
})
export class GuiaListComponent implements OnInit {
  Guias: Guia[] = [];
  idComp: string | null = null;

  constructor(
    private authService: AuthService,
    private GuiaService: GuiaService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.idComp = this.authService.getCompanyId();
    if (this.idComp) {
      this.loadGuias(this.idComp);
    }
  }

  loadGuias(idComp: string): void {
    this.GuiaService.getGuias(idComp).subscribe({
      next: (response) => {
        this.Guias = response as Guia[];
      },
      error: (error) => {
        console.error('Error al cargar Guiaañías:', error);
      },
    });
  }

  navigateToNew(): void {
    this.router.navigate(['/Guia/new']);
  }

  editGuia(id: string): void {
    this.router.navigate(['/Guia/edit', id]);
  }

  deleteGuia(id: string): void {
    if (confirm('¿Está seguro de que desea eliminar esta Guiaañía?')) {
      this.GuiaService.deleteGuia(id).subscribe({
        next: () => {
          if (this.idComp) {
            this.loadGuias(this.idComp);
          }
        },
        error: (error) => {
          console.error('Error al eliminar Guiaañía:', error);
        },
      });
    }
  }
}
