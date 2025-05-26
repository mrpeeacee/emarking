using Saras.eMarking.Domain.Extensions;
using System;

namespace Saras.eMarking.Domain.ViewModels
{
    public class TrialMarkedScriptModel
    {
        public TrialMarkedScriptModel()
        {
        }
        public long ScriptId { get; set; }
        [XssTextValidation]
        public string ScriptName { get; set; }
        public Nullable<Boolean> IsTrialMarked { get; set; }
        public Nullable<Boolean> IsCategorized { get; set; }
        public ScriptCategorizationPoolType CategoryType { get; set; }
        public int NoOfKpsTrialMarked { get; set; }
        public Boolean IsTrailMarkedByMe { get; set; }
        [XssTextValidation]
        public string BandName { get; set; }
        public int Script_status { get; set; }
        public string RoleCode { get; set; }
    }
}
