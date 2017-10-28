@echo off
set IDN_FILE="C:\Users\Administrator\.ssh\dc_key_rsa"
set REMOTE_HOST="deploy-deb.dc.zontwelg.com"
set REMOTE_PATH="/deployment/rhms"
set SNAPSHOT_NAME="rhms-api-1.0-SNAPSHOT.jar"

scp -i %IDN_FILE% -P 2022 backend\\target\\%SNAPSHOT_NAME% root@%REMOTE_HOST%:%REMOTE_PATH%/backend-snapshot.jar
scp -i %IDN_FILE% -P 2022 backend\\Dockerfile root@%REMOTE_HOST%:%REMOTE_PATH%/Dockerfile
scp -i %IDN_FILE% -P 2022 backend\\deploy-container.sh root@%REMOTE_HOST%:%REMOTE_PATH%/deploy-container.sh

:: Done copying main files, let's run docker container
ssh -i %IDN_FILE% root@%REMOTE_HOST% -l root -p 2022 -E deployment-ssh.log '/deployment/rhms/deploy-container.sh'

pause