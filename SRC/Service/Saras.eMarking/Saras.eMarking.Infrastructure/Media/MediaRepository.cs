using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Interfaces.RepositoryInterfaces.Media;
using Saras.eMarking.Domain.ViewModels.Media;

namespace Saras.eMarking.Infrastructure.Media
{
    public class MediaRepository : BaseRepository<MediaRepository>, IMediaRepository
    {
        private readonly ApplicationDbContext context;
        private readonly AppOptions appOptions;
        public MediaRepository(ApplicationDbContext context, AppOptions _appOptions, ILogger<MediaRepository> _logger) : base(_logger)
        {
            this.context = context;
            appOptions = _appOptions;
        }

        public MediaModel GetMediaConfiguration()
        {
            return GetMediaConfigSetting();
        }

        private MediaModel GetMediaConfigSetting()
        {
            MediaModel result = new MediaModel();
            if (context != null)
            {
                try
                {
                    DataTable dt = new();
                    using (SqlConnection sqlCon = new(context.Database.GetDbConnection().ConnectionString))
                    {
                        using (SqlCommand sqlCmd = new("UspGetUrlConfigurationsDetail", sqlCon))
                        {
                            sqlCmd.CommandType = CommandType.StoredProcedure;
                            sqlCmd.Parameters.Add("@ApplicationTypeName", SqlDbType.NVarChar).Value = appOptions.AppSettings.MediaServiceConfig.ApplicationTypeName;
                            sqlCmd.Parameters.Add("@ApplicationModuleCode", SqlDbType.NVarChar).Value = appOptions.AppSettings.MediaServiceConfig.ApplicationModuleCode;
                            sqlCmd.Parameters.Add("@ProjectCode", SqlDbType.NVarChar).Value = appOptions.AppSettings.MediaServiceConfig.ProjectCode;
                            sqlCmd.Parameters.Add("@URLCode", SqlDbType.NVarChar).Value = appOptions.AppSettings.MediaServiceConfig.URLCode;

                            sqlCon.Open();

                            dt.Load(sqlCmd.ExecuteReader());
                            if (dt.Rows.Count > 0)
                            {
                                result = Newtonsoft.Json.JsonConvert.DeserializeObject<MediaModel>(Convert.ToString(dt.Rows[0]["URLINFO"]));
                            }
                            if (sqlCon.State == ConnectionState.Open)
                            {
                                sqlCon.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in MediaRepository  GetMediaConfigSetting() Method");
                    throw;
                }
            }
            return result;
        }
    }
}
