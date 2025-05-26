import { ChangeDetectorRef, Component, OnInit, ViewChild, Inject } from '@angular/core';
import { UserCreationModel, RoleDetails, ProjectUsersModel } from 'src/app/model/project/setup/user-management';
import { UserManagementService } from 'src/app/services/project/setup/user-management.service';
import { ResolutionofCoiService } from 'src/app/services/project/setup/resolution-of-coi.service';
import { CoiSchoolModel } from 'src/app/model/project/setup/resolution-of-coi';
import { first } from 'rxjs/operators';
import { FormGroup, FormBuilder, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { AlertService } from 'src/app/services/common/alert.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'emarking-user-creation',
  templateUrl: './user-creation.component.html',
  styleUrls: ['./user-creation.component.css']
})
export class UserCreationComponent implements OnInit {
  @ViewChild('createform') createform: any;
  constructor(public usermanagementService: UserManagementService, public resolutionofcoiservice: ResolutionofCoiService,
    public fb: FormBuilder, public Alert: AlertService, private changeDetectorRef: ChangeDetectorRef, @Inject(MAT_DIALOG_DATA) public data: {

      defaultValue: any;
      htmpagetitle: any;
      disabled: boolean
    },
    private dialogRef: MatDialogRef<UserCreationComponent>) { }

  minimumdate = new Date();
  ispageloading: boolean = false;
  roledata !: RoleDetails[];
  SchoolList!: CoiSchoolModel[];
  userrolelist: boolean = false;
  selectedval !: string;
  scoolsLoading: boolean = false;
  selectedschool !: string;
  createuser !: UserCreationModel;
  ProjectForm!: FormGroup;
  ValidationErrMsg: string = "";
  submitted: boolean = false;
  loginname_err !: string;
  projectusersviewlist!: ProjectUsersModel[];
  ngOnInit(): void {
    this.ValidationSet();
    this.GetUserRole();
    this.GetSchoollist();
    var minCurrentDate = new Date();
    this.minimumdate = minCurrentDate;
  }

  GetUserRole() {
    this.userrolelist = true;
    this.usermanagementService.GetUsersRoles().subscribe(data => {
      this.roledata = data;
    }, (err: any) => {
      throw (err)
    }, () => {
      this.userrolelist = false;
    })
  }

  GetSchoollist() {
    this.scoolsLoading = true;
    this.resolutionofcoiservice.GetSchoolsCOI().pipe(first()).subscribe({
      next: (data: any) => {
        this.SchoolList = data;
      },
      error: (err: any) => {
        this.scoolsLoading = false;
        throw (err);
      }, complete: () => {
        this.scoolsLoading = false;
      }
    });
  }

  SaveUser() {
    this.ispageloading = true;
    this.submitted = true;
    this.ValidationErrMsg = "";

    var isAOschool = false;
    if (this.ProjectForm.value.role != 'AO') {
      isAOschool = this.ProjectForm.value.sendingschool == '';
    }

    if (this.ProjectForm.value.username == '' || this.ProjectForm.value.loginname == null
      || isAOschool || this.ProjectForm.value.role == null
      || this.ProjectForm.value.startdate == '' || this.ProjectForm.value.enddate == null
      || this.ProjectForm.value.nric == null) {
      this.ispageloading = false;
      return;
    }

    if (this.ProjectForm.status == 'VALID') {

      let createuser = new UserCreationModel()

      createuser.UserName = this.ProjectForm.get('username')?.value;
      createuser.LoginName = this.ProjectForm.get('loginname')?.value;
      createuser.SendingSchooolName = this.ProjectForm.get('sendingschool')?.value;
      createuser.RoleCode = this.ProjectForm.get('role')?.value;
      createuser.Appointmentstartdate = this.ProjectForm.get('startdate')?.value;
      createuser.Appointmentenddate = this.ProjectForm.get('enddate')?.value;
      createuser.NRIC = this.ProjectForm.get('nric')?.value;
      createuser.Phone = this.ProjectForm.get('phone')?.value;
      
      if (createuser.Appointmentenddate < createuser.Appointmentstartdate) {
        this.Alert.warning("Appointment Start Date should be less than the Appointment End Date.");
        this.ispageloading = false;
        return;
      }

      this.usermanagementService.CreateUser(createuser).subscribe({


        next: (data: any) => {
          this.Alert.clear();
          if (data == "S001") {
            this.Alert.success("User created successfully");
            this.createform.form.reset();
            this.createuserclose();

          }
          else if (data == "E001") {
            this.Alert.error("INVALID Data.");
            this.ispageloading = false;

          }
          else if (data == "E002") {
            this.Alert.error("INVALID ROLE");
            this.ispageloading = false;

          }
          else if (data == "E003") {
            this.Alert.error("INVALID SCHOOL");
            this.ispageloading = false;

          }
          else if (data == "E004") {
            this.Alert.error(" NRIC Already exists");
            this.ispageloading = false;

          }
          else if (data == "E005") {
            this.Alert.error("Created by does not exist in this project");
            this.ispageloading = false;

          }
          else if (data == "E006") {
            this.Alert.error("Created by and Created User are same");
            this.ispageloading = false;

          }
          else if (data == "E007") {
            this.Alert.error(" LoginName and Email are not same");
            this.ispageloading = false;

          }
          else if (data == "E008") {
            this.Alert.error("LoginID Already exists");
            this.ispageloading = false;

          }
          else if (data == "E009") {
            this.Alert.error("AO Already exists");
            this.ispageloading = false;

          }
        },
        error: (err: any) => {
          this.ispageloading = false;

        },
        complete: () => {
          this.ispageloading = false;
        }
      });
    }
    else {
      this.ispageloading = false;
    }

  }

  showErrorMessage(message: string) {
    this.ValidationErrMsg = message;
  }

  ValidateFormFields = function (control: AbstractControl): ValidationErrors | null {
    let value: string = control.value || '';

    if (!value) {
      return null
    }
    let onlyalphabets = /^(?!\d\s+$)(?:[a-zA-Z][a-zA-Z0-9 .,()'\&\@\/\-\_]*)?$/gm;
    let matchOnlyAlpha = control.value.match(onlyalphabets);

    if (matchOnlyAlpha == null) {
      return { "username_err": 'Invalid' };
    }
    return null;
  }

  ValidationSet() {
    this.ProjectForm = this.fb.group({
      username: ['', [Validators.required, this.ValidateFormFields]],
      loginname: ['', [Validators.required, Validators.pattern("^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")]],
      sendingschool: ['', Validators.required],
      role: ['', Validators.required],
      startdate: ['', Validators.required],
      enddate: ['', Validators.required],
      nric: ['', [Validators.required, Validators.pattern('^[a-zA-Z0-9]{9}$')]],
      phone: ['',[Validators.pattern('^[0-9]{10}$')]]
    });
  }
  onSchoolSelChange(eve: any) {
    if (eve?.value !== 'AO') {
      this.ProjectForm.get('sendingschool')?.addValidators(Validators.required);
    } else {
      this.ProjectForm.get('sendingschool')?.clearValidators();
    }
    this.ProjectForm.controls['sendingschool'].updateValueAndValidity();
  }

  ResetForm(val: boolean) {
    this.ProjectForm.controls['username'].reset();
    this.ProjectForm.controls['loginname'].reset();
    this.ProjectForm.controls['sendingschool'].reset();
    this.ProjectForm.controls['role'].reset();
    this.ProjectForm.controls['startdate'].reset();
    this.ProjectForm.controls['enddate'].reset();
    this.ProjectForm.controls['nric'].reset();
    this.ProjectForm.controls['phone'].reset();

    this.ProjectForm.get("username")?.setErrors(null);
    this.ProjectForm.get("loginname")?.setErrors(null);
    this.ProjectForm.get("sendingschool")?.setErrors(null);
    this.ProjectForm.get("role")?.setErrors(null);
    this.ProjectForm.get("startdate")?.setErrors(null);
    this.ProjectForm.get("enddate")?.setErrors(null);
    this.ProjectForm.get("nric")?.setErrors(null);
    this.ProjectForm.get("phone")?.setErrors(null);
  }

  CheckmandatoryFields(element: any) {
    console.log(element.target.getAttribute('formControlName')) // item_name 
    this.ValidationSet();
  }

  createuserclose() {
    this.dialogRef.close();
  }


}
