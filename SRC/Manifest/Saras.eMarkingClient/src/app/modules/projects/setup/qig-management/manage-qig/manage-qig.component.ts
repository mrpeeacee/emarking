import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TagqigComponent } from '../tagqig/tagqig.component';
import { TranslateService } from '@ngx-translate/core';
import { CreateqigComponent } from '../createqig/createqig.component';
import { CommonService } from 'src/app/services/common/common.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { QigManagementService } from '../../../../../services/project/setup/qig-management/qig-management.service';
import {
  ManageQigs,
  QigDetails,
  QigQuestionsDetails,
  GetManagedQigListDetails,
  ManageQigsCounts,
  FinalRemarks,
} from '../../../../../model/project/setup/qig-management/qig-management-model';
import { Router, ActivatedRoute } from '@angular/router';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { first } from 'rxjs/operators';
import { ProjectLevelConfigService } from '../../../../../services/project/setup/project-level-config.service';
import { QigService } from 'src/app/services/project/qig.service';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from 'src/environments/environment';
import * as CryptoJS from 'crypto-js';
@Component({
  selector: 'emarking-manage-qig',
  templateUrl: './manage-qig.component.html',
  styleUrls: ['./manage-qig.component.css'],
})
export class ManageQigComponent implements OnInit {
  @ViewChild('qnspopmodel') modalClose: any;
  @ViewChild('qigpopmodel') qigmodalCloses: any;
  @ViewChild('openModalPopup') openModalPopup!: ElementRef;
  @ViewChild('closeAddExpenseModal') closeAddExpenseModal!: ElementRef;
  managedQigs: ManageQigs[] = [];
  managedQigListDetails!: GetManagedQigListDetails;
  manageQigsCounts!: ManageQigsCounts;
  mappedqigs: QigQuestionsDetails[] = [];
  final = new FinalRemarks();
  Qigdetails!: QigDetails;
  ProjectQigId!: number;
  popuploading: boolean = false;
  Ispageloading: boolean = false;
  qigsetupstats?: boolean = false;
  totalnoofQuestions: any;
  totalnoofQig!: any;
  totalnoofuntaggedquestion!: any;
  totalnooftaggedquestion!: any;
  emptyQigs!: ManageQigs[];
  Isfinalized: number = 0;
  remarks: string = '';
  showdiv: number = 0;
  isShowDiv:boolean=false;
  Isprojectclosed: number = 0;
  nullqig: number = 0;
  Noofkeyword!: number;
  isClosed: any;
  ValidationErrMsg: string = "";
  ValidationErrMsgUserBlock: string = "";
  @ViewChild('txtsrch') TxtSearch: any;
  signinForm: FormGroup;
  encyptionRequried: boolean = true;

  constructor(
    public fb: FormBuilder,
    public dialog: MatDialog,
    public translate: TranslateService,
    public commonService: CommonService,
    public qigmanagementService: QigManagementService,
    public projectConfigService: ProjectLevelConfigService,
    public Alert: AlertService,
    private route: Router,
    private router: ActivatedRoute,
    public qigservice: QigService
  ) {
    this.signinForm = this.fb.group({
      loginname: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  openDialog(
    enterAnimationDuration: string,
    exitAnimationDuration: string
  ): void {
    this.dialog.open(TagqigComponent, {
      panelClass: ['tag_move', 'modal-lg', 'modal-dialog'],
      enterAnimationDuration,
      exitAnimationDuration,
    });
  }

  createqig(
    enterAnimationDuration: string,
    exitAnimationDuration: string
  ): void {
    this.dialog.open(CreateqigComponent, {
      panelClass: ['tag_move', 'modal-lg', 'modal-dialog'],
      enterAnimationDuration,
      exitAnimationDuration,
    });
  }

  ngOnInit(): void {
    this.Getqigworkflowtracking();
    this.textIntenationalization();
    this.GetManageQigDetails();
    this.GetUntaggedQuestionsDetails();
    this.remarks = '';
    this.final.Remarks = '';



    this.translate.get('Login.IsPasswordEncryptedInClient').subscribe((translated: string) => {
      this.encyptionRequried = JSON.parse(translated);
    });



  }

  GetManageQigDetails() {
    this.Ispageloading = true;
    this.qigmanagementService.GetManagedQigDetails().subscribe({
      next: (data: any) => {
        this.managedQigListDetails = data;
        this.nullqig =
          this.managedQigListDetails.ManageQigsCountsList.TotalNoOfQIGs;
        this.emptyQigs = this.managedQigListDetails.ManageQigsList.filter(
          (a) => a.NoOfQuestions == 0
        );
        this.Isfinalized = this.managedQigListDetails.ManageQigsList.filter(
          (a) => a.IsQigSetupFinalized
        ).length;
        if (this.Isfinalized) {
          this.remarks = this.managedQigListDetails.ManageQigsList[0].Remarks;
        }
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.Ispageloading = false;
      },
    });
  }

  SaveQigdetails(qigdetails: QigDetails) {
    this.Ispageloading = true;
    if (
      qigdetails.MandatoryQuestion == 0 &&
      qigdetails.qigQuestions.length > 0
    ) {
      this.translate
        .get('qigmanagement.manage-qig.mandtryError')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    } else if (
      qigdetails.MandatoryQuestion != null &&
      qigdetails.qigQuestions.length < qigdetails.MandatoryQuestion
    ) {
      this.translate
        .get('qigmanagement.create-qig.mantryquesnnotgreaterthantotl')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    } else {
      this.qigmanagementService.SaveQigDetails(qigdetails).subscribe({
        next: (data: any) => {
          if (data != undefined) {
            if (data == 'SU001') {
              this.translate
                .get('qigmanagement.manage-qig.mantorysucesstext')
                .subscribe((translated: string) => {
                  this.Alert.clear();
                  this.Alert.success(translated);
                  this.modalClose.nativeElement.click();
                });
            } else if (data == 'ERR02') {
              this.translate
                .get('qigmanagement.manage-qig.ErrorMesg')
                .subscribe((translated: string) => {
                  this.Alert.clear();
                  this.Alert.warning(translated);
                  this.Ispageloading = false;
                });
            }
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.Ispageloading = false;
        },
      });
    }
  }

  getqigdetails(ProjectQigId: number) {
    this.Ispageloading = true;
    this.ProjectQigId = ProjectQigId;
    this.Alert.clear();
    this.qigmanagementService.GetQigDetails(ProjectQigId).subscribe({
      next: (data: any) => {
        if (data != null && data != undefined) {
          this.Qigdetails = data;

          if (data.IsQigSetup) {
            this.translate
              .get('qigmanagement.manage-qig.qigsetupcompleted')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.warning(translated);
              });
          }
        }
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.Ispageloading = false;
      },
    });
  }

  private textIntenationalization() {
    this.translate
      .get('qigmanagement.manage-qig.title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });

    this.translate
      .get('qigmanagement.manage-qig.pgedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
  }

  keyPressAlphaNumericWithCharacters(event: KeyboardEvent) {
    var inp = event.key;
    if (/[0-9]/.test(inp)) {
      return true;
    } else {
      event.preventDefault();
      return false;
    }
  }

  SaveQigQuestions(remarks: string) {
    this.Alert.clear();
    this.final.Remarks = remarks;
    this.final.projectqigId = this.managedQigListDetails.ManageQigsList.map(
      (a) => Object.assign({ projectqigId: a.projectqigId })
    );

    if (this.final.Remarks.trim() == '') {
      this.translate
        .get('qigmanagement.manage-qig.rmksfieldisempty')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    } else {
      this.qigmanagementService.SaveQigQuestions(this.final).subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            setTimeout(() => {
              if (data == 'S001') {
                this.updateProjectRcSetting();
                this.translate
                  .get(
                    'qigmanagement.manage-qig.qigsetupisbeenfinalizedsuccessfly'
                  )
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                    this.Alert.clear();
                  });
              }
            }, 100);
            this.qigmodalCloses.nativeElement.click();
          }
          if (data == 'ERR01') {
            this.translate
              .get('qigmanagement.manage-qig.qigsetupfinalizefailed')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.warning(translated);
              });
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.GetManageQigDetails();
          this.Ispageloading = false;
        },
      });
    }
  }

  updateProjectRcSetting() {
    this.projectConfigService
      .updateProjectRandomCheck()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            this.Alert.clear();
            this.GetManageQigDetails();
          } else {
            this.Alert.error('Error while finalising the QIG.');
          }
        },
        error: (a: any) => {
          this.Ispageloading = false;
          throw a;
        },
        complete: () => {
          this.Ispageloading = false;
        },
      });
  }

  NavigatetoCreateQIG(QigId: number) {
    this.route.navigateByUrl(
      'projects/setup/QigManagement/' + QigId + '/editqig'
    );
  }

  GetUntaggedQuestionsDetails() {
    this.qigmanagementService.GetUntaggedQuestionsDetails().subscribe({
      next: (data: any) => {
        this.mappedqigs = data;
        this.Ispageloading = false;
      },
      complete: () => {
        this.remarks = '';
        this.Ispageloading = false;
      },
    });
    this.isShowDiv=false;
  }

  DeleteQig(QidId: number,QigName:string,QigType:number) {
    const TempManageQigObject = new ManageQigs()
    TempManageQigObject.projectqigId = QidId;
    TempManageQigObject.QigName = QigName;
    TempManageQigObject.QigType = QigType;
    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: 'Are you sure you want to delete?',
      },
    });
    confirmDialog.afterClosed().subscribe((result) => {
      this.Alert.clear();
      this.Ispageloading = true;
      if (result === true) {
        this.qigmanagementService.DeleteQig(TempManageQigObject).subscribe({
          next: (data: any) => {
            if (data == 'D001') {
              this.GetManageQigDetails();
              this.Alert.success('QIG Deleted successfully.');
            }
          },
          complete: () => {
            this.Ispageloading = false;
          },
        });
      }
    });
  }

  QigReset() {
    const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
      data: {
        message: "Are you sure you want to Reset QIG's?",
      },
    });
    confirmDialog.afterClosed().subscribe((result) => {
      this.Alert.clear();
     
      if (result === true) {
       
        this.signinForm.controls['loginname'].reset();


        this.signinForm.controls['password'].reset();

        this.openModalPopup.nativeElement.click();

    


      }
    });
    // this.qigmanagementService.QigReset().subscribe({
    //   next:(data:any) => {

    //   }
    // });
  }

  openfinalize() {
    this.showdiv = 0;
  }

  Getqigworkflowtracking() {
    this.qigservice
      .Getqigworkflowtracking(0, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          let WorkFlowStatusTracking = data;
          if(WorkFlowStatusTracking.length > 0){
            this.isClosed = WorkFlowStatusTracking[0].ProjectStatus;
          }
          else{
            this.isClosed = 0;
          }
        
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

  loginUser() {
    if (!this.Ispageloading) {
      this.Ispageloading = true;
      if (!this.signinForm.valid) {
        this.Ispageloading = false;
        return;
      }
      this.ValidationErrMsg = "";
      this.ValidationErrMsgUserBlock = "";
      if (this.encyptionRequried) {
        this.signinForm.patchValue({
          password: this.encryptUsingAES256(this.signinForm.get('password')?.value).toString()
        });
      }
      this.qigmanagementService.Login(this.signinForm.value).pipe(first()).subscribe({
        next: (data: any) => {
          if(data=="SERROR"){
            this.Alert.warning("Please enter valid user credential");
          }       
          else if (data == "S001") {

            this.qigmanagementService.QigReset().subscribe({
              next: (res: any) => {
                if (res == 'S001') {
                  this.closeAddExpenseModal.nativeElement.click();
                  this.Alert.success('QIG reset completed successfully.');
                  this.GetManageQigDetails();
                  this.Ispageloading = false;
                 
                }
              },
              complete: () => {
                this.Ispageloading = false;
              },
            });

          }
          else if (data == "E002") {
            this.Alert.warning("Please entere valid user credential");
            this.Ispageloading = false;
          }
          else if (data == "E003") {
            this.Alert.warning("Entered user credentials is not belongs to service admin");
            this.Ispageloading = false;
          }
          else if (data == "NOTMAP") {
            this.Alert.warning("Entered user credential is not map to this project");
            this.Ispageloading = false;
          }
          else if (data == "DUPLICATE") {
            this.Alert.warning("Enter other service admin login");
            this.Ispageloading = false;
          }
          else if (data == "SCRPTNOTIMPORTED") {
            this.Alert.warning("R-pack is not imported yet");
            this.Ispageloading = false;
          }
          else if (data == "USERDISABLED") {
            this.Alert.warning("Your account is disabled. Please contact Admin.");
            this.Ispageloading = false;
          }
        },
        error: (a: any) => {
          this.signinForm.controls['password'].reset();
          this.signinForm.get("password")?.setErrors(null);
          this.showErrorMessage(a.message);
          this.Ispageloading = false;
        },
        complete: () => {
         // this.Ispageloading = false
        }
      });
    }
  }

  private key = CryptoJS.enc.Utf8.parse(environment.enyKey);
  private iv = CryptoJS.enc.Utf8.parse(environment.enyKey);

  encryptUsingAES256(encString: string) {
    return CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(encString), this.key, {
      keySize: 128 / 8,
      iv: this.iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    });
  }

  showErrorMessage(message: string) {
    this.ValidationErrMsg = message;
  }

  NoOfUntaggedQuest(){
    this.GetUntaggedQuestionsDetails();
    if(this.managedQigListDetails?.ManageQigsCountsList
      .TotalNoOfUnTaggedQuestions == 0)
    {
      this.showdiv=3;
    }
    else{ 
      this.showdiv=1;
    }
    this.isShowDiv=true;
  }

}
