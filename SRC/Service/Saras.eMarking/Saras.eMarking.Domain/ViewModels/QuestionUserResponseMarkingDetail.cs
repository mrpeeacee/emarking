using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels
{
    public class QuestionUserResponseMarkingDetailModel
    {
        public QuestionUserResponseMarkingDetailModel()
        {
        }
        public long ScriptID { get; set; }
        public long ProjectQuestionResponseID { get; set; }
        public Nullable<long> CandidateID { get; set; }
        public Nullable<long> ScheduleUserID { get; set; }
    
        public string Annotation { get; set; }
        [XssTextValidation]
        public string ImageBase64 { get; set; }
        [XssTextValidation]
        public string Comments { get; set; }
        public Nullable<long> BandID { get; set; }
        public Nullable<decimal> Marks { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<long> MarkedBy { get; set; }
        public Nullable<System.DateTime> Markeddate { get; set; }
        public Nullable<byte> MarkingStatus { get; set; }
        public Nullable<int> WorkflowstatusID { get; set; }
        public bool LastVisited { get; set; }

        public byte? Markedtype { get; set; }
        public string Remarks { get; set; }
        public decimal Timer { get; set; }
        public Nullable<long> TotalMarks { get; set; }
        public List<QuestionScoreComponentMarkingDetail> ScoreComponentMarkingDetail { get; set; }
    }
}
