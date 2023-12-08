1. Install Ubuntu
    1. Go to https://releases.ubuntu.com/
    1. Download ubuntu ISO
    1. Install on windows hyper v. 
        1. For windows prepare, read: Can use https://linuxhint.com/install-ubuntu-22-04-windows-hyper-v/
        1. For enabling virtualization on windows home edition: https://www.makeuseof.com/tag/create-virtual-machine-using-windows-10-hyper-v/
1. Ubuntu after installation
    1. Install xrdp server for remote desktop: https://www.digitalocean.com/community/tutorials/how-to-enable-remote-desktop-protocol-using-xrdp-on-ubuntu-22-04
1. Docker install: https://docs.docker.com/engine/install/ubuntu/
2. How can I use Docker without sudo? *sudo setfacl -m user:$USER:rw /var/run/docker.sock*  
1. Install RabbitMQ: https://www.cherryservers.com/blog/how-to-install-and-start-using-rabbitmq-on-ubuntu-22-04
    1. Set user for access from outside: https://stackoverflow.com/questions/23669780/rabbitmq-3-3-1-can-not-login-with-guest-guest
    1. Using docker run daemon: *sudo docker run -d rabbitmq*
    1. Using docker run manager: *sudo docker run -d --rm -it -p 15672:15672 -p 5672:5672 rabbitmq:management*
1. Install Redis
    1. Install: https://www.digitalocean.com/community/tutorials/how-to-install-and-secure-redis-on-ubuntu-20-04
    1. Install windows client: https://redis-desktop-manager.software.informer.com/0.7/
    1. Configuration redis.conf for outside. Bind to ubuntu ip address: bind 127.0.0.1 172.26.63.61
    1. Using docker run daemon: *sudo docker run -d --name redis-stack-server -p 6379:6379 redis/redis-stack-server* 
1. Install neo4j
    1. Install: https://www.digitalocean.com/community/tutorials/how-to-install-and-configure-neo4j-on-ubuntu-22-04
    1. Connect to port 7474. Default credentials: neo4j/1234
    1. Using docker: *sudo docker run -d --publish=7474:7474 --publish=7687:7687 --volume=$HOME/neo4j/data:/data neo4j* 
1. Install elastic
    1. Install: https://www.digitalocean.com/community/tutorials/how-to-install-and-configure-elasticsearch-on-ubuntu-22-04
    1. Configuration elasticsearch.yml for outside:
        1. transport.host: localhost 
        1. transport.tcp.port: 9300 
        1. http.port: 9200
        1. network.host: 0.0.0.0
    1. Docker pull: *docker pull docker.elastic.co/elasticsearch/elasticsearch:7.17.15*
    2. Docker run: *docker run -p 127.0.0.1:9200:9200 -p 127.0.0.1:9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:7.17.15*
1. Install Kibana
    1.   https://www.digitalocean.com/community/tutorials/how-to-install-elasticsearch-logstash-and-kibana-elastic-stack-on-ubuntu-22-04

1. Docker
    1. Click right mouse on project and select: Add->Docker support. Select Linux
    1. On Linux OS go to solution folder. In our case is */Documents/Sources/ComputersInformationSystem/ComputersInformationSystem*
    1. Use the following Docker CLI command to build the image by Docker Engine: *sudo docker build -t zzfima/configuration-sqlite-crud-service -f ConfigurationSqliteCRUDService/Dockerfile .* where zzfima/configuration-sqlite-crud-service is <Docker Hub ID>/<Project Name>:<Version>
    1. Check docker images: *sudo docker images*
    1. run it locally: *sudo docker run -p 5200:80 zzfima/configuration-sqlite-crud-service*
    1. Commands:
        1. docker container ls -a
        1. docker image ls
        1. docker container rm <container_id>
        1. docker image rm <image_id>
        1. docker rmi -f <image_id>
	1. run terminal on container where is 07260139810f container id: *docker exec -it 07260139810f /bin/bash*
        1. check logs of container 07260139810f: *docker logs 07260139810f*

Setup config file through ConfigurationSqliteCRUDService http://localhost:5200/swagger/index.html :

    "id"                            //any number
    "fromIPAddress"                 //start address for scanning. For example "192.168.1.10"
    "toIPAddress"                   //end address for scanning. For example "192.168.1.15"
    "userName"                      //remote PC user name. For example "admin"
    "password"                      //remote PC password. For example "1234qwer"
    "discoverFrequencyMinutes"      //Discovery frequency. Checking from the very beginning all IP addresses. For example 500
    "updateFrequencyMinutes"        //Discovery frequency. Checking only live IP addresses. For example 30
    "isToDeleteDeathRemoteMachine"  //If dead IP machine shall be deleted from DB
    
    "configurationSqliteCRUDServiceURL"     //Address of Configuration SQlite CRUD Service. For example "http://localhost:5200/"
    "remoteMachinesSqliteCRUDServiceURL"    //Address of Remote Machines SQlite CRUD Service. For example "http://localhost:5201/"
    "remoteMachinesNeo4jCRUDServiceURL"     //Address of Neo4J CRUD Service. For example "http://localhost:5202/"
    "iPsSqliteCRUDServiceURL"               //Address of IP SQlite CRUD Service. For example "http://localhost:5203"
    
    "mqVersionGatherProducerServiceURL"    //Address of producer service VersionGather. For example "http://localhost:5210/"
    "mqAliveIPGatherProducerServiceURL"    //Address of producer service IP Gather. For example "http://localhost:5211/"

    "toolsInformationSystemSchedulerServiceURL"    //Address of Scheduler Service. For example "http://localhost:5220/"

    "loggingServiceURL"                            //Address of logging service. For example "http://localhost:5230/"

    "cacheServiceURL"                              //Address of Cache service. For example "http://localhost:5240/"

    "mqHostName"                    //RabbitMQ IP address. For example "172.26.63.61"
    "mqPassword"                    //RabbitMQ password. For example "1234qwer"
    "mqUserName"                    //RabbitMQ user name. For example "user"
    "mqAliveIPGatherRoutingKey"     //RabbitMQ topic name for alive IP. For example "AliveIPGather"
    "mqVersionGatherRoutingKey"     //RabbitMQ topic name for Version. For example "VersionGather"

    "neo4jHostName"      //Neo4j IP address. For example "172.26.63.61:7687"    
    "neo4jUserName"      //Neo4j user name. For example "neo4j"
    "neo4jPassword"      //Neo4j password. For example "neo4j"

    "redisServerHostName"                          //Address of Redis. For example "172.26.63.61:6379"
    "cacheServiceTTLAbsoluteExpirationMinutes"     //Configure TTL for Redis. For example "200"

    "elasticHostName"             //Elastic IP address. For example "http://172.26.63.61:9200"
    "elasticIndexName":           //Elastic index name for logs. For example "log-versions-information"

    "installedVersions": [          //Which versions to check. For example path = "C:\Program Files\Git\", name = git-cmd.exe
    {
	  "id": 3,
	  "path": "\\c$\\Program Files\\Git\\git-bash.exe",
	  "name": "git_bash",
	  "configurationId": 1
	},
	{
	  "id": 4,
	  "path":" \\c$\\Program Files\\Git\\git-cmd.exe",
	  "name": "git_cmd",
	  "configurationId": 1
	}
    ]
 
