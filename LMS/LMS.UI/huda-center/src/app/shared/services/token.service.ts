import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor() { }

  setTokens(accessToken: string, refreshToken: string){
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    console.log(accessToken);
    console.log(refreshToken);
    
    
  }

  setAccessToken(accessToken: string){
    localStorage.setItem('accessToken', accessToken);
  }

  
  setRefreshToken(refreshToken: string){
    localStorage.setItem('refreshToken', refreshToken);
  }

  clearTokens(){
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('accessToken');
  }

  getTokens() : object{
    const accessToken = localStorage.getItem('accessToken');
    const refreshToken = localStorage.getItem('refreshToken');
    let tupple : object = {
      accessToken,
      refreshToken
    };
    return tupple;
  }

  getAccessToken() : string{
    return localStorage.getItem('accessToken') || '';
  }
}
