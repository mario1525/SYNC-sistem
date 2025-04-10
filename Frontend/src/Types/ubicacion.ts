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
  