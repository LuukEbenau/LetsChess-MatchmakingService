docker build . -t sacation/letschess-matchmakingservice
docker push sacation/letschess-matchmakingservice
kubectl delete pods -l letschess.service=matchmakingservice -n letschess