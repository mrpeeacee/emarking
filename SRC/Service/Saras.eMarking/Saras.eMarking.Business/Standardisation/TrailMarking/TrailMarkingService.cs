using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Standardisation;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Standardisation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation;

namespace Saras.eMarking.Business.Standardisation
{
    public class TrailMarkingService : BaseService<TrailMarkingService>, ITrailMarkingService
    {
        readonly ITrailMarkingRepository _trialMarkinRepository;
        public TrailMarkingService(ITrailMarkingRepository trialMarkinRepository,
             ILogger<TrailMarkingService> _logger) : base(_logger)
        {
            _trialMarkinRepository = trialMarkinRepository;
        }

        public async Task<bool> ResponseMarking(List<QuestionUserResponseMarkingDetailModel> markingResponseDetails, long projectid, long ProjectUserRoleID, long qigid, bool IsAutoSave)
        {
            logger.LogInformation("Trial Marking Service >> ResponseMarking() started");
            try
            {
                return await _trialMarkinRepository.ResponseMarking(markingResponseDetails, projectid, ProjectUserRoleID, qigid, IsAutoSave);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while Creating QuestionUserResponseMarking: Method Name: CreateQuestionUserResponseMarking()");
                throw;
            }
        }
        public async Task<RecQuestionModel> GetScriptQuestionResponse(long ProjectId, long ScriptId, long ProjectQuestionId, bool IsDefault)
        {

            logger.LogInformation("Trial Marking Service >> ResponseMarking() started");
            try
            {
                RecQuestionModel questionresp = await _trialMarkinRepository.GetScriptQuestionResponse(ProjectId, ScriptId, ProjectQuestionId, IsDefault);


                return questionresp;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while  GetScriptQuestionResponse: Method Name: GetScriptQuestionResponse()");
                throw;
            }

        }
        public async Task<IList<UserScriptResponseModel>> UserScriptMarking(UserScriptMarkingDetails UsersScriptMarkingDetails)
        {
            logger.LogInformation("Trial Marking Service >> UserScriptMarking() started");
            try
            {
                return await _trialMarkinRepository.UserScriptMarking(UsersScriptMarkingDetails);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while Creating UserScriptMarkingdetails: Method Name: UserScriptMarking()");
                throw;
            }
        }
        public async Task<QuestionUserResponseMarkingDetail> ResponseMarkingDetails(long Scriptid, long ProjectQuestionResponseID, Nullable<long> ProjectUserRoleID, long? workflowstatusid, long? UserScriptMarkingRefId)
        {
            logger.LogInformation("Trial Marking Service >> CreateQuestionUserResponseMarking() started");
            try
            {
                return await _trialMarkinRepository.ResponseMarkingDetails(Scriptid, ProjectQuestionResponseID, ProjectUserRoleID, workflowstatusid, UserScriptMarkingRefId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while getting ResponseMarkingDetails: Method Name: ResponseMarkingDetails()");
                throw;
            }

        }
        public async Task<CatagarizedandS1Completedmodel> Getcatagarizedands1configureddetails(long? qigid, long? scriptid, long? workflowid)
        {
            logger.LogInformation("Trial Marking Service >> Getcatagarizedands1configureddetails() started");
            try
            {
                return await _trialMarkinRepository.Getcatagarizedands1configureddetails(qigid, scriptid, workflowid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while Getting catagarized and s1configured details: Method Name: Getcatagarizedands1configureddetails()");
                throw;
            }
        }


        public async Task<List<Trialmarkingannotationmodel>> Getannoatationdetails(long qigid)
        {
            logger.LogInformation("Trial Marking Service >> Getannoatationdetails() started");
            try
            {
                return await _trialMarkinRepository.Getannoatationdetails(qigid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while Getting annotationdetails: Method Name: Getannoatationdetails()");
                throw;
            }
        }
        public async Task<bool> MarkingScriptTimeTracking(MarkingScriptTimeTrackingModel MarkingScriptTimeTracking)
        {
            logger.LogInformation("Trial Marking Service >> MarkingScriptTimeTracking() started");
            try
            {
                return await _trialMarkinRepository.MarkingScriptTimeTracking(MarkingScriptTimeTracking);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while submiting marking: Method Name: MarkingScriptTimeTracking()");
                throw;
            }
        }

        public async Task<ValidateAnnotationsanddescreteModel> validateannotation(long projectid, long qigid, byte EntityType, string ProjectUserRoleCode, long ProjectUserRoleID)
        {
            logger.LogInformation("Trial Marking Service >> Markingsubmit() started");
            try
            {
                return await _trialMarkinRepository.validateannotation(projectid, qigid, EntityType, ProjectUserRoleCode, ProjectUserRoleID);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while submiting marking: Method Name: MarkingSubmit()");
                throw;
            }
        }

        public async Task<bool> MarkingSubmit(long scriptid, long projectid, long ProjectUserRoleID, long? workflowstatusid)
        {
            logger.LogInformation("Trial Marking Service >> Markingsubmit() started");
            try
            {
                return await _trialMarkinRepository.MarkingSubmit(scriptid, projectid, ProjectUserRoleID, workflowstatusid);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while submiting marking: Method Name: MarkingSubmit()");
                throw;
            }
        }

        public async Task<DownloadMarkschemeModel> Viewanddownloadmarkscheme(long projectquestionId, long projectId, long? markschemeid = null)
        {
            logger.LogInformation("Trial Marking Service >> Markingsubmit() started");
            try
            {
                DownloadMarkschemeModel downloadMarkschemeModel = await _trialMarkinRepository.Viewanddownloadmarkscheme(projectquestionId, projectId, markschemeid);
                //Html Decode Band Descriptions
                if (downloadMarkschemeModel != null && downloadMarkschemeModel.MarkSchemes != null && downloadMarkschemeModel.MarkSchemes.Count > 0)
                {
                    downloadMarkschemeModel.MarkSchemes.ForEach(scheme =>
                    {
                        if (scheme != null && scheme.Bands != null && scheme.Bands.Count > 0)
                        {
                            scheme.Bands.ForEach(bn =>
                            {
                                bn.BandDescription = HtmlDecode(bn.BandDescription);
                            });
                        }
                    });
                }

                return downloadMarkschemeModel;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in Trial Marking page while View and Downloading the Marks Scheme.: Method Name: MarkingSubmit()");
                throw;
            }
        }
        public async Task<List<ViewScriptModel>> ViewScript(long projectID, long projectUserRoleID, ViewScriptModel objView, UserTimeZone userTimeZone)
        {

            try
            {
                logger.LogInformation("Trial Marking Service >> ViewScript() started");
                return await _trialMarkinRepository.ViewScript(projectID, projectUserRoleID, objView, userTimeZone);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Trial Marking page while getting script: Method Name: ViewScript()");
                throw;
            }
        }



    }
}