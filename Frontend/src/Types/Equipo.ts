import { Ubicacion } from './ubicacion';

export interface Equipo {
  id: string;
  nombre: string;
  descripcion: string;
  idComp: string;
  modelo: string;
  nSerie: string;
  ubicacion: Ubicacion;
  fabricante: string;
  marca: string;
  funcion: string;
  peso: number;
  cilindraje: number;
  potencia: number;
  ancho: number;
  alto: number;
  largo: number;
  capacidad: number;
  anioFabricacion: number;
  caracteristicas: string;
  seccion: string;
  estado: boolean;
  fecha_log: string;
}

export interface EquipoResponse {
  data: Equipo[];
  message: string;
  success: boolean;
}

export interface EquipoCreateRequest {
  nombre: string;
  descripcion: string;
  idComp: string;
  modelo: string;
  nSerie: string;
  ubicacion: Ubicacion;
  fabricante: string;
  marca: string;
  funcion: string;
  peso: number;
  cilindraje: number;
  potencia: number;
  ancho: number;
  alto: number;
  largo: number;
  capacidad: number;
  anioFabricacion: number;
  caracteristicas: string;
  seccion: string;
  estado: boolean;
  fecha_log: string;
}

export interface EquipoUpdateRequest extends EquipoCreateRequest {
  id: string;
}
