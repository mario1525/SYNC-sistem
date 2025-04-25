export interface AreaFuncional {
  id: string;
  idPlanta: string;
  nombre: string;
  estado: boolean;
  fecha_log: string;
}

export interface AreaFuncionalResponse {
  data: AreaFuncional[];
  message: string;
  success: boolean;
}

export interface AreaFuncionalCreateRequest {
  idPlanta: string;
  nombre: string;
  estado: boolean;
  fecha_log: string;
}

export interface AreaFuncionalUpdateRequest extends AreaFuncionalCreateRequest {
  id: string;
}
