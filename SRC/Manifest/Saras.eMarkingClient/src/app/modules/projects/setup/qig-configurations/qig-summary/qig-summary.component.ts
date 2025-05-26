import {
  Component,
  EventEmitter,
  Output,
} from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { QigSummaryService } from 'src/app/services/project/setup/qig-configuration/qig-summary.service';
import { first } from 'rxjs/operators';
import { StdSettingService } from 'src/app/services/project/setup/qig-configuration/std-setting.service';

@Component({
  selector: 'emarking-qig-summary',
  templateUrl: './qig-summary.component.html',
})
export class QigSummaryComponent {
  @Output() getStdSetting = new EventEmitter<number | undefined>();

  constructor(
    public translate: TranslateService,
    public Alert: AlertService,
    public qigservice: QigSummaryService,
    public StdServicecls: StdSettingService
  ) {}

  QigId!: number;
  QigSummaryData: any = [];
  IsQigSetup!: boolean;
  IsLiveMarkingEnable!: boolean;

  GetQigSummary(QigId: number) {
    this.QigId = QigId;

    this.qigservice
      .GetQigSummary(this.QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.QigSummaryData = data;
          this.IsQigSetup = this.QigSummaryData.IsQigSetup;
          this.IsLiveMarkingEnable = this.QigSummaryData.IsLiveMarkingStart;
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  saveQigSummary() {
    this.qigservice
      .saveQigSummary(this.QigId, this.QigSummaryData)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data == 'SU001') {
            this.GetQigSummary(this.QigId);
            this.Alert.success('Saved Successfully');
            this.getStdSetting.emit(this.QigId);
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }
}
