import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { ProjectQigUserModel, ProjectUsersModel } from 'src/app/model/project/setup/user-management';
import { AlertService } from 'src/app/services/common/alert.service';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { Location } from '@angular/common';
import { first } from 'rxjs';
import { CommonService } from 'src/app/services/common/common.service';

@Component({
  selector: 'emarking-user-import',
  templateUrl: './user-import.component.html',
  styleUrls: ['./user-import.component.css']
})
export class UserImportComponent implements OnInit {
  @ViewChild('myFile') myInputVariable!: ElementRef;

  constructor(public usermanagementService: UserManagementService,
    public translate: TranslateService,
    public Alert: AlertService,
    private route: ActivatedRoute, private router: Router, private location: Location, public commonService: CommonService) { }

  ngOnInit() {
    this.activeQig = this.route.snapshot.params['qigid'];
    this.translate.get('SetUp.UserManagement.tit').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('SetUp.UserManagement.desc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.translate.get('SetUp.UserManagement.template').subscribe((translated: string) => {
      this.intMessages.template = translated;
    });
    this.translate.get('SetUp.UserManagement.login').subscribe((translated: string) => {
      this.intMessages.login = translated;
    });
    this.translate.get('SetUp.UserManagement.reporting').subscribe((translated: string) => {
      this.intMessages.reporting = translated;
    });
    this.translate.get('SetUp.UserManagement.role').subscribe((translated: string) => {
      this.intMessages.role = translated;
    });
    this.translate.get('SetUp.UserManagement.fileuploadsuccess').subscribe((translated: string) => {
      this.intMessages.fileuploadsuccess = translated;
    });
    this.translate.get('SetUp.UserManagement.validdata').subscribe((translated: string) => {
      this.intMessages.validdata = translated;
    });
    this.translate.get('SetUp.UserManagement.errorimport').subscribe((translated: string) => {
      this.intMessages.errorimport = translated;
    });
    this.translate.get('SetUp.UserManagement.selectfile').subscribe((translated: string) => {
      this.intMessages.selectfile = translated;
    });
    this.translate.get('SetUp.UserManagement.filename').subscribe((translated: string) => {
      this.intMessages.filename = translated;
    });
    this.translate.get('SetUp.UserManagement.invalidfile').subscribe((translated: string) => {
      this.intMessages.invalidfile = translated;
    });
    this.translate.get('SetUp.UserManagement.emptyfile').subscribe((translated: string) => {
      this.intMessages.emptyfile = translated;
    });
    this.translate.get('SetUp.UserManagement.Invaliddata').subscribe((translated: string) => {
      this.intMessages.Invaliddata = translated;
    });

    this.getProjectUsers();
  }

  file: any;
  qiguserslist: ProjectQigUserModel[] = [];
  nodata: boolean = false;
  Qiglevelenable: boolean = false;
  activeQig: number = 0;
  loading: boolean = false;
  projectusersviewlist!: ProjectUsersModel[];
  invaliddata!: any[];
  emptyfile!: any[];
  datalist!: any[];

  intMessages: any = {
    template: '',
    login: '',
    reporting: '',
    role: '',
    fileuploadsuccess: '',
    validdata: '',
    errorimport: '',
    selectfile: '',
    filename: '',
    invalidfile: '',
    emptyfile: '',
    Invaliddata: ''
  };

  getProjectUsers() {
    this.usermanagementService.Projectuserview().subscribe(data => {
      if (data.length > 0 || data != null || data != undefined) {
        this.projectusersviewlist = data;
      }
    }, (err: any) => {
      throw (err)
    });
  }

  onChange(event: any) {
    this.file = event.target.files[0];
  }

  QIGLevelUpload() {
    this.loading = true;
    this.Alert.clear();
    if (this.file != undefined && this.file.type == 'text/csv') {
      this.usermanagementService.QigUsersImportFile(this.file, this.activeQig).pipe(first()).subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            this.loading = true;
            this.qiguserslist = data;
            this.invaliddata = this.qiguserslist.filter(a => a.returnstatus == "Invaliddata");
            this.emptyfile = this.qiguserslist.filter(a => a.returnstatus == "Emptyfile");

            this.Alert.clear();
            if (data.length == 0) { 
              this.myInputVariable.nativeElement.value = "";
              this.file = undefined;
              setTimeout(() => {
             
                this.router.navigateByUrl('projects/setup/user-management/' + this.activeQig);
                this.Alert.success(this.intMessages.fileuploadsuccess);
              }, 100);
             
              return;
            }
            else if (this.emptyfile.length > 0) {
              this.Alert.warning(this.intMessages.emptyfile);
              this.myInputVariable.nativeElement.value = "";
              this.file = undefined;
              return;
            }
            else if (this.invaliddata.length > 0) {
              this.Alert.warning(this.intMessages.Invaliddata);
              this.myInputVariable.nativeElement.value = "";
              this.file = undefined;
              return;
            }
            else {
              this.Alert.warning(this.intMessages.validdata);
              this.myInputVariable.nativeElement.value = "";
              this.file = undefined;
              return;
            }
          }
        },
        error: (err: any) => {
          this.loading = false;
          this.Alert.error(this.intMessages.errorimport);
          this.qiguserslist = [];
          throw (err);
        }, complete: () => {
          this.loading = false;
        }
      });
    }
    else if (this.file == undefined || this.myInputVariable.nativeElement.value == "") {
      this.Alert.warning(this.intMessages.selectfile);
      this.myInputVariable.nativeElement.value = "";
      this.loading = false;
    }
    else if (this.file.type != 'text/csv') {
      this.Alert.warning(this.intMessages.invalidfile);
      this.myInputVariable.nativeElement.value = "";
      this.qiguserslist = [];
      this.loading = false;
    }
  }

  back() {
    this.router.navigateByUrl('projects/setup/user-management/' + this.activeQig);
  }

  download(val: any) {
    let fileName = val == 0 ? this.intMessages.template : this.intMessages.filename;
    let columnNames = [this.intMessages.login, this.intMessages.role, this.intMessages.reporting];
    let header = columnNames.join(',');

    let csv = header;
    csv += '\r\n';

    if (val == 0) {
      this.datalist = [
        {
          "LoginName": null,
          "Role": null,
          "ReportingTo": null
        }
      ];

      this.datalist.forEach(c => {
        csv += [c["LoginName"], c["Role"], c["ReportingTo"]].join(',');
        csv += '\r\n';
      })
    }
    else {
      this.projectusersviewlist.forEach(element => {
        this.datalist = [
          {
            "LoginName": element == null ? null : element.LoginName,
            "Role": element == null ? null : element.RoleID,
            "ReportingTo": null
          }
        ];

        this.datalist.forEach(c => {
          csv += [c["LoginName"], c["Role"], c["ReportingTo"]].join(',');
          csv += '\r\n';
        })

      });
    }

    var blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });

    var link = document.createElement("a");
    if (link.download !== undefined) {
      var url = URL.createObjectURL(blob);
      link.setAttribute("href", url);
      link.setAttribute("download", fileName);
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
    }
  }

}
