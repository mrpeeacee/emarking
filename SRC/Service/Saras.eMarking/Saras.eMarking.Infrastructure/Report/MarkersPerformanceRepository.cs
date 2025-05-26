using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Report;
using Saras.eMarking.Domain.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Report
{
    public class MarkersPerformanceRepository : BaseRepository<MarkersPerformanceRepository>, IMarkersPerformanceRepository
    {
        private readonly ApplicationDbContext context;
        public MarkersPerformanceRepository(ApplicationDbContext context, ILogger<MarkersPerformanceRepository> _logger) : base(_logger)
        {
            this.context = context;
        }

        public Task<List<SchoolDetails>> GetSchoolDetails(long ProjectId)
        {
            List<SchoolDetails> result = new List<SchoolDetails>();
            try
            {
                result = (from SI in context.SchoolInfos
                          where SI.ProjectId == ProjectId && !SI.IsDeleted 
                          select new SchoolDetails
                          {
                              SchoolId = SI.SchoolId,
                              SchoolCode = SI.SchoolCode,
                              SchoolName = SI.SchoolName,
                          }).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in MarkersPerformanceRepository :Method Name:GetSchoolDetails()");
                throw;
            }
            return Task.FromResult(result);
        }
        public Task<MarkerPerformanceStatistics> GetMarkerPerformanceDetails(MarkerDetails markerDetails)
        {
            MarkerPerformanceStatistics result = new MarkerPerformanceStatistics();
            try
            {
                DataSet ds = GetMarkerPerformanceTable(markerDetails);
                DataTable dataTable = ds.Tables[1];
                result.TotalMarkerCount = Convert.ToInt64(dataTable.Rows[0]["TotalMarkerCount"]);
                result.TotalSchoolCount = Convert.ToInt64(dataTable.Rows[0]["TotalSchoolCount"]);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Error in StudentsResultRepository :Method Name:GetStudentResultDetails()");
                throw;
            }
            return Task.FromResult(result);
        }
        public Task<List<MarkerPerformance>> GetMarkerPerformance(MarkerDetails markerDetails)
        {
            List<MarkerPerformance> result = new List<MarkerPerformance>();
            try
            {
                DataSet ds = GetMarkerPerformanceTable(markerDetails);
                DataTable dt = ds.Tables[0];

                foreach (DataRow reader in dt.Rows)
                {
                    MarkerPerformance objMarking = new()
                    {
                        MarkerName = Convert.ToString(reader["MPName"]),
                        School = Convert.ToString(reader["SchoolName"]),
                        TotalScripts = Convert.ToInt64(reader["TotalScripts"]),
                        RC1 = Convert.ToInt64(reader["RC1DoneCount"]),
                        RC2 = Convert.ToInt64(reader["RC2DoneCount"]),
                        Adhoc = Convert.ToInt64(reader["AdhocCount"]),
                        RealLocated = Convert.ToInt64(reader["Reallocated"])
                    };
                    result.Add(objMarking);
                }

            }

            catch (Exception ex)
            {
                logger.LogError(ex,"Error in MarkersPerformanceRepository :Method Name:GetMarkerPerformance()");
                throw;
            }
            return Task.FromResult(result);
        }
        private DataSet GetMarkerPerformanceTable(MarkerDetails markerDetails)
        {
            DataSet result = new DataSet();

            using SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString);
            using SqlCommand sqlCmd = new("[Marking].[UspGetMarkingPersonnelReport]", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.Parameters.Add("@ProjectID", SqlDbType.BigInt).Value = markerDetails.ProjectId;
            sqlCmd.Parameters.Add("@QIGID", SqlDbType.BigInt).Value = markerDetails.QigID;
            sqlCmd.Parameters.Add("@ExamYear", SqlDbType.Int).Value = markerDetails.ExamYear;
            sqlCmd.Parameters.Add("@MarkerName", SqlDbType.NVarChar).Value = markerDetails.MarkerName;
            sqlCmd.Parameters.Add("@SchoolCode", SqlDbType.NVarChar).Value = markerDetails.SchoolCode;
            sqlCon.Open();
            SqlDataAdapter sda = new SqlDataAdapter(sqlCmd);

            sda.Fill(result);
            if (sqlCon.State != ConnectionState.Closed)
            {
                sqlCon.Close();
            }
            return result;
        }
    }
}
