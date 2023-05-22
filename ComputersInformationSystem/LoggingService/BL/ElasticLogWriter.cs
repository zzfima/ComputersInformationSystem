using Elasticsearch.Net;
using Entities;
using Nest;

namespace LoggingService.BL
{
    public class ElasticLogWriter : ILogWriter
    {
        private ElasticClient _elasticClient;

        public ElasticLogWriter(Configuration configuration)
        {
            var nodes = new Uri[]
            {
                new Uri(configuration.ElasticHostName),
            };

            var connectionPool = new StaticConnectionPool(nodes);
            var connectionSettings = new ConnectionSettings(connectionPool).DisableDirectStreaming();
            _elasticClient = new ElasticClient(connectionSettings.DefaultIndex(configuration.ElasticIndexName));

            var indexExists = _elasticClient.Indices.Exists(configuration.ElasticIndexName);
            if (!indexExists.Exists)
            {
                var response = _elasticClient.Indices.Create(configuration.ElasticIndexName,
                   index => index.Map<LogRecord>(
                       x => x.AutoMap()));
            }
        }

        public void Write(LogRecord record)
        {
            _elasticClient.IndexDocument(record);
        }
    }
}
