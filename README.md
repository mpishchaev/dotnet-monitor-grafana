# About
A deployable  k8s bundle to test dotnet-monitor in conjunction with Prometheus and Grafana

## Required apps:
  * docker
  * dotnet sdk, only if you want to build/run test application on your own

## Execute the following in the command line to host the example:
To build the test application as a docker image, pull the rest of required images from the docker registry and finally host them together as a unit do the following in the current folder:
```
docker compose up -d
```
To build the test application image do the following in the current folder:
```
docker build -t <your_registry_name>/memoryleak -f ./MemoryLeak/MemoryLeak/Dockerfile .

```
To push the built image login to docker hub first and then push the image to the hub:
```
docker login
docker push <your_registry_name>/memoryleak
```

## Application launched by docker has following urls:
  * Memory leaks application: http://localhost:8080/
  * Dotnet monitor: http://localhost:52323
    Here is the list of the endpoints in the monitor apps: https://github.com/dotnet/dotnet-monitor/tree/main/documentation
  * Prometheus: http://localhost:9090/
  * Grafana: http://localhost:3000/

## To see the results do following:
  * Open http://localhost:3000/ and login as admin/admin
  * Then go to the Configuration/Add data source and add Prometheus endpoint as one of the sources:
    * set URL to http://host.docker.internal:30090/
    * leave everything else as default
    * click Save & test
  * Go to Dashboards/Import and upload .\Grafana\sample-dotnet-monitor-dashboard.json 
  * Finally open http://localhost:8080/docs and do few calls to see the changes in the imported Grafana dashboard

## Inspired by:
 * https://dotnetos.org/blog/2021-11-22-dotnet-monitor-grafana/
 * https://habr.com/ru/company/sdventures/blog/653277/

Memory apps was taken and upgraded from: https://github.com/sebastienros/memoryleak

The fresh version of dotnet monitor was announced here: https://devblogs.microsoft.com/dotnet/announcing-dotnet-monitor-in-net-6/