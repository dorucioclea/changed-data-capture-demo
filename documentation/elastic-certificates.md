# ElasticSearch stack documentation

First, we will need to run the bellow command in order to create the certificates

```sh
> docker-compose -f create-certs.yml run --rm create_certs
```

This installs several certificates. We will be using 3 nodes and Kibana, so this installs a certificate for each. This is all configured on the create-certs.yml file.