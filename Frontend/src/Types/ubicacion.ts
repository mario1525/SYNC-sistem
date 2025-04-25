export interface Ubicacion {
  id: string;
  idEquipo: string;
  tipoUbicacion: string;
  idPlanta: string;
  idAreaFuncional: string;
  idBodega: string | null;
  idSeccionBodega: string | null;
  idPatio: string | null;
  estado: boolean;
  fecha_log: string;
}

export interface UbicacionResponse {
  data: Ubicacion[];
  message: string;
  success: boolean;
}

export interface UbicacionCreateRequest {
  idEquipo: string;
  tipoUbicacion: string;
  idPlanta: string;
  idAreaFuncional: string;
  idBodega: string | null;
  idSeccionBodega: string | null;
  idPatio: string | null;
  estado: boolean;
  fecha_log: string;
}

export interface UbicacionUpdateRequest extends UbicacionCreateRequest {
  id: string;
}
