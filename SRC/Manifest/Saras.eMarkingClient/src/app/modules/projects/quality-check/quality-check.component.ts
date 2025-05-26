import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { QigUserModel } from 'src/app/model/project/qig';
import { QualityCheckService } from '../../../services/project/quality-check/quality-check.service';
import {
  QualityCheckScriptDetails,
  QualityCheckSummary,
} from 'src/app/model/project/quality-check/marker-tree-view-model';
import { ScriptListViewComponent } from './script-list-view/script-list-view.component';
import { ScriptDetailsViewComponent } from './script-details-view/script-details-view.component';
import { MarkerTreeViewComponent } from '../../shared/marker-tree/marker-tree-view/marker-tree-view.component';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { StdAssessmentService } from 'src/app/services/project/standardisation/std-two/std-assessment.service';
import { first } from 'rxjs';

@Component({
  selector: 'emarking-quality-check',
  templateUrl: './quality-check.component.html',
  styleUrls: ['./quality-check.component.css'],
})
export class QualityCheckComponent implements OnInit {
  @ViewChild(MarkerTreeViewComponent)
  userhierarchymarkertreeviewcomponent!: MarkerTreeViewComponent;
  @ViewChild(ScriptListViewComponent)
  scriptlistviewcomponent!: ScriptListViewComponent;
  @ViewChild(ScriptDetailsViewComponent)
  scriptdetailsviewcomponent!: ScriptDetailsViewComponent;

  activeQig!: QigUserModel;
  qcScriptSummary!: QualityCheckSummary;
  totalsummarycount!: QualityCheckSummary;
  toggle: boolean = false;
  isScriptDetailsview: boolean = false;
  activeScript!: QualityCheckScriptDetails;
  ProjectUserRoleId: number = 0;
  Roleid: any;
  scriptpool: number = 3;
  selectedmarkerdata: any;
  title!: string;
  activeaccordintab: number = 1;
  activefilterTab: number = 1;
  typeinfo: number = 1;
  qasCountloading: boolean = true;
  filterclick: boolean = false;
  pool!: number;
  panelOpenState: boolean = false;
  markingpanelOpenState: boolean = false;
  qcPanelOpenState: boolean = false;
  teamPanelOpenState: boolean = false;
  status: string = '100';
  UserStatuss: string = '';
  constructor(
    public qaCheckService: QualityCheckService,
    public translate: TranslateService,
    public commonService: CommonService,
    private eRef: ElementRef,
    public stdAssessmentservice: StdAssessmentService
  ) {}

  ngOnInit(): void {
    this.textinternationalization();
  }

  getQigDetails(selectedqig: QigUserModel) {
    this.filterclick = false;
    this.qasCountloading = true;
    this.ProjectUserRoleId = 0;
    this.selectedmarkerdata = null;
    this.isScriptDetailsview = false;
    if (selectedqig != null && selectedqig.QigId > 0) {
      this.activefilterTab = 1;
      this.activeaccordintab = 1;
      this.activeQig = selectedqig;
      this.userhierarchymarkertreeviewcomponent.getMarkerTreeview(
        this.activeQig.QigId
      );
    }
    this.qasCountloading = false;
  }

  getScriptCountsByUser(node: any) {
    this.isScriptDetailsview = false;
    this.selectedmarkerdata = node;
    this.qasCountloading = false;
    this.ProjectUserRoleId =
      node.ProjectUserRoleID == undefined ? 0 : node.ProjectUserRoleID;
    this.GetQualityCheckTeamSummary(
      this.activeQig.QigId,
      this.ProjectUserRoleId
    );
    this.scriptlistviewcomponent.getQigTeamScriptListDetails(
      this.activeQig.QigId,
      this.ProjectUserRoleId,
      this.activeaccordintab,
      this.activefilterTab,
      this.selectedmarkerdata
    );
    this.qasCountloading = true;

    this.UserStatus(this.ProjectUserRoleId, this.activeQig.QigId);

    if (!node.iskp) {
      this.stdAssessmentservice
        .AssessmentStatus(this.activeQig.QigId, this.ProjectUserRoleId)
        .pipe(first())
        .subscribe({
          next: (data: any) => {
            this.status = data;
          },
          error: (err: any) => {
            throw err;
          },
        });
    }
  }

  getScriptCountsByFilter(filterid: number) {
    this.isScriptDetailsview = false;
    this.activefilterTab = filterid;
    this.GetQualityCheckTeamSummary(
      this.activeQig.QigId,
      this.ProjectUserRoleId
    );
    this.scriptlistviewcomponent.getQigTeamScriptListDetails(
      this.activeQig.QigId,
      this.ProjectUserRoleId,
      this.activeaccordintab,
      this.activefilterTab,
      this.selectedmarkerdata
    );
  }

  accordintabs(tabid: number) {
    this.activeaccordintab = tabid;
    this.activefilterTab = 1;
    this.GetQualityCheckTeamSummary(
      this.activeQig.QigId,
      this.ProjectUserRoleId
    );
    this.scriptlistviewcomponent.getQigTeamScriptListDetails(
      this.activeQig.QigId,
      this.ProjectUserRoleId,
      this.activeaccordintab,
      this.activefilterTab,
      this.selectedmarkerdata
    );
  }

  getScriptDetails(script: QualityCheckScriptDetails) {
    this.isScriptDetailsview = true;
    this.activeScript = script;
    this.scriptdetailsviewcomponent.getScriptDetails(
      this.activeQig.QigId,
      this.activeScript.ScriptId
    );
  }
  navbacktoscripts() {
    this.isScriptDetailsview = false;
    this.getScriptCountsByFilter(this.activefilterTab);
  }

  public GetQualityCheckTeamSummary(QigId: number, ProjectUserRoleId: number) {
    this.filterclick = false;
    this.qasCountloading = true;
    this.qaCheckService
      .GetTeamStatistics(
        QigId,
        ProjectUserRoleId,
        this.activeaccordintab,
        this.activefilterTab
      )
      .subscribe({
        next: (data: any) => {
          if (data != null) {
            this.qcScriptSummary = data;
          }
        },
        error: (a: any) => {
          this.qasCountloading = false;
          throw a;
        },
        complete: () => {
          this.qasCountloading = false;
        },
      });
  }

  event_pop() {
    this.filterclick = true;
  }

  textinternationalization() {
    this.translate
      .get('Quality-Check.QualityCheckSummary.desc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });

    this.translate
      .get('Quality-Check.QualityCheckSummary.title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
        this.title = translated;
      });
  }

  panelOpened(event: any) {
    this.markingpanelOpenState = true;
  }
  QcpanelOpened(event: any) {
    this.qcPanelOpenState = true;
  }
  TeampanelOpened(event: any) {
    this.teamPanelOpenState = true;
  }

  togglepanel() {
    this.panelOpenState = !this.panelOpenState;
  }

  UserStatus(ProjectUserRoleId: number, QigId: number) {
    this.qaCheckService
      .UserStatus(ProjectUserRoleId, QigId)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.UserStatuss = data;
        },
        error: (err: any) => {
          throw err;
        },
      });
  }
}
