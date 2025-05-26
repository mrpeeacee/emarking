import { Component, OnInit, ViewChild } from '@angular/core';
import {
  MarkScheme,
  QuestionTag,
  QuestionText,
} from 'src/app/model/project/mark-scheme/mark-scheme-model';
import { MarkSchemeService } from 'src/app/services/project/mark-scheme/mark-scheme.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { first } from 'rxjs/operators';
import { MarkSchemeQuestionsComponent } from '../mark-scheme-questions/mark-scheme-questions.component';
import { QigService } from 'src/app/services/project/qig.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';

@Component({
  templateUrl: './mark-scheme-library.component.html',
  styleUrls: ['./mark-scheme-library.component.css'],
})
export class MarkSchemeLibraryComponent implements OnInit {
  @ViewChild('qnspopmodel') modalClose: any;
  markSchemes: MarkScheme[] = [];
  schemeId!: MarkScheme;
  question: QuestionTag[] = [];
  markSchemeId!: number;
  status: any;
  displayConfirmStyle!: string;
  selectedQuestions: QuestionTag[] = [];
  PanelOpenState: boolean = false;
  questionText!: QuestionText;
  qsncount!: number;
  isClosed: any;

  constructor(
    private schemeservice: MarkSchemeService,
    public Alert: AlertService,
    private dialog: MatDialog,
    public commonService: CommonService,
    public translate: TranslateService,
    public qigservice: QigService
  ) {}

  intMessages: any = {
    QustagSuccss: '',
    SelQus: '',
    ErrWhlTag: '',
    ConDelete: '',
    SelMrkSch: '',
    TrlQsn:'',
  };

  ngOnInit(): void {
    this.Getqigworkflowtracking();
    this.translate.get('mark.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('mark.pgedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.translate.get('mark.qustag').subscribe((translated: string) => {
      this.intMessages.QustagSuccss = translated;
    });
    this.translate.get('mark.selqus').subscribe((translated: string) => {
      this.intMessages.SelQus = translated;
    });
    this.translate.get('mark.errtag').subscribe((translated: string) => {
      this.intMessages.ErrWhlTag = translated;
    });
    this.translate.get('mark.condel').subscribe((translated: string) => {
      this.intMessages.ConDelete = translated;
    });
    this.translate.get('mark.selmark').subscribe((translated: string) => {
      this.intMessages.SelMrkSch = translated;
    });
    this.translate.get('mark.trailqsn').subscribe((translated: string) => {
      this.intMessages.TrlQsn = translated;
    });
    this.getAllMarkSchemes();
  }

  getQuestions(maxMark: any, markschmeid: number) {
    this.markSchemeId = markschmeid;
    this.selectedQuestions = [];
    this.schemeservice
      .getQuestions(maxMark, this.markSchemeId)
      .subscribe((data) => {
        this.question = data;
      });
  }

  selectQuestion(selQns: QuestionTag) {
    let qnsIndex = this.selectedQuestions.findIndex(
      (a) => a.ProjectQuestionId == selQns.ProjectQuestionId
    );
    if (qnsIndex < 0) {
      selQns.MarkSchemeId = this.markSchemeId;
      this.selectedQuestions.push(selQns);
    }
  }

  TagQnsToMarkScheme() {
    if (this.selectedQuestions.length > 0) {
      this.schemeservice
        .TagQnsToMarkScheme(this.selectedQuestions)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data == 'SU001') {
              this.getAllMarkSchemes();
              this.Alert.success(this.intMessages.QustagSuccss);
              this.modalClose.nativeElement.click();
              this.selectedQuestions = [];
            } else if (data == 'MANDFD') {
              this.Alert.warning(this.intMessages.SelQus);
            } else if (data == 'TQ001'){
              this.Alert.warning(this.intMessages.TrlQsn);
            } else {
              this.Alert.error(this.intMessages.ErrWhlTag);
            }
          },
          error: (a: any) => {
            throw(a);
          },
        });
    } else {
      this.Alert.warning(this.intMessages.SelQus);
    }
  }

  getAllMarkSchemes() {
    this.schemeservice.getAllMarkSchemes().subscribe((data) => {
      this.markSchemes = data;
    });
  }

  deleteMarkScheme() {
    let selectedScheme: number[] = [];
    this.markSchemes.forEach((scme) => {
      if (scme.Selected) {
        selectedScheme.push(scme.ProjectMarkSchemeId);
      }
    });
    if (selectedScheme.length > 0) {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: this.intMessages.ConDelete,
        },
      });
      confirmDialog.afterClosed().subscribe((res) => {
        if (res === true) {
          this.schemeservice
            .deleteMarkScheme(selectedScheme)
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data == 'SU001') {
                  this.getAllMarkSchemes();
                  this.Alert.success('Mark Scheme deleted successfully.');
                } else if (data == 'SALRMP') {
                  this.Alert.warning(
                    'Mark Scheme is already tagged to question.'
                  );
                } else if (data == 'MANDFD') {
                  this.Alert.warning('Please select mandatory field.');
                } else {
                  this.Alert.error('Error while deleting the mark scheme.');
                }
              },
              error: (a: any) => {
                throw(a);
              },
            });
        }
      });
    } else {
      this.Alert.warning(this.intMessages.SelMrkSch);
    }
  }

  expandQnsPanel(qns: QuestionTag) {
    this.question.forEach((qnselement) => {
      if (qnselement.QuestionId == qns.QuestionId) {
        qns.PanelOpenState = !qns.PanelOpenState;
      } else {
        qns.PanelOpenState = false;
      }
    });
  }

  getQuestionText(qnsId: number) {
    this.questionText = {} as QuestionText;
    this.selectedQuestions = [];
    this.schemeservice.getQuestionText(qnsId).subscribe((data) => {
      this.questionText = data;
    });
  }
  expandavlquestions() {
    this.question.forEach((element) => {
      element.PanelOpenState = false;
    });
  }

  RedirectToTrialMarking() {
    const dialogRef = this.dialog.open(MarkSchemeQuestionsComponent, {
      panelClass: 'queviewpop',
    });
    dialogRef.afterClosed().subscribe(() => {
      this.translate.get('mark.pgedesc').subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
      this.getAllMarkSchemes();
    });
  }

  Getqigworkflowtracking() {
    this.qigservice
      .Getqigworkflowtracking(0, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          let WorkFlowStatusTracking = data;
          this.isClosed = WorkFlowStatusTracking.find(
            (x: any) => x.ProjectStatus
          )?.ProjectStatus;
          if (this.isClosed == 3) {
            this.translate
              .get('This project is closed')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
        }
      });
  }
}
