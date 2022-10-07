FROM debian:buster-slim

RUN apt-get update && \
    apt-get -y upgrade && \
    apt-get install -y --no-install-recommends ca-certificates=20200601~deb10u2 tzdata curl iproute2 unzip rsync git tini schroot

COPY . /hugh-login
WORKDIR /hugh-login
EXPOSE 7349 7350 7351
