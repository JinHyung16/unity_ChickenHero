#!/bin/bash

function docker_log() {

  echo
  echo "##########################################################"
  echo "#########       Docker_Log        ###########"
  echo "##########################################################"

  cd login-server
  sudo docker-compose logs -f --tail 10 hugh-login
  cd ..
}

docker_log
