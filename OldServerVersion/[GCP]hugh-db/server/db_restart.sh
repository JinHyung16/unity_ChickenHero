#!/bin/bash

function db_restart() {

  echo
  echo "##########################################################"
  echo "#########       Database_Restart        ###########"
  echo "##########################################################"

      
  cd hugh-db
  sudo docker-compose stop
  sudo docker-compose start
 
}

db_restart
