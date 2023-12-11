## Order of starting supporting services

Important: docker containers use a network address for communication. In this example, we use 172.22.148.100. The same address can be received by typing *ifconfig* and check *eth0*

1. Redis
    1. *sudo docker run -d --name redis-stack-server -p 6379:6379 redis/redis-stack-server*
1. neo4j
    1. *docker run -d --publish=7474:7474 --publish=7687:7687 --volume=$HOME/neo4j/data:/data neo4j*
1. RabbitMQ
    1. *sudo docker run -d rabbitmq*
    2. *sudo docker run -d --rm -it -p 15672:15672 -p 5672:5672 rabbitmq:management*
    3. Go to http://172.22.148.100:15672/#/users/guest, Set Permission
1. elastic
    1. *docker network create elastic*
    2. *docker pull docker.elastic.co/elasticsearch/elasticsearch:7.17.15*
    3. *docker run -d --name es01-test --net elastic -p 172.22.148.100:9200:9200 -p 172.22.148.100:9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:7.17.15*
1. kibana
    1. *docker pull docker.elastic.co/kibana/kibana:7.17.15*
    2. *docker run -d --name kib01-test --net elastic -p 172.22.148.100:5601:5601 -e "ELASTICSEARCH_HOSTS=http://es01-test:9200" docker.elastic.co/kibana/kibana:7.17.15*

## Order of starting 'ComputersInformationSystem' services

*for building dockers go to: *..ComputersInformationSystem/ComputersInformationSystem*

### Data Tier services:
1. ConfigurationSqliteCRUDService
    1. *docker build -t zzfima/configuration-sqlite-crud-service -f ConfigurationSqliteCRUDService/Dockerfile .* 
    2. *docker run -d -p 5200:80 zzfima/configuration-sqlite-crud-service*
    3. Running not from docker: *dotnet run  --urls=http://172.22.148.100:5200*
    4. After starting ConfigurationSqliteCRUDService PUT configuration file
1. LoggingService
    1. *docker build -t zzfima/logging-service -f LoggingService/Dockerfile .*
    2. *docker run -d -p 5230:80 zzfima/logging-service*
    3. Running not from docker: *dotnet run --urls=http://172.22.148.100:5230*
1. RemoteMachinesNeo4jCRUDService
    1. *docker build -t zzfima/remote-machines-neo4j-crud-service -f RemoteMachinesNeo4jCRUDService/Dockerfile .*
    2. *docker run -d -p 5202:80 zzfima/remote-machines-neo4j-crud-service*
1. RemoteMachinesSQLiteCRUDService
    1. *docker build -t zzfima/remote-machines-sqlite-crud-service -f RemoteMachinesSqliteCRUDService/Dockerfile .*
    2. *docker run -d -p 5201:80 zzfima/remote-machines-sqlite-crud-service*
1. IPsSQLiteCRUDService
    1. *docker build -t zzfima/ips-sqlite-crud-service -f IPsSQLiteCRUDService/Dockerfile .*
    2. *docker run -d -p 5203:80 zzfima/ips-sqlite-crud-service*
1. CacheService
    1. *docker build -t zzfima/cache-service -f CacheService/Dockerfile .*
    2. *docker run -d -p 5240:80 zzfima/cache-service*

### MQ producers/consumers services:
1. MQAliveIPGatherConsumerService
    1. *docker build -t zzfima/mq-alive-ip-gather-consumer-service -f MQAliveIPGatherConsumerService/Dockerfile .*
    2. *docker run -d -i zzfima/mq-alive-ip-gather-consumer-service* 
1. MQAliveIPGatherProducerService
    1. *docker build -t zzfima/mq-alive-ip-gather-producer-service -f MQAliveIPGatherProducerService/Dockerfile .*
    2. *docker run -d -p 5211:80 zzfima/mq-alive-ip-gather-producer-service* 
1. MQVersionGatherConsumerService
    1. *docker build -t zzfima/mq-version-gather-consumer-service -f MQVersionGatherConsumerService/Dockerfile .*
    2. *docker run -d -i zzfima/mq-version-gather-consumer-service* 
1. MQVersionGatherProducerService
    1. *docker build -t zzfima/mq-version-gather-producer-service -f MQVersionGatherProducerService/Dockerfile .*
    2. *docker run -d -p 5210:80 zzfima/mq-version-gather-producer-service* 

### Scheduler service:
1. ToolsInformationSystemSchedulerService
    1. *docker build -t zzfima/tools-information-system-scheduler-service -f ToolsInformationSystemSchedulerService/Dockerfile .*
    2. *docker run -d -p 5220:80 zzfima/tools-information-system-scheduler-service*
