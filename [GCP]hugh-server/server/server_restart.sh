#!/bin/bash

function clearContainers() {

  echo
  echo "##########################################################"
  echo "#########       Clear docker containers        ###########"
  echo "##########################################################"

  CONTAINER_IDS="${CONTAINER_IDS} $(docker ps -a | awk '(/hugh-server*/) {print $1}')"

  echo "CONTAINER_IDS"
  echo $CONTAINER_IDS

  if [ -z "$CONTAINER_IDS" -o "$CONTAINER_IDS" == " " ]; then
    echo "---- No containers available for deletion ----"
  else
    docker rm -f $CONTAINER_IDS
  fi
}


function removeImages() {

  echo
  echo "##########################################################"
  echo "#########         Remove docker images         ###########"
  echo "##########################################################"


  DOCKER_IMAGE_IDS="${DOCKER_IMAGE_IDS} $(docker images | awk '($1 ~ /nakama-hugh/) {print $3}')"

  echo "DOCKER_IMAGE_IDS"
  echo $DOCKER_IMAGE_IDS
  if [ -z "$DOCKER_IMAGE_IDS" -o "$DOCKER_IMAGE_IDS" == " " ]; then
    echo "---- No images available for deletion ----"
  else
    docker rmi -f $DOCKER_IMAGE_IDS
  fi
}

function nakama_game_restart() {

  echo
  echo "##########################################################"
  echo "#########       Nakama_Game_Restart        ###########"
  echo "##########################################################"

  cd hugh-server
  sudo chmod 777 nakama
  cd ..

  cd hugh-server
  docker-compose up -d
  cd ..
}


function plugin_restart() {

  echo
  echo "##########################################################"
  echo "#########       Plugin_Restart        ###########"
  echo "##########################################################"

  sudo chmod 777 hugh-server/data/modules/HughCommon.so

  cd hugh-server
  docker-compose stop
  docker-compose start
  cd ..

  cd hugh-server
  docker-compose ps
  cd ..
}

clearContainers
removeImages
nakama_game_restart
plugin_restart
