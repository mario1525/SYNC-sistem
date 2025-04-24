export interface Esp {
  id: string;
  nombre: string;
  idComp: string;
  estado: boolean;
  fecha_log: string;
}

export interface EspResponse {
  data: Esp[];
  message: string;
  success: boolean;
}

export interface EspCreateRequest {
  id: string;
  nombre: string;
  idComp: string;
  estado: boolean;
  fecha_log: string;
}

export interface EspUpdateRequest extends EspCreateRequest {
  id: string;
}
