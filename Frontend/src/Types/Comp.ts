export interface Comp {
    id: string;
    nombre: string;
    ciudad: string;
    nit: string;
    direccion: string;
    sector: string;
    estado: boolean;
    fecha_log: string;
  }
  
  export interface CompResponse {
    data: Comp[];
    message: string;
    success: boolean;
  }
  
  export interface CompCreateRequest {
    nombre: string;
    ciudad: string;
    nit: string;
    direccion: string;
    sector: string;
    estado: boolean;
  }
  
  export interface CompUpdateRequest extends CompCreateRequest {
    id: string;
  }