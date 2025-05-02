import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SeccionBFormComponent } from './seccion-b-form.component';

describe('SeccionBFormComponent', () => {
  let component: SeccionBFormComponent;
  let fixture: ComponentFixture<SeccionBFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SeccionBFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(SeccionBFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
