#!/bin/bash

function stop_containers() {
  
  echo
  echo "#########################################################"
  echo "#########        Stop Docker Containers         #########"
  echo "#########################################################"

  cd login-server
  docker-compose stop
  cd ..
}

function clear_containers() {

  echo
  echo "##########################################################"
  echo "#########       Clear Docker Containers        ###########"
  echo "##########################################################"

  CONTAINER_IDS="${CONTAINER_IDS} $(docker ps -a | awk '(/login-server*/) {print $1}')"

  echo "CONTAINER_IDS"
  echo $CONTAINER_IDS

  if [ -z "$CONTAINER_IDS" -o "$CONTAINER_IDS" == " " ]; then
    echo "---- No containers available for deletion ----"
  else
    docker rm -f $CONTAINER_IDS
  fi
}


function remove_images() {

  echo
  echo "##########################################################"
  echo "#########         Remove Docker Images         ###########"
  echo "##########################################################"


  DOCKER_IMAGE_IDS="${DOCKER_IMAGE_IDS} $(docker images | awk '($1 ~ /hugh-login/) {print $3}')"

  echo "DOCKER_IMAGE_IDS"
  echo $DOCKER_IMAGE_IDS
  if [ -z "$DOCKER_IMAGE_IDS" -o "$DOCKER_IMAGE_IDS" == " " ]; then
    echo "---- No images available for deletion ----"
  else
    docker rmi -f $DOCKER_IMAGE_IDS
  fi
}

function start_docker() {

  echo
  echo "##########################################################"
  echo "#########         Up docker containers         ###########"
  echo "##########################################################"

  cd login-DB
  sudo chmod -R 777 db
  cd ..

  cd login-server
  sudo chmod 777 nakama
  cd ..

  cd login-server
  docker-compose up -d
  cd ..
}

function state_docker() {
  
  echo
  echo "############################################################"
  echo "##########         PS Docker Containers           ##########"
  echo "############################################################"

  cd login-DB
  docker-compose ps
  cd ..

  cd login-server
  docker-compose ps
  cd ..
}

stop_containers
clear_containers
remove_images
start_docker
state_docker

