import { Component, ChangeDetectorRef } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { Qiguserdataviewmodel } from 'src/app/model/project/setup/user-management';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';


export class GameNode {
  children: BehaviorSubject<GameNode[]>;
  constructor(public item: string, children?: GameNode[], public parent?: GameNode, public iskp?: string, public S2S3Clear?: string, public RC1Count?: string, public RC2Count?: string, public Adhoc?: string, public role?: string,public Isactive?:boolean) {
    this.children = new BehaviorSubject(children === undefined ? [] : children);
  }
}

@Component({
  selector: 'emarking-user-hierarchical-view',
  templateUrl: './user-hierarchical-view.component.html',
  styleUrls: ['./user-hierarchical-view.component.css']
})
export class UserHierarchicalViewComponent {

  qigusershierachyviewlist!: Qiguserdataviewmodel[];
  showhideval: boolean = true;
  QigIdVal!: number;
  heirarchyviewloading: boolean = true;
  objarray: GameNode[] = [];
  childarray: GameNode[] = [];
  treeControl: NestedTreeControl<GameNode>;
  dataSource: MatTreeNestedDataSource<GameNode>;
  IsQIGVal!: boolean;

  constructor(private changeDetectorRef: ChangeDetectorRef, public usermanagementService: UserManagementService) {
    this.treeControl = new NestedTreeControl<GameNode>(this.getChildren);
    this.dataSource = new MatTreeNestedDataSource();
    this.dataSource.data = this.objarray;
  }

  getChildren = (node: GameNode) => {
    return node.children;
  };

  hasChildren = (index: number, node: GameNode) => {
    return node.children.value.length > 0;
  }

  ShowHierarchyComponent(showhideval: boolean, IsQIG: boolean = false) {
    this.IsQIGVal = IsQIG;
    this.showhideval = showhideval;
  }

  ShowHideHierarchyComponent() {
    this.showhideval = false;
  }

  getQigusersheirachytview(QigId: number) {
    this.QigIdVal = QigId;
    this.heirarchyviewloading = true;
    var objQiguserdata = new Qiguserdataviewmodel();
    objQiguserdata.ProjectUserRoleID = 0; 
    objQiguserdata.QIGId = QigId;
    this.usermanagementService.QigdataorHierarchyview(objQiguserdata).subscribe(data => {
      if (data.length > 0 || data != null || data != undefined) {
        this.qigusershierachyviewlist = data;
        var aolists = this.qigusershierachyviewlist.filter(a => a.RoleID == 'AO');

        aolists.forEach(aoelement => {
          this.objarray = [];
          var aoarr: GameNode[] = [];
          var objchild = new GameNode(aoelement.LoginName, aoarr, undefined, aoelement.IsKPVal, aoelement.S2S3Clear, aoelement.RC1Count, aoelement.RC2Count, aoelement.Adhoc, aoelement.RoleID, aoelement.Isactive);

          this.qigusershierachyviewlist.filter(a => a.ReportingTo == aoelement.ProjectUserRoleID).
            forEach(cmelement => {
              var cmarr: GameNode[] = [];
              aoarr.push(new GameNode(cmelement.LoginName, cmarr, undefined, cmelement.IsKPVal, cmelement.S2S3Clear, cmelement.RC1Count, cmelement.RC2Count, cmelement.Adhoc, cmelement.RoleID, cmelement.Isactive));

              this.qigusershierachyviewlist.filter(a => a.ReportingTo == cmelement.ProjectUserRoleID).forEach(acmelement => {
                var acmarr: GameNode[] = [];
                cmarr.push(new GameNode(acmelement.LoginName, acmarr, undefined, acmelement.IsKPVal, acmelement.S2S3Clear, acmelement.RC1Count, acmelement.RC2Count, acmelement.Adhoc, acmelement.RoleID, acmelement.Isactive));

                this.qigusershierachyviewlist.filter(a => a.ReportingTo == acmelement.ProjectUserRoleID).forEach(tlelement => {
                  var tlarr: GameNode[] = [];
                  acmarr.push(new GameNode(tlelement.LoginName, tlarr, undefined, tlelement.IsKPVal, tlelement.S2S3Clear, tlelement.RC1Count, tlelement.RC2Count, tlelement.Adhoc, tlelement.RoleID, tlelement.Isactive));

                  this.qigusershierachyviewlist.filter(a => a.ReportingTo == tlelement.ProjectUserRoleID).forEach(atlmelement => {
                    var atlarr: GameNode[] = [];
                    tlarr.push(new GameNode(atlmelement.LoginName, atlarr, undefined, atlmelement.IsKPVal, atlmelement.S2S3Clear, atlmelement.RC1Count, atlmelement.RC2Count, atlmelement.Adhoc, atlmelement.RoleID, atlmelement.Isactive));

                    this.qigusershierachyviewlist.filter(a => a.ReportingTo == atlmelement.ProjectUserRoleID).forEach(markerelement => {
                      var markerarr: GameNode[] = [];
                      atlarr.push(new GameNode(markerelement.LoginName, markerarr, undefined, markerelement.IsKPVal, markerelement.S2S3Clear, markerelement.RC1Count, markerelement.RC2Count, markerelement.Adhoc, markerelement.RoleID, markerelement.Isactive));
                    });

                  });

                });

              });

            });

          this.objarray.push(objchild);
          this.dataSource = new MatTreeNestedDataSource();
          this.dataSource.data = this.objarray;
        });
      }
    }, (err: any) => {
      throw (err)
    }, () => {
      this.heirarchyviewloading = false;
    });
  }

}
