using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.MarkScheme;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Banding;
using Saras.eMarking.Domain.ViewModels.File;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.MarkScheme
{
    public class MarkSchemeRepository : BaseRepository<MarkSchemeRepository>, IMarkSchemeRepository
    {
        private readonly ApplicationDbContext context;
        public MarkSchemeRepository(ApplicationDbContext context, ILogger<MarkSchemeRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        /// <summary>
        /// To get all MarkScheme Details
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<IList<MarkSchemeModel>> GetAllMarkScheme(long projectId)
        {
            List<MarkSchemeModel> markSchemes = null;
            try
            {
                logger.LogDebug($"MarkSchemeRepository  GetAllMarkScheme() Method started.  projectId = {projectId}");

                markSchemes = (await (from PB in context.ProjectMarkSchemeTemplates
                                      where PB.ProjectId == projectId && !PB.IsDeleted
                                      select new MarkSchemeModel
                                      {
                                          SchemeCode = PB.SchemeCode,
                                          SchemeName = PB.SchemeName,
                                          Marks = PB.Marks,
                                          ProjectMarkSchemeId = PB.ProjectMarkSchemeId,
                                          MarkScheme = (int)PB.MarkingSchemeType
                                      }).ToListAsync()).ToList();

                var markSchemeQns = await context.ProjectMarkSchemeQuestions.Where(sm => sm.ProjectId == projectId && !sm.Isdeleted).ToListAsync();

                markSchemes.ForEach(scms =>
                {
                    scms.CountOfTaggedQuestions = markSchemeQns.Count(a => a.ProjectMarkSchemeId == scms.ProjectMarkSchemeId);
                });

                markSchemes = markSchemes.OrderBy(a => a.SchemeName).ToList();

                logger.LogDebug($"MarkSchemeRepository  GetAllMarkScheme() Method completed.  projectId = {projectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeRepository  GetAllMarkScheme() Method  projectId = {projectId}");
                throw;
            }
            return markSchemes;
        }

        /// <summary>
        /// To get MarkScheme Detail and Band Details
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public async Task<MarkSchemeModel> MarkSchemeWithId(long projectId, long schemeId)
        {
            MarkSchemeModel markSchemeResponse = new();
            try
            {
                logger.LogDebug($"MarkSchemeRepository  MarkSchemeWithId() Method started.  projectId {projectId} and schemeId = {schemeId}");

                //----> Get Mark scheme details.
                markSchemeResponse = (await (from PMT in context.ProjectMarkSchemeTemplates
                                             where PMT.ProjectId == projectId
                                                  && PMT.ProjectMarkSchemeId == schemeId
                                                  && !PMT.IsDeleted
                                             select new MarkSchemeModel
                                             {
                                                 ProjectMarkSchemeId = PMT.ProjectMarkSchemeId,
                                                 SchemeName = PMT.SchemeName,
                                                 Marks = PMT.Marks,
                                                 SchemeDescription = PMT.SchemeDescription,
                                                 IsBandExist = PMT.IsBandExist,
                                                 MarkSchemeType = PMT.MarkingSchemeType == null ? MarkSchemeType.QuestionLevel : (MarkSchemeType)PMT.MarkingSchemeType
                                             }).ToListAsync()).FirstOrDefault();
                if (markSchemeResponse != null)
                {
                    //----> Get band details.
                    markSchemeResponse.Bands = (await (from PMT in context.ProjectMarkSchemeTemplates
                                                       join PMSB in context.ProjectMarkSchemeBandDetails on
                                                       PMT.ProjectMarkSchemeId equals PMSB.ProjectMarkSchemeId
                                                       where PMT.ProjectId == projectId
                                                  && PMSB.ProjectMarkSchemeId == schemeId
                                                  && !PMT.IsDeleted && !PMSB.IsDeleted
                                                       select new BandModel
                                                       {
                                                           BandId = PMSB.BandId,
                                                           BandName = PMSB.BandName,
                                                           BandFrom = PMSB.BandFrom,
                                                           BandTo = PMSB.BandTo,
                                                           BandDescription = PMSB.BandDescription
                                                       }).ToListAsync()).ToList();


                    markSchemeResponse.IsQuestionTagged = context.ProjectMarkSchemeTemplates.Any(pmt => pmt.ProjectMarkSchemeId == schemeId && !pmt.IsDeleted && pmt.IsTagged);

                    //----> Get file details uplaoded file.
                    var projectFiles = context.ProjectFiles.Where(x => x.EntityId == schemeId && !x.IsDeleted && x.IsActive).ToList();
                    markSchemeResponse.filedetails = new();
                    if (projectFiles != null && projectFiles.Count > 0)
                    {
                        projectFiles.ForEach((file) =>
                        {
                            FileModel filedtls = new()
                            {
                                Id = file.FileId,
                                FileName = file.FileName
                            };
                            markSchemeResponse.filedetails.Add(filedtls);
                        });
                    }
                }

                logger.LogDebug($"MarkSchemeRepository  MarkSchemeWithId() Method started.  projectId {projectId} and schemeId = {schemeId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeRepository  MarkSchemeWithId() Method started.  projectId {projectId} and schemeId = {schemeId}");
                throw;
            }
            return markSchemeResponse;
        }

        /// <summary>
        /// To Create a new MarkScheme
        /// </summary>
        /// <param name="markScheme"></param>
        /// <param name="projectId"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <returns></returns>
        public Task<string> CreateMarkScheme(MarkSchemeModel markScheme, long projectId, long ProjectUserRoleID)
        {
            string status = "ER001";
            long latestId = 0;
            try
            {
                logger.LogDebug($"MarkSchemeRepository CreateMarkScheme() Method started. List of {markScheme} and projectId = {projectId} and userId = {ProjectUserRoleID}");

                if (markScheme == null)
                {
                    return Task.FromResult(status);
                }

                //----> Checking schemename exist or not.
                var Schemecount = (from PT in context.ProjectMarkSchemeTemplates
                                   where PT.ProjectId == projectId && PT.SchemeName == markScheme.SchemeName && !PT.IsDeleted
                                   select new MarkSchemeModel
                                   {
                                       SchemeName = PT.SchemeName
                                   }).ToList();
                if (Schemecount == null || Schemecount.Count == 0)
                {
                    //----> Inserting new scheme details.
                    ProjectMarkSchemeTemplate markSchemeEntity = new()
                    {
                        SchemeName = markScheme.SchemeName,
                        Marks = markScheme.Marks,
                        SchemeDescription = markScheme.SchemeDescription,
                        ProjectId = projectId,
                        CreatedBy = ProjectUserRoleID,
                        CreatedDate = DateTime.UtcNow,
                        IsDeleted = false,
                        MarkingSchemeType = (byte)markScheme.MarkSchemeType,
                        IsBandExist = markScheme.IsBandExist,

                    };

                    context.ProjectMarkSchemeTemplates.Add(markSchemeEntity);
                    context.SaveChanges();
                    latestId = markSchemeEntity.ProjectMarkSchemeId;

                    if (!markScheme.IsBandExist)
                    {
                        status = "SU001";
                        UploadMarkSchemeFiles(markScheme.filedetails, latestId, ProjectUserRoleID);
                        return Task.FromResult(status);
                    }

                    if (latestId > 0 && markScheme.Bands != null && markScheme.Bands.Count > 0)
                    {
                        markScheme.Bands.ForEach(band =>
                        {
                            //----> Inserting scheme band details.
                            ProjectMarkSchemeBandDetail bandEntity = new()
                            {
                                ProjectMarkSchemeId = latestId,
                                BandName = band.BandName,
                                BandFrom = band.BandFrom,
                                BandTo = band.BandTo,
                                BandDescription = band.BandDescription,
                                CreatedBy = ProjectUserRoleID,
                                CreatedDate = DateTime.UtcNow,
                                IsDeleted = false
                            };
                            context.ProjectMarkSchemeBandDetails.Add(bandEntity);
                            context.SaveChanges();
                        });

                    }
                    else
                    {
                        logger.LogDebug($"MarkSchemeRepository  CreateMarkScheme() Method. Project SchemeID is null or Band count is null");
                    }

                    status = "SU001";
                }
                else
                {
                    status = "SN001";
                }

                UploadMarkSchemeFiles(markScheme.filedetails, latestId, ProjectUserRoleID);
                
                logger.LogDebug($"MarkSchemeRepository  CreateMarkScheme() Method completed. List of {markScheme} and projectId = {projectId} and userId = {ProjectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeRepository  CreateMarkScheme() Method. List of {markScheme} and projectId = {projectId} and userId = {ProjectUserRoleID}");
                throw;
            }
            return Task.FromResult(status);
        }

        private void UploadMarkSchemeFiles(List<FileModel> filedetails, long latestId, long ProjectUserRoleID)
        {
            if (filedetails != null && filedetails.Count > 0)
            {
                filedetails.ForEach(resp =>
                {
                    //----> Inserting scheme file details.
                    ProjectFile updateFileValues = (from PMSB in context.ProjectFiles
                                                    where PMSB.FileId == resp.Id
                                                    select PMSB).FirstOrDefault();
                    if (updateFileValues != null)
                    {
                        updateFileValues.EntityId = latestId;
                        updateFileValues.EntityType = (int)EnumFilesEntityType.MarkScheme;
                        updateFileValues.FileType = "Document";
                        updateFileValues.ModifiedBy = ProjectUserRoleID;
                        updateFileValues.ModifiedDate = DateTime.UtcNow;
                        context.ProjectFiles.Update(updateFileValues);
                        context.SaveChanges();
                    }
                });

            }
            else
            {
                logger.LogDebug($"MarkSchemeRepository  UpdateMarkScheme() Method completed. File details is null or File details is null");
            }
        }

        /// <summary>
        /// Updates Mark Scheme and also the Banding Values
        /// </summary>
        /// <param name="markScheme"></param>
        /// <param name="projectId"></param>
        /// <param name="schemeId"></param>
        /// <param name="ProjectUserRoleID"></param>
        /// <returns></returns>
        public async Task<string> UpdateMarkScheme(MarkSchemeModel markScheme, long projectId, long schemeId, long ProjectUserRoleID)
        {
            string status = "ER001";
            ProjectMarkSchemeTemplate updateValues;
            bool IsSame = false;
            try
            {
                logger.LogDebug($"MarkSchemeRepository  UpdateMarkScheme() Method started. List of {markScheme} and projectId = {projectId} and schemeId = {schemeId} and userId = {ProjectUserRoleID}");

                //----> Checking schemename exist or not.
                var Schemecount = (from PT in context.ProjectMarkSchemeTemplates
                                   where PT.ProjectId == projectId && PT.SchemeName == markScheme.SchemeName && !PT.IsDeleted
                                   select new MarkSchemeModel
                                   {
                                       ProjectMarkSchemeId = PT.ProjectMarkSchemeId,
                                       SchemeName = PT.SchemeName
                                   }).ToList();

                if (Schemecount != null && Schemecount.Count > 0)
                {
                    IsSame = Schemecount.Any(s => s.ProjectMarkSchemeId != markScheme.ProjectMarkSchemeId);
                }

                //----> Updating scheme details.

                updateValues = await context.ProjectMarkSchemeTemplates.Where(a => a.ProjectId == projectId && a.ProjectMarkSchemeId == schemeId && !a.IsDeleted).FirstOrDefaultAsync();


                if (updateValues != null)
                {
                    if (IsSame)
                    {
                        status = "SN001";
                    }
                    else
                    {
                        UpdateSchemeDetails(updateValues, Schemecount, markScheme, schemeId, ProjectUserRoleID);
                        status = "SU001";
                    }
                }
                else
                {
                    logger.LogDebug($"MarkSchemeRepository  UpdateMarkScheme() Method completed. Update values getting null");
                }


                logger.LogDebug($"MarkSchemeRepository  UpdateMarkScheme() Method completed. List of {markScheme} and projectId = {projectId} and schemeId = {schemeId}  and userId = {ProjectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeRepository  UpdateMarkScheme() Method. List of {markScheme} and  projectId = {projectId} and schemeId = {schemeId}  and userId = {ProjectUserRoleID}");
                throw;
            }
            return status;
        }
        private void UpdateSchemeDetails(ProjectMarkSchemeTemplate updateValues, List<MarkSchemeModel> schemecount, MarkSchemeModel markScheme, long schemeId, long projectUserRoleID)
        {
            string schName = schemecount.Where(w => w.ProjectMarkSchemeId == markScheme.ProjectMarkSchemeId).Select(w => w.SchemeName).FirstOrDefault();
            if (schName != markScheme.SchemeName)
            {
                updateValues.SchemeName = markScheme.SchemeName;
            }

            updateValues.Marks = markScheme.Marks;
            updateValues.SchemeDescription = markScheme.SchemeDescription;
            updateValues.ModifiedBy = projectUserRoleID;
            updateValues.ModifiedDate = DateTime.UtcNow;
            updateValues.IsDeleted = false;
            updateValues.MarkingSchemeType = (byte)markScheme.MarkSchemeType;
            updateValues.IsBandExist = markScheme.IsBandExist;
            context.ProjectMarkSchemeTemplates.Update(updateValues);

            long latestId = updateValues.ProjectMarkSchemeId;

            if (latestId > 0 && markScheme.Bands != null && markScheme.Bands.Count > 0)
            {
                //----> Updating band details.
                List<ProjectMarkSchemeBandDetail> updateBandValues = (from PMSB in context.ProjectMarkSchemeBandDetails
                                                                            where PMSB.ProjectMarkSchemeId == latestId
                                                                            select PMSB).ToList();

                List<long> bandIds = context.ProjectMarkSchemeBandDetails.Where(ms => ms.ProjectMarkSchemeId == schemeId && !ms.IsDeleted).Select(ms => ms.BandId).ToList();

                var isQuestionTagged = context.QuestionUserResponseMarkingDetails.Any(qr => bandIds.Contains((long)qr.BandId) && !qr.IsDeleted);

                //----> Updating existing bands.
                UpdatingExistingBands(markScheme, updateBandValues, projectUserRoleID, isQuestionTagged);

                //----> Updating newly added bands.
                AddNewlyBands(markScheme, updateBandValues, latestId, projectUserRoleID);

                FileInsertDelete(markScheme, schemeId, projectUserRoleID);
            }
            else
            {
                logger.LogDebug($"MarkSchemeRepository  UpdateMarkScheme() Method completed. Project SchemeID is null or Band count is null");
            }
            context.SaveChanges();
        }
        private void UpdatingExistingBands(MarkSchemeModel markScheme, List<ProjectMarkSchemeBandDetail> updateBandValues, long ProjectUserRoleID, bool isQuestionTagged)
        {
            updateBandValues.ForEach(resp =>
            {
                BandModel bandModel = markScheme.Bands.FirstOrDefault(a => a.BandId == resp.BandId);

                if (markScheme.IsBandExist)
                {
                    if (bandModel != null && !isQuestionTagged)
                    {
                        resp.BandName = bandModel.BandName;
                        resp.BandFrom = bandModel.BandFrom;
                        resp.BandTo = bandModel.BandTo;
                        resp.BandDescription = bandModel.BandDescription;
                        resp.ModifiedBy = ProjectUserRoleID;
                        resp.ModifiedDate = DateTime.UtcNow;
                        resp.IsDeleted = false;
                    }
                    else if (bandModel != null && isQuestionTagged)
                    {
                        resp.BandName = bandModel?.BandName;
                        resp.BandFrom = bandModel.BandFrom;
                        resp.BandTo = bandModel.BandTo;
                        resp.BandDescription = bandModel?.BandDescription;
                        resp.ModifiedBy = ProjectUserRoleID;
                        resp.ModifiedDate = DateTime.UtcNow;
                        resp.IsDeleted = false;
                    }
                    else
                    {
                        resp.IsDeleted = true;
                        resp.ModifiedBy = ProjectUserRoleID;
                        resp.ModifiedDate = DateTime.UtcNow;
                    }
                }
                else
                {

                    resp.IsDeleted = true;
                    resp.ModifiedBy = ProjectUserRoleID;
                    resp.ModifiedDate = DateTime.UtcNow;
                }

                context.ProjectMarkSchemeBandDetails.Update(resp);
            });
        }
        private void AddNewlyBands(MarkSchemeModel markScheme, List<ProjectMarkSchemeBandDetail> updateBandValues, long latestId, long ProjectUserRoleID)
        {
            markScheme.Bands.ForEach(resp =>
            {
                var bandenties = updateBandValues.FirstOrDefault(a => a.BandId == resp.BandId);
                if (bandenties == null && markScheme.IsBandExist)
                {
                    ProjectMarkSchemeBandDetail bandEntity = new()
                    {
                        ProjectMarkSchemeId = latestId,
                        BandName = resp.BandName,
                        BandFrom = resp.BandFrom,
                        BandTo = resp.BandTo,
                        BandDescription = resp.BandDescription,
                        CreatedBy = ProjectUserRoleID,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedBy = ProjectUserRoleID,
                        ModifiedDate = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.ProjectMarkSchemeBandDetails.Add(bandEntity);
                }
                else
                {
                    logger.LogDebug($"MarkSchemeRepository  UpdateMarkScheme() Method completed. BandEntity is null");
                }
            });
        }
        private void FileInsertDelete(MarkSchemeModel markScheme, long schemeId, long ProjectUserRoleID)
        {
            var fileCount = context.ProjectFiles.Where(x => x.EntityId == schemeId && !x.IsDeleted).ToList();
            if (fileCount.Count > markScheme.filedetails.Count)
            {
                //----> Delete files.
                var newList = markScheme.filedetails.Select(x => x.Id).ToList();
                var removedFile = fileCount.Where(x => !newList.Contains(x.FileId)).ToList();
                if (removedFile != null)
                {
                    removedFile.ForEach(resp =>
                    {
                        resp.IsDeleted = true;
                        resp.ModifiedBy = ProjectUserRoleID;
                        resp.ModifiedDate = DateTime.UtcNow;
                        context.ProjectFiles.Update(resp);
                    });

                }
            }
            else
            {
                //----> Add files.
                markScheme.filedetails.ForEach(resp =>
                {
                    ProjectFile updateFileValues = (from PMSB in context.ProjectFiles
                                                    where PMSB.FileId == resp.Id
                                                    select PMSB).FirstOrDefault();
                    if (updateFileValues != null)
                    {
                        updateFileValues.EntityId = schemeId;
                        updateFileValues.EntityType = (int)EnumFilesEntityType.MarkScheme;
                        updateFileValues.FileType = "Document";
                        updateFileValues.ModifiedBy = ProjectUserRoleID;
                        updateFileValues.ModifiedDate = DateTime.UtcNow;
                        context.ProjectFiles.Update(updateFileValues);
                    }
                });
            }
        } 

        /// <summary>
        /// Get all Project Questions
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="maxMark"></param>
        /// <returns></returns>
        public async Task<IList<ProjectTaggedQuestionModel>> GetAllQuestions(long projectId, long schmeId, decimal maxMark)
        {
            List<ProjectTaggedQuestionModel> questionList = null;
            try
            {
                logger.LogDebug($"MarkSchemeRepository  GetAllQuestions() Method started. projectId = {projectId} and schemeId = {schmeId} and Max Mark = {maxMark}");

                var isscoringcomponent = context.ProjectMarkSchemeTemplates.Any(ms => ms.ProjectMarkSchemeId == schmeId &&
                !ms.IsDeleted && ms.MarkingSchemeType == 2);

                if (isscoringcomponent)
                {
                    questionList = await (from PQ in context.ProjectQuestions
                                          join pqsc in context.ProjectQuestionScoreComponents on PQ.ProjectQuestionId equals pqsc.ProjectQuestionId
                                          where PQ.ProjectId == projectId && !PQ.IsDeleted && PQ.QuestionType == 10 && pqsc.MaxMarks == maxMark
                                          select new ProjectTaggedQuestionModel
                                          {
                                              ProjectQuestionId = PQ.ProjectQuestionId,
                                              MaxMark = pqsc.MaxMarks,
                                              QuestionCode = PQ.QuestionCode,
                                              IsScoringComponentExist = PQ.IsScoreComponentExists,
                                              ComponentName = pqsc.ComponentName

                                          }).ToListAsync();
                }
                else
                {
                    questionList = await (from PQ in context.ProjectQuestions
                                          where PQ.ProjectId == projectId && PQ.QuestionMarks == maxMark && !PQ.IsDeleted && PQ.QuestionType == 10
                                          && !PQ.IsScoreComponentExists
                                          select new ProjectTaggedQuestionModel
                                          {
                                              ProjectQuestionId = PQ.ProjectQuestionId,
                                              MaxMark = PQ.QuestionMarks,
                                              QuestionCode = PQ.QuestionCode,
                                              IsScoringComponentExist = PQ.IsScoreComponentExists

                                          }).ToListAsync();

                }


                var listOfQuestions = await (from PMSQ in context.ProjectMarkSchemeQuestions
                                             where PMSQ.ProjectId == projectId && !PMSQ.Isdeleted
                                             select new ProjectTaggedQuestionModel
                                             {
                                                 MarkSchemeId = PMSQ.ProjectMarkSchemeId,
                                                 ProjectQuestionId = PMSQ.ProjectQuestionId
                                             }).ToListAsync();

                questionList.ForEach(result =>
                {
                    result.IsTagged = listOfQuestions.Any(a => a.ProjectQuestionId == result.ProjectQuestionId && a.MarkSchemeId == schmeId);
                    result.MarkSchemeId = listOfQuestions.FirstOrDefault(a => a.ProjectQuestionId == result.ProjectQuestionId)?.MarkSchemeId;
                });
                questionList = questionList.Where(a => a.MarkSchemeId == schmeId || a.MarkSchemeId == null).OrderBy(x => x.ProjectQuestionId).ToList();

                logger.LogDebug($"MarkSchemeRepository  GetAllQuestions() Method completed. projectId = {projectId} and schemeId = {schmeId} and Max Mark = {maxMark}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeRepository  GetAllQuestions() Method. projectId = {projectId} and schemeId = {schmeId} and Max Mark = {maxMark}");
                throw;
            }
            return questionList;
        }

        /// <summary>
        /// Get question text
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public async Task<ProjectQuestionsModel> GetQuestionText(long projectId, long questionId)
        {
            ProjectQuestionsModel _questionText;
            try
            {
                logger.LogDebug($"MarkSchemeRepository  GetQuestionText() Method started.  projectId = {projectId} and questionId = {questionId}");

                _questionText = (await (from PQ in context.ProjectQuestions
                                        where PQ.ProjectId == projectId && PQ.ProjectQuestionId == questionId && !PQ.IsDeleted
                                        select new ProjectQuestionsModel
                                        {
                                            QuestionText = PQ.QuestionText
                                        }).ToListAsync()).FirstOrDefault();

                logger.LogDebug($"MarkSchemeRepository  GetQuestionText() Method completed.  projectId = {projectId} and questionId = {questionId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeRepository  GetQuestionText() Method  projectId = {projectId} and questionId = {questionId}");
                throw;
            }
            return _questionText;
        }

        /// <summary>
        /// update Mark Scheme under a question
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        public async Task<string> MarkSchemeMapping(long ProjectID, List<ProjectTaggedQuestionModel> questionList, long ProjectUserRoleID)
        {
            string status = "ER001";
            try
            {
                logger.LogDebug($"MarkSchemeRepository  MarkSchemeMapping() Method started. projectId = {ProjectID} and questionList = {questionList} and Userid = {ProjectUserRoleID}");

                if (questionList != null && questionList.Count > 0)
                {

                    List<long> qsnIds = questionList.Select(x => x.ProjectQuestionId).ToList();
                    var MaxMarks = questionList.Select(x => x.MaxMark).FirstOrDefault();

                    List<ProjectQuestion> pmsq = await context.ProjectQuestions.Where(x => qsnIds.Contains(x.ProjectQuestionId) && x.ProjectId == ProjectID && !x.IsDeleted && x.QuestionMarks == MaxMarks).ToListAsync();

                    if (!qsnIds.Any(ds => !pmsq.Any(db => db.ProjectQuestionId == ds)))
                    {
                        long[] qnsids = questionList.Select(x => x.ProjectQuestionId).ToArray();

                        List<ProjectMarkSchemeQuestion> projectMarkSchemeQuestions = await context.ProjectMarkSchemeQuestions.Where(x => qnsids.Contains(x.ProjectQuestionId) && x.ProjectId == ProjectID && !x.Isdeleted).ToListAsync();

                        questionList.ToList().ForEach(qns =>
                        {
                            var chkQuestion = (from QURMD in context.QuestionUserResponseMarkingDetails
                                               join
                                               PUQR in context.ProjectUserQuestionResponses on QURMD.ProjectQuestionResponseId equals PUQR.ProjectUserQuestionResponseId
                                               join PMBD in context.ProjectMarkSchemeBandDetails on QURMD.BandId equals PMBD.BandId
                                               join PMST in context.ProjectMarkSchemeTemplates on PMBD.ProjectMarkSchemeId equals PMST.ProjectMarkSchemeId
                                               where PUQR.ProjectQuestionId == qns.ProjectQuestionId && PMST.ProjectMarkSchemeId == qns.MarkSchemeId && !QURMD.IsDeleted &&
                                               !PUQR.Isdeleted && !PMBD.IsDeleted && !PMST.IsDeleted
                                               select new { QURMD.Id }).FirstOrDefault();
                            if (chkQuestion != null && !qns.IsTagged)
                            {
                                status = "TQ001";
                            }
                            else
                            {
                                GetQuestionEntity(projectMarkSchemeQuestions, qns, ProjectID, ProjectUserRoleID);
                            }
                        });
                        if (status != "TQ001")
                        {
                            context.SaveChanges();
                            status = "SU001";
                        }
                    }
                }
                else
                {
                    logger.LogDebug($"MarkSchemeRepository  MarkSchemeMapping() Method. Questionlist getting null");
                }

                logger.LogDebug($"MarkSchemeRepository  MarkSchemeMapping() Method completed. projectId = {ProjectID} and questionList = {questionList} and Userid = {ProjectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeRepository  MarkSchemeMapping() Method. projectId = {ProjectID} and questionList = {questionList} and Userid = {ProjectUserRoleID}");
                throw;
            }
            return status;
        }
        private void GetQuestionEntity(List<ProjectMarkSchemeQuestion> projectMarkSchemeQuestions, ProjectTaggedQuestionModel qns, long ProjectID, long ProjectUserRoleID)
        {
            if (projectMarkSchemeQuestions.Any(pm => pm.ProjectQuestionId == qns.ProjectQuestionId))
            {

                ProjectMarkSchemeQuestion qnsEntity = projectMarkSchemeQuestions.FirstOrDefault(pm => pm.ProjectQuestionId == qns.ProjectQuestionId);
                if (qnsEntity != null)
                {
                    qnsEntity.ProjectMarkSchemeId = (long)qns.MarkSchemeId;
                    qnsEntity.ProjectId = ProjectID;
                    qnsEntity.Isdeleted = !qns.IsTagged;
                    qnsEntity.ModifiedBy = ProjectUserRoleID;
                    qnsEntity.ModifiedDate = DateTime.UtcNow;
                    context.ProjectMarkSchemeQuestions.Update(qnsEntity);
                }
            }
            else
            {
                context.ProjectMarkSchemeQuestions.Add(new ProjectMarkSchemeQuestion
                {
                    ProjectQuestionId = qns.ProjectQuestionId,
                    ProjectMarkSchemeId = (long)qns.MarkSchemeId,
                    ProjectId = ProjectID,
                    Isdeleted = false,
                    CreatedBy = ProjectUserRoleID,
                    CreatedDate = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Delete unmapped markschemes
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="markSchemeids"></param>
        /// <returns></returns>
        public async Task<string> DeleteMarkScheme(long projectId, List<long> markSchemeids, long ProjectUserRoleID)
        {
            string status = "ER001";

            List<ProjectMarkSchemeQuestion> projectMarkSchemeQuestion = new();
            try
            {
                logger.LogDebug($"MarkSchemeRepository > DeleteMarkScheme() started. projectId = {projectId} and List Of markSchemeids = {markSchemeids} and Userid = {ProjectUserRoleID}");
                ProjectMarkSchemeTemplate projectMarkSchemeTemplate;

                if (markSchemeids.Count != 0)
                {
                    projectMarkSchemeQuestion = context.ProjectMarkSchemeQuestions.Where(x => markSchemeids.Contains(x.ProjectMarkSchemeId) && x.ProjectId == projectId && !x.Isdeleted).ToList();

                    if (projectMarkSchemeQuestion.Count == 0)
                    {
                        foreach (var schmeid in markSchemeids)
                        {
                            projectMarkSchemeTemplate = await context.ProjectMarkSchemeTemplates.FirstOrDefaultAsync(x => x.ProjectMarkSchemeId == schmeid && x.ProjectId == projectId && !x.IsDeleted);
                            projectMarkSchemeTemplate.IsDeleted = true;
                            projectMarkSchemeTemplate.ModifiedBy = ProjectUserRoleID;
                            projectMarkSchemeTemplate.ModifiedDate = DateTime.UtcNow;

                            context.ProjectMarkSchemeTemplates.Update(projectMarkSchemeTemplate);

                            //------> Upade Band Details

                            List<ProjectMarkSchemeBandDetail> deleteBandValues = context.ProjectMarkSchemeBandDetails.Where(x => markSchemeids.Contains(x.ProjectMarkSchemeId)).ToList();

                            deleteBandValues.ForEach(resp =>
                            {
                                resp.IsDeleted = true;
                                resp.ModifiedBy = ProjectUserRoleID;
                                resp.ModifiedDate = DateTime.UtcNow;

                                context.ProjectMarkSchemeBandDetails.Update(resp);
                            });
                        }
                        context.SaveChanges();
                        status = "SU001";
                    }
                    else
                    {
                        status = "SALRMP";
                    }
                }
                else
                {
                    status = "MANDFD";
                }
                logger.LogDebug($"MarkSchemeRepository > DeleteMarkScheme() started. projectId = {projectId} and List Of markSchemeids = {markSchemeids} and Userid = {ProjectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"MarkSchemeRepository > DeleteMarkScheme() method. projectId = {projectId} and List Of markSchemeids = {markSchemeids} and Userid = {ProjectUserRoleID}");
                throw;
            }
            return status;
        }

        /// <summary>
        /// Get all question list
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<IList<ProjectTaggedQuestionsModel>> GetQuestionsUnderProject(long projectId, int? pagenumber)
        {
            List<ProjectTaggedQuestionsModel> questionList = null;
            try
            {
                logger.LogDebug($"MarkSchemeRepository  GetQuestionsUnderProject() Method started. projectId = {projectId} and Page number = {pagenumber}");

                questionList = await (from PQ in context.ProjectQuestions
                                      where PQ.ProjectId == projectId && !PQ.IsDeleted
                                      select new ProjectTaggedQuestionsModel
                                      {
                                          QuestionId = PQ.QuestionId,
                                          MaxMark = PQ.QuestionMarks,
                                          QuestionCode = PQ.QuestionCode,


                                      }).ToListAsync();

                var listOfQuestions = await (from PMSQ in context.ProjectMarkSchemeQuestions
                                             where PMSQ.ProjectId == projectId && !PMSQ.Isdeleted
                                             select new ProjectTaggedQuestionsModel
                                             {
                                                 MarkSchemeId = PMSQ.ProjectMarkSchemeId,
                                                 QuestionId = PMSQ.ProjectQuestionId
                                             }).ToListAsync();

                var listOfScheme = await (from PMT in context.ProjectMarkSchemeTemplates
                                          where PMT.ProjectId == projectId && !PMT.IsDeleted
                                          select new ProjectTaggedQuestionsModel
                                          {
                                              MarkSchemeId = PMT.ProjectMarkSchemeId,
                                              SchemeName = PMT.SchemeName
                                          }).ToListAsync();



                questionList.ForEach(result =>
                {
                    result.IsTagged = listOfQuestions.Any(a => a.QuestionId == result.QuestionId);
                    result.MarkSchemeId = listOfQuestions.FirstOrDefault(a => a.QuestionId == result.QuestionId)?.MarkSchemeId;
                    if (result.MarkSchemeId != null)
                    {
                        result.SchemeName = listOfScheme.FirstOrDefault(z => z.MarkSchemeId == result.MarkSchemeId)?.SchemeName.ToString();
                    }

                    result.TotalRows = questionList.Count;
                });

                logger.LogDebug($"MarkSchemeRepository  GetQuestionsUnderProject() Method completed. projectId = {projectId} and Page number = {pagenumber}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkSchemeRepository  GetQuestionsUnderProject() Method. projectId = {projectId} and Page number = {pagenumber}");
                throw;
            }
            return questionList.ToList();
        }

    }
}
