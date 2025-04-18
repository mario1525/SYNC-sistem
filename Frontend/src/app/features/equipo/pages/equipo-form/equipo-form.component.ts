import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { EquipoService } from '../../services/equipo.service';
import { MenuComponent } from '../../../../shared/Components/menu/menu.component';
import { LogoHeaderComponent } from '../../../../shared/Components/logo-header/logo-header.component';

@Component({
  selector: 'app-equipo-form',
  templateUrl: './equipo-form.component.html',
  styleUrls: ['./equipo-form.component.less'],
})
export class EquipoFormComponent implements OnInit {
  equipoForm: FormGroup;
  isEditing = false;
  equipoId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private equipoService: EquipoService,
    private router: Router,
    private route: ActivatedRoute,
  ) {
    this.equipoForm = this.fb.group({
      nombre: ['', Validators.required],
      descripcion: [''],
      estado: [true],
    });
  }

  ngOnInit(): void {
    this.equipoId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.equipoId) {
      this.isEditing = true;
      this.loadEquipo();
    }
  }

  get nombre() {
    return this.equipoForm.get('nombre')!;
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
    this.router.navigate(['/home']);
  }
}
