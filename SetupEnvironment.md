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