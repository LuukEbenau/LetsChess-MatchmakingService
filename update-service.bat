@ECHO OFF
ECHO Lets update the service!

docker build -t sacation/letschess-matchmakingservice  .
docker push sacation/letschess-matchmakingservice