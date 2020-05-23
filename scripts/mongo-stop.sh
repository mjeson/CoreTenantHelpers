#!/bin/bash
MNAME=mongoDB

docker stop ${MNAME}
docker rm ${MNAME}