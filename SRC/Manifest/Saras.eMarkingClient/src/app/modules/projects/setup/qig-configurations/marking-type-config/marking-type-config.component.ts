import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { first } from 'rxjs/operators';
import { QigSettingService } from 'src/app/services/project/setup/qig-configuration/qig-setting.service';
import { QigConfigurationsComponent } from '../qig-configurations.component';

@Component({
  selector: 'emarking-marking-type-config',
  templateUrl: './marking-type-config.component.html',
  styleUrls: ['./marking-type-config.component.css']
})
export class MarkingTypeConfigComponent {
  QigConfigData: any = [];
  QigId!: number;
  LiveMarkingorTrialMarkingStarted!: boolean;
  markingtypeloading:boolean=false;

  constructor(public QigconfigServicecls: QigSettingService,
    public translate: TranslateService, public Alert: AlertService,
    public _qigConfigurationsComponent: QigConfigurationsComponent) { }

  getQigConfigSettings(QigId: number) {
    this.QigId = QigId;
    this.CheckLiveMarkingorTrialMarkingStarted(QigId);
    this.QigconfigServicecls.getQigConfigSettings(QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.QigConfigData = data;
        },
        error: (a: any) => {
          throw (a);
        },
      });
  }
  saveQigConfigSettings() {
    this.markingtypeloading=true;
    this.Alert.clear();
    this.QigconfigServicecls.SaveMarkingTypeQigConfigSettings(
      this.QigId,
      this.QigConfigData
    )
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data == 'Q001') {
            this.translate
              .get('SetUp.QigConfig.MarkingTypeUpdated')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.success(translated);
                this.markingtypeloading=false;
                this.getQigConfigSettings(this.QigId);
                this._qigConfigurationsComponent.GetQIGConfigDetails(this.QigId);
              });
          } else if (data != null && data == 'Q002') {
            this.translate
              .get('SetUp.QigConfig.UpdateError')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.warning(translated);
                this.markingtypeloading=false;
              });
          }
        },
        error: (a: any) => {
          this.markingtypeloading=false;
          throw (a);
        },
      });
  }
  CheckLiveMarkingorTrialMarkingStarted(QigId: number) {
    this.QigconfigServicecls.CheckLiveMarkingorTrialMarkingStarted(QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.LiveMarkingorTrialMarkingStarted = data;
        },
        error: (a: any) => {
          throw (a);
        },
      });
  }
}
