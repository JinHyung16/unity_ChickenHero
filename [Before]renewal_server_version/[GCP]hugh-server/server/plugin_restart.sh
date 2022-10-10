#!/bin/bash

function plugin_restart() {

  echo
  echo "##########################################################"
  echo "#########       Plugin_Restart        ###########"
  echo "##########################################################"

  sudo chmod 777 hugh-server/data/modules/HughCommon.so

  cd hugh-server
  docker-compose stop
  docker-compose start
}

plugin_restart
