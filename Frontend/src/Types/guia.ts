export interface Valid {
  id: string;
  idGuia: string;
  nombre: string;
  descripcion: string;
  estado: boolean;
  fecha_log: string;
}
export interface Proced {
  id: string;
  idGuia: string;
  nombre: string;
  descripcion: string;
  estado: boolean;
  fecha_log: string;
  valid: Valid[];
}
export interface Guia {
  id: string;
  nombre: string;
  descripcion: string;
  proceso: string;
  inspeccion: string;
  herramientas: string;
  idComp: string;
  idEsp: string;
  seguridadInd: string;
  seguridadAmb: string;
  intervalo: number;
  importante: string;
  insumos: string;
  personal: number;
  duracion: number;
  logistica: string;
  situacion: string;
  notas: string;
  createdBy: string;
  updateBy: string;
  fechaUpdate: string;
  estado: boolean;
  fecha_log: string;
  proced: Proced[];
}

export interface GuiaResponse {
  data: Guia[];
  message: string;
  success: boolean;
}

export interface GuiaCreateRequest {
  id: string;
  nombre: string;
  descripcion: string;
  proceso: string;
  inspeccion: string;
  herramientas: string;
  idComp: string;
  idEsp: string;
  seguridadInd: string;
  seguridadAmb: string;
  intervalo: number;
  importante: string;
  insumos: string;
  personal: number;
  duracion: number;
  logistica: string;
  situacion: string;
  notas: string;
  createdBy: string;
  updateBy: string;
  fechaUpdate: string;
  estado: boolean;
  fecha_log: string;
}

export interface GuiaUpdateRequest extends GuiaCreateRequest {
  id: string;
}
