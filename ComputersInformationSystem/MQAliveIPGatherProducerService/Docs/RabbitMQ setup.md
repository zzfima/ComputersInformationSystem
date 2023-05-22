# Guide RabbitMQ on ubuntu 18.04

## based on https://computingforgeeks.com/how-to-install-latest-rabbitmq-server-on-ubuntu-linux/

### Install Erlang:
* *sudo apt update*
* *sudo apt install curl software-properties-common apt-transport-https lsb-release*
* *curl -fsSL https://packages.erlang-solutions.com/ubuntu/erlang_solutions.asc | sudo gpg --dearmor -o /etc/apt/trusted.gpg.d/erlang.gpg*
* *echo "deb https://packages.erlang-solutions.com/ubuntu $(lsb_release -cs) contrib" | sudo tee /etc/apt/sources.list.d/erlang.list*
* *sudo apt update*
* *sudo apt install erlang*

Test erlang: $ erl

### Add RabbitMQ Repository to Ubuntu
* curl -s https://packagecloud.io/install/repositories/rabbitmq/rabbitmq-server/script.deb.sh | sudo bash

### Install RabbitMQ Server
* *sudo apt update*
* *sudo apt install rabbitmq-server*
* check the status of RabbitMQ service: *systemctl status rabbitmq-server.service*
* check the service is configured to start on boot: *systemctl is-enabled rabbitmq-server.service*
* enable it: *sudo systemctl enable rabbitmq-server*

### Enable the RabbitMQ Management Dashboard
* enable the RabbitMQ Management Web dashboard for easy management: *sudo rabbitmq-plugins enable rabbitmq_management*
* check ports: *sudo ss -tunelp | grep 15672*

### Management
*  If you have an active UFW firewall, open both ports 5672 and 15672: *sudo ufw allow proto tcp from any to any port 5672,15672*
* Go to RabbitMQ Management Dashboard: *http://[server IP|Hostname]:15672*
* User and password (for example user name is 'momo' and password is 'koko123'):
    * create new user 'momo' with password 'koko123': *sudo rabbitmqctl add_user momo koko123*
    * add it to admins: *sudo rabbitmqctl set_user_tags momo administrator*
    * delete User 'momo': *rabbitmqctl delete_user momo*
    * change User 'momo' password to 'koko567' *rabbitmqctl change_password momo koko567*