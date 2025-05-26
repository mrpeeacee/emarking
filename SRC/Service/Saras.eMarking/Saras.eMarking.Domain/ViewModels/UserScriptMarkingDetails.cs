using System;


namespace Saras.eMarking.Domain.ViewModels
{
  public class UserScriptMarkingDetails
    {
        public UserScriptMarkingDetails()
        {
        }
        
      
        public long ScriptID { get; set; }
        public long ProjectId { get; set; }
        public Nullable<long> CandidateId { get; set; }
        public Nullable<long> ScheduleUserId { get; set; }
        public Nullable<byte> TotalNoOfQuestions { get; set; }
        public Nullable<byte> MarkedQuestions { get; set; }
        public Nullable<byte> ScriptMarkingStatus { get; set; }
        public Nullable<int> WorkFlowStatusID { get; set; }
        public Nullable<long> MarkedBy { get; set; }
        public bool? scriptstatus { get; set; }
        public bool? IsViewMode { get; set; }
        public long? UserScriptMarkingRefId { get; set; }
}



}
