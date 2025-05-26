import { Pipe, PipeTransform } from '@angular/core';


@Pipe({
  name: 'highlight'
})
export class HighlightSearch implements PipeTransform {
  transform(value: string, args: string, IsActive: any): any {
    if (args && value) {

      if (IsActive == 1) {
        value = value.replace("<div id='" + args + "' class='blankDiv'>", "<div id='" + args + "' class='blankDiv highlighted-text'>");
      } else {
        value = value.replace("<span id='" + args + "'>", "<span id='" + args + "' class='highlighted-text'>");
      }

    }
    return value;
  }

}
