using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.MarkScheme;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Project.ScoringComponentLibrary;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Dashboard;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using Saras.eMarking.Infrastructure.Project.MarkScheme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Saras.eMarking.Domain.ViewModels.Project.ScoringComponentLibrary.ScoringComponentLibraryModel;

namespace Saras.eMarking.Infrastructure.Project.ScoringComponentLibrary
{
    public class ScoringComponentLibraryRepository : BaseRepository<ScoringComponentLibraryRepository>, IScoringComponentLibraryRepository
    {
        private readonly ApplicationDbContext context;
        public ScoringComponentLibraryRepository(ApplicationDbContext context, ILogger<ScoringComponentLibraryRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        /// <summary>
        /// To Get all Scoring Component Libraries.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<IList<ScoreComponentLibraryName>> GetAllScoringComponentLibrary(long projectId)
        {
            List<ScoreComponentLibraryName> scoreComponentLibrary = null;
            try
            {
                logger.LogDebug($"ScoringComponentLibraryRepository  GetAllScoringComponentLibrary() Method started.  projectId = {projectId}");
                await using (SqlConnection connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand("[Marking].[GetScoreComponentDetails]", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@Componenttype", SqlDbType.BigInt).Value = 1;
                    connection.Open();
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        scoreComponentLibrary = new List<ScoreComponentLibraryName>();
                        while (reader.Read())
                        {
                            ScoreComponentLibraryName ScoringComponent = new()
                            {
                                ScoreComponentId = (long)reader["ScoreComponentID"],
                                ComponentName = (string)reader["ComponentName"],
                                Marks = (decimal)reader["Marks"],
                                IsQuestionTagged = (bool)reader["IsTagged"]
                            };
                            scoreComponentLibrary.Add(ScoringComponent);

                        }
                    }
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                logger.LogDebug($"ScoringComponentLibraryRepository  GetAllScoringComponentLibrary() Method completed.  projectId = {projectId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ScoringComponentLibraryRepository  GetAllScoringComponentLibrary() Method  projectId = {projectId}");
                throw;
            }
            return scoreComponentLibrary;
        }

		/// <summary>
		/// Create New Scoring Component Libraries.
		/// </summary>
		/// <param name="ScoreComponentLibraryName"></param>
		/// <param name="projectId"></param>
		/// <param name="ProjectUserRoleID"></param>
		/// <returns></returns>

		public Task<string> CreateScoringSomponentLibrary(ScoreComponentLibraryName ScoreComponentLibraryName, long projectId, long ProjectUserRoleID)
		{
			string status = "";

			try
			{
				bool Isexits = ISScoringComponentLibrary(ScoreComponentLibraryName);
				if (!Isexits)
				{
					using (SqlConnection sqlCon = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
					{
						using (SqlCommand sqlCmd = new SqlCommand("Marking.[uspManageScoreComponentsAndDetails]", sqlCon))
						{
							sqlCmd.CommandType = CommandType.StoredProcedure;
							sqlCmd.Parameters.Add("@ActionType", SqlDbType.NVarChar).Value = "Insert";
							sqlCmd.Parameters.Add("@ComponentCode", SqlDbType.NVarChar).Value = ScoreComponentLibraryName.ComponentCode;
							sqlCmd.Parameters.Add("@ComponentName", SqlDbType.NVarChar).Value = ScoreComponentLibraryName.ComponentName;
							sqlCmd.Parameters.Add("@Marks", SqlDbType.NVarChar).Value = ScoreComponentLibraryName.Marks;
							sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = projectId;
							sqlCmd.Parameters.Add("@CreatedBy", SqlDbType.BigInt).Value = ProjectUserRoleID;
							//sqlCmd.Parameters.AddWithValue("@ScoreComponentDetails", ScoreComponentLibraryName.ScoringComponentDetails);

							DataTable scoreComponentDetailsTable = ConvertToDataTable(ScoreComponentLibraryName.ScoringComponentDetails);
							SqlParameter tvpParam = sqlCmd.Parameters.AddWithValue("@ScoreComponentDetails", scoreComponentDetailsTable);
							tvpParam.SqlDbType = SqlDbType.Structured;
							tvpParam.TypeName = "[Marking].[ScoreComponentDetailType]";

							sqlCmd.Parameters.Add("@status", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;


							sqlCon.Open();
							sqlCmd.ExecuteNonQuery();
							sqlCon.Close();
							status = sqlCmd.Parameters["@status"].Value.ToString();
						}
					}

				}
				else
				{
					status = "Exists";
				}
			}
			catch (Exception ex)
			{
			}
			return Task.FromResult(status);
		}

		public Boolean ISScoringComponentLibrary(ScoreComponentLibraryName ScoreComponentLibraryName)
		{
			bool IsExists = false;
			try
			{
				IsExists = context.ScoreComponents
					   .Any(sc => sc.ComponentName == ScoreComponentLibraryName.ComponentName);

			}
			catch (Exception e)
			{

			}
			return IsExists;

		}
		public DataTable ConvertToDataTable(List<ScoringComponentDetails> details)
		{
			DataTable table = new DataTable();
			table.Columns.Add("ComponentCode", typeof(string));
			table.Columns.Add("ComponentName", typeof(string));
			table.Columns.Add("Marks", typeof(decimal));
			table.Columns.Add("ComponentOrder", typeof(int));
			table.Columns.Add("CreatedBy", typeof(long));
			table.Columns.Add("CreatedDate", typeof(DateTime));
			foreach (var detail in details)
			{
				table.Rows.Add(detail.ComponentCode, detail.ScoringComponentName, detail.Marks, detail.Order, detail.CreatedBy, DateTime.UtcNow);
			}

			return table;
		}


		/// <summary>
		/// Delete unmapped Scoring Component Libraries.
		/// </summary>
		/// <param name="projectId"></param>
		/// <param name="markSchemeids"></param>
		/// <returns></returns>
		public async Task<string> DeleteScoringComponentLibrary(long projectId, List<long> ScoreComponentID, long ProjectUserRoleID)
        {
            string status = "ER001";

            List<ProjectMarkSchemeQuestion> projectMarkSchemeQuestion = new();
            try
            {
                logger.LogDebug($"MarkSchemeRepository > DeleteScoringComponentLibrary() started. projectId = {projectId} and List Of ScoreComponentID = {ScoreComponentID} and Userid = {ProjectUserRoleID}");
                ProjectMarkSchemeTemplate projectMarkSchemeTemplate;

                if (ScoreComponentID.Count != 0)
                {
                    projectMarkSchemeQuestion = context.ProjectMarkSchemeQuestions.Where(x => ScoreComponentID.Contains(x.ProjectMarkSchemeId) && x.ProjectId == projectId && !x.Isdeleted).ToList();

                    if (projectMarkSchemeQuestion.Count == 0)
                    {
                        foreach (var schmeid in ScoreComponentID)
                        {
                            projectMarkSchemeTemplate = await context.ProjectMarkSchemeTemplates.FirstOrDefaultAsync(x => x.ProjectMarkSchemeId == schmeid && x.ProjectId == projectId && !x.IsDeleted);
                            projectMarkSchemeTemplate.IsDeleted = true;
                            projectMarkSchemeTemplate.ModifiedBy = ProjectUserRoleID;
                            projectMarkSchemeTemplate.ModifiedDate = DateTime.UtcNow;

                            context.ProjectMarkSchemeTemplates.Update(projectMarkSchemeTemplate);

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
                logger.LogDebug($"ScoringComponentLibraryRepository > DeleteScoringComponentLibrary() started. projectId = {projectId} and List Of ScoreComponentID = {ScoreComponentID} and Userid = {ProjectUserRoleID}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ScoringComponentLibraryRepository > DeleteScoringComponentLibrary() method. projectId = {projectId} and List Of ScoreComponentID = {ScoreComponentID} and Userid = {ProjectUserRoleID}");
                throw;
            }
            return status;
        }

        /// <summary>
        ///Detailed View of  Scoring Component Libraries
        /// </summary>
        /// <param name="ComponentId"></param>
        /// <returns></returns>
        public async Task<ScoreComponentLibraryName> ViewScoringComponentLibrary(long ComponentId)
        {
            ScoreComponentLibraryName scoreComponentLibrary = new ScoreComponentLibraryName();
            try
            {
                logger.LogDebug($"ScoringComponentLibraryRepository  ViewScoringComponentLibrary() Method started.  ComponentId = {ComponentId}");
                await using (SqlConnection connection = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
                {
                    SqlCommand sqlCommand = new SqlCommand("[Marking].[GetScoreComponentDetails]", connection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@ScoreComponentID", SqlDbType.BigInt).Value = ComponentId;
                    sqlCommand.Parameters.Add("@Componenttype", SqlDbType.BigInt).Value = 2;
                    connection.Open();
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    if (reader.HasRows)
                    {
                        scoreComponentLibrary.ScoringComponentDetails = new List<ScoringComponentDetails>();
                        while (reader.Read())
                        {

                            scoreComponentLibrary.ScoreComponentId = (long)reader["ScoreComponentID"];
                            scoreComponentLibrary.ComponentName = (string)reader["ComponentName"];
                            ScoringComponentDetails details = new()
                            {
                                ComponentDetailID = (long)reader["ComponentDetailID"],
                                ScoringComponentName = (string)reader["ComponentName"],
                                Marks = (decimal)reader["ComponentMarks"],
                                ComponentCode = reader["ComponentCode"].ToString(),
                                IsQuestionTagged = (bool)reader["IsTagged"]
                            };
                            scoreComponentLibrary.ScoringComponentDetails.Add(details);
                        }
                    }
                    if (!reader.IsClosed)
                    {
                        reader.Close();
                    }
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                logger.LogDebug($"ScoringComponentLibraryRepository  ViewScoringComponentLibrary() Method completed.  ComponentId = {ComponentId}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in ScoringComponentLibraryRepository  ViewScoringComponentLibrary() Method  ComponentId = {ComponentId}");
                throw;
            }
            return scoreComponentLibrary;
        }

    }
}
