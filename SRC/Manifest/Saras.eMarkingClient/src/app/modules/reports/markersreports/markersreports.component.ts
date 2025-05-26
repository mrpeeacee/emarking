import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import { MarkerReportServices } from 'src/app/services/reports/markersreports.service';
import { MarkerDetails, MarkerPerformance, MarkerPerformanceStatistics, SchoolDetails } from 'src/app/model/reports/markersreports';
import { CommonDashboardService } from 'src/app/services/dashboard/common-dashboard.service';
import { AllExamYear, DashboardProject } from 'src/app/model/dashboard/dashboard';
import { QigConfigService } from 'src/app/services/project/qigconfig.service';
import { IUserQig } from 'src/app/model/standardisation/UserQIG';

@Component({
  selector: 'emarking-markersreports',
  templateUrl: './markersreports.component.html'
})
export class MarkersreportsComponent implements OnInit {

  MarkerLoading=false;
  markerperformance!:MarkerPerformance[];
  MarkerPerformanceStatistics!:MarkerPerformanceStatistics;
  LtSchools!:SchoolDetails[];
  examYearList!: AllExamYear[];
  projectList!: DashboardProject[];
  IsSelected!: boolean;
  Ltqigs!:IUserQig[];

  constructor(public markerReportServices: MarkerReportServices,
     public Alert: AlertService, public translate: TranslateService,
     public commonServices: CommonDashboardService, public qigconfigService: QigConfigService) 
     { 


     }

  ngOnInit(): void {
    this.GetExamYear();
    this.GetQigs(198);
  }

  GetMarkerDetails(){
    this.MarkerLoading = true;

    var markerdetails = new MarkerDetails();

    markerdetails.QigID = 1297;
    markerdetails.ProjectId =232;
    markerdetails.ExamYear = 2022;
    markerdetails.MarkerName = "";
    markerdetails.SchoolCode = "";

    this.getMarkersCount(markerdetails);
  
    this.markerReportServices.MarkerDetailsListReport(markerdetails).
    subscribe({
      next:(data : MarkerPerformance[]) => {

        this.markerperformance = data;

      },
      error: (a: any) => {
        throw (a);
      },
      complete: () => {
        this.MarkerLoading = false;
      }

    });

  }

  getMarkersCount(markerdetails:MarkerDetails){

    this.markerReportServices.MarkerSchoolCountReport(markerdetails).
    subscribe({
      next:(data : MarkerPerformanceStatistics) => {

        this.MarkerPerformanceStatistics = data;

      },
      error: (a: any) => {
        throw (a);
      },
      complete: () => {
        this.MarkerLoading = false;
      }

    });
  }


  GetExamYear() {
    this.commonServices.getExamYear().subscribe({
      next: (data: AllExamYear[]) => {
        if (data != null || data != undefined) {
          this.examYearList = data;
        }
      },
      error: (ad: any) => {
        throw ad;
      },
    });
  }

  onExamYearSelected(year: number) {
    if (year != 0) {
      this.getProjectsLists(year);
    }
    else{
      this.IsSelected = false;
    }
  }

  getProjectsLists(year: number) {
    this.commonServices.getProjectsLists(year).subscribe({
      next: (data: DashboardProject[]) => {
        if (data != null || data != undefined) {
          this.projectList = data;
          this.IsSelected = true;
        }
        else{
          this.IsSelected = false;
        }
      },
      error: (ad: any) => {
        throw ad;
      },
    });
  }


  onProjectSelected(projectId: number) {
      this.GetSchools(projectId);
      this.GetQigs(projectId);
   
  }


  GetSchools(projectId: number){
    this.markerReportServices.GetSchoolDetails(projectId).
    subscribe({
      next:(data : SchoolDetails[]) => {
        this.LtSchools=data;
      },
      error: (a: any) => {
        throw (a);
      },
      complete: () => {
        this.MarkerLoading = false;
      }
    });
  }

  GetQigs(projectId: number){
    this.markerReportServices.getQigs(projectId).
    subscribe({
      next:(data : IUserQig[]) => {
        this.Ltqigs=data;
      },
      error: (a: any) => {
        throw (a);
      },
      complete: () => {
        this.MarkerLoading = false;
      }
    });
  }

}
