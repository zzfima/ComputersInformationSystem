1. Install Ubuntu
    1. Go to https://releases.ubuntu.com/
    1. Download ubuntu ISO
    1. Install on windows hyper v. 
        1. For windows prepare, read: Can use https://linuxhint.com/install-ubuntu-22-04-windows-hyper-v/
        1. For enabling virtualization on windows home edition: https://www.makeuseof.com/tag/create-virtual-machine-using-windows-10-hyper-v/
1. Ubuntu after installation
    1. Install xrdp server for remote desktop: https://www.digitalocean.com/community/tutorials/how-to-enable-remote-desktop-protocol-using-xrdp-on-ubuntu-22-04
1. Install RabbitMQ: https://www.cherryservers.com/blog/how-to-install-and-start-using-rabbitmq-on-ubuntu-22-04
    1. Set user for access from outside: https://stackoverflow.com/questions/23669780/rabbitmq-3-3-1-can-not-login-with-guest-guest
1. Install Redis
    1. Install: https://www.digitalocean.com/community/tutorials/how-to-install-and-secure-redis-on-ubuntu-20-04
    1. Install windows client: https://redis-desktop-manager.software.informer.com/0.7/
    1. Configuration redis.conf for outside: bind 127.0.0.1 172.26.63.61
1. Install neo4j
    1. Install: https://www.digitalocean.com/community/tutorials/how-to-install-and-configure-neo4j-on-ubuntu-22-04
    1. Connect to port 7474
1. Install elastic
    1. Install: https://www.digitalocean.com/community/tutorials/how-to-install-and-configure-elasticsearch-on-ubuntu-22-04
    1. Configuration elasticsearch.yml for outside:
        1. transport.host: localhost 
        1. transport.tcp.port: 9300 
        1. http.port: 9200
        1. network.host: 0.0.0.0

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

    "elasticHostName"             //Elastic IP address. For example "172.26.63.61:9200"
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
 