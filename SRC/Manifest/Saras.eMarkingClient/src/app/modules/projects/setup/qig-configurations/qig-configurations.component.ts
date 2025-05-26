import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { QigConfigService } from 'src/app/services/project/qigconfig.service';
import { CommonService } from 'src/app/services/common/common.service';
import {
  QigModel,
  QigQuestionModel,
  QigUserModel,
  EnumQigConfigDetails,
  QigconfigDetails,
} from 'src/app/model/project/qig';
import { AlertService } from 'src/app/services/common/alert.service';
import { ProjectQigModel } from 'src/app/model/project/qigconfig';
import { first } from 'rxjs/operators';
import { AnnotationSettingsComponent } from './annotation-settings/annotation-settings.component';
import { StatndardisationSettingsComponent } from './statndardisation-settings/statndardisation-settings.component';
import { RandomCheckComponent } from './random-check/random-check.component'; 
import { LiveMarkingSettingsComponent } from './live-marking-settings/live-marking-settings.component';
import {
  trigger,
  transition,
  style,
  animate,
  state,
} from '@angular/animations';
import { MarkingTypeConfigComponent } from './marking-type-config/marking-type-config.component';
import { OtherQigSettingsComponent } from './other-qig-settings/other-qig-settings.component';
import { QigQuestionsComponent } from './qig-questions/qig-questions.component';
import { QigSummaryComponent } from './qig-summary/qig-summary.component';
import { StdSettingService } from 'src/app/services/project/setup/qig-configuration/std-setting.service';

@Component({
  selector: 'emarking-qig-configurations',
  templateUrl: './qig-configurations.component.html',
  styleUrls: ['./qig-configurations.component.css'],
  animations: [
    trigger('sideMenu', [
      state(
        'closed',
        style({
          right: '-400px',
        })
      ),
      state(
        'open',
        style({
          right: '0px',
        })
      ),

      transition('open => closed', animate('400ms ease-out')),
      transition('closed => open', animate('300ms ease-out')),
    ]),
  ],
})
export class QigConfigurationsComponent implements OnInit {
  @ViewChild(LiveMarkingSettingsComponent)
  settingcomponent!: LiveMarkingSettingsComponent;
  @ViewChild(AnnotationSettingsComponent)
  annotationsettingscomponent!: AnnotationSettingsComponent;
  @ViewChild(StatndardisationSettingsComponent)
  statndardizationsettingscomponent!: StatndardisationSettingsComponent;
  @ViewChild(RandomCheckComponent) randomcheckcomponent!: RandomCheckComponent; 
  @ViewChild(MarkingTypeConfigComponent)
  markingtypeconfigcomponent!: MarkingTypeConfigComponent;
  @ViewChild(OtherQigSettingsComponent)
  otherqigsettingscomponent!: OtherQigSettingsComponent;
  @ViewChild(QigQuestionsComponent)
  qigquestionscomponent!: QigQuestionsComponent;
  @ViewChild(QigSummaryComponent) qigsummery!: QigSummaryComponent;

  configloading: boolean = false;
  Qigs: any[] = [];
  datafound: boolean = true;
  errMessage: string = '';
  state = 'closed';
  isShown = false;
  questionview: any = {
    QnsLabel: '',
    QnsCout: 0,
    QigId: 0,
  };
  qigquestions: QigQuestionModel[] = [];
  qigquestionandmarks!: ProjectQigModel;
  setupststuslst!: any[];

  settingpanelOpenState = false;
  teampanelOpenState = false;
  anntpanelOpenState = false;
  stdpanelOpenState = false;
  rdmpanelOpenState = false;
  qigquestionOpenState = false;
  othersOpenState = false;
  qigsummeryOpenState = false;

  MarkingTypeState = false;

  activeQig!: QigUserModel;

  Qigstdlist: any;
  IsLiveMarked: boolean = false;

  QigConfigEnums!: EnumQigConfigDetails;
  QigconfigDetailsLst!: QigconfigDetails[];

  QigQuestionCode!: QigconfigDetails[];
  MarkingTypeCode!: QigconfigDetails[];
  StdSettingCode!: QigconfigDetails[];
  AnnotationCode!: QigconfigDetails[];
  LiveMarkingCode!: QigconfigDetails[];
  RcSettingCode!: QigconfigDetails[];


  constructor(
    public translate: TranslateService,
    public commonService: CommonService,
    public qigconfigservice: QigConfigService,
    public Alert: AlertService,
    public StdServicecls: StdSettingService
  ) { }

  ngOnInit(): void {
    this.translate
      .get('SetUp.QigConfig.Title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.translate
      .get('SetUp.QigConfig.PageDescription')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    this.GetSetupStatus();
  }

  Quantity: number = 1;

  i = 1;

  plus() {
    this.i++;
    this.Quantity = this.i;
  }
  minus() {
    this.i--;
    this.Quantity = this.i;
  }

  closeallmenu(SelectedQigId: QigModel) {
    if (SelectedQigId != SelectedQigId) {
      SelectedQigId.ShowQns = false;
    }
  }

  viewqigquestions() {
    this.questionview.QnsLabel = this.activeQig?.QigName;
    this.questionview.QigId = this.activeQig?.QigId;
    this.qigconfigservice
      .getAllViewQigQuestions(this.activeQig?.QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.qigquestions = data;
          if (this.qigquestions != null) {
            this.questionview.QnsCout = this.qigquestions?.length;
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }
  GetQigQuestionandMarks(QigId: number) {
    this.configloading = true;
    this.qigconfigservice
      .GetQigQuestionandMarks(this.activeQig?.QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.qigquestionandmarks = data;
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.configloading = false;
        },
      });
  }

  GetSetupStatus() {
    this.qigconfigservice
      .GetSetupStatus()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.setupststuslst = data;
          if (this.setupststuslst?.length == 0) {
            this.translate.get('SetUp.QigConfig.setupstatuswarning').subscribe((translated: string) => {
              this.Alert.clear();
              this.Alert.warning(translated);
            });
          }
        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  GetQIGConfigDetails(QigId: any) {
    this.qigconfigservice
      .GetQIGConfigDetails(this.activeQig?.QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.QigconfigDetailsLst = data;
          this.QigQuestionCode = this.QigconfigDetailsLst.filter(a => a.AppsettingKey == EnumQigConfigDetails.QigQuesstions && a.SettingGroupCode == EnumQigConfigDetails.QigConfigDetailsGroupCode && a.Value);
          this.MarkingTypeCode = this.QigconfigDetailsLst.filter(a => a.AppsettingKey == EnumQigConfigDetails.MarkingType && a.SettingGroupCode == EnumQigConfigDetails.QigConfigDetailsGroupCode && a.Value);
          this.StdSettingCode = this.QigconfigDetailsLst.filter(a => a.AppsettingKey == EnumQigConfigDetails.StdSetting && a.SettingGroupCode == EnumQigConfigDetails.QigConfigDetailsGroupCode && a.Value);
          this.AnnotationCode = this.QigconfigDetailsLst.filter(a => a.AppsettingKey == EnumQigConfigDetails.AnnotationSetting && a.SettingGroupCode == EnumQigConfigDetails.QigConfigDetailsGroupCode && a.Value);
          this.LiveMarkingCode = this.QigconfigDetailsLst.filter(a => a.AppsettingKey == EnumQigConfigDetails.LiveMarkingSetting && a.SettingGroupCode == EnumQigConfigDetails.QigConfigDetailsGroupCode && a.Value);
          this.RcSettingCode = this.QigconfigDetailsLst.filter(a => a.AppsettingKey == EnumQigConfigDetails.RandomCheck && a.SettingGroupCode == EnumQigConfigDetails.QigConfigDetailsGroupCode && a.Value);

        },
        error: (a: any) => {
          throw a;
        },
      });
  }

  getQigDetails(selectedqig: QigUserModel) {
    this.settingpanelOpenState = false;
    this.teampanelOpenState = false;
    this.anntpanelOpenState = false;
    this.stdpanelOpenState = false;
    this.rdmpanelOpenState = false;
    this.qigquestionOpenState = false;
    this.othersOpenState = false;
    this.qigsummeryOpenState = false;
    this.MarkingTypeState = false;

    if (selectedqig != null && selectedqig?.QigId > 0) {
      this.activeQig = selectedqig;
      this.markingtypeconfigcomponent.getQigConfigSettings(
        this.activeQig?.QigId
      );
      this.statndardizationsettingscomponent.GetQigStdSettingsandPracticeMandatory(
        this.activeQig?.QigId, this.setupststuslst?.length
      );
      this.qigquestionscomponent.GetQigQuestionandMarks(this.activeQig?.QigId, this.setupststuslst?.length);

      this.settingcomponent.getQigConfigSettings(this.activeQig?.QigId);
      this.annotationsettingscomponent.GetQigAnnotationSetting(
        this.activeQig?.QigId
      );
      this.randomcheckcomponent.GetRandomcheckQIGs(this.activeQig?.QigId); 
      this.otherqigsettingscomponent.getQigConfigSettings(this.activeQig?.QigId);
      this.GetQigQuestionandMarks(this.activeQig?.QigId);
      this.qigsummery.GetQigSummary(this.activeQig?.QigId);
      this.GetQIGConfigDetails(this.activeQig?.QigId);
    }
  }
  qigpnsclick(event: PointerEvent, pnstate: boolean) {
    // if (event != null && pnstate == true && event.clientY > 100) {
    //   window.scroll({
    //     top: event.clientY,
    //     left: 0,
    //     behavior: 'smooth'
    //   });
    // }

  }
  reloadStdSetting(stdQigId: any): void {
    this.statndardizationsettingscomponent.GetQigStdSettingsandPracticeMandatory(
      stdQigId, this.setupststuslst?.length
    );
  }
  setQuestionExpandCollapse() {
    this.qigquestionOpenState = false;
    this.qigquestionscomponent.CollapseAll();
  }
  loadQuestions() {
    this.qigquestionscomponent.viewqigquestions(this.activeQig?.QigId);
  }

}
