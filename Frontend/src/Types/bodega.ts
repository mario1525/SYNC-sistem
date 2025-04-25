export interface Bodega {
  id: string;
  idPlanta: string;
  nombre: string;
  estado: boolean;
  fecha_log: string;
}

export interface BodegaResponse {
  data: Bodega[];
  message: string;
  success: boolean;
}

export interface BodegaCreateRequest {
  idPlanta: string;
  nombre: string;
  estado: boolean;
  fecha_log: string;
}

export interface BodegaUpdateRequest extends BodegaCreateRequest {
  id: string;
}
