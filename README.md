# appinsight_logging

Logging with Azure Application Insight feature 

Substitute the environmental variable 'APPLICATIONINSIGHTS_CONNECTION_STRING' with the actual Azure Application Insight connection string in _launchSettings.json_:

```json
{
  "profiles": {
    "Swagger": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "swagger",
      "environmentVariables": {
        "APPLICATIONINSIGHTS_CONNECTION_STRING": "<Test Connection String>",
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "applicationUrl": "https://localhost:5001;http://localhost:5000"
    }
  }
}
```

If it's assumed to start applications in k8s cluster, an additional substitution is needed in _k8s.yaml_:

```yaml
...

apiVersion: apps/v1
kind: Deployment
metadata:
  name: appinsight-logging
spec:
  replicas: 1
  selector:
    matchLabels:
      service: webapp
  template:
    metadata:
      labels:
        app: appinsight-logging
        service: webapp
    spec:
      containers:
      - name: appinsight-logging-container
        image: appinsight-logging:dev
        imagePullPolicy: IfNotPresent
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Development"
        - name: APPLICATIONINSIGHTS_CONNECTION_STRING
          value: "<Test Connection String>"

...
```


The _Azure Application Insight connection string_ is usually of a form 'InstrumentationKey=\<some Guid-like data\>;IngestionEndpoint=\<some URL\>'

To run the application in the k8s cluster do:

```
cd src/WebAPI
docker build -t appinsight-logging:dev .
kubectl create -f k8s.yaml
```

The result is available at http://localhost:8282/swagger/index.html

Useful commands

```
kubectl delete -f k8s.yaml

kubectl get pods

kubectl exec <pod name> -- printenv

kubectl describe pod <pod name>

kubectl logs <pod name>
```
