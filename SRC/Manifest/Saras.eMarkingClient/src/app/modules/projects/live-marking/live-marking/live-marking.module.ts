import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { LiveMarkingRoutingModule } from './live-marking-routing.module';
import { LiveMarkingComponent, FormatTimePipe } from './live-marking.component';
import { MarkingPlayerModule } from '../../marking-player/marking-player.module';
import { SharedModule } from 'src/app/modules/shared/shared.module';

import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatMomentDateModule } from "@angular/material-moment-adapter";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { TranslateService, TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { DownloadedscriptuserlistComponent } from '../downloadedscriptuserlist/downloadedscriptuserlist.component';

export function createTranslateLoader(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/project/live-marking/', '.json');
}

@NgModule({
  declarations: [
    LiveMarkingComponent,
    FormatTimePipe,
    DownloadedscriptuserlistComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    LiveMarkingRoutingModule,
    MarkingPlayerModule,
    FormsModule,
    MatInputModule,
    MatDatepickerModule,
    MatMomentDateModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient]
      },
      isolate: true
    })
  ]
})
export class LiveMarkingModule { 
  constructor(translate: TranslateService) {
    translate.addLangs(["en", "fr"]);
    translate.setDefaultLang('en');
    translate.use('en');
  }
}
