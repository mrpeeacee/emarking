import {
  Component,
  Inject,
  OnInit,
  ViewChild,
  HostListener,
} from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialogRef,
  MatDialog,
} from '@angular/material/dialog';
import { Router } from '@angular/router';
import { first, interval, Subscription, timer } from 'rxjs';

import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import {
  WorkflowStatus,
  WorkflowProcessStatus,
} from 'src/app/model/common-model';
import { ScriptQuestionModel } from 'src/app/model/project/scriptquestion';
import {
  UserScriptMarking,
  UserScriptResponseModel,
  Scorecompnentdetails,
  QuestionScoreComponentMarkingDetail,
  QuestionUserResponseMarkingDetailsmodel,
  MarkingScriptTimeTracking,
} from 'src/app/model/project/trialmarking';
import { WorkflowStatusTrackingModel } from 'src/app/model/standardisation/Assessment';
import { IRecommedData } from 'src/app/model/standardisation/recommendation';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { QigService } from 'src/app/services/project/qig.service';
import { TrialmarkingService } from 'src/app/services/project/trialmarking.service';
import { MarkingPlayerComponent } from '../marking-player/marking-player.component';

import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';

@Component({
  selector: 'emarking-question-annotator',
  templateUrl: './question-annotator.component.html',
  styleUrls: ['./question-annotator.component.css'],
})
export class QuestionAnnotatorComponent implements OnInit {
  autosaveSubscription = new Subscription();
  timetakenSubscription = new Subscription();
  @ViewChild(MarkingPlayerComponent) child!: MarkingPlayerComponent;
  @HostListener('paste', ['$event']) blockPaste(e: KeyboardEvent) {
    e.preventDefault();
  }

  @HostListener('copy', ['$event']) blockCopy(e: KeyboardEvent) {
    e.preventDefault();
  }

  @HostListener('cut', ['$event']) blockCut(e: KeyboardEvent) {
    e.preventDefault();
  }

  @HostListener('window:beforeunload', ['$event'])
  beforeUnloadHandler(event: any) {
    let mode = 2;
    if (this._isviewmode) {
      mode = 1;
    }
    this.saveMarkingScriptTimeTracking(mode, 4);
  }

  templetjson: any =
    '{ "pen" : true ,"text" : "false", "line" : { "horizontal" : "false", "vertical"  : "false","curlyhorixontal" : "false" ,"curlyvertical": "false"}}';
  _projectquestionResponseid!: number;
  isLoading: boolean = false;
  _scriptid!: number;
  curid!: number;
  _change: boolean = false;
  UserID!: number;
  public questiondet: any[] = [];
  qidname?: string;
  validateannotation?: boolean = false;
  annotationcolor: string = '';
  _isviewmode: boolean = false;
  scriptname?: string;
  firstclick: boolean = false;
  totalmarks?: number;
  recminhide: boolean = false;
  _ProjectQuestionId?: number;
  scoringcompbandid?: number | null;
  _IsScoreComponentbandExists?: boolean = false;
  awardedmarks?: number;
  _markedtype?: number;
  _stepvalue?: number = 1;
  ProjectUserRoleID?: number;
  WorkFlowStatusTracking: WorkflowStatusTrackingModel[] = [];
  _UserScriptMarking!: UserScriptMarking[];
  _UserScriptResponseModel!: UserScriptResponseModel[];
  _QuestionScoreComponentMarkingDetail!: QuestionScoreComponentMarkingDetail[];
  _QuestionUserResponseMarkingDetails!: QuestionUserResponseMarkingDetailsmodel[];
  _QuestionUserResponseMarkingTrace!: QuestionUserResponseMarkingDetailsmodel[];
  _MarkingScriptTimeTracking!: MarkingScriptTimeTracking[];
  _scoreassigned: number = 0;
  _IRecommedData?: IRecommedData;
  _userscriptid?: number = 0;
  max_score?: number;
  _marksschemeexist?: boolean;
  _marksschmemesubmit?: boolean;
  maxmarks_score?: number;
  min_score?: number = 0;
  counter = 0;
  tick = 1000;
  score_awarded?: any = null;
  public scorecompnentdetail!: Scorecompnentdetails[];
  public scorecompnentde!: Scorecompnentdetails[];
  public scorecompnentBand?: Scorecompnentdetails[];

  deptment: any;
  _selectedid: number = -1;

  stasticsloading: boolean = false;
  isDisabled: boolean = false;

  isDisabledsubmit: boolean = true;

  nxtdisabled: boolean = false;
  prevdisabled: boolean = false;
  isquestiondet: boolean = false;
  UserScriptMarkingRefId?: number;
  QigId!: number;
  state = 'closed';
  toggletopmenu: boolean = false;
  isShown = false;
  public _banddet: any[] = [];
  constructor(
    public trialmarkingService: TrialmarkingService,
    private qigservice: QigService,

    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: IRecommedData,
    public dialogRef: MatDialogRef<QuestionAnnotatorComponent>,
    public router: Router,
    public commonService: CommonService,
    public Alert: AlertService
  ) {}
  public prjquest!: ScriptQuestionModel[];
  _markedby: any;

  _workflowstatusid: any;
  ngAfterViewInit() {
    window.setTimeout(() => {
      this.ngafterinit();
    }, 1000);
  }
  ngafterinit() {
    this.gettimecount();
    this.stasticsloading = true;
    this.firstclick = true;
    this.child._stasticsloading = true;
    this.child.scorecompnentdetail1 = [];
    this.child._IsScoreComponentExists = null;
    if (this.data == undefined) {
      this.Alert.info('Data undefined');

      location.reload();
    } else {
      this._scriptid = this.data.ScriptId;
      this.UserID = 0;
      if (this.data.Markedby == undefined) {
        this._markedby = null;
      } else {
        this._markedby = this.data.Markedby;
      }
      if (this.data.UserScriptMarkingRefId != null) {
        this.UserScriptMarkingRefId = this.data.UserScriptMarkingRefId;
      } else {
        this.UserScriptMarkingRefId = 0;
      }
      if (this.data.Workflowid == undefined) {
        this._workflowstatusid = 0;
      } else {
        this._workflowstatusid = this.data.Workflowid;
      }
      if (this.data.IsViewMode != undefined) {
        if (this.data.IsViewMode) {
          this.child.isDisabled = true;
          this.child.isauto = true;
          this.child.isDisabledband = true;
          this.isDisabledsubmit = true;
          this.child.isDisabledSave = true;
          this.child.isDisabledsc = true;
          if (
            this.data.Markedby == undefined ||
            this.data.Markedby == 0 ||
            this.data.Markedby == null
          ) {
            this.Alert.info(
              'Please Send valid markedby details , Markedby cannot be null or 0 or undefined .  Markedby data sent is : ' +
                this.data.Markedby
            );
            this.dialogRef.close();
          }
        }
      }

      if (this.data.QigId != undefined) {
        if (this.data.QigId != null) {
          this.QigId = this.data.QigId;
          this.trialmarkingService
            .Validateannotations(this.data.QigId, AppSettingEntityType.QIG)
            .pipe(first())
            .subscribe({
              next: (data: any) => {
                if (data) {
                  if (data.annotaion == 'TRUE') {
                    this.validateannotation = true;
                  } else {
                    this.validateannotation = false;
                  }
                  this.annotationcolor = data.color;
                  if (data.discrete == 'TRUE') {
                    this.child.discrete = true;
                  }

                  this.child.ProjectUserRoleId = data.ProjectUserRoleID;
                  this.child.annotationpath = data.annotationpathname;
                  this.child.getannotations();
                  if (data.autosavetimeperiod != null) {
                    if (this.data.IsViewMode != undefined) {
                      if (!this.data.IsViewMode) {
                        this.autosaveSubscription = interval(
                          data.autosavetimeperiod
                        ).subscribe((x) => {
                          this.autosave();
                        });
                      }
                    }
                  }
                }
              },
              error: (a: any) => {
                throw a;
              },
            });
        }
      }

      this.getquestionid(true);
    }
  }
  ngOnInit(): void {
    if (this.data.IsViewMode != undefined) {
      if (this.data.IsViewMode) {
        this.isDisabled = true;
        this._isviewmode = true;
        this.child.isview = true;
        this.child.isDisabledSave = true;
        this.child.isDisabledband = true;
        this.isDisabledsubmit = true;
        this.child.isDisabledsc = true;
        this.child.isauto = true;
      }
    }
  }
  time_convert(num: any) {
    var hours = Math.floor(num / 60);
    var minutes = num % 60;
    return hours + ':' + minutes;
  }
  getminmaxnoscorecomponent(event: any) {
    const value = event.target.id;
    var checkrow = value.slice(0, -1);
    this.scorecompnentde = this.scorecompnentdetail.filter(
      (k) => k.ScoreComponentId == checkrow
    );
    const bandvalue = event.target.value;

    if (bandvalue != '-1') {
      var bamddet = this.scorecompnentde[0].band.filter(
        (k) =>
          k.BandId == bandvalue &&
          k.BandName == event.target.selectedOptions[0].text
      );

      var bani = value.slice(0, -1) + 'I';
      let dynband = document.getElementById(bani) as HTMLInputElement;
      dynband.min = bamddet[0].BandFrom.toString();
      dynband.max = bamddet[0].BandTo.toString();
      dynband.value = bamddet[0].BandFrom.toString();
    } else {
      var bani1 = value.slice(0, -1) + 'I';
      let dynband = document.getElementById(bani1) as HTMLInputElement;
      dynband.min = '0';
      dynband.max = this.scorecompnentde[0].MaxMarks.toString();
      dynband.value = '0';
    }
  }
  getminmaxno(event: any) {
    const value = event.target.value;
    if (value != '-1') {
      var banddet = this._banddet.filter((k) => k.id == value)[0];
      this.min_score = banddet.BandFrom;
      this.max_score = banddet.BandTo;
      this.score_awarded = this.min_score;
    } else {
      this.min_score = 0;
      this.max_score = this.maxmarks_score;
    }
  }

  gettimecount() {
    this.timetakenSubscription = timer(0, this.tick).subscribe((count: any) => {
      ++this.counter;
    });
  }
  validatescoringcompsave() {
    for (let i = 0; i < this.child.scorecompnentdetail1.length; i++) {
    var text = document.getElementById(this.child.scorecompnentdetail1[i].ScoreComponentId + 'I') as HTMLSelectElement;

    if(text){
      text.classList.remove('red-border');
    }
  }

    for (let i = 0; i < this.child.scorecompnentdetail1.length; i++) {
      let bandvlaid = '0';

      if (
        (
          document.getElementById(
            this.child.scorecompnentdetail1[i].ScoreComponentId + 'S'
          ) as HTMLSelectElement
        ).options.length > 1
      ) {
        bandvlaid = (
          document.getElementById(
            this.child.scorecompnentdetail1[i].ScoreComponentId + 'S'
          ) as HTMLSelectElement
        ).options[1].value;
      }
      let bandid = (
        document.getElementById(
          this.child.scorecompnentdetail1[i].ScoreComponentId + 'S'
        ) as HTMLSelectElement
      ).value;
      let markssc = (
        document.getElementById(
          this.child.scorecompnentdetail1[i].ScoreComponentId + 'I'
        ) as HTMLSelectElement
      ).value;
      if (
        bandid == '-1' &&
        !this.child._IsScoreComponentbandExists &&
        bandvlaid != '0'
      ) {
        this.Alert.warning('Please select BAND.');
        return false;
      }
      if (markssc == '') {
       
        var text = document.getElementById(this.child.scorecompnentdetail1[i].ScoreComponentId + 'I') as HTMLSelectElement;

        if(text){
          text.focus();
          text.style.outline='none';
          text.classList.add('red-border');
        }

        this.Alert.warning(
          'Awarded marks cannot be empty and must be within the specified range.'
        );
        return false;
      }
    }
    if (this.validateannotation) {
      let _savedet = this.child.onsaveclick(this.child._selectedid);
      let markingdat = JSON.parse(_savedet);
      if (markingdat.Markings == '[]') {
        this.Alert.warning('Annotation is mandatory.');
        return false;
      }
    }
    return true;
  }
  
  validatesave() {
    var text =  document.getElementById('marks') as HTMLInputElement;
    //// if(text){
    ////  text.classList.remove('red-border');
    //// }
    var bandsel = document.getElementById('_banddet') as HTMLSelectElement;
    this.child.isDisabledSave = false;
    if (!this.child.discrete) {
      var banddetid = document.getElementById('_banddet') as HTMLSelectElement;
      let bandid = '-1';
      if (banddetid != null) {
        bandid = banddetid.value;
      }

      if (
        (this.child.score_awarded! != null &&
          this.child.score_awarded! < this.child.min_score!) ||
        this.child.score_awarded! > this.child.max_score!
      ) {
        if (banddetid != null) {
          this.Alert.warning('Scores must fall within the specified band range. Score Awarded: ' + this.child.score_awarded);

        }
        else{
          this.Alert.warning('Awarded marks cannot be empty and must be within the specified range. Marks Awarded: ' + this.child.score_awarded);
        }

        return false;
      }

      if (
        this.child.score_assigned === null ||
        this.child.score_assigned! > this.child.max_score! ||
        this.child.score_assigned < 0 || this.child.score_awarded === null || this.child.score_awarded === ""
      ) {

       var text =  document.getElementById('marks') as HTMLInputElement;


       if(text){
        text.focus();
        text.style.outline='none';
        text.classList.add('red-border');
       }

        this.Alert.warning(
          'Awarded marks cannot be empty and must be within the specified range.'
        );

        return false;
      }
      
      // else if (bandid == "-1" && this.child._marksschemeexist) {
      //   this.Alert.warning('Please select BAND.');
      //   return false;
      // }
      else if (this.validateannotation) {
        let _savedet = this.child.onsaveclick(this.child._selectedid);
        let markingdat = JSON.parse(_savedet);
        if (markingdat.Markings == '[]') {
          this.Alert.warning('Annotation is mandatory.');
          return false;
        }
      }
    } else if (this.child.discrete) {
      var qwardmarks = document.getElementById(
        'score_assigned'
      ) as HTMLInputElement;
      if (qwardmarks.innerHTML == '') {
        this.Alert.warning('Awarded marks cannot be null.');
        return false;
      }
    }
    if(bandsel != null){
      if(bandsel.value! === "-1"){
        this.Alert.warning('Please select band value.');
        return false;
      }
    }
    
    return true;
  }

  cancel(evnt: any) {
    if (this.data.IsViewMode) {
      this.dialogRef.close({ status: 0 });
    } else {
      this.recminhide = true;
      this.child.recminhide = true;
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message:
            'All unsaved data will be lost! are you sure you want to Cancel ?',
        },
        panelClass: 'confirmationpop',
      });
      confirmDialog.afterClosed().subscribe((res: any) => {
        this.recminhide = false;
        this.child.recminhide = false;
        if (res === true) {
          (document.getElementById('deleteall') as HTMLAnchorElement).click();
          let mode = 2;
          if (this._isviewmode) {
            mode = 1;
          }
          this.saveMarkingScriptTimeTracking(mode, 3);
          this.autosaveSubscription.unsubscribe();
          this.timetakenSubscription.unsubscribe();
          window.setTimeout(() => {
            this.dialogRef.close({ status: 0 });
          }, 700);
        }
      });
    }
  }

  saveMarkingScriptTimeTracking(_Mode: number, _Action: number) {
    this._MarkingScriptTimeTracking = [
      {
        Qigid: this.QigId,
        ProjectQuestionId: this._ProjectQuestionId!,
        UserScriptMarkingRefId: this._userscriptid!,
        WorkFlowStatusId: this._workflowstatusid,
        Mode: _Mode,
        Action: _Action,
        TimeTaken: this.time_convert(this.counter!),
      },
    ];
    if (this._MarkingScriptTimeTracking[0].UserScriptMarkingRefId != 0) {
      this.trialmarkingService
        .MarkingScriptTimeTracking(this._MarkingScriptTimeTracking[0])
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data) {
              this.counter = 0;
              this.timetakenSubscription.unsubscribe();
              this.gettimecount();
            }
          },
          error: (a: any) => {
            throw a;
          },
        });
    }
  }

  autosave() {
    this.isDisabledsubmit = true;
    if (!this.data.IsViewMode) {
      this._markedby = null;
      this.UserScriptMarkingRefId = 0;
      let bandid: any;
      let banddet = document.getElementById('_banddet') as HTMLSelectElement;
      if (banddet != null) {
        bandid = banddet.value;
      }

      let prjqid = this._projectquestionResponseid;
      let ProjectUserQuestionResponseID = this._UserScriptResponseModel.filter(
        (item) => item.ProjectUserQuestionResponseID == prjqid
      )[0].ProjectUserQuestionResponseID;
      this._UserScriptResponseModel = this._UserScriptResponseModel.filter(
        (item) => item.ProjectUserQuestionResponseID != prjqid
      );
      let qwardmarks: any;

      let marks = null;
      if (this.child._IsScoreComponentExists) {
        qwardmarks = document.getElementById('score_assigned') as HTMLElement;

        marks = qwardmarks.innerText == '' ? null : +qwardmarks.innerText;
      } else {
        qwardmarks = document.getElementById(
          'score_assigned'
        ) as HTMLInputElement;
        marks = qwardmarks.innerText == '' ? null : +qwardmarks.innerText;
      }

      let _savedet = this.child.onsaveclick(bandid);
      let Markingdata = JSON.parse(_savedet);
      if (Markingdata.bandid == -1) {
        Markingdata.bandid = null;
      }
      this._QuestionScoreComponentMarkingDetail = [];
      if (this.child._IsScoreComponentExists) {
        let markssc = null;
        for (let i = 0; i < this.child.scorecompnentdetail1.length; i++) {
          let bandidn = (
            document.getElementById(
              this.child.scorecompnentdetail1[i].ScoreComponentId + 'S'
            ) as HTMLSelectElement
          ).value;
          markssc = (
            document.getElementById(
              this.child.scorecompnentdetail1[i].ScoreComponentId + 'I'
            ) as HTMLSelectElement
          ).value;

          if (bandidn == '-1') {
            this.scoringcompbandid = null;
          } else {
            this.scoringcompbandid = +bandidn;
          }

          this.child._QuestionScoreComponentMarkingDetailmarking = [
            {
              UserScriptMarkingRefId: 0,
              QuestionUserResponseMarkingRefId: 0,
              ScoreComponentId:
                this.child.scorecompnentdetail1[i].ScoreComponentId,
              MaxMarks: this.child.scorecompnentdetail1[i].MaxMarks,
              AwardedMarks: markssc == '' ? null : +markssc,
              BandId: this.scoringcompbandid,
              MarkingStatus: 1,
              WorkflowStatusId: this._workflowstatusid,
              IsActive: true,
              IsDeleted: false,
              MarkedBy: 0,
            },
          ];
          this._QuestionScoreComponentMarkingDetail.push(
            this.child._QuestionScoreComponentMarkingDetailmarking[0]
          );
        }
      }
      this._QuestionUserResponseMarkingDetails = [
        {
          ScriptID: this._scriptid,
          ProjectQuestionResponseID: ProjectUserQuestionResponseID,
          CandidateID: 0,
          ScheduleUserID: 0,
          Annotation: Markingdata.Markings,
          //// ImageBase64: Markingdata.AnnotatedImagebase64,
          ImageBase64: '',
          Comments: Markingdata.GeneralComments,
          BandID: Markingdata.bandid,
          Marks: marks,
          IsActive: true,
          IsDeleted: false,
          MarkedBy: null,
          Markeddate: new Date(),
          MarkingStatus: 0,
          WorkflowstatusID: this._workflowstatusid,
          LastVisited: true,
          TotalMarks: this.totalmarks,
          Remarks: this.child.Remarks,
          ScoreComponentMarkingDetail:
            this._QuestionScoreComponentMarkingDetail,
          Timer: 0,
        },
      ];
      this.addtracedetails();
      this.trialmarkingService
        .ResponseMarking(
          this._QuestionUserResponseMarkingDetails,
          this.QigId,
          true
        )
        .pipe(first())
        .subscribe({
          next: (data1: any) => {
            if (data1) {
              this.getquestionid(false);
              let mode = 2;
              if (this._isviewmode) {
                mode = 1;
              }
              this.saveMarkingScriptTimeTracking(mode, 2);
            }
          },
          error: (a: any) => {
            throw a;
          },
        });
    }
  }

  savemarking() {
    this.child.isDisabledSave = true;
    this._markedby = null;
    this.UserScriptMarkingRefId = 0;
    let bandid: any;
    let banddet = document.getElementById('_banddet') as HTMLSelectElement;
    if (banddet != null) {
      bandid = banddet.value;
      this._selectedid = bandid;
    }

    let isvalid = false;

    var scr_asigned = document.getElementById('score_assigned') as HTMLElement;
    var awared_marks = 0;
    if (scr_asigned.innerHTML != null) {
      awared_marks = parseFloat(scr_asigned.innerHTML);
    }
    if (
      (this.child._responseText == null ||
        this.child._responseText == '-No Response(NR)-') &&
      awared_marks! > 0
    ) {
      this.Alert.warning('NR Marks value must be Zero.');
    } else if (this.child._IsScoreComponentExists) {
      isvalid = this.validatescoringcompsave();
    } else {
      isvalid = this.validatesave();
    }

    if (isvalid) {
      this.trialmarkingService
        .Getcatagarizedands1configureddetails(
          this.data.QigId,
          this.data.ScriptId,
          this._workflowstatusid
        )
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data) {
              if (data.s1complete) {
                this.Alert.warning('cannot process , S1 completed ');
              } else if (data.catagarized) {
                this.Alert.warning('cannot process , Already Catagorized');
              } else {
                let prjqid = this._projectquestionResponseid;
                let ProjectUserQuestionResponseID =
                  this._UserScriptResponseModel.filter(
                    (item) => item.ProjectUserQuestionResponseID == prjqid
                  )[0].ProjectUserQuestionResponseID;
                this._UserScriptResponseModel =
                  this._UserScriptResponseModel.filter(
                    (item) => item.ProjectUserQuestionResponseID != prjqid
                  );
                let qwardmarks: any;
                let marks: number;
                if (this.child._IsScoreComponentExists) {
                  qwardmarks = document.getElementById(
                    'score_assigned'
                  ) as HTMLElement;
                  marks = +qwardmarks.innerText;
                } else {
                  if (this.child.discrete) {
                    qwardmarks = document.getElementById(
                      'score_assigned'
                    ) as HTMLInputElement;
                    marks = +qwardmarks.innerText;
                  } else {
                    qwardmarks = document.getElementById(
                      'marks'
                    ) as HTMLInputElement;
                    marks = +qwardmarks.value;
                  }
                }

                let _savedet = this.child.onsaveclick(bandid);
                let Markingdata = JSON.parse(_savedet);
                if (Markingdata.bandid == -1) {
                  Markingdata.bandid = null;
                }
                this._QuestionScoreComponentMarkingDetail = [];
                if (this.child._IsScoreComponentExists) {
                  for (
                    let i = 0;
                    i < this.child.scorecompnentdetail1.length;
                    i++
                  ) {
                    let bandidn = (
                      document.getElementById(
                        this.child.scorecompnentdetail1[i].ScoreComponentId +
                          'S'
                      ) as HTMLSelectElement
                    ).value;
                    let markssc = (
                      document.getElementById(
                        this.child.scorecompnentdetail1[i].ScoreComponentId +
                          'I'
                      ) as HTMLSelectElement
                    ).value;

                    if (bandidn == '-1') {
                      this.scoringcompbandid = null;
                    } else {
                      this.scoringcompbandid = +bandidn;
                    }

                    this.child._QuestionScoreComponentMarkingDetailmarking = [
                      {
                        UserScriptMarkingRefId: 0,
                        QuestionUserResponseMarkingRefId: 0,
                        ScoreComponentId:
                          this.child.scorecompnentdetail1[i].ScoreComponentId,
                        MaxMarks: this.child.scorecompnentdetail1[i].MaxMarks,
                        AwardedMarks: +markssc,
                        BandId: this.scoringcompbandid,
                        MarkingStatus: 1,
                        WorkflowStatusId: this._workflowstatusid,
                        IsActive: true,
                        IsDeleted: false,
                        MarkedBy: 0,
                      },
                    ];
                    this._QuestionScoreComponentMarkingDetail.push(
                      this.child._QuestionScoreComponentMarkingDetailmarking[0]
                    );
                  }
                }

                this._QuestionUserResponseMarkingDetails = [
                  {
                    ScriptID: this._scriptid,
                    ProjectQuestionResponseID: ProjectUserQuestionResponseID,
                    CandidateID: 0,
                    ScheduleUserID: 0,
                    Annotation: Markingdata.Markings,
                    //// ImageBase64: Markingdata.AnnotatedImagebase64,
                    ImageBase64: '',
                    Comments: Markingdata.GeneralComments,
                    BandID: Markingdata.bandid,
                    Marks: marks,
                    IsActive: true,
                    IsDeleted: false,
                    MarkedBy: null,
                    Markeddate: new Date(),
                    MarkingStatus: 0,
                    WorkflowstatusID: this._workflowstatusid,
                    LastVisited: true,
                    Markedtype: this._markedtype,
                    TotalMarks: this.totalmarks,
                    Remarks: this.child.Remarks,
                    ScoreComponentMarkingDetail:
                      this._QuestionScoreComponentMarkingDetail,
                  },
                ];
                this.addtracedetails();
                this.trialmarkingService
                  .ResponseMarking(
                    this._QuestionUserResponseMarkingDetails,
                    this.QigId,
                    false
                  )
                  .pipe(first())
                  .subscribe({
                    next: (data1: any) => {
                      if (data1) {
                        this.getquestionid(false);
                        let mode = 2;
                        if (this._isviewmode) {
                          mode = 1;
                        }
                        this.saveMarkingScriptTimeTracking(mode, 2);
                        this.checksubmit();
                        this.Alert.info('Saved Successfully.');

                        this._change = false;
                      }
                    },
                    error: (a: any) => {
                      throw a;
                    },
                    complete: () => {
                      this.stasticsloading = false;
                      this.child._stasticsloading = false;
                      this.isquestiondet = false;
                      this.child.isDisabledSave = false;
                      window.setTimeout(() => {
                        this.updatepencolor();
                      }, 700);
                    },
                  });
              }
            }
          },
          error: (a: any) => {
            throw a;
          },
        });
    } else {
      this.isquestiondet = false;
      this.stasticsloading = false;
      this.child._stasticsloading = false;
    }

    this.child.isDisabledSave = false;
  }
  updatepencolor() {
    (document.getElementById('Pen') as HTMLAnchorElement).style.color =
      this.annotationcolor;
  }
  addtracedetails() {
    var IsActive: boolean = false;
    for (let i = 0; i < this._UserScriptResponseModel.length; i++) {
      if (this._UserScriptResponseModel[i].Marks != null) IsActive = true;
      else IsActive = false;

      this._QuestionUserResponseMarkingTrace = [
        {
          ScriptID: this._scriptid,
          ProjectQuestionResponseID:
            this._UserScriptResponseModel[i].ProjectUserQuestionResponseID,
          CandidateID: 0,
          ScheduleUserID: 0,
          Annotation: this._UserScriptResponseModel[i].Annotation,
          ImageBase64: '',
          Comments: this._UserScriptResponseModel[i].Comments,
          BandID: this._UserScriptResponseModel[i].BandID,
          Marks: this._UserScriptResponseModel[i].Marks,
          IsActive: IsActive,
          IsDeleted: false,
          MarkedBy: null,
          Markeddate: new Date(),
          MarkingStatus: 0,
          Markedtype: this._markedtype,
          WorkflowstatusID: this._workflowstatusid,
          LastVisited: false,
          TotalMarks: this.totalmarks,
          Remarks: this._UserScriptResponseModel[i].Remarks,
          ScoreComponentMarkingDetail:
            this._UserScriptResponseModel[i].ScoreComponentMarkingDetail,
        },
      ];

      this._QuestionUserResponseMarkingDetails.push(
        this._QuestionUserResponseMarkingTrace[0]
      );
    }
  }
  getPrevElmnts() {
    if (!this.stasticsloading) {
      let mode = 2;
      if (this._isviewmode) {
        mode = 1;
      }
      this.saveMarkingScriptTimeTracking(mode, 5);

      this.isquestiondet = true;
      this.stasticsloading = true;
      this.child.Clearalldata();
      this.child.score_awarded = null;
      this.child.score_assigned = null;
      this.child.scorecompnentdetail1 = [];
      this.child.Remarks = null;

      this._banddet = [];
      let quarray = document.getElementsByClassName(
        'mat-focus-indicator navgate_number active_button mat-button mat-button-base'
      );
      var prevelement: any = quarray[0].previousElementSibling;
      let Qlength = quarray.length;
      for (var i = 0; i < Qlength; i++) {
        quarray[i].setAttribute(
          'class',
          'mat-focus-indicator navgate_number mat-button mat-button-base ng-tns-c249-10 ng-star-inserted'
        );
      }
      if (prevelement == null) {
        this.getprojectdetails(
          this._UserScriptResponseModel[
            this._UserScriptResponseModel.length - 1
          ].ProjectUserQuestionResponseID
        );
      } else {
        this.getprojectdetails(prevelement.id);
      }

      this.checkprevdisplay(prevelement.id);
    }
  }
  getPrevelement() {
    this.isDisabledsubmit = true;
    if (this._change && !this.child.isauto && !this._isviewmode) {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: 'Some changes have been made. Do you want to save?',
        },
        panelClass: 'confirmationpop',
      });
      confirmDialog.afterClosed().subscribe((res: any) => {
        if (res === true) {
          this.autosave();
          window.setTimeout(() => {
            this.getPrevElmnts();
          }, 1000);

          this._change = false;
        } else {
          this.getPrevElmnts();
        }
      });
    } else {
      this.getPrevElmnts();
    }
  }
  getNextelmts() {
    if (!this.stasticsloading) {
      let mode = 2;
      if (this._isviewmode) {
        mode = 1;
      }
      this.saveMarkingScriptTimeTracking(mode, 5);
      this.isquestiondet = true;
      this.stasticsloading = true;
      this.child._stasticsloading = true;
      this.child.Clearalldata();
      this.child.scorecompnentdetail1 = [];
      this._banddet = [];
      this.child.score_awarded = null;
      this.child.score_assigned = null;
      this.child.Remarks = null;
      let quarray = document.getElementsByClassName(
        'mat-focus-indicator navgate_number active_button mat-button mat-button-base'
      );
      var nextelement: any = quarray[0].nextElementSibling;
      let Qlength = quarray.length;
      for (var i = 0; i < Qlength; i++) {
        quarray[i].setAttribute(
          'class',
          'mat-focus-indicator navgate_number mat-button mat-button-base ng-star-inserted'
        );
      }

      if (nextelement == null) {
        this.getprojectdetails(
          this._UserScriptResponseModel[0].ProjectUserQuestionResponseID
        );
      } else {
        this.getprojectdetails(nextelement.id);
      }

      this.checknextdisplay(nextelement.id);
    }
  }
  getnextelement() {
    this.isDisabledsubmit = true;
    if (this._change && !this.child.isauto && !this._isviewmode) {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: 'Some changes have been made. Do you want to save?',
        },
        panelClass: 'confirmationpop',
      });
      confirmDialog.afterClosed().subscribe((res: any) => {
        if (res === true) {
          this.autosave();
          window.setTimeout(() => {
            this.getNextelmts();
          }, 1000);
          this._change = false;
        } else {
          this.getNextelmts();
        }
      });
    } else {
      this.getNextelmts();
    }
  }
  checkprevdisplay(id: any) {
    if (id == this._UserScriptResponseModel[0].ProjectUserQuestionResponseID) {
      this.prevdisabled = true;
    } else {
      this.prevdisabled = false;
    }
  }
  checknextdisplay(id: any) {
    if (
      id ==
      this._UserScriptResponseModel[this._UserScriptResponseModel.length - 1]
        .ProjectUserQuestionResponseID
    ) {
      this.nxtdisabled = true;
    } else {
      this.nxtdisabled = false;
    }
  }
  getquestionid(sts: boolean) {
    this._UserScriptMarking = [
      {
        ScriptID: this._scriptid,
        ProjectId: 0,
        CandidateId: 0,
        ScheduleUserId: 0,
        TotalNoOfQuestions: 0,
        MarkedQuestions: 0,
        ScriptMarkingStatus: 0,
        WorkFlowStatusID: this._workflowstatusid,
        MarkedBy: this._markedby,
        scriptstatus: sts,
        IsViewMode: this.data.IsViewMode,
        UserScriptMarkingRefId: this.UserScriptMarkingRefId,
      },
    ];
    this.trialmarkingService
      .UserscriptMarking(this._UserScriptMarking[0])
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (
            data[0].ScriptId != this._scriptid ||
            (data[0].Workflowstatusid != this._workflowstatusid &&
              this._workflowstatusid != 0)
          ) {
            this.Alert.info('Invalid data');

            window.setTimeout(() => this.dialogRef.close({ status: 0 }), 300);
          } else if (data == null) {
            this.Alert.info('NO Question Details available for The Script');

            location.reload();
          } else {
            if (data[0].MarkingStatus == 2) {
              if (this._markedby == null) {
                this.child.isDisabled = true;
                this.child.isDisabledSave = true;
                this.isDisabledsubmit = true;
                this.data.IsViewMode = true;
                this.child.isDisabledsc = true;
                this.child.isDisabledband = true;
                this._isviewmode = true;
                this.child.disableannotation();
              }
            }

            if (sts) {
              this.questiondet = [];
              this.totalmarks = 0;
              for (let i = 0; i < data.length; i++) {
                var numb = i + 1;
                var det = {
                  id: data[i].ProjectUserQuestionResponseID,
                  name: data[i].QuestionCode,
                  markedtype: data[i].MarkedType,
                };
                this.questiondet.push(det);
              }
            }

            this._UserScriptResponseModel = data;

            this._userscriptid = data[0].userscriptID;

            let LastVisited = this._UserScriptResponseModel.filter(
              (item) => item.Lastvisited
            );

            if (sts) {
              if (LastVisited.length == 0 || this._markedby != null)
                this.getprojectdetails(
                  this._UserScriptResponseModel[0].ProjectUserQuestionResponseID
                );
              else
                this.getprojectdetails(
                  LastVisited[0].ProjectUserQuestionResponseID
                );
            } else {
              window.setTimeout(() => this.Seticonelement(), 200);
            }
            this.qidname = data[0].QIGName;
            this.scriptname = data[0].ScriptName;
            this.totalmarks = 0;
            this.totalmarks = this._UserScriptResponseModel[0].TotalMarks;

            this.awardedmarks = this._UserScriptResponseModel[0].awardedmarks;
          }
        },

        complete: () => {
          this.stasticsloading = false;
          this.child._stasticsloading = false;
          if (sts) {
            window.setTimeout(() => (this.isDisabledsubmit = true), 300);
          }
        },
      });
  }

  getresponsetext(id: any) {
    this.isquestiondet = true;
    this.stasticsloading = true;
    this.child._stasticsloading = true;
    this.child._IsScoreComponentExists = null;
    var mode = 2;
    if (this._isviewmode) {
      mode = 1;
    }
    this.saveMarkingScriptTimeTracking(mode, 5);

    this.child.Clearalldata();

    this.child.score_awarded = null;
    this.child.score_assigned = null;
    this.child.scorecompnentdetail1 = [];
    this.child.Remarks = null;
    this.child._banddet = [];
    var questionarray;
    var sarray;

    questionarray = document.getElementsByClassName(
      'mat-focus-indicator navgate_number active_button mat-button mat-button-base'
    );
    sarray = document.getElementsByClassName('icon-complete active_numner');
    let Qlength = questionarray.length;
    for (var i = 0; i < Qlength; i++) {
      questionarray[i].setAttribute(
        'class',
        'mat-focus-indicator navgate_number mat-button mat-button-base ng-star-inserted'
      );
    }

    for (var j = 0; j < sarray.length; j++) {
      sarray[j].setAttribute('class', 'icon-complete');
    }
    this.getprojectdetails(id);
  }

  getResponsetText(evt: any) {
    this.isDisabledsubmit = true;

    if (
      this._change &&
      !this.child.isauto &&
      !this._isviewmode &&
      evt.currentTarget.id != this._projectquestionResponseid
    ) {
      this.curid = evt.currentTarget.id;
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: 'Some changes have been made. Do you want to save?',
        },
        panelClass: 'confirmationpop',
      });
      confirmDialog.afterClosed().subscribe((res: any) => {
        if (res === true) {
          this.autosave();
          window.setTimeout(() => {
            this.getresponsetext(this.curid);
          }, 1000);

          this._change = false;
        } else {
          this.getresponsetext(this.curid);
          this._change = false;
        }
      });
    } else {
      this.getresponsetext(evt.currentTarget.id);
    }
  }

  getprojectdetails(id: any) {
    let ProjectQuestionId: any = this._UserScriptResponseModel.filter(
      (item) => item.ProjectUserQuestionResponseID == id
    );
    if (ProjectQuestionId != null) {
      ProjectQuestionId = ProjectQuestionId[0].ProjectQuestionId;
    }

    this._ProjectQuestionId = ProjectQuestionId;
    this.child.getquestionresponsetext(
      id,
      this._scriptid,
      ProjectQuestionId,
      this.UserScriptMarkingRefId
    );
    window.setTimeout(() => this.Setclickedelement(id), 200);

    this._projectquestionResponseid = id;
    this.child.maxmarks_score = this._UserScriptResponseModel.filter(
      (item) => item.ProjectUserQuestionResponseID == id
    )[0].QuestionMarks;
    this.child.min_score = 0;
    this.child.max_score = this.child.maxmarks_score;
  }
  Seticonelement() {
    let id: any;
    let keyid: number = 0;
    for (let j = 0; j < this._UserScriptResponseModel.length; j++) {
      if (
        this._UserScriptResponseModel[j].ScoreComponentMarkingDetail != null &&
        this._UserScriptResponseModel[j].ScoreComponentMarkingDetail!.length > 0
      ) {
        for (
          let l = 0;
          l <
          this._UserScriptResponseModel[j].ScoreComponentMarkingDetail!.length;
          l++
        ) {
          if (
            this._UserScriptResponseModel[j].ScoreComponentMarkingDetail![l]
              .AwardedMarks == null
          ) {
            keyid++;
          }
        }
      } else if (this._UserScriptResponseModel[j].Marks == null) {
        keyid++;
      }

      if (this.validateannotation) {
        if (
          this._UserScriptResponseModel[j].Marks != null &&
          this._UserScriptResponseModel[j].Annotation == '[]'
        ) {
          keyid++;
        }
      }

      if (keyid == 0) {
        id =
          this._UserScriptResponseModel[j].ProjectUserQuestionResponseID + 'S';
        let gbtn = document.getElementById(id) as HTMLElement;
        gbtn.className = 'icon-complete active_numner';
      } else {
        id =
          this._UserScriptResponseModel[j].ProjectUserQuestionResponseID + 'S';
        let gbtn = document.getElementById(id) as HTMLElement;
        gbtn.className = 'icon-complete';
      }
      keyid = 0;
    }
  }
  checksubmit() {
    this.isDisabledsubmit = true;
    if (!this.data.IsViewMode && !this.firstclick) {
      let validatedresult: any = '';
      let keyid = 0;
      for (let i = 0; i < this._UserScriptResponseModel.length; i++) {
        if (
          (this._UserScriptResponseModel[i].MarkedType == 3 ||
            this._UserScriptResponseModel[i].MarkedType == null) &&
          this._UserScriptResponseModel[i].ResponseText != null &&
          this._UserScriptResponseModel[i].ResponseText != '-No Response(NR)-'
        ) {
          var id: any =
            this._UserScriptResponseModel[i].ProjectUserQuestionResponseID;
          var div = document.getElementById(id) as HTMLElement;
          var span = div.getElementsByTagName('span');
          var det;
          if (span.length != 0) {
            det = span[0].innerText;
          }
          if (this.validateannotation) {
            if (this._UserScriptResponseModel[i].Annotation == '[]') {
              keyid++;
              validatedresult = 'Annotation is mandatory.';
            }
          }
          if (this._UserScriptResponseModel[i].Marks == null) {
            keyid++;
            validatedresult = validatedresult + det + ': ' + 'Marks' + ', ';
          }
          if (
            this._UserScriptResponseModel[i].ScoreComponentMarkingDetail != null
          ) {
            if (
              this._UserScriptResponseModel[i].ScoreComponentMarkingDetail!
                .length != 0
            ) {
              for (let l = 0; l < this.child.scorecompnentdetail1.length; l++) {
                let markssc = (
                  document.getElementById(
                    this.child.scorecompnentdetail1[l].ScoreComponentId + 'I'
                  ) as HTMLSelectElement
                ).value;

                if (markssc == '') {
                  validatedresult = validatedresult + 'Please Add Marks' + ', ';
                }
              }
            }
          }
          var scomp = 1;
          if (
            this._UserScriptResponseModel[i].ScoreComponentMarkingDetail ==
              null ||
            this._UserScriptResponseModel[i].ScoreComponentMarkingDetail!
              .length == 0
          ) {
            scomp = 0;
          }

          if (
            this._UserScriptResponseModel[i].ScoreComponentMarkingDetail != null
          ) {
            for (
              let l = 0;
              l <
              this._UserScriptResponseModel[i].ScoreComponentMarkingDetail!
                .length;
              l++
            ) {
              if (
                this._UserScriptResponseModel[i].ScoreComponentMarkingDetail![l]
                  .AwardedMarks == null
              ) {
                keyid++;
                validatedresult = 'Please Select Banddetails';
              }
            }
          }
          if (scomp != 0) {
            if (
              this._UserScriptResponseModel[i].BandID == null &&
              this._UserScriptResponseModel[i].Marks != null
            ) {
              this._marksschmemesubmit = false;
            } else {
              this._marksschmemesubmit = true;
            }
            if (
              this._marksschmemesubmit &&
              this._UserScriptResponseModel[i].ScoreComponentMarkingDetail!
                .length == 0
            ) {
              keyid++;
              validatedresult = validatedresult + 'Band Details' + ', ';
            }
          } else {
            if (
              this._UserScriptResponseModel[i].BandID == null &&
              this._UserScriptResponseModel[i].Marks != null
            ) {
              this._marksschmemesubmit = false;
            } else {
              this._marksschmemesubmit = true;
            }

            if (
              this._marksschmemesubmit &&
              this._UserScriptResponseModel[i].BandID == null
            ) {
              keyid++;
              validatedresult = validatedresult + 'Band Details' + ', ';
            }
          }
        }
      }

      if (keyid == 0) {
        this.isDisabledsubmit = false;
      }
    }

    this.firstclick = false;
  }
  onsubmitclick() {
    if (!this.stasticsloading) {
      if (!this.isDisabledsubmit) {
        let validatedresult: any = '';
        let keyid = 0;
        this.qigservice
          .Getqigworkflowtracking(this.QigId, AppSettingEntityType.QIG)
          .pipe(first())
          .subscribe({
            next: (data: any) => {
              let a = -1;
              if (data != null) {
                this.WorkFlowStatusTracking = data;

                a = this.WorkFlowStatusTracking.findIndex(
                  (ai) =>
                    ai.WorkflowStatusCode == WorkflowStatus.Pause &&
                    ai.ProcessStatus == WorkflowProcessStatus.OnHold
                );
              }
              if (a >= 0) {
                this.Alert.warning(
                  'This QIG is currently under “Paused state”.'
                );
                this.child.isDisabled = true;
                this.child.isDisabledSave = true;
                this.isDisabledsubmit = true;
                this.child.isDisabledband = true;
                this.child.isDisabledsc = true;
                window.setTimeout(
                  () => this.dialogRef.close({ status: 1 }),
                  300
                );
              } else {
                for (let i = 0; i < this._UserScriptResponseModel.length; i++) {
                  if (
                    (this._UserScriptResponseModel[i].MarkedType == 3 ||
                      this._UserScriptResponseModel[i].MarkedType == null) &&
                    this._UserScriptResponseModel[i].ResponseText != null &&
                    this._UserScriptResponseModel[i].ResponseText !=
                      '-No Response(NR)-'
                  ) {
                    var id: any =
                      this._UserScriptResponseModel[i]
                        .ProjectUserQuestionResponseID;
                    var div = document.getElementById(id) as HTMLElement;
                    var span = div.getElementsByTagName('span');
                    var det;
                    if (span.length != 0) {
                      det = span[0].innerText;
                    }
                    if (
                      this._UserScriptResponseModel[i]
                        .ScoreComponentMarkingDetail != null
                    ) {
                      for (
                        let l = 0;
                        l <
                        this._UserScriptResponseModel[i]
                          .ScoreComponentMarkingDetail!.length;
                        l++
                      ) {
                        if (
                          this._UserScriptResponseModel[i]
                            .ScoreComponentMarkingDetail![l].AwardedMarks ==
                          null
                        ) {
                          keyid++;
                          validatedresult = 'Please Select Banddetails';
                        }
                      }
                    }
                    if (this._UserScriptResponseModel[i].Marks == null) {
                      keyid++;
                      validatedresult =
                        validatedresult + det + ': ' + 'Marks' + ', ';
                    }
                    if (this.validateannotation) {
                      if (this._UserScriptResponseModel[i].Annotation == '[]') {
                        keyid++;
                        validatedresult = 'Annotation is mandatory.';
                      }
                    }
                    if (
                      this._UserScriptResponseModel[i]
                        .ScoreComponentMarkingDetail != null
                    ) {
                      if (
                        this._UserScriptResponseModel[i]
                          .ScoreComponentMarkingDetail!.length != 0
                      ) {
                        for (
                          let l = 0;
                          l < this.child.scorecompnentdetail1.length;
                          l++
                        ) {
                          let markssc = (
                            document.getElementById(
                              this.child.scorecompnentdetail1[l]
                                .ScoreComponentId + 'I'
                            ) as HTMLSelectElement
                          ).value;

                          if (markssc == '') {
                            validatedresult =
                              validatedresult + 'Please Add Marks' + ', ';
                          }
                        }
                      }
                    }
                    var scomp = 1;
                    if (
                      this._UserScriptResponseModel[i]
                        .ScoreComponentMarkingDetail == null ||
                      this._UserScriptResponseModel[i]
                        .ScoreComponentMarkingDetail!.length == 0
                    ) {
                      scomp = 0;
                    }

                    if (scomp != 0) {
                      if (
                        this._UserScriptResponseModel[i].BandID == null &&
                        this._UserScriptResponseModel[i].Marks != null
                      ) {
                        this._marksschmemesubmit = false;
                      } else {
                        this._marksschmemesubmit = true;
                      }
                      if (
                        this._marksschmemesubmit &&
                        this._UserScriptResponseModel[i]
                          .ScoreComponentMarkingDetail!.length == 0
                      ) {
                        keyid++;
                        validatedresult =
                          validatedresult + 'Band Details' + ', ';
                      }
                    } else {
                      if (
                        this._UserScriptResponseModel[i].BandID == null &&
                        this._UserScriptResponseModel[i].Marks != null
                      ) {
                        this._marksschmemesubmit = false;
                      } else {
                        this._marksschmemesubmit = true;
                      }

                      if (
                        this._marksschmemesubmit &&
                        this._UserScriptResponseModel[i].BandID == null
                      ) {
                        keyid++;
                        validatedresult =
                          validatedresult + 'Band Details' + ', ';
                      }
                    }
                  }
                }
                if (keyid != 0) {
                  this.Alert.info('Please ensure all  questions are marked.');
                } else {
                  this.trialmarkingService
                    .Markingsubmit(
                      this._scriptid,
                      this._workflowstatusid,
                      this.QigId
                    )
                    .pipe(first())
                    .subscribe({
                      next: (data1: any) => {
                        if (data1) {
                          this.Alert.info('Script Submitted successfully.');
                          (
                            document.getElementById(
                              'deleteall'
                            ) as HTMLAnchorElement
                          ).click();
                          let mode = 2;
                          if (this._isviewmode) {
                            mode = 1;
                          }
                          this.saveMarkingScriptTimeTracking(mode, 1);
                          this.autosaveSubscription.unsubscribe();
                          this.timetakenSubscription.unsubscribe();
                          this.child.isDisabled = true;
                          this.child.isDisabledSave = true;
                          this.isDisabledsubmit = true;
                          this.child.isDisabledsc = true;
                          this.child.isDisabledband = true;
                          window.setTimeout(
                            () => this.dialogRef.close({ status: 1 }),
                            300
                          );
                        } else if (!data1) {
                          this.Alert.warning(
                            'Script submission unsuccessful, submission cannot be done without saving.'
                          );
                        }
                      },
                      error: (a1: any) => {
                        throw a1;
                      },
                      complete: () => {
                        this.stasticsloading = false;
                        this.child._stasticsloading = false;
                      },
                    });
                }
              }
            },
            error: (a: any) => {
              throw a;
            },
            complete: () => {
              this.stasticsloading = false;
              this.child._stasticsloading = false;
            },
          });
      }
    }
  }

  Setclickedelement(id: any) {
    let gbtn = document.getElementById(id) as HTMLElement;
    if (gbtn != null) {
      gbtn.className =
        'mat-focus-indicator navgate_number active_button mat-button mat-button-base';
    }

    this.checknextdisplay(id);
    this.checkprevdisplay(id);
  }
}
