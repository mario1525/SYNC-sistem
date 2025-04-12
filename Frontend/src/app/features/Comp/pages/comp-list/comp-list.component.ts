import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CompService } from '../../services/comp.service';
import { Comp } from '../../../../../Types/Comp';

@Component({
  selector: 'app-comp-list',
  templateUrl: './comp-list.component.html',
  styleUrls: ['./comp-list.component.less']
})
export class CompListComponent implements OnInit {
  comps: Comp[] = [];

  constructor(
    private compService: CompService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadComps();
  }

  loadComps(): void {
    this.compService.getComps().subscribe({
      next: (response) => {
        this.comps = response as Comp[];
      },
      error: (error) => {
        console.error('Error al cargar compañías:', error);
      }
    });
  }

  navigateToNew(): void {
    this.router.navigate(['/comp/new']);
  }

  editComp(id: string): void {
    this.router.navigate(['/comp/edit', id]);
  }

  deleteComp(id: string): void {
    if (confirm('¿Está seguro de que desea eliminar esta compañía?')) {
      this.compService.deleteComp(id).subscribe({
        next: () => {
          this.loadComps();
        },
        error: (error) => {
          console.error('Error al eliminar compañía:', error);
        }
      });
    }
  }
} 