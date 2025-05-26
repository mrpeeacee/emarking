import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { PracticeBenchmarkService } from 'src/app/services/project/standardisation/std-two/practice-assessment.service';
import { ActivatedRoute, Router } from '@angular/router';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { MatDialog } from '@angular/material/dialog';
import { QigUserModel } from 'src/app/model/project/qig';
import {
  PracticeQuestionDetails,
  StandardisationQualifyAssessmentModel,
  StandardisationScriptClass,
  WorkflowStatusTrackingModel,
} from 'src/app/model/standardisation/Assessment';
import { AlertService } from 'src/app/services/common/alert.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import {
  WorkflowProcessStatus,
  WorkflowStatus,
} from 'src/app/model/common-model';
import { first } from 'rxjs/operators';
import { AuthService } from 'src/app/services/auth/auth.service';
import { QuestionAnnotatorComponent } from 'src/app/modules/projects/marking-player/question-annotator/question-annotator.component';
@Component({
  templateUrl: './practice-assessment.component.html',
  styleUrls: ['./practice-assessment.component.css']
})
export class PracticeAssessmentComponent implements OnInit {
  PracticeScript!: StandardisationQualifyAssessmentModel;
  expandedscript!: StandardisationScriptClass[];
  practiceQusDtls!: PracticeQuestionDetails[];
  standScript!: StandardisationScriptClass;
  qigId!: number;
  qigName!: string;
  sumofDef!: number;
  sumofTot!: number;
  activeQig!: QigUserModel;
  process!: number;
  title!: string;
  selectedPoolType = '0';
  scriptloading: boolean = false;
  questionloading: boolean = false;
  Isopened = false;
  statusTracking: WorkflowStatusTrackingModel[] = [];
  intMessages: any = {
    Markingprocesspaused: '',
  };
  constructor(
    public translate: TranslateService,
    public commonService: CommonService,
    public services: PracticeBenchmarkService,
    private router: ActivatedRoute,
    private route: Router,
    private dialog: MatDialog,
    public Alert: AlertService,
    private authService: AuthService
  ) {
  }

  ngOnInit(): void {
    this.pageDesc();
    this.translate
      .get('Std.SetUp.Markingprocesspaused')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocesspaused = translated;
      });
    this.qigId = this.router.snapshot.params['qigid'];
    this.process = 0;
    let curntrole = this.authService.getCurrentRole();
    curntrole.forEach((role) => {
      if (role == 'TL' || role == 'ATL') {
        this.process = 2;
      }
      if (role == 'MARKER') {
        this.process = 3;
      }
    });

    this.qigName = this.router.snapshot.params['qig'];

    this.PracticeScript = {} as StandardisationQualifyAssessmentModel;
    this.PracticeScript.Scripts = [{}, {}, {}] as StandardisationScriptClass[];

    this.getPracticeScript();
    this.setPageTitle();
  }
  pageDesc() {
    this.translate.get('benchmark.pagedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
  }
  closeBtn() {
    if (this.process == 2) {
      this.route.navigate([`projects/dashboards/tl-atl`, this.qigId]);
    } else if (this.process == 3) {
      this.route.navigate([`projects/dashboards/marker`, this.qigId]);
    }
  }
  setPageTitle() {
    if (this.process == 2) {
      this.translate
        .get('benchmark.S2title')
        .subscribe((translated: string) => {
          this.commonService.setHeaderName(translated);
          this.title = translated;
        });
    } else if (this.process == 3) {
      this.translate
        .get('benchmark.S3title')
        .subscribe((translated: string) => {
          this.commonService.setHeaderName(translated);
          this.title = translated;
        });
    }
  }
  indexExpanded: number = -1;

  togglePanels(index: number) {
    if (!this.scriptloading) {
      this.indexExpanded = index == this.indexExpanded ? -1 : index;
    }
  }
  getPracticeScript() {
    this.scriptloading = true;
    if (this.process == 2) {
      this.services
        .GetS2PracticeScript(this.qigId)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data.Qigname == this.qigName) {
              this.PracticeScript = data;
            } else {
              this.route.navigateByUrl('/login');
            }
            if (this.PracticeScript.ProcessStatus == 4) {
              this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + this.PracticeScript.PauseRemarks+'.');
            }
          },
          error: (a: any) => {
            throw (a);
          },
          complete: () => {
            this.scriptloading = false;
          },
        });
    } else if (this.process == 3) {
      this.services
        .GetS3PracticeScript(this.qigId)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data.Qigname == this.qigName) {
              this.PracticeScript = data;
            } else {
              this.route.navigateByUrl('/login');
            }
            if (this.PracticeScript.ProcessStatus == 4) {
              this.Alert.warning(this.intMessages.Markingprocesspaused + '<br>' + 'Remarks : ' + this.PracticeScript.PauseRemarks+'.');
            }
          },
          error: (a: any) => {
            throw (a);
          },
          complete: () => {
            this.scriptloading = false;
          },
        });
    }
  }

  PracticeQuestionDetails(scriptId: number, iscompleted: boolean) {
    this.questionloading = true;
    this.practiceQusDtls = [{}, {}, {}] as PracticeQuestionDetails[];
    if (this.process == 2) {
      this.services
        .S2PracticeQuestionDetails(this.qigId, scriptId, iscompleted)
        .pipe(first())
        .subscribe({
          next: (result: any) => {
            this.practiceQusDtls = result;
            this.sumofDef = this.practiceQusDtls.reduce(
              (sum, current) => sum + current.DefenetiveMarks,
              0
            );
            this.sumofTot = this.practiceQusDtls.reduce(
              (sum, current) => sum + current.AwardedMarks,
              0
            );
          },
          error: (a: any) => {
            throw (a);
          },
          complete: () => {
            this.questionloading = false;
          },
        });
    } else if (this.process == 3) {
      this.services
        .S3PracticeQuestionDetails(this.qigId, scriptId, iscompleted)
        .pipe(first())
        .subscribe({
          next: (result: any) => {
            this.practiceQusDtls = result;
            this.sumofDef = this.practiceQusDtls.reduce(
              (sum, current) => sum + current.DefenetiveMarks,
              0
            );
            this.sumofTot = this.practiceQusDtls.reduce(
              (sum, current) => sum + current.AwardedMarks,
              0
            );
          },
          error: (a: any) => {
            throw (a);
          },
          complete: () => {
            this.questionloading = false;
          },
        });
    }
  }

  private initCategorise() {
    this.translate.get('benchmark.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
  }

  playerOpening: boolean = false;
  trailmarking(markingdata: IRecommedData) {
    if (!this.playerOpening) {
      this.playerOpening = true;
      this.services
        .Getqigworkflowtracking(this.qigId, AppSettingEntityType.QIG)
        .subscribe({
          next: (data: any) => {
            this.statusTracking = data;

            let val = this.statusTracking.findIndex(
              (a) =>
                a.WorkflowStatusCode == WorkflowStatus.Pause &&
                a.ProcessStatus == WorkflowProcessStatus.OnHold
            );
            if (val >= 0) {
              this.getPracticeScript();
            } else {
              markingdata.QigId = this.qigId;
              const dialogRef = this.dialog.open(QuestionAnnotatorComponent, {
                data: markingdata,
                panelClass: 'fullviewpop',
              });
              dialogRef.afterClosed().subscribe((result) => {
                this.pageDesc();
                this.initCategorise();
                this.getPracticeScript();
                this.setPageTitle();
              });
            }
          },
          error: (err: any) => {
            throw (err);
          },
          complete: () => {
            this.playerOpening = false;
          }
        });
    }
  }
}
