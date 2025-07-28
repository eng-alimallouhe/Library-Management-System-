import { Injectable } from '@angular/core';


export enum userData {
  accessToken = 'accessToken',
  refreshToken = 'refreshToken',
  id = 'id',
  userName = 'userName',
  fullName = 'fullName',
  email = 'email',
  role = 'role',
  language = 'language',
  theme = 'theme'
}


@Injectable({
  providedIn: 'root'
})
export class StorageService {
  constructor() { }

  setItem(item: userData, value: string) {
    const data = JSON.parse(localStorage.getItem('userData') || '{}');
    data[item] = value;
    localStorage.setItem('userData', JSON.stringify(data));
  }

  getItem(item: userData): string | null {
    const data = JSON.parse(localStorage.getItem('userData') || '{}');
    return data[item] || null;
  }

  removeItem(item: userData) {
    const data = JSON.parse(localStorage.getItem('userData') || '{}');
    delete data[item];
    localStorage.setItem('userData', JSON.stringify(data));
  }

  clear() {
    localStorage.removeItem('userData');
  }
}
