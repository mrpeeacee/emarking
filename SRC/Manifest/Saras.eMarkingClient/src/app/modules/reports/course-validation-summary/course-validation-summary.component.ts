import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CourseValidationModel } from 'src/app/model/reports/studentreports';
import { CommonService } from 'src/app/services/common/common.service';
import { StudentReportsService } from 'src/app/services/reports/studentreports.service';

@Component({
  templateUrl: './course-validation-summary.component.html',
  styleUrls: ['./course-validation-summary.component.css']
})
export class CourseValidationSummaryComponent implements OnInit {

  CourseValidation !: CourseValidationModel[];
  isLoading: boolean = true;
  constructor(
    public studentreportServices: StudentReportsService,
    public translate: TranslateService,
    public commonService: CommonService) { }

  ngOnInit(): void {
    this.commonService.setHeaderName("Course Validation Summary");
    this.commonService.setPageDescription("Course Validation Summary");
    this.getCourseValidationLists();
  }

  getCourseValidationLists() {
    this.isLoading = true;
    this.studentreportServices.GetCourseValidationResult().subscribe({
      next: (data: CourseValidationModel[]) => {
        if (data != null || data != undefined) {
          this.CourseValidation = data;
        }
        this.isLoading = false;
      },
      error: (ad: any) => {
        throw ad;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

}
