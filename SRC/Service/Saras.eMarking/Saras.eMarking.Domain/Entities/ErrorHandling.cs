using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ErrorHandling
{
    public int ErrorHandlingId { get; set; }

    public int ErrorNumber { get; set; }

    public string ErrorMessage { get; set; }

    public short ErrorSeverity { get; set; }

    public short ErrorState { get; set; }

    public string ErrorProcedure { get; set; }

    public int ErrorLine { get; set; }

    public string ErrorDetail { get; set; }

    public string UserName { get; set; }

    public string HostName { get; set; }

    public DateTime TimeStamp { get; set; }

    public bool? IsFixed { get; set; }

    public string DatabaseName { get; set; }
}
