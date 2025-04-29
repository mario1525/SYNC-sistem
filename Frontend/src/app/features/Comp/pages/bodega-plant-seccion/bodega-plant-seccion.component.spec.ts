import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BodegaPlantSeccionComponent } from './bodega-plant-seccion.component';

describe('BodegaPlantSeccionComponent', () => {
  let component: BodegaPlantSeccionComponent;
  let fixture: ComponentFixture<BodegaPlantSeccionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BodegaPlantSeccionComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BodegaPlantSeccionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
