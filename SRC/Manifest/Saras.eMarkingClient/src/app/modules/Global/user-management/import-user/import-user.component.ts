import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { PerfectScrollbarComponent } from 'ngx-perfect-scrollbar';
import { first } from 'rxjs';
import { UserCreations } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { GlobalUserManagementService } from 'src/app/services/Global/UserManagement/UserManagementService';

@Component({
  selector: 'emarking-import-user',
  templateUrl: './import-user.component.html',
  styleUrls: ['./import-user.component.css']
})
export class ImportUserComponent implements OnInit {
  @ViewChild('myFile') myInputVariable!: ElementRef;
  @ViewChild('perfectScroll') perfectScroll!: PerfectScrollbarComponent;
  file: any;
  datalist!: any[];
  isSubmitted: boolean = false;
  userCreations!: UserCreations;
  title!: string;
  config = {
    suppressScrollX: true,
    scrollToTop: 0
  };
  constructor(public globalusermanagementservice: GlobalUserManagementService,
    public Alert: AlertService, public translate: TranslateService, public commonService: CommonService, private route: Router) { }

  ngOnInit(): void {
    this.textinternationalization();
  }


  textinternationalization() {
    this.translate
      .get('usermanage.uipagedesc')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });

    this.translate
      .get('usermanage.uititle')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
        this.title = translated;
      });
  }

  UploadFile(type: number) {
    if (this.file != undefined && this.file != "") {
      var ext = ".csv";
      var extension = this.file.name.substr(this.file.name.length - ext.length, ext.length).toLowerCase();
      if (extension == ext) {
        if (this.file != undefined && this.file.type == 'text/csv') {
          this.globalusermanagementservice.UserCreations(this.file, type).pipe(first()).subscribe({
            next: (data: UserCreations) => {
              this.userCreations = data;
              if (this.userCreations.status == "S001" && type == 2) {
                this.isSubmitted = false;
                this.translate
                .get('usermanage.userimportsuccess')
                .subscribe((translated: string) => {
                  this.GotoMain();
                  this.Alert.success(translated);              
                });
               
              }
              else if (this.userCreations.status == "S001" && type == 1) {
                this.isSubmitted = true;
                this.translate
                  .get('usermanage.filevalidationsuccess')
                  .subscribe((translated: string) => {
                    this.Alert.success(translated);
                  });
              }
              else if (this.userCreations.status == "FAILED") {
                this.file = "";
                this.isSubmitted = false;
                this.translate
                  .get('usermanage.uploadvalidfile')
                  .subscribe((translated: string) => {
                    this.Alert.error(translated);
                  });
              }
              else if (this.userCreations.status == "NODATA") {
                this.translate
                  .get('usermanage.nodataexcel')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
              else if (this.userCreations.status == "NOTCSV") {
                this.translate
                  .get('usermanage.csvfile')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
              else if (this.userCreations.status == "INVALIDFILE") {
                this.file = "";
                this.isSubmitted = false;
                this.translate
                  .get('usermanage.samplecsvfile')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
              else if (this.userCreations.status == "INVALIDFORMAT") {
                this.file = "";
                this.isSubmitted = false;
                this.translate
                  .get('usermanage.invalidsamplecsvfile')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
              else if (this.userCreations.status == "EMPTYFILE") {
                this.file = "";
                this.isSubmitted = false;
                this.translate
                  .get('usermanage.emptyfileupload')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
              else if (this.userCreations.status == "EMPTYROW") {
                this.file = "";
                this.isSubmitted = false;
                this.translate
                  .get('usermanage.refersamplefile')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
              else if(this.userCreations.status == "Duplicate"){
                this.file = "";
                this.isSubmitted = false;
                this.translate
                  .get('usermanage.Duplicaterecord')
                  .subscribe((translated: string) => {
                    this.Alert.warning(translated);
                  });
              }
            },
            error: (err: any) => {
              this.myInputVariable.nativeElement.value = "";
              this.translate
                .get('usermanage.selectcsvfile')
                .subscribe((translated: string) => {
                  this.Alert.warning(translated);
                });
            }, complete: () => {
              this.perfectScroll?.directiveRef!.update();
              this.perfectScroll?.directiveRef!.scrollToTop(0, 0);
              this.perfectScroll?.directiveRef!.scrollToLeft(0, 0);
            }
          });
        }
      }
      else {
        this.isSubmitted = false;
        this.myInputVariable.nativeElement.value = "";
        this.translate
          .get('usermanage.selectcsvfileonly')
          .subscribe((translated: string) => {
            this.Alert.warning(translated);
          });
      }
    }
    else if (this.file == undefined || this.file == "") {
      this.isSubmitted = false;
      this.myInputVariable.nativeElement.value = "";
      this.translate
        .get('usermanage.selectcsvfile')
        .subscribe((translated: string) => {
          this.Alert.warning(translated);
        });
    }
  }

  onChange(event: any) {
    this.file = event.target.files[0];
  }

  ClearFile(event: any) {
    this.file = '';
    this.myInputVariable.nativeElement.value = "";
  }

  downloadsamplefile() {
    let fileName = 'users';
    let columnNames = ['Name', 'LoginName', 'NRIC', 'SchoolCode', 'SchoolName', 'BaseRole'];
    let header = columnNames.join(',');

    let csv = header;
    csv += '\r\n';

    this.datalist = [
      {
        "Name": null,
        "LoginName": null,
        "NRIC": null,
        "SchoolCode": null,
        "SchoolName": null,
        "BaseRole": null
      }
    ];

    this.datalist.forEach(c => {
      csv += [c["Name"], c["LoginName"], c["NRIC"], c["SchoolCode"], c["SchoolName"], c["BaseRole"]].join(',');
      csv += '\r\n';
    })

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

  GotoMain() {
    this.route.navigate(['userManagement/ApplicationUsermanagement']);
  }
}
