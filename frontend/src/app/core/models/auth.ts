export interface Usuario {
  id: number;
  nombreCompleto: string;
  email: string;
  rol: string;
  fechaCreacion: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  nombreCompleto: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  token: string;
  tokenType: string;
  expiresAt: string;
  usuario: Usuario;
}
