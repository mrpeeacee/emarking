using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Foundatio.Caching;
using Foundatio.Extensions.Hosting.Startup;
using Foundatio.Jobs;
using Foundatio.Messaging;
using Foundatio.Queues;
using Foundatio.Repositories.Elasticsearch;
using Foundatio.Repositories.Elasticsearch.Configuration;
using Microsoft.Extensions.Logging;
using Nest;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.Configuration
{

    public sealed class ProjectConfiguration : ElasticConfiguration, IStartupAction
    {
        private readonly AppOptions _appOptions;

        public ProjectConfiguration()
        { }

        public ProjectConfiguration(
            AppOptions appOptions,
            IQueue<WorkItemData> workItemQueue,
            ICacheClient cacheClient,
            IMessageBus messageBus,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory
        ) : base(workItemQueue, cacheClient, messageBus, loggerFactory)
        {
            if (serviceProvider is null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            _appOptions = appOptions;

            _logger.LogInformation("All new indexes will be created with {ElasticsearchNumberOfShards} Shards and {ElasticsearchNumberOfReplicas} Replicas", _appOptions.ElasticsearchOptions.NumberOfShards, _appOptions.ElasticsearchOptions.NumberOfReplicas);
            MigrationIndex index = Migrations = new MigrationIndex(this, _appOptions.ElasticsearchOptions.ScopePrefix + "migrations", appOptions.ElasticsearchOptions.NumberOfReplicas);
            AddIndex(index);            
        }

        public Task RunAsync(CancellationToken shutdownToken = default)
        {
            if (_appOptions.ElasticsearchOptions.DisableIndexConfiguration)
                return Task.CompletedTask;

            return ConfigureIndexesAsync();
        }

        public ElasticsearchOptions Options => _appOptions.ElasticsearchOptions;
        public MigrationIndex Migrations { get; } 

        protected override IElasticClient CreateElasticClient()
        {
            var connectionPool = CreateConnectionPool();
            var settings = new ConnectionSettings(connectionPool);

            foreach (var index in Indexes)
                index.ConfigureSettings(settings);

            if (!string.IsNullOrEmpty(_appOptions.ElasticsearchOptions.UserName) && !string.IsNullOrEmpty(_appOptions.ElasticsearchOptions.Password))
                settings.BasicAuthentication(_appOptions.ElasticsearchOptions.UserName, _appOptions.ElasticsearchOptions.Password);

            var client = new ElasticClient(settings);
            return client;
        }

        protected override IConnectionPool CreateConnectionPool()
        {
            var serverUris = Options?.ServerUrl.Split(',').Select(url => new Uri(url));
            return new StaticConnectionPool(serverUris);
        }

        protected override void ConfigureSettings(ConnectionSettings settings)
        {
            if (_appOptions.AppMode == AppMode.Development)
                settings.EnableDebugMode();

            settings.EnableTcpKeepAlive(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2))
                .DefaultFieldNameInferrer(p => p.ToLowerUnderscoredWords())
                .MaximumRetries(5);
        }
    }
}
