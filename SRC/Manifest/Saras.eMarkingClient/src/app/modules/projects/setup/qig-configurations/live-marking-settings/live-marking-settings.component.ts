import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { first } from 'rxjs/operators';
import { QigSettingService } from 'src/app/services/project/setup/qig-configuration/qig-setting.service';
import { QigConfigurationsComponent } from '../qig-configurations.component';

@Component({
  selector: 'emarking-live-marking-settings',
  templateUrl: './live-marking-settings.component.html',
  styleUrls: ['./live-marking-settings.component.css']
})
export class LiveMarkingSettingsComponent {
  previousGraceValue!: number;
  previousDownloadBatValue!: number;
  pausemarkingprocessstate!: boolean;
  qigclosurestate!: boolean;
  enableclosureremarks: boolean = false;
  enablepauseremarks: boolean = false;
  QigId!: number;
  ErrorMessage: string = "";
  annloading: boolean = false;

  constructor(
    public QigconfigServicecls: QigSettingService,
    public translate: TranslateService,
    public Alert: AlertService,
    public _qigConfigurationsComponent: QigConfigurationsComponent
  ) { }
  QigConfigData: any = [];
  pauseremarksvalue: any;
  closureremarksvalue: any;

  getQigConfigSettings(QigId: number) {
    this.enableclosureremarks = false;
    this.enablepauseremarks = false;
    this.QigId = QigId;
    this.annloading = true;
    this.QigconfigServicecls.getQigConfigSettings(QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.QigConfigData = data;
          this.previousGraceValue = this.QigConfigData?.GracePeriod;
          this.previousDownloadBatValue = this.QigConfigData?.DownloadBatchSize;
          this.pausemarkingprocessstate =
            this.QigConfigData?.IsPauseMarkingProcessEnabled;
          this.qigclosurestate = this.QigConfigData?.IsQiGClosureEnabled;
          this.pauseremarksvalue =
            this.QigConfigData?.PauseMarkingProcessRemarks;
          this.closureremarksvalue = this.QigConfigData?.QiGClosureRemarks;
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.annloading = false;
        },
      });
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

  ValidateLivemarkingSave(GracePeriod: any, DownloadBatchSize: any): boolean {
    var validationstatus: boolean = true;

    if (GracePeriod == null || GracePeriod < 0 || GracePeriod > 60) {
      this.ErrorMessage = 'SetUp.QigConfig.Graceperiodshouldzerotosisty';
      validationstatus = false;
    }

    else if (DownloadBatchSize == null || DownloadBatchSize < 1 || DownloadBatchSize > 99) {
      this.ErrorMessage = 'SetUp.QigConfig.Downloadbatchsizeshouldonetonintynine';
      validationstatus = false;
    }
    return validationstatus;

  }

  saveQigConfigSettings(GracePeriod: any, DownloadBatchSize: any) {
    this.Alert.clear();
    if (!this.ValidateLivemarkingSave(GracePeriod, DownloadBatchSize)) {
      this.ShowMessage(this.ErrorMessage, 'warning');
    }
    else {
      this.annloading = true;
      this.QigconfigServicecls.SaveQigConfigLiveMarkSettings(
        this.QigId,
        this.QigConfigData
      )
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data != null && data == 'Q001') {
              this.translate
                .get('SetUp.QigConfig.Livemarkingsettingsupdate')
                .subscribe((translated: string) => {
                  this.Alert.clear();
                  this.Alert.success(translated);
                  this.getQigConfigSettings(this.QigId);
                  this._qigConfigurationsComponent.GetQIGConfigDetails(this.QigId);
                });
            } else if (data != null && data == 'Q002') {
              this.translate
                .get('SetUp.Qig.UpdateError')
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
  }

  validateNumber(event: any) {
    var invalidChars = ["-", "e", "+", "E"];
    if (invalidChars.includes(event.key)) {
      event.preventDefault();
    }
  }
  changepausemarkingstate() {
    var pauseremarksval = this.pauseremarksvalue;
    if (
      this.pausemarkingprocessstate !=
      this.QigConfigData?.IsPauseMarkingProcessEnabled
    ) {
      this.enablepauseremarks = true;
      this.QigConfigData.PauseMarkingProcessRemarks = '';
    } else {
      this.enablepauseremarks = false;
      this.QigConfigData.PauseMarkingProcessRemarks = pauseremarksval;
    }
  }

  changeclosurestate() {
    var closureremarksval = this.closureremarksvalue;
    if (this.qigclosurestate != this.QigConfigData?.IsQiGClosureEnabled) {
      this.enableclosureremarks = true;
      this.QigConfigData.QiGClosureRemarks = '';
    } else {
      this.enableclosureremarks = false;
      this.QigConfigData.QiGClosureRemarks = closureremarksval;
    }
  }
  numericOnly(event: { key: string }): boolean {
    let patt = /^([0-9])$/;
    return patt.test(event.key);
  }
}
