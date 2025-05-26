using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Dashboard
{
    public class DashboardProjectModel
    {
        public DashboardProjectModel()
        {
        }
        public long ProjectID { get; set; }
        public long ProjectQIGID { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public byte? CreationType { get; set; }
        public byte ProjectStatus { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? UserIsActive { get; set; }
        public int? Year { get; set; }
        public string RoleCode {  get; set; }
        public ProjectUserroleModel ProjectUserRole { get; set; }
        public List<DashboardProjectQigModel> ProjectQigs { get; set; }
        public List<AllExamYear> ExamYears { get; set; }

    }

    public class DashboardProjectQigModel
    {
        public DateTime? QigCreatedDate { get; set; }
        public string QigName { get; set; }
        public string QigStatus { get; set; }
        public string QigCode { get; set; }
        public long ProjectQigid { get; set; }
        public long? ProjectId { get; set; }
    }
    public class AllExamYear
    {
        public int YearId { get; set; }
        public int Year { get; set; }
    }
    public class ProjectDetails
    {
        public long ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public byte? CreationType { get; set; }
        public byte ProjectStatus { get; set; }
        public bool? IsArchive { get; set; }
        public bool? UserIsActive { get; set; }
        public long UserId { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public long ProjectUserRoleId { get; set; }
        public long? ReportingTo { get; set; }
        public int ExamYear { get; set; }
        public DateTime ProjectCreatedDate { get; set; }
    }


    public class ProjectStatusDetails
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime curDateTime { get; set; }
        public byte DayType { get; set; }
        public string status { get; set; }
    }
}
