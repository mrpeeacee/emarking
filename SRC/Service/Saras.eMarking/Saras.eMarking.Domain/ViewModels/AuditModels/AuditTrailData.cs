using Saras.eMarking.Domain.ViewModels.AuditModels;
using System;
using System.Text.Json;

namespace Saras.eMarking.Domain.ViewModels.AuditTrail
{

    public interface IAuditTrail
    {
    }

    [Serializable]
    public class AuditTrailData
    {
        public AuditTrailData()
        {
            SetActionDate(DateTime.UtcNow);
        }
        public AuditTrailEntity Entity { get; set; }
        public AuditTrailModule Module { get; set; }
        public AuditTrailEvent Event { get; set; }
        public long? UserId { get; set; }
        public long? ProjectUserRoleID { get; set; }
        public long? AssetRef { get; set; }
        private DateTime actionDate;
        public long EntityId { get; set; }
        public object Remarks { get; set; }
        private string Remark;
        public string ActionFromIP { get; set; }
        public string ActionFromBrowser { get; set; }
        public string SessionId { get; set; }
        public AuditTrailResponseStatus ResponseStatus { get; set; }
        public string Response { get; set; }


        private void SetActionDate(DateTime value)
        {
            actionDate = value;
        }
        public DateTime GetActionDate()
        {
            return actionDate;
        }
        public string GetRemarks()
        {
            if (Remarks != null)
            {
                Remark = JsonSerializer.Serialize(Remarks);
            }

            return Remark;
        }
        public void SetRemarks(object remark)
        {
            if (remark != null)
            {
                Remark = JsonSerializer.Serialize(remark);
            }
        }
    }

    public enum AuditTrailResponseStatus
    {
        Success = 1,
        Error = 2,
        LoggedIn =3,
        LoginFailed = 4,
        PasswordChanged = 5,
        LoggedOut = 6,
        IncompleteData = 7,
        InvalidUser = 8 ,
        QigSummaryCompleted= 9,
        QigSummaryfailed= 10,
        QigRCCompleted= 11,
        QigRCFailed= 12,
        QigAnnotationcompleted= 13,
        QigAnnotationfailed= 14,
        QigStandardisationCompleted=15,
        QigStandardisationFailed=16,
        LiveMarkingSettingCompleted=17,
        LiveMarkingSettingFailed=18,
        othersettingCompleted=19,
        othersettingfailed=20,
        keypersonalupdateCompleted=21,
        Keypersonalupdatefailed=22,
        Suspended=23,
       
    }

}
