# **Changed data capture demo**

Warning: This is a work in progress. At the end of this journey we will have a simple but functional (and production ready) changed data capture that syncs a table in elasticsearch

The CDC sync to elasticsearch will be performed with `NEST`

**About Nest**

NEST is a high level .NET Elastic Search Client that provides strongly typed one-to-one mapping .Net queries to Elastic Search queries. It takes advantage of specific .NET features to provide higher level abstractions such as auto mapping of CLR types.

**Tested in:**
- Windows 10
- Visual Studio 2019

**Requirements:**

- [Docker Desktop for Windows](https://docs.docker.com/get-docker/)
- [Nuget Data](https://nusearch.blob.core.windows.net/dump/nuget-data-jul-2017.zip)
- Cloned this repository, locally
- 4GB RAM

## CONTENT

 1. [Preparing the Elastic Stack Container with Security Certificates](documentation/prep-elk-container.md)
 2. [Building the Elastic Stack Container](documentation/build-elk-container.md)
 3. [Securing the Elastic Stack and Changing Settings](documentation/secure-elk-container.md)
 4. [Testing Kibana](documentation/testing-kibana.md)
