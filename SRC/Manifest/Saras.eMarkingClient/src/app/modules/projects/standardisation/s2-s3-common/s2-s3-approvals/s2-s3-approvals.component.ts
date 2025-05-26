import {
  Component,
  OnInit,
  ViewChild,
} from '@angular/core';
import {
  trigger,
  transition,
  style,
  animate,
  state,
} from '@angular/animations';
import {
  MarkingPersonal,
  S2S3Approvals,
} from 'src/app/model/project/standardisation/s2-s3-approvals';
import { S2S3ApprovalsService } from 'src/app/services/project/standardisation/s2-s3-approvals.service';
import { QigUserModel } from 'src/app/model/project/qig';
import { AlertService } from 'src/app/services/common/alert.service';
import { Router } from '@angular/router';
import { ScriptListViewComponent } from '../../../quality-check/script-list-view/script-list-view.component';
import { MarkerTreeViewComponent } from 'src/app/modules/shared/marker-tree/marker-tree-view/marker-tree-view.component';
import { CommonService } from 'src/app/services/common/common.service';
import { TranslateService } from '@ngx-translate/core';
import { AppSettingEntityType } from 'src/app/model/appsetting/app-setting-model';
import { QigService } from 'src/app/services/project/qig.service';
import { WorkflowProcessStatus, WorkflowStatus } from 'src/app/model/common-model';

@Component({
  selector: 'emarking-s2-s3-approvals',
  templateUrl: './s2-s3-approvals.component.html',
  styleUrls: ['./s2-s3-approvals.component.css'],
  animations: [
    trigger('event_pop', [
      state(
        'closed',
        style({ height: '0px', opacity: '0', overflow: 'hidden' })
      ),
      state(
        'open',
        style({
          height: '*',
          opacity: '1',
        })
      ),
      transition('open => closed', animate('200ms ease-out')),
      transition('closed => open', animate('300ms ease-out')),
    ]),
  ],
})
export class S2S3ApprovalsComponent implements OnInit {

  @ViewChild(ScriptListViewComponent)
  scriptlistviewcomponent!: ScriptListViewComponent;
  @ViewChild(MarkerTreeViewComponent)
  userhierarchymarkertreeviewcomponent!: MarkerTreeViewComponent;


  constructor(
    public appServices: S2S3ApprovalsService,
    public Alert: AlertService,
    private route: Router,
    public commonService: CommonService,
    public translate: TranslateService,
    public qigservice: QigService,
  ) { }

  ApprovalStatusDetls!: S2S3Approvals;
  PersonalDetails!: MarkingPersonal[];
  PersonalSearchDetails!: MarkingPersonal[];
  activeQig!: QigUserModel;
  activeScriptId: number = 0;
  ProjectUserRoleId!: number;
  SearchName!: any;
  IsQigPaused: boolean = false;
  Ispause: any;
  filterclick: boolean = false;
  IsS1Available: boolean = true;

  intMessages: any = {
    CatError: '',
    uncatsucc: '',
    RecCnfirm: '',
    RecCatOthCnfm: '',
  };

  ngOnInit(): void {
    this.titleNames();
  }

  titleNames() {
    this.translate.get('s2s3app.title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('s2s3app.desc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });

    this.translate
      .get('s2s3app.Qigpaused')
      .subscribe((translated: string) => {
        this.intMessages.Markingprocesspaused = translated;
      });
  }

  getQigDetails(selectedqig: QigUserModel) {
    this.Alert.clear();
    this.filterclick = false;
    this.selectedmarkerdata = null;
    if (selectedqig != null && selectedqig.QigId > 0) {
      this.activeQig = selectedqig;
      this.IsS1Available = selectedqig.IsS1Available;
      this.getApprovalStatus(selectedqig.QigId);
      this.GetMarkingPersonal(selectedqig.QigId);
      setTimeout(() => {
        this.userhierarchymarkertreeviewcomponent?.getMarkerTreeview(
          this.activeQig.QigId
        );
      }, 300);

    }
  }

  activefilterTab: number = 3;
  selectedmarkerdata: any;

  getMarkingDetails(node: any) {
    this.selectedmarkerdata = node;
    this.ProjectUserRoleId = node.ProjectUserRoleID;
    this.GetMarkingPersonal(this.activeQig.QigId, this.ProjectUserRoleId);
  }

  getApprovalStatus(QigId: number) {
    this.appServices
      .GetApprovalStatus(QigId)
      .subscribe((data: S2S3Approvals) => {
        this.ApprovalStatusDetls = data;
      });
  }

  GetMarkingPersonal(QigId: number, UserRoleId: number = 0) {
    this.Getqigworkflowtracking();
    this.appServices
      .GetMarkingPersonal(QigId, UserRoleId)
      .subscribe((data: MarkingPersonal[]) => {
        this.PersonalDetails = data;
        this.PersonalSearchDetails = data;
      });
  }

  Redirection(UserRoleId: number) {
    this.route.navigate([
      '/projects/s2-s3-approvals/' +
      this.activeQig.QigId +
      '/' +
      UserRoleId +
      '/MarkersCompleteReport',
    ]);
  }

  selected: any[] = [];
  SearchMPName() {
    var SearchName = this.SearchName;
    this.PersonalDetails = this.PersonalSearchDetails.filter(function (el) {
      return el.MPName?.toLowerCase().includes(
        SearchName.trim().toLowerCase()
      );
    });
  }

  Getqigworkflowtracking() {
    this.qigservice
      .Getqigworkflowtracking(this.activeQig.QigId, AppSettingEntityType.QIG)
      .subscribe((data) => {
        if (data != null && data != undefined) {
          let WorkFlowStatusTracking = data;
          this.Ispause = WorkFlowStatusTracking.filter(
            (a: {
              WorkflowStatusCode: WorkflowStatus;
              ProcessStatus: WorkflowProcessStatus;
            }) =>
              a.WorkflowStatusCode == WorkflowStatus.Pause &&
              a.ProcessStatus == WorkflowProcessStatus.OnHold
          );
          if (this.Ispause.length > 0) {
            this.translate
              .get('s2s3app.Qigpaused')
              .subscribe((translated: string) => {
                this.Alert.warning(translated + '<br>' + 'Remarks : ' + this.Ispause[0].Remark+'.');
              });
          }
          this.titleNames();
        }
      });
  }

}
