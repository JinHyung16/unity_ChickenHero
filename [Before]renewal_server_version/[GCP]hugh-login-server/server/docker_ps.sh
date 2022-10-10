#!/bin/bash

function all_docker_ps() {

  echo
  echo "##########################################################"
  echo "#########             All_Docker_PS            ###########"
  echo "##########################################################"

  echo
  echo "********************Login-DB-Server********************"
  cd login-DB
  docker-compose ps
  cd ..

  echo
  echo "********************Login-Server******************"
  cd login-server
  docker-compose ps
  cd ..
}

all_docker_ps
