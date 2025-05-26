import { Component } from '@angular/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { QigModel } from 'src/app/model/project/qig';
import { first } from 'rxjs/operators';
import {
  AppSettingModel,
  AppSettingValueType,
} from 'src/app/model/appsetting/app-setting-model';
import { TranslateService } from '@ngx-translate/core';
import { RandomCheckSetting } from 'src/app/services/project/setup/qig-configuration/random-check-setting.service';
import { QigConfigurationsComponent } from '../qig-configurations.component';


@Component({
  selector: 'emarking-random-check',
  templateUrl: './random-check.component.html',
  styleUrls: ['./random-check.component.css']
})
export class RandomCheckComponent {

  constructor(
    public QigconfigServicecls: RandomCheckSetting,
    public Alert: AlertService,
    public translate: TranslateService,
    public _qigConfigurationsComponent: QigConfigurationsComponent
  ) { }

  QigRandomcheckData!: QigModel;
  totlQigs: number = 0;
  QigId!: number;
  questionview: any = {
    QnsLabel: '',
    QnsCout: 0,
    QigId: 0,
  };
  Rc1scripts!: any;
  Rc2scripts!: any;

  annloading: boolean = false;

  numericOnly(event: { key: string }): boolean {
    let patt = /^([0-9])$/;
    return patt.test(event.key);
  }

  toggleRcType(
    KeyID: number,
    qtype: AppSettingModel[],
    QigRandomcheckData: QigModel
  ) {
    if (qtype != null && qtype?.length > 0) {
      qtype?.forEach((element) => {

        if (element?.Children != null && element?.Children?.length > 0) {
          element?.Children?.forEach((child) => {

            if (element?.AppsettingKey == "RCT1") {
              if (child?.AppsettingKey == "SMPLRTT1") {
                this.percFunRc1(child?.Value, this.QigRandomcheckData?.LivePoolCount, this.QigRandomcheckData?.SubmittedCount)
              }
            }

            if (child?.AppsettingKey == "SMPLRTT2") {
              this.percFunRc2(child?.Value, this.QigRandomcheckData?.RC1SubmittedCount, this.QigRandomcheckData?.LivePoolCount);
            }


          });
        }

        if (element?.AppSettingKeyID != KeyID) {
          element.Value = false;
        } else {
          element.Value = true;
          if (element?.AppsettingKey == 'RCT2') {
            QigRandomcheckData.RcType = 2;
          } else if (element?.AppsettingKey == 'RCT1') {
            QigRandomcheckData.RcType = 1;
          }
        }
      });
    }
  }
  toggleMaking(martype: AppSettingModel, mrkparent: AppSettingModel[]) {
    mrkparent.forEach((element) => {
      element.Value = false;
    });
    martype.Value = true;
  }

  validateNumber(event: any) {
    var invalidChars = ["-", "e", "+", "E"];
    if (invalidChars.includes(event.key)) {
      event.preventDefault();
    }
  }

  GetRandomcheckQIGs(QigIdValue: number) {
    this.QigId = QigIdValue;
    this.annloading = true;
    this.QigconfigServicecls.GetRandomcheckQIGs(QigIdValue)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.QigRandomcheckData = data;
          if (this.QigRandomcheckData != null) {
            let showwarn = false;
            if (
              this.QigRandomcheckData?.RandomCheckSettings != null &&
              this.QigRandomcheckData?.RandomCheckSettings?.length > 0
            ) {
              this.QigRandomcheckData?.RandomCheckSettings?.forEach((antn) => {
                if (antn?.ValueType == AppSettingValueType.Bit) {
                  antn.Value = JSON.parse(antn?.Value.toLowerCase() || "null");
                }
                if (antn?.Children != null && antn?.Children?.length > 0) {
                  antn?.Children?.forEach((child) => {
                    child.Value = JSON.parse(child?.Value.toLowerCase() || "null");

                    if (antn?.AppsettingKey == "RCT1") {
                      if (child?.AppsettingKey == "SMPLRTT1") {
                        this.percFunRc1(child?.Value, this.QigRandomcheckData?.LivePoolCount, this.QigRandomcheckData?.SubmittedCount);
                      }
                    }

                    if (child?.AppsettingKey == "SMPLRTT2") {
                      this.percFunRc2(child?.Value, this.QigRandomcheckData?.RC1SubmittedCount, this.QigRandomcheckData?.LivePoolCount);
                    }


                  });
                }
              });
            }

            if (showwarn) {
              this.Alert.warning(
                `Team is not assigned to one or more Qig's`,
                false
              );
            }
          } else {
            this.translate
            .get('SetUp.QigConfig.NoQigWarning')
            .subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.warning(translated);
            });
          }
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.annloading = false;
        },
      });
  }

  validatedata(QigRandomcheckData: QigModel) {
    let flag = true;
    if (QigRandomcheckData != null) {

      var Rc01 = QigRandomcheckData.RandomCheckSettings.filter(a => a.AppsettingKey == 'RCT1');
      var Rc1 = Rc01[0]?.Children.filter(b => b.AppsettingKey == 'SMPLRTT1' && b.Value == 0);
      var Rc02 = QigRandomcheckData.RandomCheckSettings.filter(c => c.AppsettingKey == 'RCT2');
      var Rc2 = Rc02[0]?.Children.filter(d => d.AppsettingKey == 'SMPLRTT2' && d.Value > 0);

      QigRandomcheckData?.RandomCheckSettings?.forEach((parent) => {
        parent?.Children?.forEach((element) => {
          if (
            QigRandomcheckData?.RcType == 1 ||
            QigRandomcheckData?.RcType == 2
          ) {
            if (
              element?.AppsettingKey == 'SMPLRTT1' &&
              (element?.Value < 0 || element?.Value ==null || element?.Value > 100)
            ) {
              this.translate
                  .get('SetUp.QigConfig.SamplingRatezerohundred')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.warning(translated);
                  });
              flag = false;
            }
            else if (
              element?.AppsettingKey == 'JBTMT1' &&
              (element?.Value < 0 || element?.Value == null ||  element?.Value > 1440)
            ) {
              this.translate
              .get('SetUp.QigConfig.JobDurationwarning')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.warning(translated);
              });
              flag = false;
            }
          }
          if (QigRandomcheckData?.RcType == 2) {
            if (
              element?.AppsettingKey == 'SMPLRTT2' &&
              (element?.Value < 0 || element?.Value ==null || element?.Value > 100)
            ) {
              this.translate
              .get('SetUp.QigConfig.SamplingRatezerohundred')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.warning(translated);
              });
              flag = false;
            }
            else  if (Rc1[0]?.Value == 0 && Rc2 != null && Rc2[0]?.Value > 0) {
              this.translate
              .get('SetUp.QigConfig.Rc1Rc2shouldzero')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.warning(translated);
              });
              flag = false;
            }

            else if (
              element?.AppsettingKey == 'JBTMT2' &&
              (element?.Value < 0 || element?.Value == null || element?.Value > 1440)
            ) {
              this.translate
              .get('SetUp.QigConfig.JobDurationwarning')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.warning(translated);
              });
              flag = false;
            }
          }
        });
      });
    }
    return flag;
  }

  saveqig(QigRandomcheckData: QigModel) {
    this.Alert.clear();
    if (this.validatedata(QigRandomcheckData)) {
      if (QigRandomcheckData != null) {
        this.annloading = true;
        this.QigconfigServicecls.patchRandomcheckQIGs(QigRandomcheckData)
          .pipe(first())
          .subscribe({
            next: (data: any) => {
              if (data != null && data != undefined && data) {
                this.translate
                  .get('SetUp.QigConfig.QigRandomcheckUpdated')
                  .subscribe((translated: string) => {
                    this.Alert.clear();
                    this.Alert.success(translated);
                    this.GetRandomcheckQIGs(this.QigId);
                    this._qigConfigurationsComponent.GetQIGConfigDetails(this.QigId);
                  });
              } else {
                this.Alert.error(
                  'Error while updating the Qig Random check setting'
                );
              }
            },
            error: (a: any) => {
              throw (a);
            },
            complete: () => {
              this.annloading = false;
            },
          });
      }
    }
  }
  percFunRc1(rcper: any, livepool: any, submittedcount: any) {
    var rc1count = livepool - submittedcount;
    if (rc1count > 0) {
      this.Rc1scripts = Math.ceil((rcper / 100) * rc1count);
    } else {
      this.Rc1scripts = "-"
    }
  }

  percFunRc2(rcper: any, RC1Submitted: any, livepool: any) {
    if (this.Rc1scripts != "-") {
      this.Rc2scripts = Math.ceil((rcper / 100) * this.Rc1scripts);
    } else {
      this.Rc2scripts = "-";
    }
  }
}
