## Build/Run

### Requirements

- [.NET Core 3.1 SDK](https://www.microsoft.com/net/download/core)
- [SQL Server 2017](https://docs.microsoft.com/en-us/sql/index)

### API

```
cd src/Api
dotnet restore
dotnet build
dotnet run
```

## My Note

- From Api : proxy web, identity
- Replace Licensing Service With Noop Service
- Install docker bitwarden/mssql
```
docker run --name=mssql --network="postgres_default" -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=T0ps3cret123.' -p 1433:1433 -v /docker/mssql/data:/var/opt/mssql/data -d bitwarden/mssql
```
- Migrate database using srv/utils/setup project
- Remove Self Hosted Restriction in OrganizationController
- Set Seats Null, Set MaxCollections Null

## SSO
update SsoConfig set data='{"configType":1,"authority":"http://localhost:5000","clientId":"bitwarden","clientSecret":"random value!"}' where id=1