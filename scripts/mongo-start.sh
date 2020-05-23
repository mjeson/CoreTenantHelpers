#!/bin/bash
MPORT=27017
MNAME=mongoDB
#UNAME=root
#PASSW=Secret

docker stop ${MNAME}
docker rm ${MNAME}

docker run -d -p ${MPORT}:${MPORT} --name ${MNAME} \
    mongo --noauth

# printf "Connection String: \n mongodb://${UNAME}:${PASSW}@localhost:${MPORT}"
printf "Connection String: \n mongodb://localhost:${MPORT}"