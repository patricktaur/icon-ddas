
import { Pipe, PipeTransform } from '@angular/core';
// @Pipe({ name: 'highlight' })
// export class HighLightPipe implements PipeTransform {
//   transform(text: string, [search]): string {
//     return search ? text.replace(new RegExp(search, 'i'), `<span class="highlight">${search}</span>`) : text;
//   }
// }

@Pipe({ name: 'highlight' })
export class HighlightPipe implements PipeTransform {
  transform(text: string, search:any): string {
    var pattern = search.replace(/[\-\[\]\/\{\}\(\)\*\+\?\.\\\^\$\|]/g, "\\$&");
    pattern = pattern.split(' ').filter((t:any) => {
      return t.length > 0;
    }).join('|');
    var regex = new RegExp(pattern, 'gi');

    //return search ? text.replace(regex, (match) => `<span style=\'background-color: yellow;\'>${match}</span>`) : text;
    return search ? text.replace(regex, (match) => `<mark>${match}</mark>`) : text;
  }
}

/** Usage: 
* <input type="text" [(ngModel)]="filter">
* <div [innerHTML]="myAwesomeText  | highlight : filter"></div>
* 
*/ 