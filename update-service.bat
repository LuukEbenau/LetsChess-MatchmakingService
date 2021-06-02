@ECHO OFF
ECHO Lets update the service!
cd ../
docker build -t sacation/letschess-matchmakingservice  . -f LetsChess-MatchmakingService/Dockerfile
docker push sacation/letschess-matchmakingservice