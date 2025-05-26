import { NestedTreeControl } from '@angular/cdk/tree';
import { Component, Output, EventEmitter, HostListener } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { QualifyingAssessmentService } from 'src/app/services/standardisation/qualifying-assessment.service';
import { QualityCheckService } from '../../../../services/project/quality-check/quality-check.service';
import { MarkerTreeView } from 'src/app/model/project/quality-check/marker-tree-view-model';
import { trigger, state, style, transition, animate } from '@angular/animations';

export class GameNode {
  children: BehaviorSubject<GameNode[]>;
  constructor(public item: string, children?: GameNode[], public ProjectUserRoleID?: number, public role?: string, public iskp?: boolean) {
    this.children = new BehaviorSubject(children === undefined ? [] : children);
  }
} 

@Component({
  selector: 'emarking-marker-tree-view',
  templateUrl: './marker-tree-view.component.html',
  styleUrls: ['./marker-tree-view.component.css'],
  animations: [
    trigger('event_pop', [
      state('closed', style({ height: '0px', opacity: '0', overflow: 'hidden', })),
      state('open', style({
        height: '*',
        opacity: '1',
      })),
      transition('open => closed', animate('200ms ease-out')),
      transition('closed => open', animate('300ms ease-out'))
    ]),
  ]
})
export class MarkerTreeViewComponent {
  qigmarkerstreeviewlist!: MarkerTreeView[];
  showhideval: boolean = true;
  QigIdVal!: number;
  heirarchyviewloading: boolean = false;
  objarray: GameNode[] = [];
  treeControl: NestedTreeControl<GameNode>;
  dataSource: MatTreeNestedDataSource<GameNode>;
  ProjectuserroleId: number = 0;

  superparent: any[] = [];

  parents: any[] = [];

  children: any[] = [];

  filterclick: boolean = false;
  state = "closed"
  statepop = "popclosed"

  @Output() selectTreeNodeEvent = new EventEmitter<GameNode>();

  constructor(public usermanagementService: UserManagementService,
    public qualifyingAssessmentService: QualifyingAssessmentService,
    public qaCheckService: QualityCheckService) {
    this.treeControl = new NestedTreeControl<GameNode>(this.getChildren);
    this.dataSource = new MatTreeNestedDataSource();
    this.dataSource.data = this.objarray;

  }

  getChildren = (node: GameNode) => {
    return node.children;
  };

  getNodeDetails(node: any, event: any) {
    if (node != undefined && node != null) {
      this.leftmenuopen(2, event);
      this.ProjectuserroleId = node.ProjectUserRoleID;
      this.selectTreeNodeEvent.emit(node);
    }
  }
  hasChildren = (_index: number, node: GameNode) => {
    return node.children.value.length > 0;
  }

  getMarkerTreeview(QigId: number) {
    this.QigIdVal = QigId;
    this.heirarchyviewloading = true;
    this.qaCheckService.GetQIGProjectUserReportees(this.QigIdVal).subscribe(data => {
      this.qigmarkerstreeviewlist = data;
      var superparents = this.qigmarkerstreeviewlist.filter(a => a.ReportingTo == 0);
      superparents.forEach(aoelement => {
        this.objarray = [];
        var aoarr: GameNode[] = [];
        var aoarr1: GameNode[] = [];
        aoarr1.push(new GameNode(aoelement.ProjectUserName, [], aoelement.ProjectUserRoleID, aoelement.RoleCode, aoelement.IsKp));
        this.selectTreeNodeEvent.emit(aoarr1[0]);
        var objchild = new GameNode(aoelement.ProjectUserName, aoarr, aoelement.ProjectUserRoleID, aoelement.RoleCode, aoelement.IsKp);
        this.qigmarkerstreeviewlist.filter(a => a.ReportingTo == aoelement.ProjectUserRoleID).
          forEach(cmelement => {
            var cmarr: GameNode[] = [];
            aoarr.push(new GameNode(cmelement.ProjectUserName, cmarr, cmelement.ProjectUserRoleID, cmelement.RoleCode, cmelement.IsKp));
            this.qigmarkerstreeviewlist.filter(a => a.ReportingTo == cmelement.ProjectUserRoleID).
              forEach(acmelement => {
                var acmarr: GameNode[] = [];
                cmarr.push(new GameNode(acmelement.ProjectUserName, acmarr, acmelement.ProjectUserRoleID, acmelement.RoleCode, acmelement.IsKp));
                this.qigmarkerstreeviewlist.filter(a => a.ReportingTo == acmelement.ProjectUserRoleID).
                  forEach(tlelement => {
                    var tlarr: GameNode[] = [];
                    acmarr.push(new GameNode(tlelement.ProjectUserName, tlarr, tlelement.ProjectUserRoleID, tlelement.RoleCode, tlelement.IsKp));
                    this.qigmarkerstreeviewlist.filter(a => a.ReportingTo == tlelement.ProjectUserRoleID).
                      forEach(atlelement => {
                        var atlarr: GameNode[] = [];
                        tlarr.push(new GameNode(atlelement.ProjectUserName, atlarr, atlelement.ProjectUserRoleID, atlelement.RoleCode, atlelement.IsKp));
                        this.qigmarkerstreeviewlist.filter(a => a.ReportingTo == atlelement.ProjectUserRoleID).
                          forEach(markerelement => {
                            var markerarr: GameNode[] = [];
                            atlarr.push(new GameNode(markerelement.ProjectUserName, markerarr, markerelement.ProjectUserRoleID, markerelement.RoleCode, markerelement.IsKp));
                          });
                      });
                  });
              });
          });
        this.objarray.push(objchild);
        this.dataSource = new MatTreeNestedDataSource();
        this.dataSource.data = this.objarray;
      });
    }, (err: any) => {
      throw (err)
    }, () => {
      this.heirarchyviewloading = false;
    });
  }

  ShowHierarchyComponent(showhideval: boolean) {
    this.showhideval = showhideval;
  }

  ShowHideHierarchyComponent() {
    this.showhideval = false;
  }

  event_pop() {
    this.filterclick = true;
    this.state = "open"
  }


  @HostListener('document:click', ['$event'])
  ondocumentClick(event: MouseEvent) {
    this.leftmenuopen(2, event);
  }


  @HostListener('document:keydown', ['$event'])
  onKeyDown(ev: KeyboardEvent) {
    if (ev.key === "Escape") {
      this.leftmenuopen(2, ev);
    }
  }

  leftmenuopen(id: number, event: any) {
    if (id == 1) {
      if (this.state == "closed") {
        this.state = "open";
        //this.getMarkerTreeview(this.QigIdVal)
        event.stopPropagation();
      }
    }
    else {
      this.state = "closed";
    }
  }



}
