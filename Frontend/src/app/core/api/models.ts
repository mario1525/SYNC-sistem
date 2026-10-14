// Modelos de la API SYNC (JSON camelCase, igual que las entidades del backend).

export interface Entidad {
  id?: string;
  estado: boolean;
  fecha_log?: string;
}

/** Respuesta estándar de escritura. */
export interface Mensaje {
  mensaje: string;
  id?: string;
}

/** Error estándar (RFC 7807) devuelto por el backend. */
export interface ProblemDetails {
  status?: number;
  title?: string;
  detail?: string;
}

export type Rol = 'Root' | 'Admin' | 'Supervisor' | 'Tecnico';
export const ROLES: Rol[] = ['Root', 'Admin', 'Supervisor', 'Tecnico'];

// ---------------------------------------------------------------- Organización
export interface Comp extends Entidad {
  nombre: string;
  ciudad?: string;
  nit?: string;
  direccion?: string;
  sector?: string;
}

export interface Esp extends Entidad {
  nombre: string;
  descripcion?: string;
  idComp?: string | null;
}

// ---------------------------------------------------------------- Ubicaciones
export interface Planta extends Entidad {
  nombre: string;
  region?: string;
  idComp?: string;
}

export interface TipoArea extends Entidad {
  nombre: string;
  descripcion?: string;
}

export interface Area extends Entidad {
  idPlanta: string;
  nombre: string;
  tipoArea: string;
  tipoAreaNombre?: string;
  plantaNombre?: string;
}

export interface SeccionArea extends Entidad {
  idArea: string;
  nombre: string;
  areaNombre?: string;
  idPlanta?: string;
  plantaNombre?: string;
  tipoAreaNombre?: string;
}

// ---------------------------------------------------------------- Equipos
export type TipoUbicacion = 'Produccion' | 'Almacen';

export interface UbicacionEquipo {
  id?: string;
  tipoUbicacion: TipoUbicacion;
  idSeccionArea?: string | null;
  seccionNombre?: string;
  idArea?: string;
  areaNombre?: string;
  idPlanta?: string;
  plantaNombre?: string;
  tipoAreaNombre?: string;
  vigente?: boolean;
  fecha_log?: string;
}

export interface Equipo extends Entidad {
  nombre: string;
  descripcion?: string;
  idComp?: string;
  modelo?: string;
  nSerie?: string;
  fabricante?: string;
  marca?: string;
  funcion?: string;
  peso: number;
  cilindraje: number;
  potencia: number;
  ancho: number;
  alto: number;
  largo: number;
  capacidad: number;
  anioFabricacion: number;
  caracteristicas?: string;
  ubicacion?: UbicacionEquipo | null;
}

// ---------------------------------------------------------------- Guías
export interface Valid extends Entidad {
  idProced?: string;
  nombre: string;
  descripcion?: string;
}

export interface Proced extends Entidad {
  idGuia?: string;
  nombre: string;
  descripcion?: string;
  valid: Valid[];
}

export interface Guia extends Entidad {
  nombre: string;
  descripcion?: string;
  proceso?: string;
  inspeccion?: string;
  herramientas?: string;
  idComp?: string;
  idEsp?: string | null;
  espNombre?: string;
  seguridadInd?: string;
  seguridadAmb?: string;
  intervalo: number;
  importante?: string;
  insumos?: string;
  personal: number;
  duracion: number;
  logistica?: string;
  situacion?: string;
  notas?: string;
  updatedBy?: string;
  fechaUpdate?: string;
  proced?: Proced[] | null;
}

export interface GuiaEquipo extends Entidad {
  idGuia: string;
  idEquipo: string;
  guiaNombre?: string;
  equipoNombre?: string;
  intervalo?: number;
}

// ---------------------------------------------------------------- Actividades
export interface TipoActividad extends Entidad {
  nombre: string;
  descripcion?: string;
  idComp?: string;
}

export interface Actividad extends Entidad {
  idComp?: string;
  descripcion?: string;
  idTipoActividad: string;
  tipoActividadNombre?: string;
  ubicacion?: string;
  fechaEjecucion?: string | null;
  idCuad?: string | null;
  cuadNombre?: string;
  detalle?: string;
  intervalo: number;
  numEquipos?: number;
}

export interface ActividadEquipo extends Entidad {
  idActividad: string;
  idEquipo: string;
  idGuia?: string | null;
  equipoNombre?: string;
  guiaNombre?: string;
  fechaEjecucion?: string;
}

// ---------------------------------------------------------------- Personas
export interface Usuario extends Entidad {
  identificacion: number;
  nombre: string;
  apellido?: string;
  correo?: string;
  idComp?: string;
  compNombre?: string;
  idEsp?: string | null;
  espNombre?: string;
  idCuad?: string | null;
  cuadNombre?: string;
  cargo?: string;
  rol: Rol;
  usuario?: string | null;
  tieneCredencial?: boolean;
}

export interface Credencial extends Entidad {
  usuario: string;
  contrasenia?: string;
  idUser: string;
}

export interface Cuad extends Entidad {
  nombre: string;
  idComp?: string;
  numIntegrantes?: number;
}

export interface Turno extends Entidad {
  nombre: string;
  idComp?: string;
  horaInicio: string;
  horaFin: string;
}

export interface Horario extends Entidad {
  idTurno: string;
  turnoNombre?: string;
  horaInicio?: string;
  horaFin?: string;
  idCuad: string;
  cuadNombre?: string;
  diaSemana: number;
  esFestivo: boolean;
}

export const DIAS_SEMANA = ['Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado', 'Domingo'];

// ---------------------------------------------------------------- Auth
export interface LoginRequest {
  usuario: string;
  contrasenia: string;
}

export interface LoginResponse {
  token: string;
  expira: string;
}

/** Claims del JWT emitido por user-service. */
export interface JwtToken {
  sub: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': Rol;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
  IdComp: string;
  exp: number;
}
