using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup;
using Saras.eMarking.Infrastructure.Project.QuestionProcessing;
using System.Xml.Linq;

namespace Saras.eMarking.Infrastructure.Report
{
    public class TestcentrerwiseesponseReportRepository : BaseRepository<TestcentrerwiseesponseReportRepository>, ITestcenterReportRepository
    {
        private readonly ApplicationDbContext context;
        public AppOptions AppOptions { get; set; }

        public TestcentrerwiseesponseReportRepository(ApplicationDbContext context, ILogger<TestcentrerwiseesponseReportRepository> _logger, AppOptions _appOptions) : base(_logger)
        {
            this.context = context;
            AppOptions = _appOptions;
        }

        public async Task<IList<ExamCenterModel>> ProjectCenters(long ProjectId)
        {
            List<ExamCenterModel> Centers = new List<ExamCenterModel>();
            try
            {
                Centers = await (from cen in context.ProjectCenters
                           where cen.ProjectId == ProjectId && !cen.IsDeleted
                           select new ExamCenterModel
                           {
                               ProjectCenterID = cen.ProjectCenterId,
                               ProjectID = cen.ProjectId,
                               CenterName = cen.CenterName,
                               CenterCode = cen.CenterCode,
                               TotalNoOfScripts = cen.TotalNoOfScripts,
                               CenterID = cen.CenterId
                           }).ToListAsync();

                

            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Testcentrewisecandiateresponse Repository page while getting Centers for specific: Method Name: ProjectCenters() and project: ProjectID=" + ProjectId.ToString());
                throw;
            }
            return Centers;

        }
        public static string FillIntheBlankQuestionText(string XML)
        {
            StringBuilder sb = new();
           
                foreach (XElement item in XDocument.Parse(XML).Descendants("presentation").Elements())
                {
                    if (Convert.ToString(item.Name).ToLower() == "material")
                    {
                        sb.Append(item.Element("mattext").Value);

                    }
                    
                }
            
            return sb.ToString();
        }
        public static string FillIntheBlankDNDQuestionText(string XML)
        {
            StringBuilder sb = new();
            
                foreach (XElement item in XDocument.Parse(XML).Descendants("presentation").Elements())
                {
                    if (Convert.ToString(item.Name).ToLower() == "material")
                    {
                        sb.Append(item.Element("mattext").Value);

                    }
                    
                }
           
            return sb.ToString();
        }
        public async Task<string> getquestiondetails(long questionid, long projectid)
        {
            var questiontext = "";
            var questionxml = await (context.ProjectQuestions.Where(k => k.QuestionId == questionid && k.ProjectId == projectid).Select(x => new { QuestionXml = x.QuestionXml, QuestionType = x.QuestionType, ProjectQuestionId = x.ProjectQuestionId })).FirstOrDefaultAsync();
            if (questionxml.QuestionType == 20)
            {
                questiontext = FillIntheBlankQuestionText(questionxml.QuestionXml);
                
            }
            else if (questionxml.QuestionType == 85)
            {
                questiontext = FillIntheBlankDNDQuestionText(questionxml.QuestionXml);
                
            }

            else
            {
                questiontext = XDocument.Parse(questionxml.QuestionXml).Descendants("presentation").Elements("material").Elements("mattext").FirstOrDefault()?.Value;
               

            }
            var questionhtmlstring = questiontext;

            try
            {
                var assetnames = context.ProjectQuestionAssets.Where(k => k.ProjectQuestionId == questionxml.ProjectQuestionId && k.AssetType == 1).Select(x => new { Assetnames = x.AssetName }).ToList();
               
                if (assetnames != null)
                {

                    for (int i = 0; i < assetnames.Count; i++)
                    {
                        questionhtmlstring = QuestionProcessingRepository.bindimageurltoxml(questionhtmlstring, assetnames[i].Assetnames, AppOptions);

                    }
                     
                }



            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Testcentrewisecandiateresponse Repository page while getting Centers for specific: Method Name: getquestiondetails() and project: questionid =" + questionid.ToString());
                throw;
            }
            return questionhtmlstring;

        }
    }
}
