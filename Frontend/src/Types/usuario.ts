export interface Usuario {
  id: string;
  identificacion: number;
  nombre: string;
  apellido: string;
  correo: string;
  idComp: string;
  idEsp: string;
  idCuad: string;
  cargo: string;
  rol: string;
  estado: boolean;
  fecha_log: string;
}

export interface UsuarioResponse {
  data: Usuario[];
  message: string;
  success: boolean;
}

export interface UsuarioCreateRequest {
  id: string;
  identificacion: number;
  nombre: string;
  apellido: string;
  correo: string;
  idComp: string;
  idEsp: string;
  idCuad: string;
  cargo: string;
  rol: string;
  estado: boolean;
  fecha_log: string;
}

export interface UsuarioUpdateRequest extends UsuarioCreateRequest {
  id: string;
}
