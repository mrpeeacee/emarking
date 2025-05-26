using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace LicensingAndTransfer.DataContracts
{
    [DataContract(Namespace = "http://LicensingAndTransfer.DataContracts/2010/01", Name = "OMRScannedResponseOuput")]
    public class OMRScannedResponseOuput
    {
        private System.Int32 TotalQuestionsField;
        [DataMember(IsRequired = true, Name = "TotalQuestions", Order = 0)]
        public System.Int32 TotalQuestions
        {
            get
            {
                return TotalQuestionsField;
            }

            set
            {
                TotalQuestionsField = value;
            }
        }

        private System.Int32 AttemptedField;
        [DataMember(IsRequired = true, Name = "Attempted", Order = 1)]
        public System.Int32 Attempted
        {
            get
            {
                return AttemptedField;
            }

            set
            {
                AttemptedField = value;
            }
        }

        private System.Int32 NotAttemptedField;
        [DataMember(IsRequired = true, Name = "NotAttempted", Order = 2)]
        public System.Int32 NotAttempted
        {
            get
            {
                return NotAttemptedField;
            }

            set
            {
                NotAttemptedField = value;
            }
        }

        private System.Double TotalScoreField;
        [DataMember(IsRequired = true, Name = "TotalScore", Order = 3)]
        public System.Double TotalScore
        {
            get
            {
                return TotalScoreField;
            }

            set
            {
                TotalScoreField = value;
            }
        }

        private System.Double ActualScoreField;
        [DataMember(IsRequired = true, Name = "ActualScore", Order = 4)]
        public System.Double ActualScore
        {
            get
            {
                return ActualScoreField;
            }

            set
            {
                ActualScoreField = value;
            }
        }

        private System.Int32 CorrectField;
        [DataMember(IsRequired = true, Name = "Correct", Order = 5)]
        public System.Int32 Correct
        {
            get
            {
                return CorrectField;
            }

            set
            {
                CorrectField = value;
            }
        }

        private System.Int32 NotCorrectField;
        [DataMember(IsRequired = true, Name = "NotCorrect", Order = 6)]
        public System.Int32 NotCorrect
        {
            get
            {
                return NotCorrectField;
            }

            set
            {
                NotCorrectField = value;
            }
        }

        private System.String StatusField;
        [DataMember(IsRequired =true,Name="Status", Order = 7)]
        public System.String Status
        {
            get
            {
                return StatusField;
            }

            set
            {
                StatusField = value;
            }
        }


       

        


    }
}
