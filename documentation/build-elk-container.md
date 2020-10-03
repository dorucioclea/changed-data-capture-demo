## III. BUILDING THE ELASTIC STACK CONTAINER


We are going to use the `docker-compose.infrastructure.yml` file. Inside you'll find configuration to install 3 Elastic Search nodes and Kibana. All you have to do is run the following to start building your container.

    docker-compose -f docker-compose.infrastructure.yml up -d

With those few words, this builds and turns on your elastic stack container. 
With your Elastic Stack running, we are now ready to utilize it. However, at the moment, Kibana won't connect properly. This is related mostly to the authentication. We'll discuss and learn how to setup the connection with your own personal authentication.