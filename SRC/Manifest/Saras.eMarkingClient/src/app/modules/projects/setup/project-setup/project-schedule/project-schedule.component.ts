import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonService } from 'src/app/services/common/common.service';
import { ProjectScheduleService } from 'src/app/services/project/setup/project-schedule.service';
import { TranslateService } from '@ngx-translate/core';
import { AlertService } from 'src/app/services/common/alert.service';
import {
  MatCalendarCellCssClasses,
  MatDatepickerInputEvent,
} from '@angular/material/datepicker';
import { first } from 'rxjs';
import {
  DayWiseScheduleModel,
  ProjectScheduleModel,
} from 'src/app/model/project/setup/project-schedule-model';
import { NotificationService } from 'src/app/services/common/notification.service';

@Component({
  templateUrl: './project-schedule.component.html',
  styleUrls: ['./project-schedule.component.css'],
})
export class ProjectScheduleComponent implements OnInit {
  minimumdate = new Date();
  tominimumdate = new Date();
  serverdatetime: Date = new Date();
  IsUpdate: boolean = false;
  ProjectScheduleList: any;
  EditStartDate: any;
  EditEndDate: any;
  selinput: any;
  pickedate: any;
  displayConfirmStyle: string = 'none';
  Isvalid: boolean = false;
  disablebtncurrdate: boolean = false;
  starttime: any;
  endtime: any;
  isLoading: boolean = false;
  projectschedulesaveloading: boolean = false;
  Daywisesaveloading: boolean = false;
  workingdaysdefaultconfig: any = {
    Monday: true,
    Tuesday: true,
    Wednesday: true,
    Thursday: true,
    Friday: true,
    Saturday: false,
    Sunday: false,
  };
  PSList: any = {
    sdate: '',
    edate: '',
    sdisable: false,
    Disableall: false,
    sdatedisable: true,
  };
  daywiseconfigdata: any = {
    strttime: '',
    endtime: '',
    remarks: '',
    daytype: '',
    calendarid: '',
    ChoosenDate: '',
    scheduleid: '',
    SelectedDate: Date,
  };
  weekend: any = {
    saturday: false,
    sunday: false,
  };
  intMessages: any = {
    Futuredatewarning: '',
    Emptydateswarning: '',
    Fromdatewarning: '',
    Todatewarning: '',
    Fromdatelesswarning: '',
    Fromtimewarning: '',
    Totimewarning: '',
    Fromtimelesswarning: '',
    Successalert: '',
    Updatealert: '',
    Confirmalert: '',
    Daywiseconfigupdatealert: '',
    Durationalert: '',
    Endtimealert: '',
    Sametimealert: '',
    Validtimealert: '',
    Invalidalert: '',
    RemarkLenEx: '',
  };

  constructor(
    public projectscheduleService: ProjectScheduleService,
    public commonService: CommonService,
    public translate: TranslateService,
    public Alert: AlertService,
    public router: Router,
    public notificatonservice: NotificationService
  ) {}

  ngOnInit(): void {
    this.translate
      .get('SetUp.Schedule.Title')
      .subscribe((translated: string) => {
        this.commonService.setHeaderName(translated);
      });
    this.GetProjectSchedule();
    this.translate
      .get('SetUp.Schedule.Description')
      .subscribe((translated: string) => {
        this.commonService.setPageDescription(translated);
      });
    this.translate
      .get('SetUp.Schedule.Futuredatewarning')
      .subscribe((translated: string) => {
        this.intMessages.Futuredatewarning = translated;
      });
    this.translate
      .get('SetUp.Schedule.Emptydateswarning')
      .subscribe((translated: string) => {
        this.intMessages.Emptydateswarning = translated;
      });
    this.translate
      .get('SetUp.Schedule.Fromdatewarning')
      .subscribe((translated: string) => {
        this.intMessages.Fromdatewarning = translated;
      });
    this.translate
      .get('SetUp.Schedule.Todatewarning')
      .subscribe((translated: string) => {
        this.intMessages.Todatewarning = translated;
      });
    this.translate
      .get('SetUp.Schedule.Fromdatelesswarning')
      .subscribe((translated: string) => {
        this.intMessages.Fromdatelesswarning = translated;
      });
    this.translate
      .get('SetUp.Schedule.Fromtimewarning')
      .subscribe((translated: string) => {
        this.intMessages.Fromtimewarning = translated;
      });
    this.translate
      .get('SetUp.Schedule.Totimewarning')
      .subscribe((translated: string) => {
        this.intMessages.Totimewarning = translated;
      });
    this.translate
      .get('SetUp.Schedule.Fromtimelesswarning')
      .subscribe((translated: string) => {
        this.intMessages.Fromtimelesswarning = translated;
      });
    this.translate
      .get('SetUp.Schedule.Successalert')
      .subscribe((translated: string) => {
        this.intMessages.Successalert = translated;
      });
    this.translate
      .get('SetUp.Schedule.Confirmalert')
      .subscribe((translated: string) => {
        this.intMessages.Confirmalert = translated;
      });
    this.translate
      .get('SetUp.Schedule.Updatealert')
      .subscribe((translated: string) => {
        this.intMessages.Updatealert = translated;
      });
    this.translate
      .get('SetUp.Schedule.Daywiseconfigupdatealert')
      .subscribe((translated: string) => {
        this.intMessages.Daywiseconfigupdatealert = translated;
      });
    this.translate
      .get('SetUp.Schedule.Durationalert')
      .subscribe((translated: string) => {
        this.intMessages.Durationalert = translated;
      });
    this.translate
      .get('SetUp.Schedule.Endtimealert')
      .subscribe((translated: string) => {
        this.intMessages.Endtimealert = translated;
      });
    this.translate
      .get('SetUp.Schedule.Sametimealert')
      .subscribe((translated: string) => {
        this.intMessages.Sametimealert = translated;
      });
    this.translate
      .get('SetUp.Schedule.Validtimealert')
      .subscribe((translated: string) => {
        this.intMessages.Validtimealert = translated;
      });
    this.translate
      .get('SetUp.Schedule.Invalidalert')
      .subscribe((translated: string) => {
        this.intMessages.Invalidalert = translated;
      });
    this.translate
      .get('SetUp.Schedule.RemarkLenEx')
      .subscribe((translated: string) => {
        this.intMessages.RemarkLenEx = translated;
      });
    this.notificatonservice.CurrentDateTime$.subscribe((msg) => {
      this.serverdatetime = msg;
      this.minimumdate = msg;
    });
  }

  GetProjectSchedule() {
    this.isLoading = true;
    this.projectscheduleService
      .GetProjectSchedule()
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            this.PSList.sdate = data.StartDate;
            this.PSList.edate = data.ExpectedEndDate;
            this.ProjectScheduleList = data;
            this.IsUpdate = this.ProjectScheduleList.IsUpdate;
            if (this.IsUpdate) {
              this.changeEvent(this.ProjectScheduleList.StartDate);
              this.weekend.saturday = JSON.parse(
                this.ProjectScheduleList.WorkingDaysConfigJson.Saturday
              );
              this.weekend.sunday = JSON.parse(
                this.ProjectScheduleList.WorkingDaysConfigJson.Sunday
              );

              var cmpstdate = new Date(this.ProjectScheduleList.StartDate);
              if (
                new Date(
                  cmpstdate.getFullYear(),
                  cmpstdate.getMonth(),
                  cmpstdate.getDate()
                ) <
                new Date(
                  this.serverdatetime.getFullYear(),
                  this.serverdatetime.getMonth(),
                  this.serverdatetime.getDate()
                )
              ) {
                this.PSList.sdisable = true;
              }

              var cmpexpenddte = new Date(
                this.ProjectScheduleList.ExpectedEndDate
              );
              if (
                new Date(
                  cmpexpenddte.getFullYear(),
                  cmpexpenddte.getMonth(),
                  cmpexpenddte.getDate()
                ) <
                new Date(
                  this.serverdatetime.getFullYear(),
                  this.serverdatetime.getMonth(),
                  this.serverdatetime.getDate()
                )
              ) {
                this.PSList.sdatedisable = false;
                this.PSList.Disableall = true;
              }
            } else {
              this.ProjectScheduleList.CreatedDate =
                this.serverdatetime.toLocaleDateString();
              this.ProjectScheduleList.StartDate = '';
              this.ProjectScheduleList.ExpectedEndDate = '';
              this.ProjectScheduleList.StartTime = '09:00';
              this.ProjectScheduleList.EndTime = '18:30';
              this.ProjectScheduleList.WorkingDaysConfigJson =
                this.workingdaysdefaultconfig;
            }
          }
        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.isLoading = false;
        },
      });
  }

  changeEvent(event: any) {
    this.selinput = event;
    let DayWiseScheduleobj = new DayWiseScheduleModel();
    DayWiseScheduleobj.StartTime = this.ProjectScheduleList.StartTime;
    DayWiseScheduleobj.EndTime = this.ProjectScheduleList.EndTime;
    this.pickedate = new Date(this.selinput);
    this.pickedate = new Date(
      this.pickedate.getFullYear(),
      this.pickedate.getMonth(),
      this.pickedate.getDate()
    );
    if (
      new Date(
        this.pickedate.getFullYear(),
        this.pickedate.getMonth(),
        this.pickedate.getDate()
      ) <
      new Date(
        this.serverdatetime.getFullYear(),
        this.serverdatetime.getMonth(),
        this.serverdatetime.getDate()
      )
    ) {
      this.disablebtncurrdate = true;
    } else {
      this.disablebtncurrdate = false;
    }
    DayWiseScheduleobj.SelectedDate = this.pickedate;
    this.projectschedulesaveloading = true;
    this.projectscheduleService
      .GetDayWiseConfig(DayWiseScheduleobj)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            this.daywiseconfigdata.strttime = data.StartTime;
            this.daywiseconfigdata.endtime = data.EndTime;
            this.daywiseconfigdata.remarks = data.Remarks;
            this.daywiseconfigdata.daytype = data.DayType;
            this.daywiseconfigdata.calendarid = data.ProjectCalendarID;
            this.daywiseconfigdata.ChoosenDate = data.ChoosenDate;
            this.daywiseconfigdata.ProjectCalendarID = data.ProjectCalendarID;
            this.daywiseconfigdata.SelectedDate = data.ChoosenDate;
          }
        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.projectschedulesaveloading = false;
        },
      });
  }

  fromDatechangeEvent(event: MatDatepickerInputEvent<Date>) {
    if (event != null && event.value != null) this.tominimumdate = event.value;
  }

  SaveUpdateProjectShedule(RetainOrOverwrite: number) {
    this.projectschedulesaveloading = false;
    this.Daywisesaveloading = false;
    if (!this.projectschedulesaveloading && !this.Daywisesaveloading) {
      this.Isvalid = false;
      let stdate = new Date(this.ProjectScheduleList.StartDate);
      let enddate = new Date(this.ProjectScheduleList.ExpectedEndDate);
      let sttime = this.ProjectScheduleList.StartTime.split(':');
      let endtime = this.ProjectScheduleList.EndTime.split(':');
      this.ProjectScheduleList.StartDate = new Date(
        stdate.getFullYear(),
        stdate.getMonth(),
        stdate.getDate(),
        sttime[0],
        sttime[1]
      );
      this.ProjectScheduleList.ExpectedEndDate = new Date(
        enddate.getFullYear(),
        enddate.getMonth(),
        enddate.getDate(),
        endtime[0],
        endtime[1]
      );
      if (!this.IsUpdate) {
        this.displayConfirmStyle = 'none';
        this.Alert.clear();
        this.validateProjectSchedule();
        if (!this.Isvalid) {
          this.projectschedulesaveloading = true;
          this.SaveUpdateProjectSheduleToDB(RetainOrOverwrite);
        }
      } else {
        this.Alert.clear();
        this.validateProjectSchedule();
        if (!this.Isvalid) {
          document.getElementById('ShowModalDiv')?.click();
        }
      }
    }
  }

  SaveUpdateProjectSheduleToDB(RetainOrOverwrite: number) {
    this.projectschedulesaveloading = false;
    this.Daywisesaveloading = false;
    if (!this.projectschedulesaveloading && !this.Daywisesaveloading) {
      this.displayConfirmStyle = 'none';
      var PList = this.ProjectScheduleList;
      let Projectscheduleobj = new ProjectScheduleModel();

      Projectscheduleobj.StartDate = new Date(PList.StartDate);
      Projectscheduleobj.StartDate.setDate(
        Projectscheduleobj.StartDate.getDate()
      );
      Projectscheduleobj.ExpectedEndDate = new Date(PList.ExpectedEndDate);
      Projectscheduleobj.ExpectedEndDate.setDate(
        Projectscheduleobj.ExpectedEndDate.getDate()
      );
      Projectscheduleobj.scheduleTimeModels = PList.scheduleTimeModels;
      Projectscheduleobj.StartTime = PList.StartTime;
      Projectscheduleobj.EndTime = PList.EndTime;
      Projectscheduleobj.IsUpdate = this.IsUpdate;
      let oBased: Record<string, string> = {
        Monday: this.ProjectScheduleList.WorkingDaysConfigJson.Monday,
        Tuesday: this.ProjectScheduleList.WorkingDaysConfigJson.Tuesday,
        Wednesday: this.ProjectScheduleList.WorkingDaysConfigJson.Wednesday,
        Thursday: this.ProjectScheduleList.WorkingDaysConfigJson.Thursday,
        Friday: this.ProjectScheduleList.WorkingDaysConfigJson.Friday,
        Saturday: this.weekend.saturday,
        Sunday: this.weekend.sunday,
      };
      Projectscheduleobj.WorkingDaysConfig = oBased;
      if (!this.IsUpdate) {
        this.projectschedulesaveloading = true;
        this.projectscheduleService
          .UpdateProjectSchedule(Projectscheduleobj)
          .subscribe({
            next: (data: any) => {
              if (data != null && data != undefined) {
                this.ShowProjectScheduleStatus(data);
                if (data == 'SU001' || data == 'U001') {
                  this.Alert.clear();
                  this.Alert.success(this.intMessages.Successalert);
                  this.GetProjectSchedule();
                }
              }
            },
            error: (err: any) => {
              throw err;
            },
            complete: () => {
              this.projectschedulesaveloading = false;
            },
          });
      } else {
        this.EditStartDate = PList.StartDate;
        this.EditEndDate = PList.ExpectedEndDate;

        if (PList.StartDate == this.PSList.sdate) {
          Projectscheduleobj.StartDate = PList.StartDate;
        }
        if (PList.ExpectedEndDate == this.PSList.edate) {
          Projectscheduleobj.ExpectedEndDate = PList.ExpectedEndDate;
        }
        Projectscheduleobj.Confirmdialogeventvalue = RetainOrOverwrite;
        this.projectschedulesaveloading = true;

        this.projectscheduleService
          .UpdateProjectSchedule(Projectscheduleobj)
          .pipe(first())
          .subscribe({
            next: (data: any) => {
              if (data != null && data != undefined) {
                this.ShowProjectScheduleStatus(data);
                if (data == 'SU001' || data == 'U001') {
                  this.reloadComponent();
                  this.Alert.success(this.intMessages.Updatealert);
                }
              }
            },
            error: (err: any) => {
              throw err;
            },
            complete: () => {
              this.projectschedulesaveloading = false;
            },
          });
      }
    }
  }

  ShowProjectScheduleStatus(apistatus: string) {
    this.Alert.clear();
    if (apistatus == 'Fromdatewarning') {
      this.Alert.error(this.intMessages.Fromdatewarning);
    }
    if (apistatus == 'Todatewarning') {
      this.Alert.error(this.intMessages.Todatewarning);
    }
    if (apistatus == 'Futuredatewarning') {
      this.Alert.error(this.intMessages.Futuredatewarning);
    }
    if (apistatus == 'Fromdatelesswarning') {
      this.Alert.error(this.intMessages.Fromdatelesswarning);
    }
    if (apistatus == 'Fromtimelesswarning') {
      this.Alert.error(this.intMessages.Fromtimelesswarning);
    }
  }

  UpdateDayWiseConfigurations() {
    var DaywiseList = this.daywiseconfigdata;
    let DayWiseScheduleobj = new DayWiseScheduleModel();
    DayWiseScheduleobj.StartTime = DaywiseList.strttime;
    DayWiseScheduleobj.EndTime = DaywiseList.endtime;
    DayWiseScheduleobj.Remarks = DaywiseList.remarks;
    DayWiseScheduleobj.DayType = DaywiseList.daytype;
    DayWiseScheduleobj.ProjectCalendarID = DaywiseList.ProjectCalendarID;

    let stdate = new Date(DaywiseList.SelectedDate);
    let sttime = DaywiseList.strttime.split(':');
    let endtime = DaywiseList.endtime.split(':');
    DayWiseScheduleobj.StartDateTime = new Date(
      stdate.getFullYear(),
      stdate.getMonth(),
      stdate.getDate(),
      sttime[0],
      sttime[1]
    );
    DayWiseScheduleobj.EndDateTime = new Date(
      stdate.getFullYear(),
      stdate.getMonth(),
      stdate.getDate(),
      endtime[0],
      endtime[1]
    );

    if (
      DayWiseScheduleobj.EndTime.split(':').length ==
      DayWiseScheduleobj.StartTime.split(':').length
    ) {
      if (DayWiseScheduleobj.EndTime == DayWiseScheduleobj.StartTime) {
        this.Alert.warning(this.intMessages.Sametimealert);
        return;
      }
      if (DayWiseScheduleobj.EndTime < DayWiseScheduleobj.StartTime) {
        this.Alert.warning(this.intMessages.Validtimealert);
        return;
      }
    }
    if (
      DayWiseScheduleobj.EndTime.split(':').length !=
      DayWiseScheduleobj.StartTime.split(':').length
    ) {
      if (
        DayWiseScheduleobj.EndTime.split(':')[0] +
          ':' +
          DayWiseScheduleobj.EndTime.split(':')[1] ==
        DayWiseScheduleobj.StartTime.split(':')[0] +
          ':' +
          DayWiseScheduleobj.StartTime.split(':')[1]
      ) {
        this.Alert.warning(this.intMessages.Sametimealert);
        return;
      }
      if (
        DayWiseScheduleobj.EndTime.split(':')[0] +
          ':' +
          DayWiseScheduleobj.EndTime.split(':')[1] <
        DayWiseScheduleobj.StartTime.split(':')[0] +
          ':' +
          DayWiseScheduleobj.StartTime.split(':')[1]
      ) {
        this.Alert.warning(this.intMessages.Fromtimelesswarning);
        return;
      }
    }

    this.Daywisesaveloading = true;
    this.projectscheduleService
      .UpdateDayWiseConfig(DayWiseScheduleobj)
      .pipe(first())
      .subscribe({
        next: (data: any) => {
          if (data != null && data != undefined) {
            if (data == 'Invalid') {
              this.Alert.clear();
              this.Alert.error(this.intMessages.Invalidalert);
            }
            if (data == 'Validtimealert') {
              this.Alert.clear();
              this.Alert.error(this.intMessages.Validtimealert);
            }
            if (data == 'REMARKLEN') {
              this.Alert.clear();
              this.Alert.error(this.intMessages.RemarkLenEx);
            }
            if (data == 'U001') {
              this.Alert.clear();
              this.reloadComponent();
              this.Alert.success(this.intMessages.Daywiseconfigupdatealert);
            }
          }
        },
        error: (err: any) => {
          throw err;
        },
        complete: () => {
          this.Daywisesaveloading = false;
        },
      });
  }

  validateProjectSchedule() {
    var PList = this.ProjectScheduleList;
    var startdatetoval = new Date(
      PList.StartDate.getFullYear(),
      PList.StartDate.getMonth(),
      PList.StartDate.getDate()
    );
    var enddatetoval = new Date(
      this.serverdatetime.getFullYear(),
      this.serverdatetime.getMonth(),
      this.serverdatetime.getDate()
    );
    if (
      !PList.IsUpdate &&
      PList.StartDate != '' &&
      startdatetoval < enddatetoval
    ) {
      this.Alert.warning(this.intMessages.Futuredatewarning);
      this.Isvalid = true;
      return;
    }
    if (isNaN(PList.StartDate.getTime())) {
      this.Alert.warning(this.intMessages.Fromdatewarning);
      this.Isvalid = true;
      return;
    }
    if (isNaN(PList.ExpectedEndDate.getTime())) {
      this.Alert.warning(this.intMessages.Todatewarning);
      this.Isvalid = true;
      return;
    }
    if (PList.StartTime == '') {
      this.Alert.warning(this.intMessages.Fromtimelesswarning);
      this.Isvalid = true;
      return;
    }
    if (PList.EndTime == '' || PList.EndTime == '00:00') {
      this.Alert.warning(this.intMessages.Validtimealert);
      this.Isvalid = true;
      return;
    }

    if (PList.StartDate == '' && PList.ExpectedEndDate == '') {
      this.Alert.warning(this.intMessages.Emptydateswarning);
      this.Isvalid = true;
      return;
    }
    if (PList.StartDate == '') {
      this.Alert.warning(this.intMessages.Fromdatewarning);
      this.Isvalid = true;
      return;
    }

    if (PList.ExpectedEndDate == '') {
      this.Alert.warning(this.intMessages.Todatewarning);
      this.Isvalid = true;
      return;
    }
    if (PList.ExpectedEndDate <= PList.StartDate) {
      this.Alert.warning(this.intMessages.Fromdatelesswarning);
      this.Isvalid = true;
      return;
    }

    if (!PList.IsUpdate && startdatetoval < enddatetoval) {
      this.Alert.warning(this.intMessages.Fromdatewarning);
      this.Isvalid = true;
      return;
    }
    if (new Date(PList.ExpectedEndDate) < PList.StartDate) {
      this.Alert.warning(this.intMessages.Fromdatelesswarning);
      this.Isvalid = true;
      return;
    }

    if (PList.ExpectedEndDate < new Date(PList.StartDate).setHours(0, 0, 0)) {
      this.Alert.warning(this.intMessages.Fromdatelesswarning);
      this.Isvalid = true;
      return;
    }

    var ctime =
      this.serverdatetime.getHours() + ':' + this.serverdatetime.getMinutes();

    if (PList.EndTime.split(':').length == PList.StartTime.split(':').length) {
      if (PList.EndTime == PList.StartTime) {
        this.Alert.warning(this.intMessages.Sametimealert);
        this.Isvalid = true;
        return;
      }
      if (PList.EndTime < PList.StartTime) {
        this.Alert.warning(this.intMessages.Validtimealert);
        this.Isvalid = true;
        return;
      }
    }
    if (PList.EndTime.split(':').length != PList.StartTime.split(':').length) {
      if (
        PList.EndTime.split(':')[0] + ':' + PList.EndTime.split(':')[1] ==
        PList.StartTime.split(':')[0] + ':' + PList.StartTime.split(':')[1]
      ) {
        this.Alert.warning(this.intMessages.Sametimealert);
        this.Isvalid = true;
        return;
      }
      if (
        PList.EndTime.split(':')[0] + ':' + PList.EndTime.split(':')[1] <
        PList.StartTime.split(':')[0] + ':' + PList.StartTime.split(':')[1]
      ) {
        this.Alert.warning(this.intMessages.Fromtimelesswarning);
        this.Isvalid = true;
        return;
      }
    }

    if (
      PList.EndTime < ctime &&
      new Date(PList.ExpectedEndDate).setHours(0, 0, 0) ==
        this.serverdatetime.setHours(0, 0, 0, 0) &&
      new Date(PList.StartDate).setHours(0, 0, 0) ==
        this.serverdatetime.setHours(0, 0, 0, 0)
    ) {
      this.Alert.warning(this.intMessages.Endtimealert);
      this.Isvalid = true;
    }
  }

  dateClass() {
    return (date: Date): MatCalendarCellCssClasses => {
      var isWorkingDay = false;
      var calDate = new Date(
        new Date(date).getFullYear(),
        new Date(date).getMonth(),
        new Date(date).getDate()
      ); 
      this.ProjectScheduleList.ScheduleTimeModels.forEach((_data: any) => {
        var dataDate = new Date(
          new Date(_data.CalendarDate).getFullYear(),
          new Date(_data.CalendarDate).getMonth(),
          new Date(_data.CalendarDate).getDate()
        );
        if (calDate.toDateString() == dataDate.toDateString()) {
          isWorkingDay = true;
        }
      });
      var startDate = new Date(
        new Date(this.PSList.sdate).getFullYear(),
        new Date(this.PSList.sdate).getMonth(),
        new Date(this.PSList.sdate).getDate()
      );
      var endDate = new Date(
        new Date(this.PSList.edate).getFullYear(),
        new Date(this.PSList.edate).getMonth(),
        new Date(this.PSList.edate).getDate()
      );
      return isWorkingDay ||
        !(
          startDate.getTime() <= calDate.getTime() &&
          endDate.getTime() >= calDate.getTime()
        )
        ? ''
        : 'special-date';
    };
  }

  reloadComponent() {
    const currentUrl = this.router.url;
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([currentUrl]);
    });
  }
}
