import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

interface JwtPayload {
  sub: string;         
  email: string;       
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;        
  jti: string;         
  DepartmentId?: string;
  DepartmentName?: string;
  exp: number;
  iat: number;
  iss: string;
  aud: string;
}



@Injectable({
  providedIn: 'root'
})
export class TokenReaderService {

  constructor() { }

  private decodeToken(token: string): JwtPayload | null {
    if (!token) {
      return null;
    }
    try {
      const decoded = jwtDecode<JwtPayload>(token);
      return decoded;
    } catch (Error) {
      console.error('Error decoding JWT token:', Error);
      return null;
    }
  }

  getRoleFromToken(token: string): string | null {
    const decodedToken = this.decodeToken(token);
    return decodedToken ? decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || null : null;
  }

  
  getFinalRole(token: string): string {
    const decodedToken = this.decodeToken(token);
    console.log(decodedToken);
    
    if (!decodedToken) {
      return 'customer';
    }

    const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    if (!role) {
      return 'customer';
    }

    let finalRole = '';

    if (role.toLowerCase() === 'customer' || role.toLowerCase() === 'admin') {
      finalRole = role.toLocaleLowerCase();
    }
    else {
      const departmentName = decodedToken ? decodedToken.DepartmentName + '' : 'customer12';
      finalRole = departmentName.toLocaleLowerCase();
    }
    return finalRole;
  }
}