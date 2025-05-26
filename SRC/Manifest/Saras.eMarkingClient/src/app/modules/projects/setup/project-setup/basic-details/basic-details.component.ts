import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { BasicDetailsService } from 'src/app/services/project/setup/basic-details.service';
import { BasicDetailsModel } from 'src/app/model/project/setup/basic-details-model';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { AlertService } from 'src/app/services/common/alert.service';

@Component({
  templateUrl: './basic-details.component.html',
  styleUrls: ['./basic-details.component.css']
})
export class BasicDetailsComponent implements OnInit {
  ProjectForm!: FormGroup;
  BasicDetails!: BasicDetailsModel;
  ispageloading: boolean = false;
  constructor(public basicdetailsservice: BasicDetailsService, public commonService: CommonService,
    public translate: TranslateService, private formBuilder: FormBuilder, public Alert: AlertService) { }

  ngOnInit(): void {
    this.translate.get('SetUp.Basic.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.ProjectForm = this.formBuilder.group({
      txtProjectInfo: []
    });
    this.GetBasicDetails(0);
    this.translate.get('SetUp.Basic.PageDesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
  }

  GetBasicDetails(navigate:number) {
    this.ispageloading = true;
    this.basicdetailsservice.GetBasicDetails(navigate).subscribe({
      next: (data: any) => {
        this.BasicDetails = data;
        if (this.BasicDetails != null && this.BasicDetails != undefined) {
          this.ProjectForm.setValue({
            txtProjectInfo: this.BasicDetails.ProjectInfo
          });
        }
      },
      error: (a: any) => {
        throw a;
      },
      complete: () => {
        this.ispageloading = false;
      },
    });
  }

  AddUpdateProjectDetails() {
    if (this.ProjectForm.get('txtProjectInfo')?.value == null || this.ProjectForm.get('txtProjectInfo')?.value.trim() == '') {
      this.Alert.warning(this.translate.instant('SetUp.Basic.InfoWarning'));
      return;
    }
    let Basicdetailsobj = new BasicDetailsModel();
    if (!this.ProjectForm.valid)
      this.validateAllFormFields(this.ProjectForm);
    else {
      Basicdetailsobj.ProjectInfo = this.ProjectForm.get('txtProjectInfo')?.value.trim();
      this.ispageloading = true;
      this.basicdetailsservice.UpdateBasicDetails(Basicdetailsobj).subscribe({
        next: (data: any) => {
          if (data == "P001") {
            this.Alert.clear();
            this.Alert.success(this.translate.instant('SetUp.Basic.UpdateSuccess'));
            this.GetBasicDetails(1);
          }
        },
        error: (a: any) => {
          throw(a);
        },
        complete: () => {
          this.ispageloading = false;
        },
      });
    }
  }
  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      if (control instanceof FormControl) {
        control.markAsTouched({ onlySelf: true });
      } else if (control instanceof FormGroup) {
        this.validateAllFormFields(control);
      }
    });
  }
}

