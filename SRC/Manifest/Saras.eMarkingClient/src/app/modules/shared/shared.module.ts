import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatInputModule } from '@angular/material/input';
import { MatRadioModule } from '@angular/material/radio';
import { NgbPaginationModule, NgbAlertModule, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { MatFormFieldModule } from '@angular/material/form-field';
import { TranslateModule } from '@ngx-translate/core';
import { InfoComponent } from './info/info.component';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { MatMenuModule } from '@angular/material/menu';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatListModule } from '@angular/material/list';
import { MatNativeDateModule, DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS } from '@angular/material-moment-adapter';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { CdkAccordionModule } from '@angular/cdk/accordion';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { QigTabComponent } from './qig-tab/qig-tab.component';
import { ErrorpageComponent } from './errorpage/errorpage.component';
import { HtmlEditorComponent } from './html-editor/html-editor.component'
import { NgxEditorModule } from 'ngx-editor'; 
import {MatTreeModule} from '@angular/material/tree'; 
import {MatButtonModule} from '@angular/material/button';
import { SharedRoutingModule } from './shared-routing.module';
import { MarkerTreeViewComponent } from './marker-tree/marker-tree-view/marker-tree-view.component';
import { PerfectScrollbarModule } from 'ngx-perfect-scrollbar';
import { HighlightSearch } from 'src/app/modules/shared/HighlightSearch/HighlightSearch';
import { FileUploadComponent } from './file-upload/file-upload.component';
import { SafeHtmlPipe } from './safe-html.pipe';
import { PreloaderComponent } from './preloader/preloader.component';

export const DateFormats = {
  parse: {
    dateInput: ['DD/MM/YYYY']
  },
  display: {
    dateInput: 'DD/MM/YYYY',
    monthYearLabel: 'MMM YYYY',
    dateA11yLabel: 'LL',
    monthYearA11yLabel: 'MMMM YYYY',
  },
};

@NgModule({
  declarations: [
    InfoComponent,
    ConfirmationDialogComponent,
    QigTabComponent,
    ErrorpageComponent,
    HtmlEditorComponent,
    MarkerTreeViewComponent,
    HighlightSearch,
    FileUploadComponent,
    SafeHtmlPipe,
    PreloaderComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedRoutingModule,
    TranslateModule,
    MatCheckboxModule,
    MatSelectModule,
    MatDatepickerModule,
    MatInputModule,
    MatRadioModule,
    NgbPaginationModule,
    NgbAlertModule,
    NgbModule,
    MatFormFieldModule,
    CarouselModule,
    MatMenuModule,
    MatSlideToggleModule,
    MatListModule,
    MatNativeDateModule,
    MatExpansionModule,
    MatDialogModule,
    MatIconModule,
    MatMenuModule,
    CdkAccordionModule,
    NgxEditorModule,
    PerfectScrollbarModule,
    MatTreeModule,
    MatButtonModule
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatCheckboxModule,
    MatSelectModule,
    MatDatepickerModule,
    MatInputModule,
    MatRadioModule,
    NgbPaginationModule,
    NgbAlertModule,
    NgbModule,
    MatFormFieldModule,
    TranslateModule,
    InfoComponent,
    CarouselModule,
    MatSlideToggleModule,
    MatListModule,
    MatNativeDateModule,
    MatExpansionModule,
    MatDialogModule,
    MatIconModule,
    MatMenuModule,
    CdkAccordionModule,
    PerfectScrollbarModule,
    QigTabComponent,
    ErrorpageComponent,
    NgxEditorModule,
    HtmlEditorComponent,
    MarkerTreeViewComponent,
    HighlightSearch,
    FileUploadComponent,
    SafeHtmlPipe,
    PreloaderComponent
  ],
  providers: [
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },

    { provide: MAT_DATE_FORMATS, useValue: DateFormats },
  ]
})
export class SharedModule { }
