export interface JwtToken {
  sub: string;
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': string;
  IdComp: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier': string;
  jti: string;
  exp: number;
  iss: string;
  aud: string;
}

export interface LoginRequest {
  usuario: string;
  contrasenia: string;
}

export interface LoginResponse {
  token: string;
  user: {
    username: string;
    role: string;
    idComp: string;
    userId: string;
  };
}
