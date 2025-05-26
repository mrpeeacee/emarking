using System;
using Microsoft.Extensions.Configuration;
using Saras.eMarking.Domain.Entities;

namespace Saras.eMarking.Domain.Configuration
{

    public class ElasticsearchOptions
    {
        public string ServerUrl { get; internal set; }
        public int NumberOfShards { get; internal set; } = 1;
        public int NumberOfReplicas { get; internal set; }
        public int FieldsLimit { get; internal set; } = 1500;
        public bool EnableMapperSizePlugin { get; internal set; }
        public string Scope { get; internal set; }
        public string ScopePrefix { get; internal set; }

        public bool EnableSnapshotJobs { get; set; }
        public bool DisableIndexConfiguration { get; set; }

        public string Password { get; internal set; }
        public string UserName { get; internal set; }
        public DateTime ReindexCutOffDate { get; internal set; }
        public ElasticsearchOptions ElasticsearchToMigrate { get; internal set; }

        public static ElasticsearchOptions ReadFromConfiguration(IConfiguration config, AppOptions appOptions)
        {
            var options = new ElasticsearchOptions
            {
                Scope = appOptions.Scope
            };
            options.ScopePrefix = !String.IsNullOrEmpty(options.Scope) ? options.Scope + "-" : String.Empty;

            options.DisableIndexConfiguration = config.GetValue(nameof(options.DisableIndexConfiguration), false);
            options.EnableSnapshotJobs = config.GetValue(nameof(options.EnableSnapshotJobs), String.IsNullOrEmpty(options.ScopePrefix) && appOptions.AppMode == AppMode.Production);
            options.ReindexCutOffDate = config.GetValue(nameof(options.ReindexCutOffDate), DateTime.MinValue);

            return options;
        }
    }

}
