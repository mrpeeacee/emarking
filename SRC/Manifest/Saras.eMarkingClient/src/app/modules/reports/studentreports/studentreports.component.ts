import { Component, OnInit } from '@angular/core';
import { Options } from '@angular-slider/ngx-slider';
import { StudentReportsService } from 'src/app/services/reports/studentreports.service';
import {
  ParamStdDetails,
  StudentsResult,
  StudentsResultStatistics,
} from 'src/app/model/reports/studentreports';
import { CommonDashboardService } from 'src/app/services/dashboard/common-dashboard.service';
import {
  AllExamYear,
  DashboardProject,
} from 'src/app/model/dashboard/dashboard';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from 'src/app/services/common/common.service';

@Component({
  selector: 'emarking-studentreports',
  templateUrl: './studentreports.component.html',
  styleUrls: ['./studentreports.component.css'],
})
export class StudentreportsComponent implements OnInit {
  selYear!: number;
  studentResultDetails!: StudentsResultStatistics;
  studentsResult!: StudentsResult[];
  examYearList!: AllExamYear[];
  projectList!: DashboardProject[];
  Singleproject: boolean = false;
  IsSelected: boolean = false;
  ParaDtls!: ParamStdDetails;
  Markfrom: number = 0;
  MarkTo: number = 100;
  TotalMark: number = 0;
  TotalSchool: number = 0;
  TotalStudent: number = 0;
  MaxMark: number = 0;
  Stdrptloading: boolean = false;
  Countstdloading: boolean = true;
  Subbtnloading: boolean = false;
  throttle: number = 0;
  distance: number = 2;
  options: Options = {
    floor: 0,
    ceil: 100,
  };
  constructor(
    public studentreportServices: StudentReportsService,
    public commonServices: CommonDashboardService,
    public translate: TranslateService,
    public commonService: CommonService
  ) {}

  ngOnInit(): void {
    this.setTitles();
    this.GetExamYear();
    this.getProjectsLists();
  }
  setTitles() {
    this.translate.get('StdDetails.stdres').subscribe((translated: string) => {
      this.commonService.setHeaderName(translated);
    });
    this.translate.get('StdDetails.pgedesc').subscribe((translated: string) => {
      this.commonService.setPageDescription(translated);
    });
  }

  GetStudentResultDetails(
    examYear: number,
    paraDtls: ParamStdDetails
  ) {
    this.Countstdloading = true;
    this.studentreportServices
      .GetStudentResultDetails(examYear, paraDtls)
      .subscribe({
        next: (data: StudentsResultStatistics) => {
          if (data != null || data != undefined) {
            this.studentResultDetails = data;
            if (paraDtls.Check ) {
              this.MaxMark = data.TotalMarks;
              this.options = { ceil: this.MaxMark };
            } else {
              this.MarkTo = data.TotalMarks;
              this.TotalMark = data.TotalMarks;
              this.TotalSchool = data.TotalSchoolCount;
              this.TotalStudent = data.TotalStudentsCount;
              this.options = { ceil: data.TotalMarks };
            }
          }
        },
        error: (ad: any) => {
          throw ad;
        },
        complete: () => {
          this.Countstdloading = false;
        },
      });
  }

  GetStudentsResult(
    examYear: number,
    paraDtls: ParamStdDetails
  ) {
    this.GetStudentResultDetails(examYear, paraDtls);
    this.Subbtnloading = true;
    this.studentsResult = [{}, {}, {}] as StudentsResult[];
    this.Stdrptloading = true;
    this.studentreportServices
      .GetStudentsResult(examYear, paraDtls)
      .subscribe({
        next: (data: StudentsResult[]) => {
          if (data != null || data != undefined) {
            this.studentsResult = data;
          }
        },
        error: (ad: any) => {
          throw ad;
        },
        complete: () => {
          this.Stdrptloading = false;
          this.Subbtnloading = false;
        },
      });
  }

  pageNumber: number = 2;
  onScroll(
    paraDtls: ParamStdDetails
  ): void {
    paraDtls.pageNumber = this.pageNumber++;
    paraDtls.pageSize = 10;
    this.studentreportServices
      .GetStudentsResult(this.projectList[0].Year, paraDtls)
      .subscribe({
        next: (data: StudentsResult[]) => {
          if (data != null || data != undefined) {
            this.studentsResult.push(...data);
          }
        },
        error: (ad: any) => {
          throw ad;
        },
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
      this.examYearList.forEach((exm) => {
        if (exm.Year == year) {
          exm.IsSelected = true;
        } else {
          exm.IsSelected = false;
        }
      });
    } else {
      this.IsSelected = false;
    }
  }

  getProjectsLists() {
    this.commonServices.getProjectsLists().subscribe({
      next: (data: DashboardProject[]) => {
        if (data != null || data != undefined) {
            this.projectList = data;
            this.IsSelected = true;
            if(data.length == 1){
              this.Singleproject = true;
             var mm = new ParamStdDetails();
             mm.Check = true;
              this.GetStudentResultDetails(this.projectList[0].Year, mm);
            }
            else{
              this.Singleproject = false;
            }
          } else {
            this.IsSelected = false;
          }
      },
      error: (ad: any) => {
        throw ad;
      },
    });
  }
}
