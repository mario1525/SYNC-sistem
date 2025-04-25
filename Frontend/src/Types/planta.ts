export interface Planta {
  id: string;
  nombre: string;
  region: string;
  idComp: string;
  estado: boolean;
  fecha_log: string;
}

export interface PlantaResponse {
  data: Planta[];
  message: string;
  success: boolean;
}

export interface PlantaCreateRequest {
  id: string;
  nombre: string;
  idComp: string;
  region: string;
  estado: boolean;
  fecha_log: string;
}

export interface PlantaUpdateRequest extends PlantaCreateRequest {
  id: string;
}
