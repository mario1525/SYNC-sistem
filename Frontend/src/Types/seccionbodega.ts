export interface SeccionBodega {
  id: string;
  idPlanta: string;
  nombre: string;
  estado: boolean;
  fecha_log: string;
}

export interface SeccionBodegaResponse {
  data: SeccionBodega[];
  message: string;
  success: boolean;
}

export interface SeccionBodegaCreateRequest {
  idPlanta: string;
  nombre: string;
  estado: boolean;
  fecha_log: string;
}

export interface SeccionBodegaUpdateRequest extends SeccionBodegaCreateRequest {
  id: string;
}
