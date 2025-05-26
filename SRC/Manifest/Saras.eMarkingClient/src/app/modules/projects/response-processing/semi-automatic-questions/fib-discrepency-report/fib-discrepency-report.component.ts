import { Component, OnInit, Inject, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialog } from '@angular/material/dialog';
import { AlertService } from 'src/app/services/common/alert.service';
import { SemiAutomaticQuestionsService } from 'src/app/services/project/response-processing/semi-automatic.service';
import { FibDiscrepencyReportModel, MarkerDetails, DiscrepencyNormalizeScoreModel } from 'src/app/model/project/response-processing/semi-automatic-model';
import { first } from 'rxjs/operators';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';


@Component({
  selector: 'emarking-fib-discrepency-report',
  templateUrl: './fib-discrepency-report.component.html',
  styleUrls: ['./fib-discrepency-report.component.css']
})
export class FIBDiscrepencyReportComponent implements OnInit {
  @ViewChild('closeDiscrepancy') closemodalpopup: any;

  ObjFibDiscrepencyReportModel!: FibDiscrepencyReportModel;
  fibMarkerDetailslst!: MarkerDetails[];
  ProjectQuestionId!: number;
  CandidateAnswer!: string;
  QigId!: number;
  ID!: number;
  discrepencyLoading: boolean = false;
  ActiveMarkers: boolean = false;
  scores: any;

  constructor(public translate: TranslateService,
    public commonService: CommonService,
    public Alert: AlertService,
    public semiautomaticquestionsservice: SemiAutomaticQuestionsService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<FIBDiscrepencyReportComponent>, private dialog: MatDialog) {

  }

  ngOnInit(): void {

    this.getDescripency(this.data.candidateAnswer, this.data.projectQuestionId, this.data.qigId, this.data.Id);
  }

  getDescripency(CandidateAnswer: string, ProjectQuestionId: number, QigId: number, Id: number) {

    this.discrepencyLoading = true;
    this.CandidateAnswer = CandidateAnswer;
    this.ProjectQuestionId = ProjectQuestionId;
    this.QigId = QigId;
    this.ID = Id;
    this.semiautomaticquestionsservice.GetDiscrepancyReportFIB(CandidateAnswer, ProjectQuestionId).pipe(first()).subscribe({
      next: (data: any) => {
        this.ObjFibDiscrepencyReportModel = data;
      },
      error: (err: any) => {
        this.translate
          .get('SetUp.SemiAutomatic.errorDiscrepencyReport')
          .subscribe((translated: string) => {
            this.Alert.clear();
            this.Alert.error(translated);
          });
        this.discrepencyLoading = false;
        throw (err);
      }, complete: () => {
        this.discrepencyLoading = false;
      }
    });
  }

  clickMethod(evnt: any) {
    this.dialogRef.close({ status: 0, data: this.data.isSaveClicked });
  }

  errorDiscrepencyReport() {
    this.translate
      .get('SetUp.SemiAutomatic.errorDiscrepencyReport')
      .subscribe((translated: string) => {
        this.Alert.clear();
        this.Alert.error(translated);
      });
  }

  ViewMarkersDetails(Awardedmarks: any) {
    this.ActiveMarkers = true;
    this.scores = Awardedmarks;
    this.fibMarkerDetailslst = this.ObjFibDiscrepencyReportModel.FibMarkerDetails.filter(a => a.MarksAwarded == Awardedmarks)

  }

  updateNormalizedScore(responseText: any, Normalizedscore: any, QuestionMarks: any) {
    this.discrepencyLoading = true;
    let objNormalizedscore = new DiscrepencyNormalizeScoreModel();
    objNormalizedscore.MarksAwarded = Normalizedscore;
    objNormalizedscore.ProjectQuestionID = this.ProjectQuestionId;
    objNormalizedscore.ResponseText = responseText;
    objNormalizedscore.questionMarks = QuestionMarks;

    const handleSuccess = (translated: string) => {
      this.data.isSaveClicked = 1;
      this.closemodalpopup.nativeElement.click();
      this.Alert.clear();
      this.Alert.success(translated);
    };

    const handleSecurityError = (translated: string) => {
      this.Alert.clear();
      this.Alert.warning(translated);
    };

    const handleFailedNormalized = (translated: string) => {
      this.Alert.clear();
      this.Alert.warning(translated);
      this.getDescripency(this.CandidateAnswer, this.ProjectQuestionId, this.QigId, this.ID);
    };

    const handleError = (translated: string) => {
      this.Alert.clear();
      this.Alert.error(translated);
    };

    const handleComplete = () => {
      this.discrepencyLoading = false;
    };
 
    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, { 
      data: {
        message: "Are you sure you want to Normalise?"
      },
      width: '500px' ,
    
    });
    

    confirmDialog.afterClosed().subscribe(results => {
      if (results) 
      {
    this.semiautomaticquestionsservice.UpdateNormaliseScore(objNormalizedscore)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data == 'S001') {
            this.translate
              .get('SetUp.SemiAutomatic.UpdatNormalizedScore')
              .subscribe(handleSuccess);
          } else if (data == "SERROR") {
            this.translate
              .get('SetUp.SemiAutomatic.Securityerror')
              .subscribe(handleSecurityError);
          } else {
            this.translate
              .get('SetUp.SemiAutomatic.FailedNormalized')
              .subscribe(handleFailedNormalized);
          }
        },
        error: (err: any) => {
          this.translate
            .get('SetUp.SemiAutomatic.errorDiscrepencyReport')
            .subscribe(handleError);
          this.discrepencyLoading = false;
          throw err;
        },
        complete: handleComplete
      });}
    else{
      this.getDescripency(this.data.candidateAnswer, this.data.projectQuestionId, this.data.qigId, this.data.Id);
    }})
  }


  validateNumber(event: any) {
    if (event.key != "ArrowUp" && event.key != "ArrowDown") {
      event.preventDefault();
    }

  }
  BacktoDiscrepancy() {
    this.ActiveMarkers = false;
  }

  SaveAcceptDecrepancy(ResponseText: any,NormalizeScore:number,objmarkerId:MarkerDetails[]) {

    var scriptIds = objmarkerId.map(m => m.ScriptID);

    let AcceptDescrepancyModel = new DiscrepencyNormalizeScoreModel();
    AcceptDescrepancyModel.ProjectQuestionID = this.ProjectQuestionId;
    AcceptDescrepancyModel.ResponseText = ResponseText;
    AcceptDescrepancyModel.QigId = this.QigId;
    AcceptDescrepancyModel.Id = this.ID;
    AcceptDescrepancyModel.ScriptIds=scriptIds;

    if (ResponseText == null || ResponseText == "") {
      this.translate
        .get('SetUp.SemiAutomatic.AcceptSuccessWarning')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
      return
    } else if(NormalizeScore > 0){
      this.translate
        .get('SetUp.SemiAutomatic.NormarlizeAcceptWarning')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
      return

    }


    const handleSuccess = (translated: string) => {
      this.data.isSaveClicked = 1;
      this.closemodalpopup.nativeElement.click();
      this.Alert.clear();
      this.Alert.success(translated);
    };

    const handleSecurityError = (translated: string) => {
      this.Alert.clear();
      this.Alert.warning(translated);
    };

    const handleFailedNormalized = (translated: string) => {
      this.Alert.clear();
      this.Alert.warning(translated);
      this.getDescripency(this.CandidateAnswer, this.ProjectQuestionId, this.QigId, this.ID);
    };

    const handleError = (translated: string) => {
      this.Alert.clear();
      this.Alert.error(translated);
    };

    const handleComplete = () => {
      this.discrepencyLoading = false;
    };
    
    const confirmDialog1 = this.dialog.open(ConfirmationDialogComponent, { 
      data: {
        message: "Are you sure you want to Accept Decrepancy?"
      },
      width: '500px' ,
    
    });
    

    confirmDialog1.afterClosed().subscribe(results => {
      if (results) 
        { 
          this.semiautomaticquestionsservice.UpdateAcceptDescrepancy(AcceptDescrepancyModel)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data == 'A001') {
              this.translate
                .get('SetUp.SemiAutomatic.AcceptSuccess')
                .subscribe(handleSuccess);
            } else if (data == "SERROR") {
              this.translate
                .get('SetUp.SemiAutomatic.Securityerror')
                .subscribe(handleSecurityError);
            } else {
              this.translate
                .get('SetUp.SemiAutomatic.FailedAcceptdescrepancy')
                .subscribe(handleFailedNormalized);
            }
          },
          error: (err: any) => {
            this.translate
              .get('SetUp.SemiAutomatic.errorDiscrepencyReport')
              .subscribe(handleError);
            this.discrepencyLoading = false;
            throw err;
          },
          complete: handleComplete
        });}
        else{
          this.getDescripency(this.data.candidateAnswer, this.data.projectQuestionId, this.data.qigId, this.data.Id);
        }
      })
   


  }

}
