#!/bin/bash

function docker_log() {

  echo
  echo "##########################################################"
  echo "#########       Docker_Log        ###########"
  echo "##########################################################"

  cd hugh-server
  sudo docker-compose logs -f --tail 10 nakama-hugh
}

docker_log
