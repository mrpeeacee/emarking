using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.MarkScheme;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Business.Project.MarkScheme
{
    public class MarkSchemesService : BaseService<MarkSchemesService>, IMarkSchemeService
    {
        readonly IMarkSchemeRepository _markSchemeRepository;
        public MarkSchemesService(IMarkSchemeRepository markSchemeRepository, ILogger<MarkSchemesService> _logger) : base(_logger)
        {
            _markSchemeRepository = markSchemeRepository;
        }

        public async Task<IList<MarkSchemeModel>> GetAllMarkScheme(long projectId)
        {
            logger.LogDebug($"MarkSchemeService GetAllMarkScheme() method started. ProjectId = {projectId}");

            IList<MarkSchemeModel> schemeResp = await _markSchemeRepository.GetAllMarkScheme(projectId);

            logger.LogDebug($"MarkSchemeService GetAllMarkScheme() method completed. ProjectId = {projectId}");
            return schemeResp;
        }
        public async Task<MarkSchemeModel> MarkSchemeWithId(long projectId, long schemeId)
        {
            logger.LogDebug($"MarkSchemeService MarkSchemeWithId() method started. projectId {projectId} and schemeId = {schemeId}");

            MarkSchemeModel schemeResp = await _markSchemeRepository.MarkSchemeWithId(projectId, schemeId);
            if (schemeResp != null && schemeResp.Bands != null && schemeResp.Bands.Count > 0)
            {
                schemeResp.Bands.ForEach(bn =>
                {
                    bn.BandDescription = HtmlDecode(bn.BandDescription);
                });
            }

            logger.LogDebug($"MarkSchemeService MarkSchemeWithId() method completed. projectId {projectId} and schemeId = {schemeId}");
            return schemeResp;
        }
        public async Task<string> CreateMarkScheme(MarkSchemeModel markScheme, long projectId, long ProjectUserRoleID)
        {
            logger.LogDebug($"MarkSchemeService CreateMarkScheme() method started. List of {markScheme} and projectId {projectId}, userId {ProjectUserRoleID}");
            string status = "";
            if (markScheme != null && markScheme.Bands != null)
            {
                if (Validation(markScheme))
                {
                    if (markScheme.Bands.Count > 0)
                    {
                        markScheme.Bands.ForEach(bn =>
                        {
                            bn.BandDescription = HtmlEncode(bn.BandDescription);
                        });
                    }

                    status = await _markSchemeRepository.CreateMarkScheme(markScheme, projectId, ProjectUserRoleID);
                }
                else
                {
                    status = "VALMISS";
                }
            }
            else
            {
                status = "EMPTY";
            }
            logger.LogDebug($"MarkSchemeService CreateMarkScheme() method completed. List of {markScheme} and projectId {projectId},userId {ProjectUserRoleID}");

            return status;
        }
        public static bool Validation(MarkSchemeModel markScheme)
        {
            int j = 0;
            bool status = true;

            if (markScheme.Marks == 0 || markScheme.SchemeName == "")
            {
                status = false;
            }

            if (markScheme.SchemeName.Length >= 25)
            {
                status = false;
            }

            if (markScheme.IsBandExist)
            {
                status = ValidateMarkschmeBand(markScheme, j);
            }


            return status;
        }

        private static bool ValidateMarkschmeBand(MarkSchemeModel markScheme, int j)
        {
            bool status = markSchemeBands(markScheme);
            for (var i = 0; i < markScheme.Bands.Count; i++)
            {
                if (i > 0)
                {
                    j = i - 1;
                }
                if (markScheme.Bands[i].BandFrom > markScheme.Bands[i].BandTo)
                {
                    status = false;
                }
                else if (i == 0 && markScheme.Marks != markScheme.Bands[i].BandTo)
                {
                    status = false;
                }
                else if (markScheme.Bands[i].BandTo >= markScheme.Bands[j].BandFrom && i > 0)
                {
                    status = false;
                }
                if (markScheme.Bands[i].BandName.Length >= 25)
                {
                    status = false;
                }

            }
            if (markScheme.Bands.Select(a => a.BandName).Count() != markScheme.Bands.Select(a => a.BandName).Distinct().Count())
            {
                status = false;
            }

            return status;
        }

        private static bool markSchemeBands(MarkSchemeModel markScheme)
        {
            bool status = true;
            foreach (var band in markScheme.Bands)
            {
                if (band.BandTo < 0 && band.BandFrom < 0)
                {
                    status = false;
                }
                else if (band.BandFrom == band.BandTo)
                {
                    //if (band.BandFrom != 0 && band.BandTo != 0)
                    //{
                    //    status = false;
                    //}
                    //else
                    //{
                        status = true;
                   // }
                }
                else if (IsNullOrWhiteSpace(band.BandName))
                {
                    status = false;
                }
                else if (band.BandFrom.Equals(null))
                {
                    status = false;
                }
                else if (band.BandTo.Equals(null))
                {
                    status = false;
                }
                else if (markScheme.Marks < band.BandTo)
                {
                    status = false;
                }
                else if (band.BandFrom > band.BandTo)
                {
                    status = false;
                }
            }
            return status;
        }
        private static bool IsNullOrWhiteSpace(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return true;
            }

            return String.IsNullOrEmpty(value.Trim());
        }
        public async Task<string> UpdateMarkScheme(MarkSchemeModel markScheme, long projectId, long schemeId, long ProjectUserRoleID)
        {
            logger.LogDebug($"MarkSchemeService UpdateMarkScheme() method started. List of {markScheme} and projectId {projectId} and scriptId {schemeId} and Userid {ProjectUserRoleID}");
            string status;
            if (markScheme != null && markScheme.Bands != null)
            {
                if (Validation(markScheme))
                {
                    if (markScheme.Bands.Count > 0)
                    {
                        markScheme.Bands.ForEach(bn =>
                        {
                            bn.BandDescription = HtmlEncode(bn.BandDescription);
                        });
                    }

                    status = await _markSchemeRepository.UpdateMarkScheme(markScheme, projectId, schemeId, ProjectUserRoleID);
                }
                else
                {
                    status = "VALMISS";
                }
            }
            else
            {
                status = "EMPTY";
            }

            logger.LogDebug($"MarkSchemeService UpdateMarkScheme() method completed. List of {markScheme} and projectId {projectId}, scriptId {schemeId}, Userid {ProjectUserRoleID}");

            return status;
        }
        public async Task<IList<ProjectTaggedQuestionModel>> GetAllQuestions(long projectId, long schmeId, decimal maxMark)
        {
            logger.LogDebug($"MarkSchemeService GetAllQuestions() method started. ProjectId = {projectId} and SchemeId = {schmeId} and Question Max Mark = {maxMark}");

            IList<ProjectTaggedQuestionModel> questResp = await _markSchemeRepository.GetAllQuestions(projectId, schmeId, maxMark);

            logger.LogDebug($"MarkSchemeService GetAllQuestions() method completed. ProjectId = {projectId} and SchemeId = {schmeId} and Question Max Mark = {maxMark}");
            return questResp;
        }
        public async Task<ProjectQuestionsModel> GetQuestionText(long projectId, long questionId)
        {
            logger.LogDebug($"MarkSchemeService GetQuestionText() method started. projectId = {projectId} and questionId = {questionId}");

            ProjectQuestionsModel questText = await _markSchemeRepository.GetQuestionText(projectId, questionId);

            logger.LogDebug($"MarkSchemeService GetQuestionText() method completed. projectId = {projectId} and questionId = {questionId}");
            return questText;
        }
        public async Task<string> MarkSchemeMapping(long projectId, List<ProjectTaggedQuestionModel> questionList, long ProjectUserRoleID)
        {
            logger.LogDebug($"MarkSchemeService MarkSchemeMapping() method started. projectId {projectId} and questionList {questionList} and Userid {ProjectUserRoleID}");
            string status = string.Empty;
            if (questionList != null && questionList.Count > 0)
            {
                status = await _markSchemeRepository.MarkSchemeMapping(projectId, questionList, ProjectUserRoleID);
            }
            else
            {
                status = "MANDFD";
            }

            logger.LogDebug($"MarkSchemeService MarkSchemeMapping() method completed. projectId {projectId} and questionList {questionList} and Userid {ProjectUserRoleID}");

            return status;
        }
        public async Task<string> DeleteMarkScheme(long projectId, List<long> markSchemeids, long ProjectUserRoleID)
        {
            logger.LogDebug($"MarkSchemeService > DeleteMarkScheme() started. projectId {projectId} and List Of markSchemeids {markSchemeids} and Userid {ProjectUserRoleID}");
            string status = string.Empty;
            if (markSchemeids != null && markSchemeids.Count > 0)
            {
                status = await _markSchemeRepository.DeleteMarkScheme(projectId, markSchemeids, ProjectUserRoleID);
            }
            else
            {
                status = "MANDFD";
            }
            logger.LogDebug($"MarkSchemeService > DeleteMarkScheme() started. projectId {projectId} and List Of markSchemeids {markSchemeids} and Userid {ProjectUserRoleID}");

            return status;
        }

        public async Task<PaginationModel<ProjectTaggedQuestionsModel>> GetQuestionsUnderProject(long projectId, int? pagenumber)
        {
            logger.LogDebug($"MarkSchemeService GetQuestionsUnderProject() method started. ProjectId = {projectId} and Page Number = {pagenumber}");

            IList<ProjectTaggedQuestionsModel> questResp = await _markSchemeRepository.GetQuestionsUnderProject(projectId, pagenumber);

            int pagesize = 5;

            var result = new PaginationModel<ProjectTaggedQuestionsModel>
            {
                Count = questResp.Count,
                PageIndex = pagenumber ?? 1,
                PageSize = pagesize,
                Items = questResp.Skip((pagenumber - 1 ?? 0) * pagesize).Take(pagesize).ToList()
            };

            logger.LogDebug($"MarkSchemeService GetQuestionsUnderProject() method completed. ProjectId = {projectId} and Page Number = {pagenumber}");
            return result;
        }
    }
}
