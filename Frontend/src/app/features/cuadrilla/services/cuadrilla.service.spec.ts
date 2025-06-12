import { TestBed } from '@angular/core/testing';

import { CuadrillaService } from './cuadrilla.service';

describe('CuadrillaService', () => {
  let service: CuadrillaService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CuadrillaService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
