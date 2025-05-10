import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Equipo } from '../../../../../Types/Equipo';
import { EquipoService } from '../../services/equipo.service';
import { AuthService } from '../../../auth/Services/auth.service';

@Component({
  selector: 'app-equipo-list',
  templateUrl: './equipo-list.component.html',
  styleUrl: './equipo-list.component.less',
})
export class EquipoListComponent implements OnInit {
  equipos: Equipo[] = [];
  idComp: string | null = null;
  constructor(
    private authService: AuthService,
    private equipoService: EquipoService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.idComp = this.authService.getCompanyId();
    if (this.idComp) {
      this.loadequipos(this.idComp);
    }
  }

  loadequipos(idComp: string): void {
    this.equipoService.getEquipos(idComp).subscribe({
      next: (response) => {
        this.equipos = response as Equipo[];
      },
      error: (error) => {
        console.error('Error al cargar equipo:', error);
      },
    });
  }

  navigateToNew(): void {
    this.router.navigate(['/equipo/new']);
  }

  editequipo(id: string): void {
    this.router.navigate(['/equipo/edit', id]);
  }

  deleteEquipo(id: string): void {
    if (confirm('¿Está seguro de que desea eliminar este equipo')) {
      this.equipoService.deleteEquipo(id).subscribe({
        next: () => {
          if (this.idComp) {
            this.loadequipos(this.idComp);
          }
        },
        error: (error) => {
          console.error('Error al eliminar:', error);
        },
      });
    }
  }
}
