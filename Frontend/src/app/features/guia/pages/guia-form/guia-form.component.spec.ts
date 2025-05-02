import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuiaFormComponent } from './guia-form.component';

describe('GuiaFormComponent', () => {
  let component: GuiaFormComponent;
  let fixture: ComponentFixture<GuiaFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GuiaFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GuiaFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
