import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CreateEditUser, ExamLevels, GetCreateEditUserModel } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { AlertService } from 'src/app/services/common/alert.service';
import { CommonService } from 'src/app/services/common/common.service';
import { GlobalUserManagementService } from 'src/app/services/Global/UserManagement/UserManagementService';

@Component({
  selector: 'emarking-create-user',
  templateUrl: './create-user.component.html',
  styleUrls: ['./create-user.component.css']
})
export class CreateUserComponent implements OnInit {

  Roleschoolst!: GetCreateEditUserModel;
  CreateEditUserForm!: FormGroup;
  createuserformloading: boolean = false;
  @ViewChild('matSelectrole') matSelectrole: any;
  @ViewChild('matSelectschool') matSelectschool: any;
  @ViewChild('matSelectexamlevel') matSelectexamlevel: any;
  UserId: number = 0;
  ExamLevellst!: ExamLevels[];
  roleselected: boolean = false;
  isRoleClicked: boolean = false;
  isSchoolClicked: boolean = false;
  selectedrole: string = '';
  ExamselectedValue: ExamLevels[] = [];

  selectedschool:ExamLevels[] = [];

  IsDisabledRoleUser:boolean = false;


  constructor(public globalusermanagementservice: GlobalUserManagementService, public translate: TranslateService, 
    public Alert: AlertService, private route: Router, private router: ActivatedRoute, public commonService: CommonService) {
  }

  ngOnInit(): void {
    this.UserId = this.router.snapshot.params['userId'];
    if (this.UserId == undefined) {
      this.UserId = 0;
      this.translate.get('usermanage.createtitle').subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });

    }
    else {
      this.translate.get('usermanage.edittitle').subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    }
    this.translate.get('usermanage.createdesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
    this.GetRoleSchools(this.UserId);
  }

  GetRoleSchools(UserId: number) {
    this.createuserformloading = true;
    this.globalusermanagementservice.GetCreateEditUserdetails(UserId).subscribe({
      next: (data: GetCreateEditUserModel) => {
        if (data != null && data != undefined) {
          this.Roleschoolst = data;

          if(((this.Roleschoolst?.RoleCode == 'SERVICEADMIN') || 
          (this.Roleschoolst.selectedroleinfo == 'MARKINGPERSONNEL') ||
           (this.Roleschoolst?.RoleCode == 'AO' && this.Roleschoolst.checkdataexist))){
            this.IsDisabledRoleUser = true;
          }
          else{
            this.IsDisabledRoleUser = false;
          }


              this.Roleschoolst ?.Examlevels.forEach(elem => {
                if (elem.isselected) {
                  this.selectedschool.push(elem);
                }
              });

             this.ExamselectedValue = this.selectedschool;
        }
      },
      complete: () => {
        this.createuserformloading = false;
      },
    });
  }

  Gotobacktoapp() {
    this.route.navigate(['userManagement/ApplicationUsermanagement']);
  }

  onRoleSelChange(event: any, selected: boolean) {
    this.Roleschoolst.RoleCode = event.value;
    if (this.matSelectexamlevel != undefined) {
      this.matSelectexamlevel.value = '';
    }
    this.isRoleClicked = false;
    this.selectedrole = this.matSelectrole.value;
    if (this.selectedrole == null) {
      this.isRoleClicked = true;
    }
    else if (this.selectedrole == "EO" || this.selectedrole == "EM") {
      this.roleselected = selected;
    }
    this.IsDisabledRoleUser = false;
  }
  tempcheckdataexist!: boolean;


  onSchoolClick(event: any) {
    this.isSchoolClicked = false;
    if (this.matSelectschool.value == null) {
      this.isSchoolClicked = true;
    }
  }

  CreateEditUser(Roleschoolst: GetCreateEditUserModel) {
    this.createuserformloading = true;
    var createuserobj = new CreateEditUser();
    if(this.matSelectrole.value=='EO'){
      createuserobj.RoleName= 'Exam Officer';
    }
    else if(this.matSelectrole.value=='AO'){
      createuserobj.RoleName= 'Assessment Officer';
    } else if(this.matSelectrole.value=='EM'){
      createuserobj.RoleName= 'Exam Manager';
    }
    else if (this.matSelectrole.value=='MARKINGPERSONNEL'){
      createuserobj.RoleName= 'MARKINGPERSONNEL';
    }

    if (Roleschoolst.Loginname != null && Roleschoolst.Username != null && Roleschoolst.Nric != null
      && this.matSelectrole.value != null) {     
      createuserobj.UserId = Roleschoolst.UserId;
      createuserobj.Username = Roleschoolst.Username.trim();
      createuserobj.Loginname = Roleschoolst.Loginname.trim();
      createuserobj.RoleCode = this.matSelectrole.value;
      createuserobj.Nric = Roleschoolst.Nric.trim();
      
      if (this.matSelectschool != undefined) {
        createuserobj.SchooolCode = this.matSelectschool.value;
      }
  
      if (Roleschoolst.PhoneNum != null) {
        createuserobj.PhoneNum = Roleschoolst.PhoneNum.trim();
      }
      createuserobj.Examlevels = this.Roleschoolst ?.Examlevels ?.filter(a => a.isselected);

      this.globalusermanagementservice.Createuser(createuserobj).subscribe({
        next: (data: any) => {
          this.Alert.clear();
          if (data != null && data != undefined) {
            if (data == "I001") {
            this.translate
              .get('usermanage.usercreatesuccess')
              .subscribe((translated: string) => {
                this.GotoMain();
                this.Alert.success(translated);
              });
              this.createuserformloading = false;
            }
            else if (data == "U001") {
              this.translate
                .get('usermanage.userupdatesuccess')
                .subscribe((translated: string) => {
                  this.GotoMain();
                  this.Alert.success(translated);
                });
              this.createuserformloading = false;
              
            }
            else if (data == "E001") {
              this.translate
                .get('usermanage.invalidcreatedby')
                .subscribe((translated: string) => {
                  this.Alert.error(translated);
                });
              this.createuserformloading = false;
            }
            else if (data == "E002") {
              this.translate
                .get('usermanage.invalidrole')
                .subscribe((translated: string) => {
                  this.Alert.error(translated);
                });
              this.createuserformloading = false;
            }
            else if (data == "E003") {
              this.translate
                .get('usermanage.nricexists')
                .subscribe((translated: string) => {
                  this.Alert.error(translated);
                });
              this.createuserformloading = false;
            }
            else if (data == "E005") {
              this.translate
                .get('usermanage.emailexists')
                .subscribe((translated: string) => {
                  this.Alert.error(translated);
                });
              this.createuserformloading = false;
            }
          }
        },
        complete: () => {
          this.createuserformloading = false;
        },
      });
    }
    else {
      this.createuserformloading = false;
    }
  }

  Ischecked(event: any, el: ExamLevels) {
    
    el.isselected = event.source.selected;
  }

  GotoMain() {
    this.route.navigate(['userManagement/ApplicationUsermanagement']);
  }
}
