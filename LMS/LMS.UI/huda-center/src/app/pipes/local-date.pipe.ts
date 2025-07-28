import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'localDate',
  standalone: true
})
export class LocalDatePipe implements PipeTransform {

  transform(value: Date | string | null | undefined, format: string = 'yyyy-MM-dd'): string {
    if (!value) return '';

    const date = new Date(value);
    const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60000);

    const year = localDate.getFullYear();
    const month = String(localDate.getMonth() + 1).padStart(2, '0');
    const day = String(localDate.getDate()).padStart(2, '0');

    switch (format) {
      case 'yyyy-MM-dd':
        return `${year}-${month}-${day}`;
      default:
        return localDate.toLocaleString();
    }
  }
}
