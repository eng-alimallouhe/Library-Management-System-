import { Injectable } from '@angular/core';
import { StorageService, userData } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class LanguageService {

  constructor(
    private storageService: StorageService
  ) { }

  

  getLanguageAsNumber() {
    let langauge = this.storageService.getItem(userData.language);

    if (langauge) {
      if (langauge.toUpperCase() === 'AR') {
        return 0;
      }
      else{
        return 1;
      }
    }
    else {
      return 0;
    }

  }

  getLanguage(){
    return this.storageService.getItem(userData.language) || 'en';
  }

  steLanguage(language: 'EN' | 'AR'){
    this.storageService.setItem(userData.language, language);
  }
}
