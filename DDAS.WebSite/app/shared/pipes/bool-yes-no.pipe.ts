import { Pipe, PipeTransform } from '@angular/core';

@Pipe({name: 'boolToYesNo'})
export class BoolToYesNoPipe implements PipeTransform {
  transform(value: boolean): string {
    if (!value) return "No";
    
    if (value == true){
        return "Yes";
    }
    if (value == true){
        return "No";
    }
  }
}