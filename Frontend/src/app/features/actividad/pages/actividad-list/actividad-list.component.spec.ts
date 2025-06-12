import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ActividadListComponent } from './actividad-list.component';

describe('ActividadListComponent', () => {
  let component: ActividadListComponent;
  let fixture: ComponentFixture<ActividadListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ActividadListComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(ActividadListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
