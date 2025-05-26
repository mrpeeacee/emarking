using System;

namespace Saras.eMarking.Domain.ViewModels
{
    public class MarkingScriptTimeTrackingModel
    {
        public MarkingScriptTimeTrackingModel()
        {
        }

      
        public long ProjectId { get; set; }
        public long Qigid { get; set; }
        public long ProjectQuestionId { get; set; }
        public long UserScriptMarkingRefId { get; set; }
        public int WorkFlowStatusId { get; set; }
        public long ProjectUserRoleId { get; set; }
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 1 --&gt; View , 2 --&gt; Edit
        /// </summary>
        public short? Mode { get; set; }
        /// <summary>
        /// 1 --&gt; Submit,2 --&gt; Save, 3 --&gt; Cancel , 4 --&gt; Close , 5 --&gt; Navigate
        /// </summary>
        public short? Action { get; set; }
        public string TimeTaken { get; set; }
    }
}
