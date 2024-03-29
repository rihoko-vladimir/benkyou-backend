
# Benkyou! Backend

Benkyou! helps you create kanji sets and learn new kanji with readings by matching correct ones


## Changing project configuration

Firstly you need to change API keys for SendGrid, Azure services, change Connection strings for database at [docker-compose.yml](https://github.com/rihoko-vladimir/benkyou-backend/blob/main/docker-compose.yaml)

```yaml
  version: '3.4'

services:
  notification-api:
    build:
      context: .
      dockerfile: Notification.Api/Dockerfile
    ports:
      - "8080:80"
      - "8081:443"
    environment:
      ASPNETCORE_URLS : "https://+:443;http://+:80"
      ASPNETCORE_HTTPS_PORT : 8081
      ASPNETCORE_ENVIRONMENT : Development
      AZURE_TENANT_ID: ***TENANT_ID***
      AZURE_CLIENT_ID: ***CLIENT_ID***
      AZURE_CLIENT_SECRET: ***CLIENT_SECRET***
      APP_EMAILCONFIGURATION__APIKEY: ***SENDGRID_API***
      APP_EMAILCONFIGURATION__SOURCE: ***EMAIL_DOMAIN***
      APP_EMAILCONFIGURATION__SOURCENAME: "Benkyou! Support"
    volumes:
      - "./Certificates:/Certificates"
    depends_on:
      - rabbitmq
    networks:
      - broker


  health-api:
    build:
      context: .
      dockerfile: Health.Api/Dockerfile
    ports:
      - "9080:80"
      - "9081:443"
    environment:
      ASPNETCORE_URLS: "https://+:443;http://+:80"
      ASPNETCORE_HTTPS_PORT: 9081
      ASPNETCORE_ENVIRONMENT : Development
    depends_on:
      - users-api
      - auth-api
      - statistics-api
      - gateway-api
      - sets-api
      - notification-api
    volumes:
      - "./Certificates:/Certificates"
    networks:
      - broker
      - database


  auth-api:
    build:
      context: .
      dockerfile: Auth.Api/Dockerfile
    ports:
      - "7080:80"
      - "7081:443"
    environment:
      ASPNETCORE_URLS: "https://+:443;http://+:80"
      ASPNETCORE_HTTPS_PORT: 7081
      AZURE_TENANT_ID: ***TENANT_ID***
      AZURE_CLIENT_ID: ***CLIENT_ID***
      AZURE_CLIENT_SECRET: ***CLIENT_SECRET***
      ASPNETCORE_ENVIRONMENT : Development
    volumes:
      - "./Certificates:/Certificates"
    depends_on:
      - rabbitmq
      - sql-server
    networks:
      - broker
      - database
  
  
  users-api:
    build:
      context: .
      dockerfile: Users.Api/Dockerfile
    ports:
      - "6080:80"
      - "6081:443"
    environment:
      ASPNETCORE_URLS: "https://+:443;http://+:80"
      ASPNETCORE_HTTPS_PORT: 6081
      AZURE_TENANT_ID: ***TENANT_ID***
      AZURE_CLIENT_ID: ***CLIENT_ID***
      AZURE_CLIENT_SECRET: ***CLIENT_SECRET***
      ASPNETCORE_ENVIRONMENT: Development
    volumes:
      - "./Certificates:/Certificates"
    depends_on:
      - rabbitmq
      - sql-server
      - azurite
    networks:
      - broker
      - database
    
    
  sets-api:
    build:
      context: .
      dockerfile: Sets.Api/Dockerfile
    ports:
      - "10080:80"
      - "10081:443"
    environment:
      ASPNETCORE_URLS: "https://+:443;http://+:80"
      ASPNETCORE_HTTPS_PORT: 10081
      AZURE_TENANT_ID: TENANT_ID
      AZURE_CLIENT_ID: CLIENT_ID
      AZURE_CLIENT_SECRET: CLIENT_SECRET
      ASPNETCORE_ENVIRONMENT: Development
    volumes:
      - "./Certificates:/Certificates"
    depends_on:
      - rabbitmq
      - sql-server
    networks:
      - broker
      - database
        
        
  statistics-api:
    build:
      context: .
      dockerfile: Statistics.Api/Dockerfile
    ports:
      - "1080:80"
      - "1081:443"
    environment:
      ASPNETCORE_URLS: "https://+:443;http://+:80"
      ASPNETCORE_HTTPS_PORT: 1081
      ASPNETCORE_ENVIRONMENT: Development
    volumes:
      - "./Certificates:/Certificates"
    depends_on:
      - rabbitmq
      - mongo-db
    networks:
      - broker
      - database
  
  gateway-api:
    build:
      context: .
      dockerfile: Gateway.Api/Dockerfile
    ports:
      - "3080:80"
      - "3081:443"
    environment:
      ASPNETCORE_URLS: "https://+:443;http://+:80"
      ASPNETCORE_HTTPS_PORT: 3081
      AZURE_TENANT_ID: ***TENANT_ID***
      AZURE_CLIENT_ID: ***CLIENT_ID***
      AZURE_CLIENT_SECRET: ***CLIENT_SECRET***
      ASPNETCORE_ENVIRONMENT: Development
    volumes:
      - "./Certificates:/Certificates"
    depends_on:
      - rabbitmq
    networks:
      - broker
      - database
        
      
  rabbitmq:
    image: rabbitmq:3-management-alpine
    container_name: RabbitMQ
    ports:
      - "5672:5672"
      - "15672:15672"
    restart: always
    networks:
      - broker
      - database
        
  server-initializer:
    container_name: DatabaseInitializer
    image: mcr.microsoft.com/mssql-tools
    volumes:
      - ./Scripts/:/usr/src/sql/
    command: ["/bin/sh", "/usr/src/sql/initialize.sh"]
    network_mode: service:sql-server
    
  sql-server:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: Database
    ports:
      - "1433:1433"
    environment:
      ACCEPT_EULA: Y
      MSSQL_SA_PASSWORD: nandesukaanatawa1A
    networks:
      - database
      
  azurite:
    container_name: azurite
    image: mcr.microsoft.com/azure-storage/azurite:latest
    ports:
      - "10000:10000"
      - "10001:10001"
      - "10002:10002"
    restart: unless-stopped
  
  mongo-db:
    image: mongo:latest
    container_name: StatisticsDatabase
    ports:
      - "27017:27017"
    environment:
      MONGO_INITDB_ROOT_USERNAME: rihoko
      MONGO_INITDB_ROOT_PASSWORD: nandesukaanatawa1A
    networks:
      - database
        
  elasticsearch:
    container_name: elasticsearch
    image: elasticsearch:8.6.2
    environment:
      - xpack.security.enabled=false
      - "discovery.type=single-node"
    networks:
      - es-net
    ports:
      - "9200:9200"
    volumes:
      - "./ELK/ElasticSearch/roles.yml:/usr/share/elasticsearch/config/roles.yml"
      - "./ELK/ElasticSearch/users:/usr/share/elasticsearch/config/users"
      - "./ELK/ElasticSearch/users_roles:/usr/share/elasticsearch/config/users_roles"
  
  
  kibana:
    container_name: kibana
    image: kibana:8.6.2
    environment:
      - ELASTICSEARCH_HOSTS=http://elasticsearch:9200
    networks:
      - es-net
    depends_on:
      - elasticsearch
    ports:
      - "5601:5601"
    
  logstash:
    image: logstash:8.6.2
    container_name: logstash
    ports:
      - "9600:9600"
      - "9700:9700"
      - "9602:9602"
    depends_on:
      - elasticsearch
    networks:
      - es-net
    volumes:
      - "./ELK/LogStash/PipelineConfig/http-pipeline.conf:/usr/share/logstash/pipeline/logstash.conf"
      - "./ELK/LogStash/LogStashConfig/logstash.yml:/usr/share/logstash/config/logstash.yml"
    
networks:
  broker:
    external: false
  
  database:
    external: false
    
  es-net:
    external: false
```

## It's important to configure your environment variables in [EnvironmentFiles](https://github.com/rihoko-vladimir/benkyou-backend/tree/main/EnvironmentFiles) folder

## Deploy project using docker-compose

Type the following to run the project:

```bash
docker compose up --build
```


## Tech Stack

**Server:** .NET 7, EF Core, Azure Services, Docker, Docker-Compose, Dapper, Fluent Validation, Automapper, MassTransit, MongoDB, Ocelot, Polly, SendGrid, TestContainers, Moq, xUnit


## Authors

- [@rihoko-vladimir](https://github.com/rihoko-vladimir)

