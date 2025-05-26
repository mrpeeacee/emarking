import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { first } from 'rxjs/operators';
import { QigSettingService } from 'src/app/services/project/setup/qig-configuration/qig-setting.service';

@Component({
  selector: 'emarking-other-qig-settings',
  templateUrl: './other-qig-settings.component.html',
  styleUrls: ['./other-qig-settings.component.css']
})
export class OtherQigSettingsComponent {

  pausemarkingprocessstate!: boolean;
  qigclosurestate!: boolean;
  enableclosureremarks: boolean = false;
  enablepauseremarks: boolean = false;
  QigId!: number;

  constructor(public QigconfigServicecls: QigSettingService,
    public translate: TranslateService,
    public Alert: AlertService) { }

    QigConfigData: any = [];
    pauseremarksvalue: any;
    closureremarksvalue: any;

  Quantity: number = 1;

  i = 1

  plus() {
    this.i++;
    this.Quantity = this.i;

  }
  minus() {
    this.i--;
    this.Quantity = this.i;


  }
  
  getQigConfigSettings(QigId: number) {
    this.enableclosureremarks = false;
    this.enablepauseremarks = false;
    this.QigId = QigId;
    this.QigconfigServicecls.getQigConfigSettings(QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.QigConfigData = data;
          this.pausemarkingprocessstate =
            this.QigConfigData?.IsPauseMarkingProcessEnabled;
          this.qigclosurestate = this.QigConfigData?.IsQiGClosureEnabled;
          this.pauseremarksvalue =
            this.QigConfigData?.PauseMarkingProcessRemarks;
          this.closureremarksvalue = this.QigConfigData?.QiGClosureRemarks;
        },
        error: (a: any) => {
          throw(a);
        },
      });
  }
  saveQigConfigSettings() {
    if (this.enablepauseremarks) {
      if (this.QigConfigData?.PauseMarkingProcessRemarks.trim() == '') {
        this.translate
          .get('SetUp.QigConfig.Remarksnullwarning')
          .subscribe((translated: string) => {
            this.Alert.clear();
            this.Alert.warning(translated);
          });
        return;
      }
    }
    if (this.enableclosureremarks) {
      if (this.QigConfigData?.QiGClosureRemarks.trim() == '') {
        this.translate
          .get('SetUp.QigConfig.Remarksnullwarning')
          .subscribe((translated: string) => {
            this.Alert.clear();
            this.Alert.warning(translated);
          });
        return;
      }
    }

    this.Alert.clear();
    this.QigconfigServicecls.saveQigConfigSettings(
      this.QigId,
      this.QigConfigData
    )
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data == 'Q001') {
            this.translate
              .get('SetUp.QigConfig.Otherspauseclouserupdate')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.success(translated);
                this.getQigConfigSettings(this.QigId);
              });
          } else if (data != null && data == 'Q002') {
            this.translate
              .get('SetUp.Qig.NotMappedMarkSchemeMessage')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.warning(translated);
              });
          }
        },
        error: (a: any) => {
          throw(a);
        },
      });
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
}
