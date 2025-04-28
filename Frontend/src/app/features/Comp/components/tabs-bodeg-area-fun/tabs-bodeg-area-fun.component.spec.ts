import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TabsBodegAreaFunComponent } from './tabs-bodeg-area-fun.component';

describe('TabsBodegAreaFunComponent', () => {
  let component: TabsBodegAreaFunComponent;
  let fixture: ComponentFixture<TabsBodegAreaFunComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TabsBodegAreaFunComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TabsBodegAreaFunComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
