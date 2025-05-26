namespace Saras.eMarking.Domain.ViewModels.Categorisation
{
    public class CategorisationStasticsModel
    {
        public int TrialMarkedScript { get; set; }
        public int CategorisedScript { get; set; }
        public int StandardisedScript { get; set; }
        public int AdlStandardisedScript { get; set; }
        public int BenchMarkScript { get; set; }

        public int? QigStandardisedScript { get; set; }
        public int? QigAdlStandardisedScript { get; set; }
        public int? QigBenchMarkScript { get; set; }
        public int RecommendationPoolCount { get; set; }
        public int RecommendedCount { get; set; }
    }
}
