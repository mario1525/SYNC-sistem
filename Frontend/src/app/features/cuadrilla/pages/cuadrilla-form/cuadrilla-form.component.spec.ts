import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CuadrillaFormComponent } from './cuadrilla-form.component';

describe('CuadrillaFormComponent', () => {
  let component: CuadrillaFormComponent;
  let fixture: ComponentFixture<CuadrillaFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CuadrillaFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CuadrillaFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
