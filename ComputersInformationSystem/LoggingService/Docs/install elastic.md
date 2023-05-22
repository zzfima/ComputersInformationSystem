### install elastic 
https://www.digitalocean.com/community/tutorials/how-to-install-and-configure-elasticsearch-on-ubuntu-18-04

Only change next config:
* allow remote access: *network.host: 0.0.0.0*
* IP address of the computer that is running elastic: *cluster.initial_master_nodes: ["192.168.0.105"]*

### Download manager

Install kibana: https://www.digitalocean.com/community/tutorials/how-to-install-elasticsearch-logstash-and-kibana-elastic-stack-on-ubuntu-18-04
