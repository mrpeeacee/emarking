import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MarkingPlayerComponent } from './marking-player/marking-player.component';
import { QuestionAnnotatorComponent } from './question-annotator/question-annotator.component';
import { ViewDownloadMarksSchemeComponent } from './view-download-marks-scheme/view-download-marks-scheme.component';
import { FormsModule } from '@angular/forms';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { PerfectScrollbarConfigInterface, PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG } from 'ngx-perfect-scrollbar';
import { MatDialogModule } from '@angular/material/dialog';
import {MatMenuModule} from '@angular/material/menu';
import {MatButtonModule} from '@angular/material/button';
import { SharedModule } from '../../shared/shared.module';
import { ViewCandidateResponseComponent } from './view-candidate-response/view-candidate-response.component';

const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};


@NgModule({
  declarations: [
    MarkingPlayerComponent,
    QuestionAnnotatorComponent,
    ViewDownloadMarksSchemeComponent,
    ViewCandidateResponseComponent    
  ],
  imports: [
    CommonModule,
    FormsModule,
    InfiniteScrollModule,
    PerfectScrollbarModule,
    MatDialogModule,
    MatMenuModule,
    MatButtonModule,
    SharedModule
  
  ],
  exports: [
    MarkingPlayerComponent,
    QuestionAnnotatorComponent
  ],
  providers: [
    {
      provide: PERFECT_SCROLLBAR_CONFIG,
      useValue: DEFAULT_PERFECT_SCROLLBAR_CONFIG
    }
  ]
})
export class MarkingPlayerModule { }
