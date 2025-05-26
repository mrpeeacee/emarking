import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';
import { first } from 'rxjs/operators';
import { AlertService } from 'src/app/services/common/alert.service';
import { ProjectLevelConfigModel } from 'src/app/model/project/setup/project-level-config-model';
import { ProjectLevelConfigService } from 'src/app/services/project/setup/project-level-config.service';

@Component({
  templateUrl: './project-level-configuration.component.html',
  styleUrls: ['./project-level-configuration.component.css'],
})
export class ProjectLevelConfigurationComponent implements OnInit {

  ProjectConfig: ProjectLevelConfigModel[] = [];
  toggleChecked: boolean = false;
  projectstatus!: number;
  status: boolean = false;
  buttonDisabled = false;

  constructor(
    public router: Router,
    public commonService: CommonService,
    public projectConfigService: ProjectLevelConfigService,
    public translate: TranslateService,
    public Alert: AlertService
  ) {}

  projectlevleconfig: boolean = false;
  projectlevleconfigsave: boolean = false;

  ngOnInit(): void {
    this.translate.get('SetUp.Config.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate
      .get('SetUp.Config.PageDescription')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    this.getProjectConfig();
  }

  getProjectConfig() {
    this.projectlevleconfig = true;
    this.projectConfigService
      .getProjectLevelConfig()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            this.ProjectConfig = data;
            this.projectstatus = this.ProjectConfig[0].ProjectStatus;   
             if(this.projectstatus == 1 || this.projectstatus == 2) 
              {
                this.status=true;
              }       
            this.ProjectConfig.forEach((element) => {
              element.Value = JSON.parse(element.Value.toLowerCase());
              if (element.Children != null) {
                element.Children.forEach((child) => {
                  if(element.AppsettingKey != "ANNTNCLR"){
                    child.Value = JSON.parse(child.Value.toLowerCase());
                  }
                });
              }
            });
          } else {
            this.Alert.warning('No data');
          }
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.projectlevleconfig = false;
        },
      });
  }

  toggleQuestionType(KeyID: number, qtype: ProjectLevelConfigModel[]) {
    if (qtype != null && qtype.length > 0) {
      qtype.forEach((element) => {
        if (element.AppSettingKeyID != KeyID) {
          element.Value = false;
        } else {
          element.Value = true;
        }
      });
    }
  }

  saveprojectconfig() {
    this.Alert.clear();
    this.ProjectConfig.forEach((element) => {
      if (element.Children != null && element.Children.length > 0) {
        element.Children.forEach((child) => {
          this.ProjectConfig.push(child);
        });
      }
    });

    this.projectlevleconfigsave = true;
    this.projectConfigService
      .updateProjectLevelConfig(this.ProjectConfig)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data == 'Invalid') {
            this.Alert.clear();
            this.Alert.warning('Invalid data');
            this.getProjectConfig();
          } else if (data == 'Duration') {
            this.Alert.clear();
            this.Alert.warning('Job duration must be between  1min to 1440min');
            this.getProjectConfig();
          } else if (data != null && data != undefined) {
            this.updateProjectRcSetting();
          } else {
            this.Alert.error(
              'Error while updating the project configuration setting'
            );
          }
        },
        error: (a: any) => {
          this.projectlevleconfigsave = false;
          throw a;
        },
        complete: () => {
          this.projectlevleconfigsave = false;
        },
      });
  }

  updateProjectRcSetting() {
    this.projectConfigService
      .updateProjectRandomCheck()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            this.Alert.clear();
            this.Alert.success(
              'Project configuration settings updated successfully'
            );
            this.getProjectConfig();
          } else {
            this.Alert.error(
              'Error while updating the project configuration setting'
            );
          }
        },
        error: (a: any) => {
          this.projectlevleconfigsave = false;
          throw a;
        },
        complete: () => {
          this.projectlevleconfigsave = false;
        },
      });
  }

  numericOnly(event: { key: string }): boolean {
    let patt = /^([0-9])$/;
    return patt.test(event.key);
  }

  validateNumber(event: any) {
    var invalidChars = ['-', 'e', '+', 'E'];
    if (invalidChars.includes(event.key)) {
      event.preventDefault();
    }
  }

  inputHandler(event: any) {
    const { value, maxLength } = event.target;
    if (String(value).length >= maxLength) {
      event.preventDefault();
    }
    if ((event.charCode == 45 || event.charCode == 46)) {
      event.preventDefault();
    }
  }

  updateStatus(){
    this.buttonDisabled = true;
    this.projectConfigService.updateProjectStatus().pipe(first()).subscribe({
      next:(data:any) =>{
        if (data == 'SCHERR') {  
          this.buttonDisabled = false;
          this.toggleChecked = false;
          this.Alert.clear();    
          this.Alert.warning('Project Schedule has not been finalised yet');   
        } else if (data == 'S001') {
          this.Alert.clear();
          this.Alert.success("This Project is 'In-Progress' now");  
        }       
      },
      error:(err:any) =>{
        throw err;
      },
     }); 
  }
}
