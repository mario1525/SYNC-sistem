import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Planta } from '../../../../../Types/planta';
import { PlantaService } from '../../services/planta.service';

@Component({
  selector: 'app-planta-form',
  templateUrl: './planta-form.component.html',
  styleUrl: './planta-form.component.less'
})
export class PlantaFormComponent {
  plantaForm: FormGroup;
}
