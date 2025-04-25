import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PlantaFormComponent } from './planta-form.component';

describe('PlantaFormComponent', () => {
  let component: PlantaFormComponent;
  let fixture: ComponentFixture<PlantaFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PlantaFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PlantaFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
