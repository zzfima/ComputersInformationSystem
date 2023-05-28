## Order of starting supporting services
1. Redis
1. neo4j
1. RabbitMQ

*addresses of those services shall be used in ConfigurationSqliteCRUDService*


## Order of starting 'ComputersInformationSystem' services

### Data Tier services:
1. ConfigurationSqliteCRUDService
1. RemoteMachinesNeo4jCRUDService
1. RemoteMachinesSQLiteCRUDService
1. IPsSQLiteCRUDService
1. CacheService

### MQ producers/consumers services:
1. MQAliveIPGatherConsumerService
1. MQAliveIPGatherProducerService
1. MQVersionGatherConsumerService
1. MQVersionGatherProducerService

### Scheduler service:
1. ToolsInformationSystemSchedulerService