import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BodegaFormComponent } from './bodega-form.component';

describe('BodegaFormComponent', () => {
  let component: BodegaFormComponent;
  let fixture: ComponentFixture<BodegaFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BodegaFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(BodegaFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
