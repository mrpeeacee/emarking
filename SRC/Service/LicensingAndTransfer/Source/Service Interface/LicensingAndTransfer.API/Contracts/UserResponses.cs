using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace LicensingAndTransfer.API.Contracts
{
    [MessageContract]
    public class UserResponses
    {
        [MessageBodyMember(Order = 0)]
        public long ScheduleUserID { get; set; }
        [MessageBodyMember(Order = 0)]
        public string QuestionGUID { get; set; }
        [MessageBodyMember(Order = 0)]
        public string UserResponse { get; set; }
        [MessageBodyMember(Order = 0)]
        public DateTime? CreatedDate { get; set; }
        [MessageBodyMember(Order = 0)]
        public bool IsCorrect { get; set; }
        [MessageBodyMember(Order = 0)]
        public bool IsEvaluated { get; set; }
        [MessageBodyMember(Order = 0)]
        public DateTime? TestStartDate { get; set; }
        [MessageBodyMember(Order = 0)]
        public DateTime? TestEndDate { get; set; }
        [MessageBodyMember(Order = 0)]
        public int Status { get; set; }
        [MessageBodyMember(Order = 0)]
        public Int32 NoOfAttempts { get; set; }
        [MessageBodyMember(Order = 0)]
        public bool SubmittedStatus { get; set; }
        [MessageBodyMember(Order = 0)]
        public bool ForcedSubmit { get; set; }
        [MessageBodyMember(Order = 0)]
        public Int32 TimeRemaining { get; set; }
        [MessageBodyMember(Order = 0)]
        public bool AllowUserToTakeTest { get; set; }
        [MessageBodyMember(Order = 0)]
        public string SectionTimeRemaining { get; set; }
        [MessageBodyMember(Order = 0)]
        public string CurrentSection { get; set; }
        [MessageBodyMember(Order = 0)]
        public string LastQuestionAttended { get; set; }
        [MessageBodyMember(Order = 0)]
        public string OptionalSectionSelected { get; set; }
        [MessageBodyMember(Order = 0)]
        public bool IsAllQuestionsViewed { get; set; }
        [MessageBodyMember(Order = 0)]
        public int ProctorSignal { get; set; }
        [MessageBodyMember(Order = 0)]
        public bool IsPhotoCaptured { get; set; }
        [MessageBodyMember(Order = 0)]
        public bool IsOnBreak { get; set; }
        [MessageBodyMember(Order = 0)]
        public string VisitedQuestions { get; set; }
        [MessageBodyMember(Order = 0)]
        public string LastChoiceAttended { get; set; }
        [MessageBodyMember(Order = 0)]
        public string UserTestChoiceInfo { get; set; }
    }

    [MessageContract]
    public class UserResponsesRequest
    {
        [MessageBodyMember(Order = 0)]
        public string MacID { get; set; }
    }

    [MessageContract]
    public class ProctorResponse
    {
        [MessageBodyMember(Order = 0)]
        public long ScheduleProctorID { get; set; }
        [MessageBodyMember(Order = 0)]
        public Int32 LastTrackPlayed { get; set; }
        [MessageBodyMember(Order = 0)]
        public long ScheduleID { get; set; }
        [MessageBodyMember(Order = 0)]
        public long ScheduleDetailID { get; set; }
        [MessageBodyMember(Order = 0)]
        public long ProctorID { get; set; }
        [MessageBodyMember(Order = 0)]
        public bool IsAssigned { get; set; }
        [MessageBodyMember(Order = 0)]
        public int ExamStatus { get; set; }
        [MessageBodyMember(Order = 0)]
        public DateTime? CreatedDate { get; set; }
    }
}