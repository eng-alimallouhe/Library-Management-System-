import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SearchCommunicationService {
  private nameSearchSubject = new BehaviorSubject<string>('');
  nameSearch$ = this.nameSearchSubject.asObservable();

  updateNameSearch(name: string) {
    this.nameSearchSubject.next(name);
  }
}