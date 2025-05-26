using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Saras.eMarking.Domain.ViewModels.Banding
{
    public class ScriptResponseModel
    {
        public ScriptResponseModel()
        {
        }

        public long ScriptId { get; set; }
        public long? ProjectQnsId { get; set; }
        public int? TotalNoOfQuestions { get; set; }
        [XssTextValidation]
        public string QuestionCode { get; set; }
        [XssTextValidation]
        public string QuestionText { get; set; }
        public int? QuestionOrder { get; set; }
        public QuestionResponseType ResponseType { get; set; }
        [XssTextValidation]
        public string ResponseText { get; set; }
        public BandModel RecommendedBand { get; set; }
        public List<BandModel> Bands { get; set; }
        [JsonIgnore]
        public long? RecommendedBandId { get; set; }
        public decimal? QuestionMarks { get; set; }
        public long? ProjectUserQuestionResponseID { get; set; }
        public bool IsQigLevel { get; set; }
        public bool IsMarkSchemeTagged { get; set; }
    }
}
