# dotnet-monitor with Prometheus and Grafana
Inspired by: https://dotnetos.org/blog/2021-11-22-dotnet-monitor-grafana/ </br>
Memory apps was taken and upgraded from: https://github.com/sebastienros/memoryleak </br>
The fresh version of dotnet monitor was announced here: https://devblogs.microsoft.com/dotnet/announcing-dotnet-monitor-in-net-6/

## Required apps:
  * docker
  * dotnet sdk, only if you want to build/run test application on your own

## Execute the following in the command line to host the example:
```
docker compose up
```
It builds the test application as a docker image, pulls the required images from the docker registry and finally host them together as a unit

## Application launched by docker has following urls:
  * Memory leaks application: http://localhost:8080/
  * Dotnet monitor: http://localhost:52323
    Here is the list of the endpoints in the monitor apps:
    * http://localhost:52323/processes
    * http://localhost:52323/metrics
    * http://localhost:52323/dump/{pid?}
    * http://localhost:52323/gcdump/{pid?}
    * http://localhost:52323/trace/{pid?}
    * http://localhost:52323/logs/{pid?}
  * Prometheus: http://localhost:9090/
  * Grafana: http://localhost:3000/

## To see the results do following:
  * Open http://localhost:3000/ and login as admin/admin
  * Then go to the Configuration/Add data source and add Prometheus endpoint as one of the sources:
    * set URL to http://host.docker.internal:30090/
    * leave everything else as default
    * click Save & test
  * Go to Dashboards/Import and use .\Grafana\sample-dotnet-monitor-dashboard.json
  * Finally open http://localhost:8080/docs and do few calls to see the changes in the imported Grafana dashboard