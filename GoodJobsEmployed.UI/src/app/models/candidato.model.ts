import { ApiResponse } from './perfil.model';

export interface EvaluacionDetalladaItem {
  idCaracteristica: number;
  nombreCaracteristica: string;
  descripcionCaracteristica: string;
  pesoPrioridad: number;
  criterioEspecifico: string;
  cumple: boolean;
  puntajeObtenido: number;
}

export interface AnalisisCvResponse {
  idPerfil: number;
  nombrePerfil: string;
  porcentajeCompatibilidad: number;
  justificacionGeneral: string;
  evaluacionDetallada: EvaluacionDetalladaItem[];
}

export interface CandidatoResultadoItem {
  id: string;
  nombreArchivo: string;
  tamanoFormateado: string;
  exito: boolean;
  mensajeError?: string;
  resultado?: AnalisisCvResponse;
}

export type CandidatoApiResponse = ApiResponse<AnalisisCvResponse>;
