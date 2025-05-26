using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Report
{
    /// <summary>
    /// The audit report request model.
    /// </summary>
    public class AuditReportRequestModel
    {
        /// <summary>
        /// Gets or Sets the login id.
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// Gets or Sets the start date.
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or Sets the end date.
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// Gets or Sets the module codes.
        /// </summary>
        public string ModuleCodes { get; set; }

        /// <summary>
        /// Gets or Sets the page no.
        /// </summary>
        public int PageNo { get; set; }

        /// <summary>
        /// Gets or Sets the page size.
        /// </summary>
        public int PageSize { get; set; }
    }

    /// <summary>
    /// The audit report model.
    /// </summary>
    public class AuditReportModel
    {
        /// <summary>
        /// Gets or Sets the sl no.
        /// </summary>
        public int SlNo { get; set; }

        /// <summary>
        /// Gets or Sets the entity id.
        /// </summary>
        public long? EntityId { get; set; }

        /// <summary>
        /// Gets or Sets the event id.
        /// </summary>
        public long? EventId { get; set; }

        /// <summary>
        /// Gets or Sets the login id.
        /// </summary>
        public string LoginId { get; set; }

        /// <summary>
        /// Gets or Sets the first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or Sets the remarks.
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// Gets or Sets the status.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or Sets the event date time.
        /// </summary>
        public DateTime? EventDateTime { get; set; }

        /// <summary>
        /// Gets or Sets the entity code.
        /// </summary>
        public string EntityCode { get; set; }

        /// <summary>
        /// Gets or Sets the module code.
        /// </summary>
        public string ModuleCode { get; set; }

        /// <summary>
        /// Gets or Sets the entity description.
        /// </summary>
        public string EntityDescription { get; set; }

        /// <summary>
        /// Gets or Sets the event code.
        /// </summary>
        public string EventCode { get; set; }

        /// <summary>
        /// Gets or Sets the event description.
        /// </summary>
        public string EventDescription { get; set; }

        /// <summary>
        /// Gets or Sets the IP address.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or Sets the session id.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or Sets the project name.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or Sets the total rows.
        /// </summary>
        public int TotalRows { get; set; }
    }

    /// <summary>
    /// The audit report data model.
    /// </summary>
    public class AuditReportDataModel
    {
        /// <summary>
        /// Gets or Sets the sl no.
        /// </summary>
        public int? SlNo { get; set; }

        /// <summary>
        /// Gets or Sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or Sets the log in date time.
        /// </summary>
        public DateTime? LogInDateTime { get; set; }

        /// <summary>
        /// Gets or Sets the log out date time.
        /// </summary>
        public DateTime? LogOutDateTime { get; set; }

        /// <summary>
        /// Gets or Sets the IP address.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or Sets the duration.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Gets or Sets the function performed.
        /// </summary>
        public List<string> FunctionPerformed { get; set; }

        /// <summary>
        /// Gets or Sets the total rows.
        /// </summary>
        public int? TotalRows { get; set; }
    }

    /// <summary>
    /// The application module model.
    /// </summary>
    public class ApplicationModuleModel
    {
        /// <summary>
        /// Gets or Sets the module code.
        /// </summary>
        public string ModuleCode { get; set; }

        /// <summary>
        /// Gets or Sets the module name.
        /// </summary>
        public string ModuleName { get; set; }
    }

    /// <summary>
    /// The audit template model.
    /// </summary>
    public class AuditTemplateModel
    {
        /// <summary>
        /// Gets or Sets the key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 1 = String Type, 2 = Array type. 3 = Array with name
        /// </summary>
        public int TemplateType { get; set; }

        /// <summary>
        /// JObject Name
        /// </summary>
        public string JObjectName { get; set; }
    }
}