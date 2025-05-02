import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuiaListComponent } from './guia-list.component';

describe('GuiaListComponent', () => {
  let component: GuiaListComponent;
  let fixture: ComponentFixture<GuiaListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GuiaListComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(GuiaListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
