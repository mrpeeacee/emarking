import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AutomaticQuestionsService } from 'src/app/services/project/response-processing/automatic.service';
import { AutomaticQuestionsModel, ModeratescoreModel } from 'src/app/model/project/response-processing/automatic-model';
import { first } from 'rxjs/operators';
import { AlertService } from 'src/app/services/common/alert.service';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';



@Component({
  selector: 'emarking-moderate-score',
  templateUrl: './moderate-score.component.html',
  styleUrls: ['./moderate-score.component.css']
})
export class ModerateScoreComponent implements OnInit {

  ProjectQuestionId!: number;
  allviewquestions: AutomaticQuestionsModel[] = [];
  automaticQuestionloading: boolean = true;
  intMessages: any = {
    Confirmwarning: ''
  };
  ChoiceIdentifier!: any;
  Remarks!: string;
  updatemoderateLoading: boolean = false;
  disableRemarks!: boolean;


  constructor(public translate: TranslateService,
    public commonService: CommonService,
    private route: Router,
    private router: ActivatedRoute,
    public automaticquestionsservice: AutomaticQuestionsService,
    public Alert: AlertService,
    private dialog: MatDialog) { }

  ngOnInit(): void {
    this.translate.get('SetUp.Automatic.ModerateTitle').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('SetUp.Automatic.ModeratePageDescription').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });

    this.translate.get('SetUp.Automatic.Moderatealertmessage').subscribe((translated: string) => {
      this.intMessages.Moderatealertmessage = translated;
    });
    this.translate.get('SetUp.Automatic.Moderatealertmessage1').subscribe((translated: string) => {
      this.intMessages.Moderatealertmessage1 = translated;
    });
    this.translate.get('Std.SetUp.Confirmwarning').subscribe((translated: string) => {
      this.intMessages.Confirmwarning = translated;
    });

    this.ProjectQuestionId = this.router.snapshot.params['pqid'];
    this.GetViewModerateAutomaticQuestions(this.ProjectQuestionId);

  }
  closeModerate() {
    this.route.navigateByUrl('projects/automatic-questions/'+ this.ProjectQuestionId);
  }

  GetViewModerateAutomaticQuestions(parentQuestionId: number) {
    this.automaticQuestionloading = true;
    this.automaticquestionsservice.GetViewModerateAutomaticQuestions(parentQuestionId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.allviewquestions = data;
          if (this.allviewquestions[0].ChoiceList[0].Choices.find(a => a.markingType == 2)) {
            this.Remarks = this.allviewquestions[0].ChoiceList[0].Choices[this.allviewquestions[0].ChoiceList[0].Choices.findIndex(a => a.IsCorrectAnswer)].Remarks;
            this.disableRemarks = true;
          }

        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.automaticQuestionloading = false;
        }
      });
  }
  SaveUpdateModerateScore(objquestions: any) {
    let isCorrect = this.allviewquestions[0].ChoiceList[0].Choices.filter((a: { ChoiceIdentifier: string; }) => a.ChoiceIdentifier == this.ChoiceIdentifier);

    let Remoderate = this.allviewquestions[0].GlobalMarkingType;

    let objModeratescore = new ModeratescoreModel();
    if (isCorrect.length > 0) {

      if (this.Remarks == '' || this.Remarks == null) {
        this.translate
          .get('SetUp.Automatic.Remarksnullwarning')
          .subscribe((translated: string) => {
            this.Alert.clear();
            this.Alert.warning(translated);
          });
        return
      }

      if (this.Remarks.trim() == '' || this.Remarks.trim() == null) {
        this.translate
          .get('SetUp.Automatic.Remarksnullwarning')
          .subscribe((translated: string) => {
            this.Alert.clear();
            this.Alert.warning(translated);
          });
        return;

      }

      for (let a of isCorrect) {
        objModeratescore.IsCorrectAnswer = true;
        objModeratescore.Remarks = this.Remarks;
        objModeratescore.ProjectQuestionId = objquestions.ProjectQuestionId;
        objModeratescore.MarkingType = 2;
        objModeratescore.ResponseText = a.ChoiceIdentifier;
         objModeratescore.QuestionCode = objquestions.QuestionCode;

      }

      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: !Remoderate ? this.intMessages.Moderatealertmessage:this.intMessages.Moderatealertmessage1
        }
      });
      confirmDialog.afterClosed().subscribe(result => {
        this.Alert.clear();
        if (result === true) {
          this.Alert.clear();
          this.updatemoderateLoading = true;
          this.automaticquestionsservice.UpdateModeratescore(objModeratescore)
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data == "P001" && !Remoderate) {
                  this.translate
                    .get('SetUp.Automatic.Updatemoderatesuceessmessage')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.Alert.success(translated);
                      this.GetViewModerateAutomaticQuestions(this.ProjectQuestionId);
                    });
                }
                else if (data == "P001" && Remoderate) {
                  this.translate
                    .get('SetUp.Automatic.Updatemoderatesuceessmessage1')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.Alert.success(translated);
                      this.GetViewModerateAutomaticQuestions(this.ProjectQuestionId);
                    });
                }
                else if (data == "SERROR") {
                  this.translate
                    .get('SetUp.Automatic.Securityerror')
                    .subscribe((translated: string) => {
                      this.Alert.clear();
                      this.Alert.warning(translated);
                    });
                }
              },

              complete: () => {
                this.updatemoderateLoading = false;
              },

            });
        }
      });
    }
    else {
      this.translate
        .get('SetUp.Automatic.choicetexterror')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.updatemoderateLoading = false;
          this.Alert.warning(translated);
        });
    }
  }


}
