// src/app/services/theme.service.ts
import { DOCUMENT } from '@angular/common'; // لا حاجة لـ LocationStrategy هنا
import { Inject, Injectable, Renderer2, RendererFactory2 } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs'; // استيراد BehaviorSubject و Observable
import { StorageService, userData } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private _currentTheme = new BehaviorSubject<'light_mode' | 'dark_mode'>('light_mode');

  public currentTheme$: Observable<'light_mode' | 'dark_mode'> = this._currentTheme.asObservable();

  private renderer: Renderer2;

  constructor(
    private rendererFactory: RendererFactory2, 
    private storageService: StorageService,
    @Inject(DOCUMENT) private document: Document
  ) {
    this.renderer = rendererFactory.createRenderer(null, null);
  }

  applySavedTheme(): 'light_mode' | 'dark_mode' {
    const savedTheme = this.storageService.getItem(userData.theme) as 'light_mode' | 'dark_mode';
    
    const themeToApply = savedTheme || 'light_mode';

    this._currentTheme.next(themeToApply); 
    
    this.applyThemeToBody(themeToApply); 
    
    localStorage.setItem('Theme', themeToApply);
    
    return themeToApply;
  }

  
  applyTheme(themeName: 'light_mode' | 'dark_mode') {
    let savedTheme = this.storageService.getItem(userData.theme) as 'dark_mode' | 'light_mode';

    if (!savedTheme) {
      savedTheme = 'light_mode';
    }

    this._currentTheme.next(themeName); 
    
    this.applyThemeToBody(themeName);
       
    this.storageService.setItem(userData.theme, themeName); 
  }

  
  private applyThemeToBody(themeName: 'light_mode' | 'dark_mode') {
    if (themeName === 'light_mode') {
      this.renderer.removeClass(this.document.body, 'dark_mode');
      this.renderer.addClass(this.document.body, 'light_mode');
    } else {
      this.renderer.removeClass(this.document.body, 'light_mode');
      this.renderer.addClass(this.document.body, 'dark_mode');
    }
  }

  
  getCurrentTheme(): 'light_mode' | 'dark_mode' {
    return this._currentTheme.getValue();
  }
}