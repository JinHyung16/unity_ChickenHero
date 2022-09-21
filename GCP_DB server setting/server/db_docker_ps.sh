#!/bin/bash

function db_docker_ps() {

  echo
  echo "##########################################################"
  echo ":: Hugh-db"
  echo "##########################################################"

  cd hugh-db
  sudo docker-compose ps



  echo
  
}

db_docker_ps
