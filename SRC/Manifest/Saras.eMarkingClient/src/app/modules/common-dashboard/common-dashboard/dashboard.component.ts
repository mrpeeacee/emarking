import { Component, Injectable, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { AuthService } from 'src/app/services/auth/auth.service';
import { CommonService } from 'src/app/services/common/common.service';
import { Menuitem } from 'src/app/services/common/menuitem';
import {
  DashboardProject,
  ProjectStatistics,
  ProjectStatusDetail
} from 'src/app/model/dashboard/dashboard';
import { ChartType, ChartData, ChartConfiguration } from 'chart.js';
import DatalabelsPlugin from 'chartjs-plugin-datalabels';
import { BaseChartDirective } from 'ng2-charts';
import { first } from 'rxjs/operators';
import { CommonDashboardService } from 'src/app/services/dashboard/common-dashboard.service';
import { AlertService } from 'src/app/services/common/alert.service';
import { GlobalUserManagementService } from 'src/app/services/Global/UserManagement/UserManagementService';
import { CreateEditUser } from 'src/app/model/Global/UserManagement/UserManagementModel';
import { SharedserviceService } from '../Sso-Login/SharesServices/sharedservice.service';
import { UserLogin } from '../../auth/user';
import { environment } from 'src/environments/environment';


@Component({
  selector: 'emarking-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
})

@Injectable({
  providedIn: 'root'
})

export class DashboardComponent implements OnInit {
  constructor(
    private authService: AuthService,
    public dashService: CommonDashboardService,
    public commonService: CommonService,
    public router: Router,
    public Alert: AlertService,
    private translate: TranslateService,
    public globalUserManagementService: GlobalUserManagementService,
    public sharedService: SharedserviceService


  ) { }
  @ViewChild(BaseChartDirective) chart: BaseChartDirective | undefined;
  public pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: {
        display: true,
        position: 'bottom',
        onClick: () => {
          return false;
        },
      },
    },
  };
  public pieChartData: ChartData<'pie', number[], string | string[]> = {
    labels: [
      ['Total No. of Projects'],
      ['Projects in-Progress'],
      ['Completed Projects'],
    ],
    datasets: [
      {
        data: [0, 0, 0],
      },
    ],
  };
  public pieChartType: ChartType = 'pie';
  public pieChartPlugins = [DatalabelsPlugin];

  ProjectsList!: DashboardProject[];
  projectStatistics!: ProjectStatistics;
  menulist!: Menuitem[];
  ProjectSearchValue: string = '';
  ProjectsSearchList!: DashboardProject[];
  TotalProjectsCount: number = 0;
  InProgressCount: number = 0;
  CompletedCount: number = 0;
  selected: any[] = [];
  stasticsloading: boolean = false;
  projectsloading: boolean = false;
  QigId!: number;
  projectdetailscount = new ProjectStatistics();
  Myprofile1!: CreateEditUser;
  IsEnable: boolean = false;
  objectList!: UserLogin;
  btnChange: boolean = false;
  projectStatus = new ProjectStatusDetail();


  ngOnInit(): void {
    this.projectsloading = true;

    if (environment.IsArchive) {
      this.btnChange = true;
    }
    this.authService
      .reGenerateToken(-1)
      ?.pipe(first())
      .subscribe({
        next: () => {
          this.getMyprofileDetails();
          this.GetProjectStastics();
          this.GetProjects();
        },
        error: (a: any) => {
          throw a;
        },
        complete: () => {
          this.projectsloading = false;
        },
      });
    this.ProjectsList = [{}, {}, {}, {}] as DashboardProject[];

    this.translate.get('Home.Title').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });

    this.translate.get('Home.PageDesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
  }
  GetProjectStastics() {
    this.stasticsloading = true;
    this.dashService.getProjectStatistics().subscribe(
      (data: ProjectStatistics | null | undefined) => {

        if (data != null && data != undefined) {
          this.projectStatistics = data;
          this.pieChartData.datasets = [
            {
              data: [
                this.projectStatistics.TotalProjects,
                this.projectStatistics.ProjectsInProgress,
                this.projectStatistics.ProjectsCompleted,
              ],
            },
          ];
          this.chart?.update();
        }
      },
      (err: any) => {
        throw err;
      },
      () => {
        this.stasticsloading = false;
      }
    );
  }

  viewList = 'LIST_VIEW';

  ChangeView(event: any) {
    if (event === 'LIST_VIEW') this.viewList = 'LIST_VIEW';
    else this.viewList = 'GRID_VIEW';
  }

  GetProjects() {
    this.projectsloading = true;
    this.dashService
      .getProjectsLists()
      .pipe(first())
      .subscribe({
        next: (data: any) => {

          if (data == null || data == undefined || data.length == 0) {
            this.ProjectsList = [];
            this.projectdetailscount.TotalProjects = this.ProjectsList?.length;
            this.projectdetailscount.ProjectsInProgress =
              this.ProjectsList?.filter((p) => p.ProjectStatus == 1).length;
            this.projectdetailscount.ProjectsCompleted =
              this.ProjectsList?.filter((p) => p.ProjectStatus == 3).length;
          } else {
            this.ProjectsList = data;
            this.ProjectsSearchList = data;
            this.projectdetailscount.TotalProjects = this.ProjectsList?.length;
            this.projectdetailscount.ProjectsInProgress =
              this.ProjectsList?.filter((p) => p.ProjectStatus == 1).length;
            this.projectdetailscount.ProjectsCompleted =
              this.ProjectsList?.filter((p) => p.ProjectStatus == 3).length;
          }
          this.projectsloading = false;
        },
        error: (a: any) => {
          this.projectsloading = false;
          throw a;
        },
        complete: () => {
          this.projectsloading = false;
        },
      });
  }


  projectDetails(objproject: any, projectqig: number) {

    this.Alert.clear();
    if (!objproject.UserIsActive) {
      this.Alert.warning(
        "Currently you are in 'Inactive'' status. This project is not accessible to you."
      );
      return;
    }

    if (
      (objproject.ProjectStatus != 3 ||
        (objproject.ProjectStatus == 3 &&
          (objproject.ProjectUserRole?.RoleCode == 'AO' ||
            objproject.ProjectUserRole?.RoleCode == 'EO' ||
            objproject.ProjectUserRole?.RoleCode == 'SUPERADMIN' ||
            objproject.ProjectUserRole?.RoleCode == 'SERVICEADMIN' ||
            objproject.ProjectUserRole?.RoleCode == 'EM'))) &&
      !this.projectsloading
    ) {
      this.QigId =
        projectqig == 0 && objproject.ProjectQigs.length > 0
          ? objproject.ProjectQigs[0].ProjectQigid
          : projectqig;
      localStorage.setItem('PHome', objproject.ProjectName);
      this.projectsloading = true;
      this.authService
        .reGenerateToken(objproject.ProjectID)
        ?.pipe(first())
        .subscribe({
          next: (a: any) => {

            let curntrole = this.authService.getCurrentRole();


            this.getMyprofileDetails();

            curntrole.forEach((role) => {
              if (
                role == 'EO' ||
                role == 'AO' ||
                role == 'ACM' ||
                role == 'CM' ||
                role == 'SUPERADMIN' ||
                role == 'SERVICEADMIN' ||
                role == 'EM'
              ) {
                if (role == "CM" || role == "ACM") {
                  this.dashService.getProjectStatus(objproject.ProjectID).subscribe(result => {
                    this.projectStatus = result;
                    if (this.projectStatus.status == "E001") {
                      this.Alert.warning("Project Schedule time not set yet.")
                      return;
                    }
                    else if (this.projectStatus.status == "E002") {
                      this.Alert.warning("Today is not a working day. You cannot access the marking project.")
                      return;

                    }
                    else if (this.projectStatus.status == "E003") {
                      this.Alert.warning("Something went wrong!")
                      return;
                    }
                    else if (this.projectStatus.status == "E004") {
                      this.Alert.warning("You cannot access the marking project outside the working hours.")
                    }
                    else {
                      this.router.navigate([
                        `/projects/dashboards/ao-cm`,
                        this.QigId,
                      ]);

                    }
                  })
                }
                else {
                  this.router.navigate([
                    `/projects/dashboards/ao-cm`,
                    this.QigId,
                  ]);
                }

              }
              if (role == 'TL' || role == 'ATL') {

                this.dashService.getProjectStatus(objproject.ProjectID).subscribe(result => {
                  this.projectStatus = result;
                  if (this.projectStatus.status == "E001") {
                    this.Alert.warning("Project Schedule time not set yet.")
                    return;
                  }
                  else if (this.projectStatus.status == "E002") {
                    this.Alert.warning("Today is not a working day. You cannot access the marking project.")
                    return;

                  }
                  else if (this.projectStatus.status == "E003") {
                    this.Alert.warning("Something went wrong!")
                    return;
                  }
                  else if (this.projectStatus.status == "E004") {
                    this.Alert.warning("You cannot access the marking project outside the working hours.")
                    return
                  }
                  else {
                    this.router.navigate([
                      `/projects/dashboards/tl-atl`,
                      this.QigId,
                    ]);

                  }
                })
              }
              
              if (role == 'MARKER') {
                this.dashService.getProjectStatus(objproject.ProjectID).subscribe(result => {
                  this.projectStatus = result;

                  if (this.projectStatus.status == "E001") {
                    this.Alert.warning("Project Schedule time not set yet.")
                    return;
                  }
                  else if (this.projectStatus.status == "E002") {
                    this.Alert.warning("Today is not a working day. You cannot access the marking project.")
                    return;

                  }
                  else if (this.projectStatus.status == "E003") {
                    this.Alert.warning("Something went wrong!")
                    return;
                  }
                  else if (this.projectStatus.status == "E004") {
                    this.Alert.warning("You cannot access the marking project outside the working hours.")
                  }
                  else {
                    this.router.navigate([
                      `/projects/dashboards/marker`,
                      this.QigId,
                    ]);

                  }
                })


              }

            });
          },
          error: (a: any) => {
            this.authService.clearAccessToken();
            throw a;
          },
          complete: () => {
            this.projectsloading = false;
          },
        });
    }
    if (
      objproject.ProjectStatus == 3 &&
      (objproject.ProjectUserRole?.RoleCode != 'AO' ||
        objproject.ProjectUserRole?.RoleCode != 'EO') &&
      !this.projectsloading
    ) {
      this.Alert.warning('This project is closed.');
    }
  }

  SearchProjects() {
    if (!this.projectsloading) {
      var ProjectSearchValue = this.ProjectSearchValue;
      this.ProjectsList = this.ProjectsSearchList.filter(function (el) {
        return el.ProjectName.toLowerCase().includes(
          ProjectSearchValue.trim().toLowerCase()
        );
      });
      this.ProjectsList = this.ProjectsList.filter(
        (x) =>
          this.selected.includes(x.ProjectStatus) || this.selected.length == 0
      );
    }
  }



  filterProjects(event: any, optionvalue: any) {
    if (!this.projectsloading) {
      var ProjectSearchValue = this.ProjectSearchValue;
      if (event.checked) {
        this.selected.push(optionvalue);
      } else
        this.selected = this.selected.filter((item) => item !== optionvalue);
      if (this.selected.length == 0)
        this.ProjectsList = this.ProjectsSearchList;
      else
        this.ProjectsList = this.ProjectsSearchList.filter((x) =>
          this.selected.includes(x.ProjectStatus)
        );
      this.ProjectsList = this.ProjectsList.filter(function (el) {
        return el.ProjectName.toLowerCase().includes(
          ProjectSearchValue.trim().toLowerCase()
        );
      });
    }
  }

  getMyprofileDetails() {
    this.globalUserManagementService
      .getMyprofileDetails()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          this.Myprofile1 = data;
          this.sendDataToHeader();
          if ((!environment.IsArchive)) {
            if ((this.Myprofile1.RoleCode == 'AO' || this.Myprofile1.RoleCode == 'SUPERADMIN' || this.Myprofile1.RoleCode == 'SERVICEADMIN')) {
              this.IsEnable = true
            }
          }
        },
        error: (err: any) => {
          throw err;
        },
      });
  }

  sendDataToHeader() {
    const data = this.Myprofile1;
    this.globalUserManagementService.updateData(data);
  }






}
