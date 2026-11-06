export interface ApiResponse<T> {
  correct: boolean;
  errorMessage: string | null;
  object: T | null;
  objects: T[] | null;
}

export interface Caracteristica {
  idCaracteristica: number;
  nombreCaracteristica: string;
  descripcionCaracteristica: string;
  pesoPrioridad: number;
  criterioEspecifico: string;
}

export interface Perfil {
  idPerfil: number;
  nombre: string;
  caracteristicas?: Caracteristica[];
}
