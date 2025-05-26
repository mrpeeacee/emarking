import { Component, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs/operators'
import { QigConfigService } from 'src/app/services/project/qigconfig.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { QigQuestionModel } from 'src/app/model/project/qig';
import { ProjectQigModel } from 'src/app/model/project/qigconfig';
import { MatDialog } from '@angular/material/dialog';
import { trigger, transition, style, animate, state } from '@angular/animations';
import { ViewDownloadMarksSchemeComponent } from 'src/app/modules/projects/marking-player/view-download-marks-scheme/view-download-marks-scheme.component';
import {
  MatSnackBarHorizontalPosition,
  MatSnackBarVerticalPosition,
} from '@angular/material/snack-bar';
import { throwError } from 'rxjs';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { QigConfigurationsComponent } from '../qig-configurations.component';
import { Filedetails } from 'src/app/model/project/mark-scheme/mark-scheme-model';
import { ViewScoringComponentComponent } from '../../../ScoringComponent/view-scoring-component/view-scoring-component.component';
import { ScoreComponentLibraryName } from 'src/app/model/project/Scoring-Component/Scoring-Component.model';

@Component({
  selector: 'emarking-qig-questions',
  templateUrl: './qig-questions.component.html',
  styleUrls: ['./qig-questions.component.css'],
  animations: [
    trigger('sideMenu', [
      state('closed', style({

        right: "-400px"
      })),
      state('open', style({

        right: "0px"
      })),

      transition('open => closed', animate('400ms ease-out')),
      transition('closed => open', animate('300ms ease-out'))
    ]),
  ]
})

export class QigQuestionsComponent {
  horizontalPosition: MatSnackBarHorizontalPosition = 'end';
  verticalPosition: MatSnackBarVerticalPosition = 'bottom';
  @ViewChild("accordionQuestions") accordionQuestions: any;
  @ViewChild('closebutton') closebutton: any;
  state = "closed";
  questionview: any = {
    QnsLabel: '',
    QnsCout: 0,
    QigId: 0
  };
  QigId!: number;
  qigquestions: QigQuestionModel[] = [];
  qigquestionsoreFinger: QigQuestionModel[] = [];
  Markschemesearchlst: QigQuestionModel[] = [];

  qigquestionandmarks!: ProjectQigModel
  Availablemarkschme: any;
  SearchMarkSchemeValue: string = "";
  SearchScoringComponetLibraryValue:string=""
  StimulusText: any;
  Status: any;
  markschemeType: number = 0;
  SelectedQuestion: any;
  SelectedScoringcomp: any;
  SelectedScoringMaxmarkcomp!: QigQuestionModel;
  SelectedMarkScheme!: any;
  ErrorMessage: string = "";
  previousValue: number = 0;
  showmarkschemeerror: number = 0;
  questionloading: boolean = false;
  dataloaded: number = 0;
  marksschemeloading = false;
  TextContent: any;
  SoreContent: any;
  splitwords: any;
  duplicate: boolean = false;
  scorecomploading: boolean = false;
  openModal: boolean = false;
  maxmarks: number = 0;
  maxmarksreached: boolean = false;
  MaxMarksTotal!: number;
  filelist: Filedetails[] = [];
  intMessages: any = {
    Confirmwarning: ''
  };
  questionMaxmarks!: number;
  UpdateMaxmarks!: number;
  maxmarksloading = false;
  classChangeCols!: any;
  Isfinalise!: boolean;
  qigcreationsetuplen: any;
  manditorycondition: any;
  Mmarks!: number;
  sorefingerMaxmarks!: number;
  QuestionCode!: string;
  AvailableScoringcomponentLibrary: ScoreComponentLibraryName[]=[];
  SelectedScoringComponentLibrary: any[]=[];
  TagScoringComponentLibrarydetails: any[]=[];
  ISCBPProject: any;
  Istag!: boolean;
  ChildScoringComponentDetails: any[]=[];
  dumyavailable_qu1: any;
  SearchAvailableScoringcomponentLibrary!: ScoreComponentLibraryName[];

  constructor(public qigconfigservice: QigConfigService,
    public Alert: AlertService, public translate: TranslateService,
    private dialog: MatDialog,
    public _qigConfigurationsComponent: QigConfigurationsComponent) { }

  ngOnInit(): void {
    this.translate.get('SetUp.QigConfig.maxmarkspopuwarningmessage').subscribe((translated: string) => {
      this.intMessages.maxmarkspopuwarningmessage = translated;
    });
    this.translate.get('SetUp.QigConfig.Mandatorymaxmarkspopuwarningmessage').subscribe((translated: string) => {
      this.intMessages.Mandatorymaxmarkspopuwarningmessage = translated;
    });
    this.CheckIsCBPproject()
  }
  CheckIsCBPproject()
  {
    debugger;
    this.qigconfigservice.CheckIsCBPproject()
    .pipe(first())
    .subscribe({
      next: (data: any) => {
        this.ISCBPProject=data
      }
    })
  }
  viewqigquestions(QigIdValue: number) {
    this.dataloaded = 0;
    this.questionloading = true;
    this.QigId = QigIdValue;
    this.questionview.QnsLabel = QigIdValue;
    this.questionview.QigId = QigIdValue;

    this.qigconfigservice.getAllViewQigQuestions(QigIdValue)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
    
          this.qigquestionsoreFinger = [];
          debugger;
          this.qigquestions = data;
          debugger;
          this.qigquestionsoreFinger.push(data[0])
          if(this.qigquestionsoreFinger[0].QuestionType==152)
            {
              var SFcount_A=0
              for(var i =0;i<this.qigquestions.length;i++)
                {
                  
                  SFcount_A+= this.qigquestions[i].MaxMark;
                }   
               this.sorefingerMaxmarks= SFcount_A;
            }
          this.classChangeCols = this.qigquestions.filter(a => a.QuestionType == 10);

          if (this.qigquestions != null) {
            this.dataloaded = 1;
          }
          else {
            this.dataloaded = 2;
          }
          this.qigquestions.forEach((a: any) => {
            a.QText = a.QuestionText;
            this.UpdateMaxmarks = a?.MaxMark;
            if (a.QText.includes("\n")&& a.QuestionType=="152") {
             
              a.QText = a.QText.replace(/\n/g, "<br/><br/>");
          }
          

            if (a.MarkSchemeId != null) {
              if (a.MarkSchemeId > 0) {
                a.DisableMaxmark=true;
              } else {
                a.DisableMaxmark = false;
              }
            }
            else {
              a.DisableMaxmark = false;
            }
            if (a?.Scorecomponentdetails != null) {
              if (a?.Scorecomponentdetails.find((b: any) => b.ProjectMarkSchemeId > 0)) {
                a.DisableMaxmark = true;
              }
              else {
                a.DisableMaxmark = false;
              }

            } else {
              a.DisableMaxmark = false;
            }

          });

          if (this.qigquestions != null) {
            this.questionview.QnsCout = this.qigquestions.length;
          }
        },
        error: (a: any) => {
          this.questionloading = false;
          throw (a);
        },
        complete: () => {
          this.questionloading = false;
        },
      });
  }

  viewmarksscheme(markschemeid: any) {
    const editorDialog = this.dialog.open(ViewDownloadMarksSchemeComponent, {
      data: {
        projectquestionid: 0,
        markschemeid: markschemeid,
        questionname: "markScheme"

      },
      panelClass: 'fullviewpop',
    });
    editorDialog.afterClosed().subscribe();

  }

  viewScoringComponentscheme(markschemeid: any) {
    debugger;
    const editorDialog = this.dialog.open(ViewScoringComponentComponent, {
      data: {
        IsfromQig: true,
        markschemeid: markschemeid,
        // questionname: "ScoringComponet"

      },
      panelClass: 'fullviewpop',
    });
    editorDialog.afterClosed().subscribe();
  }
  Getavailablemarkschemes(Maxmarks: any, markschemeType: any) {
    this.marksschemeloading = true;
    this.SearchMarkSchemeValue = "";
    this.SelectedMarkScheme = null;
    this.markschemeType = markschemeType;
    this.qigconfigservice.Getavailablemarkschemes(Maxmarks, this.markschemeType)
      .subscribe({
        next: (data: any) => {
          data.forEach((element: any) => {
            if(this.SelectedQuestion!=undefined){
            if (markschemeType == 1 && this.SelectedQuestion.MarkSchemeId > 0) {
              element.DisableMaxmark = true;
              element.IsTagged = element.ProjectMarkSchemeId == this.SelectedQuestion.MarkSchemeId;
              this.SelectedMarkScheme = element;
            }
            else if (markschemeType == 2 && this.SelectedScoringcomp.ProjectMarkSchemeId > 0) {
              element.DisableScoringMaxmark = true;
              element.IsTagged = element.ProjectMarkSchemeId == this.SelectedScoringcomp.ProjectMarkSchemeId;
              this.SelectedMarkScheme = element;
            }
          }
          });
        
          this.Availablemarkschme = data;
          this.Markschemesearchlst = data;
        },
        error: (a: any) => {
          this.marksschemeloading = false;
          throw (a);
        },
        complete: () => {
          this.marksschemeloading = false;
        },
      });
  }
  CloseFn(markschemeselected: any) {
    this.SearchMarkSchemeValue = "";
    this.SelectedMarkScheme = null;
  }
  SearchMarkScheme() {
    var SearchMarkSchemeValue = this.SearchMarkSchemeValue;
    this.Availablemarkschme = this.Markschemesearchlst.filter(function (el) { return (el.SchemeName.toLowerCase().includes(SearchMarkSchemeValue.trim().toLowerCase())) });
  }

  GetQuestionMarkScheme(Questions: any,isFromSF :any) {
    this.SelectedQuestion = Questions;
    var SFcount =0;
    if(isFromSF == "SF")
    {
      for(var i =0;i<this.qigquestions.length;i++)
      {
        SFcount+= this.qigquestions[i].MaxMark;
      }   
      this.Getavailablemarkschemes(SFcount, 1);
      
     
    }
    else{
     this.Getavailablemarkschemes(Questions.MaxMark, 1);
     this.GetScoringComponentLibrary(Questions.MaxMark)
    }
  }

  GetComponentMarkSchemes(Questionscomp: any,disablemarks:any) {
    this.openModal = false;
    this.SelectedScoringcomp = Questionscomp;
    this.SelectedScoringMaxmarkcomp=disablemarks;
    this.Alert.clear();

    if (Questionscomp.MaxMark != null && Questionscomp.MaxMark > 0) {
      this.openModal = true;
      this.Getavailablemarkschemes(Questionscomp.MaxMark, 2);
    }
    else if (Questionscomp.MaxMark == null) {
      this.openModal = false;
      this.translate
      .get('SetUp.QigConfig.EnterMarkscheme')
      .subscribe((translated: string) => {
        this.Alert.clear();
        this.Alert.warning(translated);
      });
      this.closebutton.nativeElement.click();
    }
    else if (Questionscomp.MaxMark == 0) {
      this.openModal = false;
      this.translate
      .get('SetUp.QigConfig.ComponentMarkschemeZero')
      .subscribe((translated: string) => {
        this.Alert.clear();
        this.Alert.warning(translated);
      });
      this.closebutton.nativeElement.click();

    }
  }

  GetQigQuestionandMarks(QigIdValue: number, statuslst: any) {
    this.QigId = QigIdValue;
    this.qigcreationsetuplen = statuslst;
    this.Isfinalise = statuslst == 0 ? true : false;
    this.qigconfigservice.GetQigQuestionandMarks(QigIdValue)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.qigquestionandmarks = data;
        },
        error: (a: any) => {
          throw (a);
        },
      });
  }
  TagQuestionMarkScheme() {
    this.SelectedQuestion.DisableMaxmark=false;
    if (this.SelectedMarkScheme != null) {
      this.SelectedQuestion.MarkSchemeName = this.SelectedMarkScheme.SchemeName;
      this.SelectedQuestion.MarkSchemeId = this.SelectedMarkScheme.ProjectMarkSchemeId;
      this.SelectedQuestion.DisableMaxmark=true;
      document.getElementById('closeQuestionMarkScheme')!.click();
    }
    else {
      this.translate
        .get('SetUp.QigConfig.SelectMarkScheme')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    }
  }
  
  TagComponenetMarkScheme() {
    this.SelectedScoringMaxmarkcomp.DisableMaxmark = false;
    if (this.SelectedMarkScheme != null) {
      this.SelectedScoringcomp.SchemeName = this.SelectedMarkScheme.SchemeName;
      this.SelectedScoringcomp.ProjectMarkSchemeId = this.SelectedMarkScheme.ProjectMarkSchemeId;
      document.getElementById('closeComponentMarkScheme')!.click();
      this.SelectedScoringMaxmarkcomp.DisableMaxmark=true;
    }
    else {
      this.translate
        .get('SetUp.QigConfig.SelectMarkScheme')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    }
  }
  
  validateNumber(event: any) {
    var invalidChars = ["-", "e", "+", "E", "."];
    if (invalidChars.includes(event.key)) {
      event.preventDefault();
    }
  }
  SetScoringComponentLibrary(selectedLibrary:any[])
  {
    this.SelectedScoringComponentLibrary=selectedLibrary
  }
  SaveUpdateMarkscheme(available_qu: any,sFrom:any) {
    this.maxmarksloading = true;
    this.Alert.clear();

    if (!this.ValidateQuestionsSave(available_qu,sFrom)) {
      this.maxmarksloading = false;
      this.ShowMessage(this.ErrorMessage, 'warning');
    }
    else {
      var qu_sf:any;
      available_qu.PassageText = "";
      available_qu.QuestionText = "";
      available_qu.QigId = this.QigId;
      var SFcount_A = 0;
      var a = available_qu.MaxMark;
      qu_sf = available_qu;
      if(available_qu.QuestionType == 152)
      { 
         qu_sf.MaxMark =available_qu.MaxMark;
       
        this.qigconfigservice.TagAvailableMarkScheme(qu_sf).pipe(first()).subscribe({
          next: (data: any) => {
            this.Status = data;
            available_qu.MaxMark = a;
            if (this.Status == 'UP001') {
              this.translate.get('SetUp.QigConfig.Questionsupdate').subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.success(translated);
                this.maxmarksloading = false;
                this.SearchMarkSchemeValue = "";
                this._qigConfigurationsComponent.GetQIGConfigDetails(this.QigId);
              });
            }
            else {
              this.translate.get('SetUp.QigConfig.QuestionsupdateFailed').subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.error(translated);
                this.SearchMarkSchemeValue = "";
                this.maxmarksloading = false;
              });
            }
          },
          error: (a: any) => {
            this.scorecomploading = false;
            return throwError(a);
          },
  
        });

      }
      else
      {
      this.qigconfigservice.TagAvailableMarkScheme(available_qu).pipe(first()).subscribe({
        next: (data: any) => {
          this.Status = data;
          if (this.Status == 'UP001') {
            this.translate.get('SetUp.QigConfig.Questionsupdate').subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.success(translated);
              this.maxmarksloading = false;
              this.SearchMarkSchemeValue = "";
              this._qigConfigurationsComponent.GetQIGConfigDetails(this.QigId);
            });
          }
          else {
            this.translate.get('SetUp.QigConfig.QuestionsupdateFailed').subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.error(translated);
              this.SearchMarkSchemeValue = "";
              this.maxmarksloading = false;
            });
          }
        },
        error: (a: any) => {
          this.scorecomploading = false;
          return throwError(a);
        },

      });
    }
    }
  }
  SaveUpdateScoringComponentLibrary(available_qu1 :any)

  {
  
    console.log(available_qu1)
    this.dumyavailable_qu1 = JSON.parse(JSON.stringify(available_qu1));
    this.dumyavailable_qu1.Scorecomponentdetails=this.ChildScoringComponentDetails
    console.log("After assigning"+this.dumyavailable_qu1)
    this.dumyavailable_qu1.ScoringComponentLibraryId= this.TagScoringComponentLibrarydetails[0].ScoreComponentId
    console.log("After assigning ScoringcomponentLibraryValue" +available_qu1+this.dumyavailable_qu1)
 this.qigconfigservice.SaveScoringcomponentLibrary(this.dumyavailable_qu1).pipe(first()).subscribe({
  next: (data: any) => {
    if(data=='E002')
    {
this.Alert.warning("Score Component Library Already Exists");
this.Istag=false;
this.viewqigquestions(this.QigId);
this.ISCBPProject()
    }
    else if(data=='S001')
    {
      this.Alert.success("Score Component Library Saved Sucessfully");
      this.Istag=false;
      this.viewqigquestions(this.QigId);
    }
    else{
      this.Alert.warning("Score Component Library Failed");
    }
  }
})

  }
  ValidateQuestionsSave(available_qu: any,sFrom:any): boolean {
    if(available_qu.QuestionType==152)
      {
        var SFcount_A=0
        for(var i =0;i<this.qigquestions.length;i++)
          {
            
            SFcount_A+= this.qigquestions[i].MaxMark;
          }   
          available_qu.MaxMark = SFcount_A;
      }
    
    var validationstatus: boolean = true;
    if (this.isScoreComponentValidationFailed(available_qu)) {
      this.ErrorMessage = "SetUp.QigConfig.Addcomp";
      validationstatus = false;
    } else if (available_qu.ToleranceLimit >= available_qu.MaxMark) {
      this.ErrorMessage = 'SetUp.QigConfig.ToleranceLessMaxmarks';
      validationstatus = false;
    } else if (this.isStepValueMismatch(available_qu)) {
      this.ErrorMessage = 'SetUp.QigConfig.StepValueDoestMatchMaxScore';
      validationstatus = false;
    } else if (this.isScoreComponentInvalid(available_qu)) {
      validationstatus = false;
    }

    return validationstatus;
  }

  private isScoreComponentValidationFailed(available_qu: any): boolean {
    return (available_qu.Scorecomponentdetails == null || available_qu.Scorecomponentdetails.length == 0) && available_qu.IsScoreComponentExists && available_qu.QigType == 3;
  }

  private isStepValueMismatch(available_qu: any): boolean {
    const { MaxMark, StepValue } = available_qu;
    if (MaxMark % 2 == 0) {
      return StepValue > 2 || StepValue < 0.5;
    } else {
      return StepValue > 1 || StepValue < 0.5;
    }
  }

  private isScoreComponentInvalid(available_qu: any): boolean {
    var maxmarks: number = 0;
    this.duplicate = false;

    for (let i = 0; i < available_qu?.Scorecomponentdetails?.length; i++) {
      const element = available_qu.Scorecomponentdetails[i];
      const matchedComponent = available_qu.Scorecomponentdetails?.filter((val: { ComponentName: any; }) => val.ComponentName === element.ComponentName);

      if (matchedComponent.length > 1) {
        this.duplicate = true;
      } else if (element.MaxMark != 0) {
        maxmarks += element.MaxMark;
      } else if (element.MaxMark == 0) {
        this.ErrorMessage = 'SetUp.QigConfig.Maxmark';
        return true;
      }
    }

    const scrorefilter = available_qu?.Scorecomponentdetails?.filter((a: { MaxMark: number | null; }) => a.MaxMark == 0 || a.MaxMark == null);
    const componentfilter = available_qu?.Scorecomponentdetails?.filter((a: { ComponentName: string | null; }) => a.ComponentName == null || a.ComponentName == '');
    if (componentfilter?.length > 0) {
      this.ErrorMessage = 'SetUp.QigConfig.ComponentFiledEmpty';
      return true;
    }
    else if (this.duplicate) {
      this.ErrorMessage = 'SetUp.QigConfig.Duplicatecomp';
      return true;
    }
    else if (scrorefilter?.length > 0) {
      this.ErrorMessage = 'SetUp.QigConfig.Maxmark';
      return true;
    } else if (available_qu?.IsScoreComponentExists && maxmarks != available_qu?.MaxMark) {
      this.ErrorMessage = 'SetUp.QigConfig.ScoreComponentMarksErrMessage';
      return true;
    }

    return false;
  }


  ShowMessage(message: string, messagetype: string) {
    this.translate.get(message).subscribe((translated: string) => {
      this.Alert.clear();
      if (messagetype == "warning") {
        this.Alert.warning(translated);
      }
      else if (messagetype == "success") {
        this.Alert.success(translated);
      }
    });

  }
  TranslateMessage(message: string): string {
    var translatedmessage = "";
    this.translate.get(message).subscribe((translated: string) => {
      translatedmessage = translated;
    });
    return translatedmessage;
  }

  SetMarkScheme(availablemarkschme: any) {
    this.SelectedMarkScheme = availablemarkschme;
  }

  ClearCompMarkScheme(ScoreComponentDetail: any, available_qu: any) {
    ScoreComponentDetail.ProjectMarkSchemeId = 0;
    ScoreComponentDetail.SchemeName = '';
    if (available_qu.Scorecomponentdetails.find((a: any) => a.ProjectMarkSchemeId > 0))
      available_qu.DisableMaxmark=true;
    else
      available_qu.DisableMaxmark=false;
  }

  ClearMarkScheme(available_qu: any) {
    available_qu.MarkSchemeId = 0;
    available_qu.SchemeName = '';
    available_qu.DisableMaxmark = false;
    this.SelectedMarkScheme = null;  
  }

  quesaccordianclick(available_qu: any, accordionval: any) {    
    this.qigquestions.forEach((a: any) => a.isActive = false);
    if (available_qu.MarkSchemeId != null) {
      if (available_qu.MarkSchemeId > 0) {
        available_qu.DisableMaxmark = true;
      } else {
        available_qu.DisableMaxmark = false;
      }
    }
    else {
      available_qu.DisableMaxmark = false;
    }
    if (available_qu?.Scorecomponentdetails != null) {
      if (available_qu?.Scorecomponentdetails.find((a: any) => a.ProjectMarkSchemeId > 0)) {
        available_qu.DisableMaxmark = true;
      }
      else {
        available_qu.DisableMaxmark = false;
      }

    } else {
      available_qu.DisableMaxmark = false;
    }

    if (document.getElementById(accordionval)!.getAttribute('aria-expanded') == "true") {
      available_qu.isActive = true;
    }
    else {
      available_qu.isActive = false;
    }
  }

  psgText:any;
  ViewStimulus(available_qu: any) {
    this.StimulusText = available_qu.PassageText; 
    this.psgText = available_qu.PassageText;
    var stimulusDiv = document.getElementById('divstimulus');
    if (stimulusDiv != null) {
      stimulusDiv.innerHTML = "";
      var newTile = document.createElement("div");
      newTile.setAttribute("class", "stimuluspopup");
      newTile.innerHTML = available_qu.PassageText.replace("<![CDATA[","").replace("]]>","");
      setTimeout(() => {
        if (stimulusDiv != null) {
          stimulusDiv.appendChild(newTile);
        }
      }, 100);   
    }
  }

  ScoreComponentChange(available_qu: any, ScoreComponentExists: any) {
    available_qu.IsScoreComponentExists = !available_qu.IsScoreComponentExists;
  }

  componentMarksChange(ScoreComponentDetail: any, event: any) {
    if (ScoreComponentDetail.ProjectMarkSchemeId > 0) {
      if (confirm(this.TranslateMessage("SetUp.QigConfig.TaggedCompMarkSchemeError"))) {
        ScoreComponentDetail.ProjectMarkSchemeId = 0;
      }
      else {
        ScoreComponentDetail.MaxMark = this.previousValue;
      }
    }
  }

  storePreviousValue(value: any) {
    this.previousValue = value;
  }

  CollapseAll() {
    this.qigquestions = [];
    this.accordionQuestions.nativeElement.querySelectorAll('.row-childrow').forEach(
      (question: any) => {
        question.classList.add('collapsed');
      }
    );
    this.accordionQuestions.nativeElement.querySelectorAll('.accordion-collapse').forEach(
      (question: any) => {
        question.classList.remove('show');
      }
    );
  }

  addForm(available_qu: QigQuestionModel) {
    if (available_qu.Scorecomponentdetails != null) {
      this.MaxMarksTotal = available_qu.Scorecomponentdetails.filter(item => item.MaxMark)
        .reduce((sum, current) => sum + current.MaxMark, 0);
      if (this.MaxMarksTotal < available_qu.MaxMark) {
        if (!available_qu.Scorecomponentdetails)
          available_qu.Scorecomponentdetails = [];
        available_qu.Scorecomponentdetails.push({
          ScoreComponentId: available_qu.Scorecomponentdetails.length + 1,
          MaxMark: 0,
          ComponentCode: '',
          ComponentName: '',
          CompMarkSchemeId: 0,
          SchemeName: '',
          ProjectMarkSchemeId: 0,
          ProjectQuestionId: available_qu.ProjectQuestionID,
          IsAutoCreated: false
        });

      } else {
        this.translate
          .get('SetUp.QigConfig.MaxMarksWarning')
          .subscribe((translated: string) => {
            this.Alert.clear();
            this.Alert.warning(translated);
          });
      }
    }
    else if (available_qu.Scorecomponentdetails == null) {
      if (!available_qu.Scorecomponentdetails)
        available_qu.Scorecomponentdetails = [];
      available_qu.Scorecomponentdetails.push({
        ScoreComponentId: available_qu.Scorecomponentdetails.length + 1,
        MaxMark: 0,

        ComponentCode: '',
        ComponentName: '',
        CompMarkSchemeId: 0,
        SchemeName: '',
        ProjectMarkSchemeId: 0,
        ProjectQuestionId: available_qu.ProjectQuestionID,
        IsAutoCreated: false
      });
    }

  }

  deleteForm(available_qu: any, scoreComponentid: any) {
    available_qu.Scorecomponentdetails.forEach((item: { ScoreComponentId: any; }, index: any) => {
      if (item.ScoreComponentId === scoreComponentid) available_qu.Scorecomponentdetails.splice(index, 1);
    });
  }

 letterOnly(event: any) {
    const charCode = event.keyCode;
  
    // Get the current value of the input
    const inputValue = event.target.value;
  
    // Allow upper and lower case letters
    if ((charCode >= 65 && charCode <= 90) || (charCode >= 97 && charCode <= 122)) {
      return true;
    } else if (charCode === 8) {
      // Allow backspace
      return true;
    } else if (charCode === 32) {
      // Check if a space is allowed
      if (inputValue.trim() === '' || /\s/.test(inputValue.charAt(inputValue.length - 1))) {
        return false;
      }
      return true;
    } else {
      return false;
    }
  }
  
  UpdateMaxMarks(ProjectQuestionId: number, questionId: number,QuestionCode:string, maxmarks: any, event: Event) {
    
    this.Mmarks = maxmarks;
this.QuestionCode=QuestionCode
    if (maxmarks != null) {
      this.questionMaxmarks = maxmarks;
      this.Alert.clear();
      this.manditorycondition = this.qigquestions.filter(a => a.noOfMandatoryQuestion != 0 && a.noOfQuestions != a.noOfMandatoryQuestion);

      let confmessage = this.manditorycondition.length > 0 ?
        this.intMessages.Mandatorymaxmarkspopuwarningmessage :
        this.intMessages.maxmarkspopuwarningmessage;

      const handleConfirmationResult = (result: boolean) => {
        this.Alert.clear();
        if (result === true) {
          this.updateMaxMarksAction(ProjectQuestionId);
        }
      };

      this.showConfirmationDialog(confmessage, handleConfirmationResult);
    } else {
      this.translate
        .get('SetUp.QigConfig.Maxmarkemptyalert')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
          this.maxmarksloading = false;
        });
    }
  }


  private showConfirmationDialog(message: string, callback: (result: boolean) => void) {
    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: message
      }
    });

    confirmDialog.afterClosed().subscribe(callback);
  }

  private updateMaxMarksAction(ProjectQuestionId: number) {
    debugger;
    let auditQigQuestionModel: QigQuestionModel = new QigQuestionModel();
    auditQigQuestionModel.QuestionCode = this.QuestionCode;
    auditQigQuestionModel.ProjectQuestionID=ProjectQuestionId
    auditQigQuestionModel.MaxMark=this.questionMaxmarks
    
    this.maxmarksloading = true;
    this.Alert.clear();
    this.qigconfigservice.UpdateMaxMarks(auditQigQuestionModel).pipe(first()).subscribe({
      next: (data: any) => {
        this.Status = data;
        if (this.Status == 'S001') {
          this.translate
            .get('SetUp.QigConfig.updateMaxmarks')
            .subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.success(translated);
              this.maxmarksloading = false;
              this._qigConfigurationsComponent.GetQigQuestionandMarks(this.QigId);
              this.viewqigquestions(this.QigId);
            });
            this.TagScoringComponentLibrarydetails=[]
            this.ChildScoringComponentDetails=[]
        } else if (this.Status == 'SERROR') {
          this.translate
            .get('SetUp.QigConfig.Securityerror')
            .subscribe((translated: string) => {
              this.maxmarksloading = false;
              this.Alert.clear();
              this.Alert.warning(translated);
            });
        } else {
          this.translate
            .get('SetUp.QigConfig.manualmarkingfailed')
            .subscribe((translated: string) => {
              this.Alert.clear();
              this.maxmarksloading = false;
              this.Alert.warning(translated);
            });
        }
      },
      error: (a: any) => {
        this.maxmarksloading = false;
        throw a;
      },
      complete: () => {
        this.maxmarksloading = false;
      }
    });
  }
  SearchScoringComponetLibrary()
  {
 var SearchScoringComponetValue = this.SearchScoringComponetLibraryValue;
    this.AvailableScoringcomponentLibrary = this.SearchAvailableScoringcomponentLibrary.filter(function (el) { return (el.ComponentName.toLowerCase().includes(SearchScoringComponetValue.trim().toLowerCase())) });
  }

  TagScoringComponentLibrary() {
    debugger;
  this.Istag=true
  this.TagScoringComponentLibrarydetails=[]
  this.ChildScoringComponentDetails=[]
    console.log(this.SelectedScoringComponentLibrary);
this.TagScoringComponentLibrarydetails.push(this.SelectedScoringComponentLibrary)
for(let i=0;i<this.TagScoringComponentLibrarydetails[0].ScoringComponentDetails.length;i++)
{
this.ChildScoringComponentDetails.push(this.TagScoringComponentLibrarydetails[0].ScoringComponentDetails[i])
console.log(this.ChildScoringComponentDetails)
}
document.getElementById('closeComponentMarkScheme1')!.click();

  }
  GetScoringComponentLibrary(Maxmarks: any) {
    this.qigconfigservice.GetScoringComponentLibrary(Maxmarks).subscribe({
      next: (data: any) => {
        this.AvailableScoringcomponentLibrary = data;
        this.SearchAvailableScoringcomponentLibrary=this.AvailableScoringcomponentLibrary
        //this.cdRef.detectChanges(); // Force Angular to check for changes
      },
      error: (err) => {
        console.error('Error fetching data:', err);
      }
    });
  }
}
