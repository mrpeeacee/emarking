using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Saras.eMarking.Infrastructure.Project.Common
{
    public class CommonRepository : BaseRepository<CommonRepository>
    {
        private readonly ApplicationDbContext context;
        public AppOptions appOptions { get; set; }
        public CommonRepository(ApplicationDbContext context, AppOptions _appOptions, ILogger<CommonRepository> _logger) : base(_logger)
        {
            this.context = context;
            appOptions = _appOptions;
        }
        public async Task<QigQuestionModel> GetQuestionText(long ProjectId, long QigId, long QuestionId)
        {
            QigQuestionModel question;
            try
            {
                question = await (from pqq in context.ProjectQigquestions
                                  join pq in context.ProjectQigs on pqq.Qigid equals pq.ProjectQigid
                                  join pqs in context.ProjectQuestions on pqq.ProjectQuestionId equals pqs.ProjectQuestionId
                                  where pq.ProjectId == ProjectId && pq.ProjectQigid == QigId && pqs.ProjectQuestionId == QuestionId && !pq.IsDeleted && !pqq.IsDeleted && !pqs.IsDeleted /*&& !pmst.IsDeleted && !pmsq.Isdeleted*/
                                  select new QigQuestionModel
                                  {
                                      QuestionCode = pqs.QuestionCode,
                                      QuestionType = pqs.QuestionType,
                                      MaxMark = pqs.QuestionMarks,
                                      QuestionText = pqs.QuestionText,
                                      QuestionXML = pqs.QuestionXml
                                  }).FirstOrDefaultAsync();
                if (question.QuestionType == 20)
                {
                    question.QuestionText = FillIntheBlankQuestionText(question.QuestionXML);
                }
                else
                {
                    question.QuestionText = XDocument.Parse(question.QuestionXML).Descendants("presentation").Elements("material").Elements("mattext").FirstOrDefault().Value;
                }
            }

            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in GetQuestionText->GetQuestionText() for specific Project and parameters are project: projectId = {ProjectId}");
                throw;
            }
            return question;
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
                else if (Convert.ToString(item.Name).ToLower() == "response_str")
                {
                    sb.Append(" " + "[" + XDocument.Parse(XML).Descendants("resprocessing").Elements("respcondition").Elements("conditionvar").Descendants("varequal").FirstOrDefault(a => a.Attribute("respident").Value == item.Attribute("ident").Value).Value + "]" + " ");
                }
            }
            return sb.ToString();
        }
    }
}
