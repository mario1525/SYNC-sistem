export interface Patio {
  id: string;
  idBodega: string;
  nombre: string;
  estado: boolean;
  fecha_log: string;
}

export interface PatioResponse {
  data: Patio[];
  message: string;
  success: boolean;
}

export interface PatioCreateRequest {
  idBodega: string;
  nombre: string;
  estado: boolean;
  fecha_log: string;
}

export interface PatioUpdateRequest extends PatioCreateRequest {
  id: string;
}
