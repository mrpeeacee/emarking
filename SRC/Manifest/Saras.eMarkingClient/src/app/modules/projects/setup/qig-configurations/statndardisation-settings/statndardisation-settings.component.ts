import { Component } from '@angular/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { TranslateService } from '@ngx-translate/core';
import { first } from 'rxjs/operators';
import { StdSettingService } from 'src/app/services/project/setup/qig-configuration/std-setting.service';
import { QIGStandardizationScriptSettings } from 'src/app/model/project/setup/qig-configuration/std-setting-model';
import { QigConfigurationsComponent } from '../qig-configurations.component';


@Component({
  selector: 'emarking-statndardisation-settings',
  templateUrl: './statndardisation-settings.component.html',
  styleUrls: ['./statndardisation-settings.component.css']
})
export class StatndardisationSettingsComponent {
  ShowMeStandardization: boolean = false;
  button: boolean = true;
  isCheckedS1: boolean = true;
  isCheckedS3: boolean = true;
  isCheckedS2: boolean = false;
  isChecked: boolean = true;
  IsLiveMarked: boolean = false;
  Status: string = "";
  IsPracticemandatory: boolean = true;
  IsArrow: boolean = false;
  QigId!: number;
  annloading: boolean = false;
  ErrorMessage: string = "";
  qigcreationsetupstatus: boolean = false;
  qigcreationsetuplen: any;
  IsLivemarking!: string;


  constructor(public QigconfigServicecls: StdSettingService, public Alert: AlertService, public translate: TranslateService,
    public _qigConfigurationsComponent: QigConfigurationsComponent) { }
  Qigstdlist: any;
  GetQigStdSettingsandPracticeMandatory(QidIdValue: number, qigmgtsetuplength: any) {
    this.qigcreationsetuplen = qigmgtsetuplength;
    if (this.qigcreationsetuplen == 0) {
      this.qigcreationsetupstatus = true;
    }
    else {
      this.qigcreationsetupstatus = false;
    }
    this.QigId = QidIdValue;
    this.annloading = true;
    this.QigconfigServicecls.GetQigStdSettingsandPracticeMandatory(QidIdValue)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Qigstdlist = data;
          if (!this.Qigstdlist?.IsS1Available || this.Qigstdlist?.IsS1Available == null) {
            this.ShowMeStandardization = false;
            this.isChecked = false;
            this.isCheckedS1 = true;
            this.isCheckedS3 = true;
            this.isCheckedS2 = false;
            this.IsPracticemandatory = this.Qigstdlist?.IsPracticemandatory;
          }
          else {
            this.IsPracticemandatory = this.Qigstdlist?.IsPracticemandatory;
            this.ShowMeStandardization = true;
            this.isChecked = true;
            this.isCheckedS1 = this.Qigstdlist?.IsS1Available;
            this.isCheckedS2 = this.Qigstdlist?.IsS2Available;
            this.isCheckedS3 = this.Qigstdlist?.IsS3Available;
          }
          this.IsLiveMarked = this.Qigstdlist?.IsLiveMarkingStart;
        },
        error: (a: any) => {
          throw (a);
        },
        complete: () => {
          this.annloading = false;
        },
      });
  }
  toogleStandardization() {
    if (!this.Qigstdlist?.Disablestandardisationreq || this.qigcreationsetuplen == 0) {
      this.ShowMeStandardization = !this.ShowMeStandardization;
      if (this.IsLiveMarked || this.qigcreationsetuplen == 0) {
        this.ShowMeStandardization = false;
      }
    }
  }
  fnSpace() {
    if (!this.IsLiveMarked) {
      this.ShowMeStandardization = !this.ShowMeStandardization;
    }
  }

  validateNumber(event: any) {
    var invalidChars = ["-", "e", "+", "E"];
    if (invalidChars.includes(event.key)) {
      event.preventDefault();
    }
  }

  ValidateStandardisationSave(std: any, benchmark: any, additional: any): boolean {
    var validationstatus: boolean = true;
    if (this.IsPracticemandatory && (benchmark == null || benchmark < 1 || benchmark > 99)) {
      this.ErrorMessage = 'SetUp.QigConfig.Benchmarkmustonetonintynine';
      validationstatus = false;
    }

    else if (!this.IsPracticemandatory && (benchmark == null || benchmark < 0 || benchmark > 99)) {
      this.ErrorMessage = 'SetUp.QigConfig.Benchmarkmustzerotonintynine';
      validationstatus = false;
    }

    else if (std == null || std < 1 || std > 99) {
      this.ErrorMessage = 'SetUp.QigConfig.Standardisationmustonetonintynine';
      validationstatus = false;
    }

    else if (this.IsPracticemandatory && (additional == null || additional < 0 || additional > 99)) {
      this.ErrorMessage = 'SetUp.QigConfig.Additionalstdmustzerotonintynine';
      validationstatus = false;
    }

    else if (!this.IsPracticemandatory && (additional == null || additional < 0 || additional > 99)) {
      this.ErrorMessage = 'SetUp.QigConfig.Additionalstdmustzerotonintynine';
      validationstatus = false;
    }
    return validationstatus;

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
  SaveUpdateQIGStandardizationSettings() {
    debugger;
    this.Alert.clear();
    if (!this.ValidateStandardisationSave(this.Qigstdlist?.StandardizationScript,
      this.Qigstdlist?.BenchmarkScript, this.Qigstdlist?.AdditionalStdScript)) {
      this.ShowMessage(this.ErrorMessage, 'warning');
    }
    else {

      let QIGStandardizationScriptSettingsobj = new QIGStandardizationScriptSettings();

      QIGStandardizationScriptSettingsobj.QIGID = this.QigId;
      QIGStandardizationScriptSettingsobj.IsLiveMarkingStart = this.IsLiveMarked;
      if (!this.ShowMeStandardization) {
        QIGStandardizationScriptSettingsobj.QIGName=this.Qigstdlist?.QIGName;
        QIGStandardizationScriptSettingsobj.IsS1Available = false;
        QIGStandardizationScriptSettingsobj.IsS2Available = false;
        QIGStandardizationScriptSettingsobj.IsS3Available = false;
        QIGStandardizationScriptSettingsobj.StandardizationScript = 1;
        QIGStandardizationScriptSettingsobj.BenchmarkScript = 1;
        QIGStandardizationScriptSettingsobj.AdditionalStdScript = 0;
        QIGStandardizationScriptSettingsobj.SettingID = this.Qigstdlist?.SettingID;
        QIGStandardizationScriptSettingsobj.IsPracticemandatory = true;
        QIGStandardizationScriptSettingsobj.RecommendMarkScheme = "QIGLVL";
      }
      else {
        QIGStandardizationScriptSettingsobj.QIGName=this.Qigstdlist?.QIGName;
        QIGStandardizationScriptSettingsobj.IsS1Available = this.isCheckedS1;
        QIGStandardizationScriptSettingsobj.IsS2Available = this.isCheckedS2;
        QIGStandardizationScriptSettingsobj.IsS3Available = this.isCheckedS3;
        QIGStandardizationScriptSettingsobj.IsPracticemandatory = this.IsPracticemandatory;
        QIGStandardizationScriptSettingsobj.StandardizationScript = this.Qigstdlist?.StandardizationScript;
        QIGStandardizationScriptSettingsobj.BenchmarkScript = this.Qigstdlist?.BenchmarkScript;
        QIGStandardizationScriptSettingsobj.AdditionalStdScript = this.Qigstdlist?.AdditionalStdScript;
        QIGStandardizationScriptSettingsobj.SettingID = this.Qigstdlist?.SettingID;
        QIGStandardizationScriptSettingsobj.RecommendMarkScheme = this.Qigstdlist?.RecommendMarkScheme;
      }

      this.annloading = true;
      this.QigconfigServicecls.UpdateQigStdSettingsandPracticeMandatory(QIGStandardizationScriptSettingsobj)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.IsLivemarking = data;
            QIGStandardizationScriptSettingsobj.Status = data;
            if (QIGStandardizationScriptSettingsobj?.Status == "P001" || QIGStandardizationScriptSettingsobj?.Status == "S001" || QIGStandardizationScriptSettingsobj?.Status == "N001") {

              this.translate.get('SetUp.QigConfig.QigStdsettingUpdated').subscribe((translated: string) => {
                this.Alert.clear();
                this.Alert.success(translated);
                this.IsLiveMarked = true;
                this.GetQigStdSettingsandPracticeMandatory(this.QigId, this.qigcreationsetuplen);
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
  }
  numericOnly(event: { key: string }): boolean {
    let patt = /^([0-9])$/;
    return patt.test(event.key);
  }
}
