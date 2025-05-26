import { AlertService } from 'src/app/services/common/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs/operators';
import { AnnotationSettingsService } from 'src/app/services/project/setup/qig-configuration/annotation-setting.service';
import { QigAnnotationDetails, AnnotationTools, AnnotationSettings } from 'src/app/model/project/setup/qig-configuration/annotation-setting-model';
import { Component, ViewChild, ElementRef } from '@angular/core';
import { TrialmarkingService } from 'src/app/services/project/trialmarking.service'
import { QigConfigurationsComponent } from '../qig-configurations.component';

@Component({
  selector: 'emarking-annotation-settings',
  templateUrl: './annotation-settings.component.html',
  styleUrls: ['./annotation-settings.component.css']
})
export class AnnotationSettingsComponent {
  @ViewChild('closeAddExpenseModal') closeAddExpenseModal!: ElementRef;
  QigAnnotationlist!: AnnotationSettings;
  Status: string = '';
  annotationrequired!: boolean;
  QigId!: number;
  annloading: boolean = false;
  QigAnnotationToolsList!: QigAnnotationDetails;
  annotationTool: AnnotationTools[] = [];
  AnnotationConfigured: string = "";

  constructor(
    public QigconfigServicecls: AnnotationSettingsService,
    public Alert: AlertService,
    public translate: TranslateService,
    public trialmarkingservice: TrialmarkingService,
    public _qigConfigurationsComponent: QigConfigurationsComponent
  ) { }

  GetQigAnnotationSetting(QigIdValue: number) {
    this.QigId = QigIdValue;
    this.annloading = true;
    this.QigconfigServicecls.GetQigAnnotationSetting(QigIdValue)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.QigAnnotationlist = data;
          if (this.QigAnnotationlist.IsTagged) {
            this.translate
              .get('General.Yes')
              .subscribe((translated: string) => {
                this.AnnotationConfigured = translated;
              });
          }
          else {
            this.translate
              .get('General.no')
              .subscribe((translated: string) => {
                this.AnnotationConfigured = translated;
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
  SaveUpdateAnnotationSettings() {
    this.Alert.clear();
    var AnnotationCount: number = 0;
    if (this.QigAnnotationlist.IsAnnotationsMandatory) {
      this.trialmarkingservice.Getannoatationdetails(this.QigId).pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data != null)
              AnnotationCount = data.length;
            if (AnnotationCount > 0) {
              this.SaveAnnotationSetting();
            }
            else {
              this.translate
                .get('General.Annotationwarningalert')
                .subscribe((translated: string) => {
                  this.Alert.clear();
                  this.Alert.warning(translated);
                });
            }
          }
        })
    }
    else {
      this.SaveAnnotationSetting();
    }

  }
  SaveAnnotationSetting() {
    this.annloading = true;
    this.QigconfigServicecls.UpdateQigAnnotationSetting(
      this.QigId,
      this.QigAnnotationlist
    )
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Status = data;
          if (
            this.Status == 'P001' ||
            this.Status == 'S001' ||
            this.Status == 'N001'
          ) {
            this.translate
              .get('SetUp.QigConfig.QigAnnotationUpdated')
              .subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.success(translated);

                this.GetQigAnnotationSetting(this.QigId);
                this._qigConfigurationsComponent.GetQIGConfigDetails(this.QigId);
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
  GetAnnotationList() {
    this.annloading = true;
    this.QigconfigServicecls.GetQigAnnotationTools(this.QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.QigAnnotationToolsList = data;
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.annloading = false;
        },
      });
  }

  saveAnnotationTemplate() {
    var res = this.QigAnnotationToolsList.AnnotationGroup;
    this.annotationTool = [];
    for (var i = 0; i < res.length; i++) {

      for (var j = 0; j < res[i].AnnotationTools.length; j++) {
        this.annotationTool.push(res[i].AnnotationTools[j]);
      }

    }

    var istrue = this.annotationTool.filter(a => a.isChecked);

    if (istrue.length > 0) {
      this.annloading = true;
      this.QigconfigServicecls.SaveAnnotationForQIG(this.QigId, this.QigAnnotationToolsList)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            if (data == "S001") {
              this.GetQigAnnotationSetting(this.QigId);
              this.translate
                .get('General.Annotationsave')
                .subscribe((translated: string) => {
                  this.Alert.clear();
                  this.Alert.success(translated);
                  this.closeAddExpenseModal.nativeElement.click();
                });
            }
          },

          complete: () => {
            this.annloading = false;
          },
        });
    }
    else {
      this.translate
        .get('General.Annotationwarningalert')
        .subscribe((translated: string) => {
          this.Alert.clear();
          this.Alert.warning(translated);
        });
    }
  }
}

