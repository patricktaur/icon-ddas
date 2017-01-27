import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'pad'})
export class PadPipe implements PipeTransform {
  transform(value: string, length:number, char:string): string {
    
    if (!value || !char || value.length >= length) {
      return value;
    }
    var max = (length - value.length)/char.length;
    for (var i = 0; i < max; i++) {
      value += char;
    }
    return value;
  }
}