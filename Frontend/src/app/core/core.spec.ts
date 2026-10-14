import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router, UrlTree } from '@angular/router';
import { toParams } from './api/crud.service';
import { EquipoService } from './api/services';
import { AuthService } from './auth/auth.service';
import { rolGuard } from './auth/auth.guard';
import { mensajeDeError } from './ui/notify.service';

/** JWT sin firma válida (solo se decodifica en el cliente). */
function token(rol: string, expEnSeg = 3600): string {
  const b64 = (o: object) => btoa(JSON.stringify(o)).replace(/=+$/, '');
  return `${b64({ alg: 'HS256' })}.${b64({
    sub: 'Ana',
    IdComp: 'comp-a',
    'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': rol,
    'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': 'u1',
    exp: Math.floor(Date.now() / 1000) + expEnSeg,
  })}.firma`;
}

describe('toParams', () => {
  it('omite valores vacíos, null y undefined', () => {
    const p = toParams({ a: 'x', b: '', c: null, d: undefined, e: false, f: 0 });
    expect(p.keys()).toEqual(['a', 'e', 'f']);
    expect(p.get('e')).toBe('false');
  });
});

describe('mensajeDeError', () => {
  it('usa el title de ProblemDetails', () => {
    const e = new HttpErrorResponse({ status: 409, error: { title: 'El registro ya existe.' } });
    expect(mensajeDeError(e)).toBe('El registro ya existe.');
  });

  it('concatena errores de validación del modelo', () => {
    const e = new HttpErrorResponse({
      status: 400,
      error: { title: 'x', errors: { Nombre: ['Requerido.'], Id: ['Inválido.'] } },
    });
    expect(mensajeDeError(e)).toBe('Requerido. Inválido.');
  });

  it('informa falta de conexión', () => {
    expect(mensajeDeError(new HttpErrorResponse({ status: 0 }))).toContain('conexión');
  });
});

describe('AuthService + interceptor + guards', () => {
  let http: HttpTestingController;

  beforeEach(() => {
    sessionStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('lee rol, compañía y usuario del token tras el login', () => {
    const auth = TestBed.inject(AuthService);
    auth.login({ usuario: 'ana', contrasenia: 'x' }).subscribe();
    http.expectOne('/apigateway/auth').flush({ token: token('Supervisor'), expira: '' });

    expect(auth.autenticado()).toBe(true);
    expect(auth.rol()).toBe('Supervisor');
    expect(auth.idComp()).toBe('comp-a');
    expect(auth.userId()).toBe('u1');
    expect(auth.tieneRol('Root', 'Admin')).toBe(false);
  });

  it('un token vencido no cuenta como sesión', () => {
    sessionStorage.setItem('sync_token', token('Admin', -10));
    expect(TestBed.inject(AuthService).autenticado()).toBe(false);
  });

  it('rolGuard redirige a /home si el rol no está permitido', () => {
    sessionStorage.setItem('sync_token', token('Tecnico'));
    const resultado = TestBed.runInInjectionContext(() =>
      rolGuard({ data: { roles: ['Root', 'Admin'] } } as never, {} as never),
    );
    expect(resultado instanceof UrlTree).toBe(true);
    expect(TestBed.inject(Router).serializeUrl(resultado as UrlTree)).toBe('/home');
  });

  it('el servicio CRUD consulta el recurso con filtros y sin IdComp vacío', () => {
    TestBed.inject(EquipoService).list({ Nombre: 'bomba', IdComp: '' }).subscribe();
    const req = http.expectOne((r) => r.url === '/apigateway/equipo');
    expect(req.request.params.get('Nombre')).toBe('bomba');
    expect(req.request.params.has('IdComp')).toBe(false);
    req.flush([]);
  });
});
