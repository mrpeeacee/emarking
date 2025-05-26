using Saras.eMarking.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels
{
    public class TrialMarkedQigModel
    {
        public TrialMarkedQigModel()
        {
        }
        public long QigId { get; set; }
        public int NoOfRecommendedScripts { get; set; }
        public int NoOfTrialMarkedScripts { get; set; }
        public int NoOfCategorizedScripts { get; set; }
        public int StandardizationScriptsCount { get; set; }
        public int AdditionalStdScriptsCount { get; set; }
        public int BenchmarkScriptsCount { get; set; }
        public Boolean IsMarkSchemeIDMapped { get; set; }
        public List<TrialMarkedScriptModel> TrialMarkedScripts { get; set; }
        [XssTextValidation]
        public string MarkSchemeLevel { get; set; }
    }
}
