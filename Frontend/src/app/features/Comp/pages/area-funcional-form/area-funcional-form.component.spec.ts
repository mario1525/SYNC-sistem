import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AreaFuncionalFormComponent } from './area-funcional-form.component';

describe('AreaFuncionalFormComponent', () => {
  let component: AreaFuncionalFormComponent;
  let fixture: ComponentFixture<AreaFuncionalFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AreaFuncionalFormComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AreaFuncionalFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
