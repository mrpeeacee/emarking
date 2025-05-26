export class ProjectScheduleModel {
    StartDate!: any;
    ExpectedEndDate!: any;
    StartTime!: string;
    EndTime!: string;
    WorkingDaysConfig!: any;
    CalendarDate!: string;
    DayType!: number;
    SelectedDate!: string;
    Remarks!: string;
    WorkingDaysConfigJson!: JSON;
    scheduleTimeModels!: ProjectScheduleCalendarModel[];
    Confirmdialogeventvalue!: number;
    IsUpdate!: boolean;
}

export class ProjectScheduleCalendarModel {
    CalendarDate!: string;
    DayType!: number;
    StartTime!: string;
    EndTime!: string;
}

export class DayWiseScheduleModel {
    SelectedDate!: Date;
    StartTime!: string;
    EndTime!: string;
    DayType!: number;
    Remarks!: string;
    ProjectCalendarID!: number;
    StartDateTime!: Date;
    EndDateTime!: Date;
}