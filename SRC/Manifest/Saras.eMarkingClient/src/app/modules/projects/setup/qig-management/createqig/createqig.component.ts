import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { QigManagementService } from '../../../../../services/project/setup/qig-management/qig-management.service';
import { CreateQigsModel, QigManagementModel, ProjectQuestionIds, GetManagedQigListDetails } from '../../../../../model/project/setup/qig-management/qig-management-model';
import { MatDialog } from '@angular/material/dialog';
import { Router, ActivatedRoute } from '@angular/router';
import { TagqigComponent } from '../tagqig/tagqig.component';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Component({
  selector: 'emarking-createqig',
  templateUrl: './createqig.component.html',
  styleUrls: ['./createqig.component.css']
})
export class CreateqigComponent implements OnInit {
  selected = 'option1';
  @ViewChild('qigname') modalClose: any;
  constructor(
    public translate: TranslateService,
    public commonService: CommonService,
    public qigmanagementService: QigManagementService,
    public Alert: AlertService,
    private sanitizer: DomSanitizer,
    public dialog: MatDialog, private route: Router, private router: ActivatedRoute
  ) { }
  Viewcomposition: boolean = true;
  Viewnoncomposition: boolean = false;
  activeQig!: any;
  mode?: any = '';
  SelectedquesntnCount: number = 0;
  max_total: number = 0;
  Qigquestions: QigManagementModel[] = [];
  qigdetails = new CreateQigsModel();
  QigName?: string;
  ProjectQigId!: number;
  qnstype: number = 2;
  isloadingpage: boolean = false;
  objprojques!: ProjectQuestionIds;
  selectedquestiontype: number = 0;
  selectedparentquestionid: number = 0;
  IsparentId!: number;
  QigId!: number;
  SelectedFilter: any[] = [];
  FilteredValue: number = 0;
  TempQigquestions: QigManagementModel[] = [];
  QuestionText!: string;
  qigtype!: number;
  selectedQIGQuestions: [] = [];
  activechecked: boolean = false;
  activechecked1: boolean = false;
  IsFinalizeddata!: GetManagedQigListDetails;
  Isfinalized: number = 0;
  IsQuestionsLoading: boolean = false;
  NoQuestionsFound: boolean = false;
  ngOnInit(): void {
    this.isloadingpage = true;
    this.qigdetails.QigMarkingType = 2;
    this.qigdetails.ManadatoryQuestions = 0;
    this.GetManageQigDetails();
    this.QigId = this.router.snapshot.params['QigId'] != undefined ? this.router.snapshot.params['QigId'] : 0;
    if (this.QigId) {
      this.IsQuestionsLoading = true;
      this.GetQigDetails(this.QigId);
    }
    else {
      this.Getqigquestions(this.qigdetails.QigMarkingType);
    }
    this.textIntenationalization();

  }
  CreateQigs() {
    this.qigdetails.projectQuestions = [];
   
    this.qigdetails.QigId = this.QigId;
    if(this.qigdetails.QigMarkingType == 2){
      this.qigdetails.QigMarkingTypeName = "Composition";
    }else {
      this.qigdetails.QigMarkingTypeName = "Non-Composition";
    }
    if (this.Qigquestions.filter(a => a.IsSelected).length > 0) {
      this.TempQigquestions.filter(a => a.IsSelected).forEach(a => {
        this.qigdetails.projectQuestions.push({ ProjectQuestionId: a.ProjectQuestionId, ParentQuestionId: 0 });
      });
    }
    else {
            this.Qigquestions.filter(a => a.IsChildExist).forEach(a => {
      a.QigFibQuestions.filter(b=>b.IsSelected).forEach(b=>{
        this.qigdetails.projectQuestions.push({ProjectQuestionId : b.ProjectQuestionId, ParentQuestionId: b.ParentQuestionId});
      })
    })
  }
    if (this.qigdetails.QigName?.trim() == '' || this.qigdetails.QigName?.trim() == undefined) {
      this.translate
        .get('qigmanagement.create-qig.errorqigempty')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
    else if (this.qigdetails.ManadatoryQuestions <= 0 || this.qigdetails.ManadatoryQuestions == undefined) {
      this.translate
        .get('qigmanagement.create-qig.errormantryquesn')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
    else if (this.SelectedquesntnCount <= 0) {
      this.translate
        .get('qigmanagement.create-qig.selectanyquesn')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
    else if (this.SelectedquesntnCount < this.qigdetails.ManadatoryQuestions) {
      this.translate
        .get('qigmanagement.create-qig.mantryquesnnotgreaterthantotl')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }

    else {
      this.isloadingpage = true;
      this.qigmanagementService.SaveQigQuestionsDetails(this.qigdetails).subscribe({
        next: (data: any) => {
          if (data == 'S001') {
            if (this.QigId) {
              this.route.navigateByUrl('/projects/setup/QigManagement/manage-qig');
              this.translate
                .get('qigmanagement.create-qig.qigupdatedsuccfly')
                .subscribe((translated: string) => {
                  this.Alert.success(translated);
                });
            }
            else {
              this.route.navigateByUrl('/projects/setup/QigManagement/manage-qig');
              this.translate
                .get('qigmanagement.create-qig.qigcreatedsuccfly')
                .subscribe((translated: string) => {
                  this.Alert.success(translated);
                });
            }
            this.Clear();
          } else if (data == 'Invalid') {
            this.translate
              .get('qigmanagement.create-qig.qigcreationfailed')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
          else if (data == 'QigNameExist' || data == 'E005') {
            this.translate
              .get('qigmanagement.create-qig.qignamealreadyexist')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
          else if (data == 'E004') {
            this.translate
              .get('qigmanagement.create-qig.totlmrksofbothquestions')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
          else if (data == 'E006') {
            this.translate
              .get('qigmanagement.create-qig.selectedblaksbelngtodiffentquestion')
              .subscribe((translated: string) => {
                this.Alert.warning(translated);
              });
          }
        },
        error: (a: any) => {
          this.isloadingpage = false;
          throw (a);
        }, complete: () => {
          this.isloadingpage = false;
        }
      })
    }

  }

  Getqigquestions(QigMarkingType: number) {
    this.IsQuestionsLoading = true;
    this.qigmanagementService.GetQigQuestions(QigMarkingType).subscribe({
      next: (data: any) => {
        if (data != null && data != undefined) {
          if (data.length == 0) {
            this.NoQuestionsFound = true;
          }
          this.Qigquestions = data;
          this.TempQigquestions = data;
          this.Qigquestions.filter(a => a.QuestionType == 20).forEach(element => {
            element.TempQigFibQuestions = element.QigFibQuestions;
          });
          if (this.QigId) {
            document.getElementById("Composite")!.setAttribute("disabled", "disabled");
            document.getElementById("NonComposite")!.setAttribute("disabled", "disabled");
            if (this.selectedquestiontype == 20 || this.selectedquestiontype == 85 || this.selectedquestiontype == 92 
              || this.selectedquestiontype == 152 || this.selectedquestiontype == 156 || this.selectedquestiontype == 16 
            ) {
              document.getElementById("mandatoryquestion")!.setAttribute("disabled", "disabled");
              this.selectedQIGQuestions.forEach((element: any) => {
                this.Qigquestions.find(c => c.ProjectQuestionId == element.ParentQuestionID)!.QigFibQuestions.find(s => s.ProjectQuestionId == element.ProjectQuestionId)!.IsSelected = true;
              });
            }
            else {

              this.selectedQIGQuestions.forEach((element: any) => {
                this.Qigquestions.find(a => a.ProjectQuestionId == element.ProjectQuestionId)!.IsSelected = true;
              });
            }
          }
        }
        else {
          this.NoQuestionsFound = true;
        }
      },
      error: (a: any) => {
        this.IsQuestionsLoading = false;
        this.isloadingpage = false;
        throw (a);
      },
      complete: () => {
        this.IsQuestionsLoading = false;
        this.isloadingpage = false;
      }
    });
  }
  GetQigDetails(QigId: number) {
    this.isloadingpage = true;

    this.qigmanagementService.GetQigDetails(QigId).subscribe({   
      next: (data: any) => {
        if (data != null && data != undefined) {
          this.qigdetails.QigName = data.QigName;
          this.qigdetails.ManadatoryQuestions = data.MandatoryQuestion;
          this.qigdetails.QigMarkingType = data.QigType;
          this.qigdetails.QuestionMarkingType = data.QigType;
          this.SelectedquesntnCount = data.NoOfQuestions;
          this.selectedQIGQuestions = data.qigQuestions;
          this.selectedquestiontype = data.qigQuestions.length > 0 ? data.qigQuestions[0].QuestionType : 0;
          this.blankProjectQuestionId = data.qigQuestions.length > 0 ? data.qigQuestions[0].ParentQuestionID : 0;
          this.showBtn = true;
          this.Getqigquestions(this.qigdetails.QigMarkingType);
        }
      },
      error: (a: any) => {
        this.isloadingpage = false;
        throw (a);
      },
      complete: () => {
        this.isloadingpage = false;
      }
    });
  }


  fnapprovalType(evnt: any) {debugger
    this.qigdetails.QigMarkingType = evnt;
   
    this.selectedquestiontype = 0;
    this.selectedparentquestionid = 0;

    document.getElementById("mandatoryquestion")!.removeAttribute("disabled");
    this.Getqigquestions(this.qigdetails.QigMarkingType);
    this.Clear();
    this.activechecked = false;
    this.activechecked1 = false;
    this.SelectedFilter = [];
  }

  Clear() { 
    this.qigdetails.ManadatoryQuestions = 0;
    this.SelectedquesntnCount = 0;
    this.IsQuestionsLoading = false;
  }


  keyPressAlphaNumericWithCharacters(event: KeyboardEvent) {
    var inp = (event.key);
    if (event.key.indexOf(' ') >= 0) {
      return { cannotContainSpace: true }
    }
    if (/[a-zA-Z0-9-_ ]/.test(inp)) {
      return true;
    } else {
      event.preventDefault();
      return false;
    }
  }


  validateNumber(event: any) {
    var invalidChars = ["-", "e", "+", "E"];
    if (invalidChars.includes(event.key)) {
      event.preventDefault();
    }

  }


  keyPressonlyNumeric(event: KeyboardEvent) {

    var inp = (event.key);
    if (/[0-9]/.test(inp)) {
      return true;
    } else {
      event.preventDefault();
      return false;
    }
  }
  onQuestionChecked(objquenscategory: any) {
    if (objquenscategory.QuestionType == 20) {
      this.selectedquestiontype = this.Qigquestions.find(a => a.ProjectQuestionId == objquenscategory.ParentQuestionId)!.TempQigFibQuestions.filter(a => a.IsSelected).length > 0 ? objquenscategory.QuestionType : 0;
      this.SelectedquesntnCount = this.Qigquestions.find(a => a.ProjectQuestionId == objquenscategory.ParentQuestionId)!.TempQigFibQuestions.filter(a => a.IsSelected).length;
      if (this.SelectedquesntnCount > 0) {
        this.selectedparentquestionid = objquenscategory.ParentQuestionId;
        document.getElementById("mandatoryquestion")!.setAttribute("disabled", "disabled");
        this.qigdetails.ManadatoryQuestions = this.SelectedquesntnCount;
      }
      else {
        this.selectedparentquestionid = 0;
        document.getElementById("mandatoryquestion")!.removeAttribute("disabled");
        this.qigdetails.ManadatoryQuestions = this.SelectedquesntnCount;
      }
    }
    else {
      document.getElementById("mandatoryquestion")!.removeAttribute("disabled");
      this.selectedquestiontype = this.TempQigquestions.filter(a => a.IsSelected).length > 0 ? objquenscategory.QuestionType : 0;
      this.SelectedquesntnCount = this.TempQigquestions.filter(a => a.IsSelected).length;
    }
  }
  showBtn: boolean = false;
  blankProjectQuestionId!: number;

  showUndoBtn(ProjectQuestionId: any) {
    if (this.blankProjectQuestionId != ProjectQuestionId) {
      this.showBtn = true;
    }
    else {
      this.showBtn = !this.showBtn;
    }
    this.blankProjectQuestionId = ProjectQuestionId;
  }

  private textIntenationalization() {
    if (this.QigId) {
      this.translate.get('qigmanagement.create-qig.editqig').subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    }
    else {
      this.translate.get('qigmanagement.create-qig.createqig').subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    }

    if (this.QigId) {
      this.translate.get('qigmanagement.create-qig.editpgedesc').subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    } else {
      this.translate.get('qigmanagement.create-qig.pgedesc').subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    }
  }


  SetfilterValue(event: any, optionvalue: any) {
    if (event.checked) {
      this.SelectedFilter.push(optionvalue);
    }
    else {
      this.SelectedFilter = this.SelectedFilter.filter(item => item != optionvalue);
    }

  }
  FilterQuestions(QigMarkingType: number) {
  
    this.Qigquestions = this.TempQigquestions;
    this.Qigquestions.filter(a => a.QuestionType == 20).forEach(element => {
      element.QigFibQuestions = element.TempQigFibQuestions;
    });

    if (this.SelectedFilter.find(a => a == 2) && !this.SelectedFilter.find(a => a == 1)) {
      this.Qigquestions = this.TempQigquestions.filter(a => a.ProjectQigId <= 0);
      this.Qigquestions.forEach(element => {
        if (element.QuestionType == 20 || element.QuestionType == 85) {
          if (element.QigFibQuestions.length == element.QigFibQuestions.filter(a => a.ProjectQigId > 0).length) {
            let index = this.Qigquestions.findIndex(d => d.ProjectQuestionId == element.ProjectQuestionId);
            this.Qigquestions.splice(index, 1);
          }
          else {
            element.QigFibQuestions = element.QigFibQuestions.filter(a => a.ProjectQigId <= 0);
          }
        }
      });

    }
    else if (this.SelectedFilter.find(a => a == 1) && !this.SelectedFilter.find(a => a == 2)) {
      this.Qigquestions = this.TempQigquestions.filter(a => a.ProjectQigId > 0 || a.QuestionType == 20 || a.QuestionType == 85);
      this.Qigquestions.forEach(element => {
        if (element.QuestionType == 20 ) {
          element.QigFibQuestions = element.QigFibQuestions.filter(a => a.ProjectQigId > 0);
        }
      });
    }
    if (this.Qigquestions.length == 0) {
      this.NoQuestionsFound = true;
    }
  }



  ViewQuestion(ProjectQigId: number, ProjQnsId: number, templateRef: any) {
    this.qigmanagementService.GetQuestionText(ProjectQigId, ProjQnsId).subscribe({
      next: (data: any) => {
        if (data != null && data != undefined) {
          if (data.status == "nullorroot") {
            this.translate.get('qigmanagement.qns-qig-mapping.questionnotmovednote').subscribe((translated: string) => {
              this.QuestionText = translated;
            });
          }
          else {
            this.QuestionText = data.QuestionText;       
         }
         
        }
      },
      error: (a: any) => {
        throw (a);
      }
    });
    setTimeout(() => {
    this.dialog.open(templateRef, {
      panelClass: 'alert_class',
      width: '2000px'
    });
  }, 800);
  }
  openDialog(enterAnimationDuration: string, exitAnimationDuration: string, Qnsdetails: QigManagementModel): void {
    Qnsdetails.QigType = this.qigtype;
    Qnsdetails.QnsType = this.qnstype;
    const confirmDialog = this.dialog.open(TagqigComponent, {
      data: Qnsdetails,
      panelClass: ['tag_move', 'modal-lg', 'modal-dialog'],
      enterAnimationDuration,
      exitAnimationDuration,
    });
    confirmDialog.afterClosed().subscribe(result => {
      this.Getqigquestions(this.qigdetails.QigMarkingType);
    });
  }
  trustedHtmlCache: { [key: string]: SafeHtml } = {};
  getTrustedHtml(htmlcode:string): SafeHtml {
    if (!this.trustedHtmlCache[htmlcode]) {
      this.trustedHtmlCache[htmlcode] = this.sanitizer.bypassSecurityTrustHtml(htmlcode);
    }
    return this.trustedHtmlCache[htmlcode];
  }
  GetManageQigDetails() {
    this.qigmanagementService.GetManagedQigDetails().subscribe({
      next: (data: any) => {
        this.IsFinalizeddata = data;
        this.Isfinalized = this.IsFinalizeddata.ManageQigsList.filter(a => a.IsQigSetupFinalized).length;
      },
      error: (a: any) => {
        throw (a);
      }
    });
  }
}
