using Microsoft.Extensions.DependencyInjection;
using Saras.eMarking.IOC.Standardisation;
using Saras.eMarking.IOC.Project.Setup;
using Saras.eMarking.IOC.Project.MarkScheme;
using Saras.eMarking.IOC.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.IOC.Project.Standardisation.StdOne.StdTwoThreeConfig;
using Saras.eMarking.IOC.Project.Standardisation;
using Saras.eMarking.IOC.Project.Setup.QigConfiguration;
using Saras.eMarking.IOC.Project.Standardisation.StdOne.TrialMarking;
using Saras.eMarking.IOC.Project.Standardisation.StdOne.Categorisation;
using Saras.eMarking.IOC.Project.Privilege;
using Saras.eMarking.IOC.Project.Standardisation.StdOne.Recommendation;
using Saras.eMarking.IOC.Project.Dashboards;
using Saras.eMarking.IOC.Project.LiveMarking;
using Saras.eMarking.IOC.Project.Setup.ProjectUsers;
using Saras.eMarking.IOC.Project.QualityChecks;
using Saras.eMarking.IOC.Project.ResponseProcessing.SemiAutomaticQuestions;
using Saras.eMarking.IOC.Project.ResponseProcessing.AutomaticQuestions;
using Saras.eMarking.IOC.Report;
using Saras.eMarking.IOC.Project.Setup.QigManagement;
using Saras.eMarking.IOC.Global;
using Saras.eMarking.IOC.Project.ScoringComponentLibrary;

namespace Saras.eMarking.IOC
{
    public static class DependencyContainer
    {
        public static void RegisterService(IServiceCollection serviceCollection)
        {
            AppCache.RegisterService(serviceCollection);
            Audit.RegisterService(serviceCollection);
            TokenUser.RegisterService(serviceCollection);
            Qig.RegisterService(serviceCollection);
            TrailMarking.RegisterService(serviceCollection);
            QigConfig.RegisterService(serviceCollection);

            BasicDetails.RegisterService(serviceCollection);
            Dashboard.Dashboard.RegisterService(serviceCollection);
            Privilege.RegisterService(serviceCollection);
            ProjectSchedule.RegisterService(serviceCollection);
            ResolutionOfCoi.RegisterService(serviceCollection);
            AnnotationSetting.RegisterService(serviceCollection);
            QigSetting.RegisterService(serviceCollection);
            RcSetting.RegisterService(serviceCollection);
            StdSetting.RegisterService(serviceCollection);
            ProjectLeveConfig.RegisterService(serviceCollection);
            MarkScheme.RegisterService(serviceCollection);
            ExamCenter.RegisterService(serviceCollection);
            KeyPersonnel.RegisterService(serviceCollection);
            StdRecSetting.RegisterService(serviceCollection);
            Recommendation.RegisterService(serviceCollection);
            RecPool.RegisterService(serviceCollection);
            TrialMarkingPool.RegisterService(serviceCollection);
            Categorisation.RegisterService(serviceCollection);
            ProjectUsers.RegisterService(serviceCollection);
            StdAssessment.RegisterService(serviceCollection);
            StdTwoStdThreeConfig.RegisterService(serviceCollection);
            PracticeAssessment.RegisterService(serviceCollection);
            QualifyingAssessment.RegisterService(serviceCollection);
            MarkingOverviews.RegisterService(serviceCollection);
            LiveMarking.RegisterService(serviceCollection);
            Media.RegisterService(serviceCollection);
            QualityChecks.RegisterService(serviceCollection);
            FrequencyDistributions.RegisterService(serviceCollection);
            QigSummery.RegisterService(serviceCollection);
            S2S3Approvals.RegisterService(serviceCollection);
            AutomaticQuestions.RegisterService(serviceCollection);
            QigManagement.RegisterService(serviceCollection);
            EmsReport.RegisterService(serviceCollection);
            AuditReport.RegisterService(serviceCollection);
            ProjectClosure.RegisterService(serviceCollection);
            TestcentreResponse.RegisterService(serviceCollection);
            #region Reports
            MarkersPerformance.RegisterService(serviceCollection);
            StudentsResult.RegisterService(serviceCollection);
            ViewSolution.RegisterService(serviceCollection);
            #endregion
            GlobalUserManagement.RegisterService(serviceCollection);
            File.File.RegisterService(serviceCollection);
            AdminTools.AdminTools.RegisterService(serviceCollection);

            ScoringComponentLibrary.RegisterService(serviceCollection);


        }
    }
}
