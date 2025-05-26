namespace Saras.eMarking.Domain.ViewModels.Project.MarkScheme
{
    public class ProjectQuestionsModel
    {
        public ProjectQuestionsModel()
        {

        }
        public long QuestionId { get; set; }
        public string QuestionCode { get; set; }
        public decimal? maxMark { get; set; }
        public string SchemeName { get; set; }
        public decimal Marks { get; set; }
        public long? MarkSchemeId { get; set; }
        public long ProjectMarkSchemeId { get; set; }
        public string QuestionText { get; set; }
        public long ProjectID { get; set; }
        public bool? TaggedQuestion { get; set; }
    }
}
