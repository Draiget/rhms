#!/bin/bash

cd /deployment
docker build -t rhms_api ./rhms
docker run -d rhms_api --name rhms-api