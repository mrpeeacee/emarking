import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs';
import {
  CheckDiscrepancyModel,
  ProjectClosureModel,
} from 'src/app/model/project/setup/project-closure-model';
import { ConfirmationDialogComponent } from 'src/app/modules/shared/confirmation-dialog/confirmation-dialog.component';
import { AlertService } from 'src/app/services/common/alert.service';
import { ProjectClosureService } from 'src/app/services/project/setup/project-closure.service';
import { CommonService } from 'src/app/services/common/common.service';

@Component({
  templateUrl: './project-closure.component.html',
  styleUrls: ['./project-closure.component.css'],
})
export class ProjectClosureComponent implements OnInit {
  constructor(
    public closureService: ProjectClosureService,
    public Alert: AlertService,
    private dialog: MatDialog,
    public translate: TranslateService,
    public commonService: CommonService
  ) {}

  closureData!: ProjectClosureModel;
  Iscompleted!: boolean;
  IsQuestionType!: boolean;
  QigName!: any;
  QigNamesList!: CheckDiscrepancyModel[];
  IsExists: boolean = true;
  discrepancy!: boolean;
  enableCheckbox: boolean = false;
  enableSaveButton: boolean = false;
  projectStatus: boolean = false;
  updateloading: boolean = false;

  intMessages: any = {
    nodata: '',
    savebtn: '',
    success: '',
    remarkval: '',
    error: '',
    comalt: '',
    remarklen: '',
    reopen: '',
    wantreopn: '',
    reopensucss: '',
    alreadclosed: '',
    discexist: '',
    disexistin: '',
    confirmclearscript: '',
    clerrcsuccess: '',
    clearrcerror: '',
  };

  transTitles() {
    this.translate
      .get('SetUp.closure.title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.translate
      .get('SetUp.closure.pgedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    this.translate
      .get('SetUp.closure.datafnd')
      .subscribe((translated: string) => {
        this.intMessages.nodata = translated;
      });
    this.translate
      .get('SetUp.closure.wantsave')
      .subscribe((translated: string) => {
        this.intMessages.savebtn = translated;
      });
    this.translate
      .get('SetUp.closure.success')
      .subscribe((translated: string) => {
        this.intMessages.success = translated;
      });
    this.translate
      .get('SetUp.closure.remarkval')
      .subscribe((translated: string) => {
        this.intMessages.remarkval = translated;
      });
    this.translate
      .get('SetUp.closure.error')
      .subscribe((translated: string) => {
        this.intMessages.error = translated;
      });
    this.translate
      .get('SetUp.closure.comalt')
      .subscribe((translated: string) => {
        this.intMessages.comalt = translated;
      });
    this.translate
      .get('SetUp.closure.rklen')
      .subscribe((translated: string) => {
        this.intMessages.remarklen = translated;
      });
    this.translate
      .get('SetUp.closure.prjclose')
      .subscribe((translated: string) => {
        this.intMessages.reopen = translated;
      });
    this.translate
      .get('SetUp.closure.wantreopn')
      .subscribe((translated: string) => {
        this.intMessages.wantreopn = translated;
      });
    this.translate
      .get('SetUp.closure.confirmclearscript')
      .subscribe((translated: string) => {
        this.intMessages.confirmclearscript = translated;
      });

    this.translate
      .get('SetUp.closure.clerrcsuccess')
      .subscribe((translated: string) => {
        this.intMessages.clerrcsuccess = translated;
      });
    this.translate
      .get('SetUp.closure.clearrcerror')
      .subscribe((translated: string) => {
        this.intMessages.clearrcerror = translated;
      });

    this.translate
      .get('SetUp.closure.reopensucss')
      .subscribe((translated: string) => {
        this.intMessages.reopensucss = translated;
      });
    this.translate
      .get('SetUp.closure.alreadclosed')
      .subscribe((translated: string) => {
        this.intMessages.alreadclosed = translated;
      });
    this.translate
      .get('SetUp.closure.discexist')
      .subscribe((translated: string) => {
        this.intMessages.discexist = translated;
      });
    this.translate
      .get('SetUp.closure.disexistin')
      .subscribe((translated: string) => {
        this.intMessages.disexistin = translated;
      });
  }

  ngOnInit(): void {
    this.transTitles();
    this.getProjectConfig(true);
  }

  rc2checking!: string;
  getProjectConfig(chkDis: boolean) {
    this.closureService
      .GetProjectClosure()
      .pipe(first())
      .subscribe({
        next: (data: ProjectClosureModel) => {
          if (data != null && data != undefined) {
            this.closureData = data;
            if (chkDis) {
              this.closureData.QigModels.forEach((val) => {
                val.IsClosed = false;
                if (this.isConditionMet(val)) {
                  if (
                    (val.RC2Exists == 0 || val.RC2Exists == 1) &&
                    val.Rc2UnApprovedCount == 0 &&
                    val.ToBeSampledForRC2 == 0
                  ) {
                    val.IsClosed = true;
                  }
                }
              });
            } else {
              this.closureData.QigModels.forEach((val) => {
                if (
                  val.TotalScriptCount <= 0 ||
                  val.Rc1UnApprovedCount != 0 ||
                  (val.Rc2UnApprovedCount == 0 ? 0 : 'NA') !=
                    (val.ToBeSampledForRC2 == 0 ? 0 : 'NA') ||
                  val.LivePoolScriptCount != 0 ||
                  val.CheckedOutScripts != 0 ||
                  (val.ManualMarkingCount <= 0 &&
                    val.SubmittedScriptCount <= 0 &&
                    val.ManualMarkingCount != val.SubmittedScriptCount)
                ) {
                  val.IsClosed = false;
                } else {
                  val.IsClosed = true;
                }
              });
              this.closureData.ProjectStatus = 'Open';

              this.IsExists = false;
              this.IsQuestionType = true;

              if (
                this.closureData.Remarks != null &&
                this.closureData.Remarks != '' &&
                this.closureData.Remarks != undefined
              ) {
                this.closureData.Remarks = '';
              }
            }
          } else {
            this.Alert.warning(this.intMessages.nodata);
          }
          if (
            this.closureData.QigModels.filter((a) => a.QuestionsType == 20)
              .length > 0
          ) {
            this.IsQuestionType = true;
          } else {
            this.IsQuestionType = false;
          }
          if (
            this.closureData.QigModels.filter(
              (a) => a.QuestionsType != 20 && a.IsClosed
            ).length == this.closureData.QigModels.length
          ) {
            this.enableSaveButton = true;
          }
          this.projectStatus = this.closureData.ProjectStatus != 'closed';
          this.enableCheckbox =
            this.closureData.QigModels.filter((a) => a.QuestionsType == 20)
              .length > 0 &&
            this.closureData.QigModels.filter((a) => a.QuestionsType == 20)
              .length ==
              this.closureData.QigModels.filter(
                (a) => a.QuestionsType == 20 && a.IsClosed
              ).length;

          if (
            this.closureData.ReopenRemarks != null &&
            this.closureData.ReopenRemarks != '' &&
            this.closureData.ReopenRemarks != undefined
          ) {
            this.closureData.ReopenRemarks = '';
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  isConditionMet(val: any): boolean {
    return (
      val.TotalScriptCount > 0 &&
      val.ManualMarkingCount > 0 &&
      val.SubmittedScriptCount > 0 &&
      val.ManualMarkingCount === val.SubmittedScriptCount &&
      val.LivePoolScriptCount === 0 &&
      val.Rc1UnApprovedCount === 0 &&
      val.CheckedOutScripts === 0
    );
  }

  checkexist(event: any) {
    if (event.checked) {
      this.CheckDiscrepancy();
    } else {
      this.getProjectConfig(true);
    }
  }

  CheckDiscrepancy() {
    this.updateloading = true;
    this.closureService
      .CheckDiscrepancy()
      .pipe(first())
      .subscribe({
        next: (data: ProjectClosureModel) => {
          if (data != null && data != undefined) {
            this.closureData = data;
            if (this.closureData.DiscrepancyModels.length > 0) {
              var disqiglist = this.closureData.DiscrepancyModels.filter(
                (x: any) => x.IsDiscrepancyExist
              );
              var disqignames = '';
              if (disqiglist != undefined && disqiglist != null) {
                disqignames = Array.prototype.map
                  .call(disqiglist, (s) => s.QigName)
                  .toString();
              }
              this.Alert.warning(
                this.intMessages.disexistin + ':' + disqignames
              );
              this.getProjectConfig(true);
            } else {
              this.Alert.success(this.intMessages.discexist);
              this.getProjectConfig(false);
              this.enableSaveButton = true;
            }
          } else {
            this.Alert.warning(this.intMessages.nodata);
          }
        },
        error: (a: any) => {
          this.updateloading = false;
          throw a;
        },
        complete: () => {
          this.updateloading = false;
          this.closureData.Remarks = '';
        },
      });
  }

  updateProjectclosure() {
    if (this.validation()) {
      if (
        this.closureData.Remarks != null &&
        this.closureData.Remarks != '' &&
        this.closureData.Remarks != undefined
      ) {
        const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
          data: {
            message: this.intMessages.savebtn,
          },
        });
        confirmDialog.afterClosed().subscribe((res) => {
          this.updateloading = true;
          if (res === true) {
            this.closureService
              .UpdateProjectClosure(this.closureData)
              .pipe(first())
              .subscribe({
                next: (response: any) => {
                  if (response == 'S001') {
                    this.Alert.success(this.intMessages.success);
                    this.getProjectConfig(true);
                    this.closureData.Remarks = '';
                  } else if (response == 'RM001') {
                    this.Alert.warning(this.intMessages.remarkval);
                  } else if (response == 'QI001') {
                    this.Alert.warning(this.intMessages.comalt);
                    this.enableSaveButton = false;
                  } else if (response == 'RL001') {
                    this.Alert.warning(this.intMessages.remarklen);
                  } else if (response == 'Closed') {
                    this.Alert.warning(this.intMessages.alreadclosed);
                    this.getProjectConfig(true);
                  } else {
                    this.Alert.warning(this.intMessages.error);
                  }
                },
                error: (a: any) => {
                  this.updateloading = false;
                  throw a;
                },
                complete: () => {
                  this.updateloading = false;
                },
              });
          }
        });
      } else {
        this.Alert.warning(this.intMessages.remarkval);
      }
    }
  }

  updateProjectreopen() {
    if (
      this.closureData.ReopenRemarks != null &&
      this.closureData.ReopenRemarks != '' &&
      this.closureData.ReopenRemarks != undefined
    ) {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: this.intMessages.wantreopn,
        },
      });
      confirmDialog.afterClosed().subscribe((res) => {
        if (res === true) {
          this.updateloading = true;
          this.closureService
            .UpdateProjectReopen(this.closureData)
            .pipe(first())
            .subscribe({
              next: (response: any) => {
                if (response == 'S001') {
                  this.Alert.success(this.intMessages.reopensucss);
                  this.enableSaveButton = false;
                  this.getProjectConfig(true);
                  this.closureData.ReopenRemarks = '';
                } else if (response == 'RM001') {
                  this.Alert.warning(this.intMessages.remarkval);
                } else if (response == 'RL001') {
                  this.Alert.warning(this.intMessages.remarklen);
                } else if (response == 'Reopened') {
                  this.Alert.warning('This project already Re-Opened');
                  this.enableSaveButton = false;
                  this.getProjectConfig(true);
                } else {
                  this.Alert.warning(this.intMessages.error);
                }
              },
              error: (a: any) => {
                this.updateloading = false;
                throw a;
              },
              complete: () => {
                this.updateloading = false;
              },
            });
        }
      });
    } else {
      this.Alert.warning('Remark cannot be blank.');
    }
  }

  private validation(): boolean {
    if (
      this.closureData.QigModels.filter((a) => a.TotalScriptCount <= 0).length >
      0
    ) {
      this.Alert.warning(this.intMessages.comalt);
      this.getProjectConfig(true);
      return false;
    } else if (
      this.closureData.QigModels.filter(
        (a) => a.SubmittedScriptCount != a.ManualMarkingCount
      ).length > 0
    ) {
      this.Alert.warning(this.intMessages.comalt);
      this.getProjectConfig(true);
      return false;
    } else if (
      this.closureData.QigModels.filter((a) => a.LivePoolScriptCount > 0)
        .length > 0
    ) {
      this.Alert.warning(this.intMessages.comalt);
      this.getProjectConfig(true);
      return false;
    } else if (
      this.closureData.QigModels.filter((a) => a.Rc1UnApprovedCount > 0)
        .length > 0
    ) {
      this.Alert.warning(this.intMessages.comalt);
      this.getProjectConfig(true);
      return false;
    } else if (
      this.closureData.QigModels.filter((a) => a.RC2Exists != 0).length > 0
    ) {
      if (
        this.closureData.QigModels.filter((a) => a.Rc2UnApprovedCount > 0)
          .length > 0
      ) {
        this.Alert.warning(this.intMessages.comalt);
        this.getProjectConfig(true);
        return false;
      } else if (
        this.closureData.QigModels.filter((a) => a.ToBeSampledForRC2 > 0)
          .length > 0
      ) {
        this.Alert.warning(this.intMessages.comalt);
        this.getProjectConfig(true);
        return false;
      }
    } else if (
      this.closureData.QigModels.filter((a) => a.CheckedOutScripts > 0).length >
      0
    ) {
      this.Alert.warning(this.intMessages.comalt);
      this.getProjectConfig(true);
      return false;
    }

    return true;
  }

  checkvalidation(chkDis: boolean) {
    this.closureService
      .GetProjectClosure()
      .pipe(first())
      .subscribe({
        next: (data: ProjectClosureModel) => {
          if (data != null && data != undefined) {
            this.closureData = data;
            if (chkDis) {
              this.closureData.QigModels.forEach((val) => {
                val.IsClosed = false;
                if (
                  val.TotalScriptCount > 0 &&
                  val.ManualMarkingCount > 0 &&
                  val.SubmittedScriptCount > 0 &&
                  val.ManualMarkingCount == val.SubmittedScriptCount &&
                  val.LivePoolScriptCount == 0 &&
                  val.Rc1UnApprovedCount == 0 &&
                  val.CheckedOutScripts == 0
                ) {
                  if (
                    (val.RC2Exists == 1 || val.RC2Exists == 0) &&
                    val.Rc2UnApprovedCount == 0 &&
                    val.ToBeSampledForRC2 == 0
                  ) {
                    val.IsClosed = true;
                  }
                }
              });
            } else {
              this.closureData.QigModels.forEach((val) => {
                if (
                  val.TotalScriptCount <= 0 ||
                  val.Rc1UnApprovedCount != 0 ||
                  (val.Rc2UnApprovedCount == 0 ? 0 : 'NA') !=
                    (val.ToBeSampledForRC2 == 0 ? 0 : 'NA') ||
                  val.LivePoolScriptCount != 0 ||
                  val.CheckedOutScripts != 0 ||
                  (val.ManualMarkingCount <= 0 &&
                    val.SubmittedScriptCount <= 0 &&
                    val.ManualMarkingCount != val.SubmittedScriptCount)
                ) {
                  val.IsClosed = false;
                } else {
                  val.IsClosed = true;
                }
              });
              this.closureData.ProjectStatus = 'Open';

              this.IsExists = false;
              this.IsQuestionType = true;
            }
          } else {
            this.Alert.warning(this.intMessages.nodata);
          }
          if (
            this.closureData.QigModels.filter((a) => a.QuestionsType == 20)
              .length > 0
          ) {
            this.IsQuestionType = true;
          } else {
            this.IsQuestionType = false;
          }
          if (
            this.closureData.QigModels.filter(
              (a) => a.QuestionsType != 20 && a.IsClosed
            ).length == this.closureData.QigModels.length
          ) {
            this.enableSaveButton = true;
          }
          this.projectStatus = this.closureData.ProjectStatus != 'closed';
          this.enableCheckbox =
            this.closureData.QigModels.filter((a) => a.QuestionsType == 20)
              .length > 0 &&
            this.closureData.QigModels.filter((a) => a.QuestionsType == 20)
              .length ==
              this.closureData.QigModels.filter(
                (a) => a.QuestionsType == 20 && a.IsClosed
              ).length;

          if (this.validation()) {
            if (
              this.closureData.Remarks != null &&
              this.closureData.Remarks != '' &&
              this.closureData.Remarks != undefined
            ) {
              const confirmDialog = this.dialog.open(
                ConfirmationDialogComponent,
                {
                  data: {
                    message: this.intMessages.savebtn,
                  },
                }
              );
              confirmDialog.afterClosed().subscribe((res) => {
                if (res === true) {
                  this.closureService
                    .UpdateProjectClosure(this.closureData)
                    .pipe(first())
                    .subscribe({
                      next: (response: any) => {
                        if (response == 'S001') {
                          this.Alert.success(this.intMessages.success);
                          this.getProjectConfig(true);
                        } else if (response == 'RM001') {
                          this.Alert.warning(this.intMessages.remarkval);
                        } else if (response == 'QI001') {
                          this.Alert.warning(this.intMessages.comalt);
                        } else if (response == 'RL001') {
                          this.Alert.warning(this.intMessages.remarklen);
                        } else if (response == 'Closed') {
                          this.Alert.warning(this.intMessages.reopen);
                          this.getProjectConfig(true);
                        } else {
                          this.Alert.warning(this.intMessages.error);
                        }
                      },
                      error: (a: any) => {
                        throw a;
                      },
                    });
                }
              });
            } else {
              this.Alert.warning(this.intMessages.remarkval);
            }
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  ClearPendingScripts(qigId: number) {
    if (qigId > 0) {
      const confirmDialog = this.dialog.open(ConfirmationDialogComponent, {
        data: {
          message: this.intMessages.confirmclearscript,
        },
      });
      confirmDialog.afterClosed().subscribe((res) => {
        if (res === true) {
          this.updateloading = true;
          this.closureService
            .ClearPendingScripts(qigId)
            .pipe(first())
            .subscribe({
              next: (response: any) => {
                if (response == 'S001') {
                  this.Alert.success(this.intMessages.clerrcsuccess);
                } else {
                  this.Alert.error(this.intMessages.clearrcerror);
                }
              },
              error: (a: any) => {
                this.updateloading = false;
                this.Alert.error(this.intMessages.clearrcerror);
                throw a;
              },
              complete: () => {
                this.updateloading = false;
                this.enableSaveButton = false;
                this.getProjectConfig(true);
              },
            });
        }
      });
    }
  }
}
