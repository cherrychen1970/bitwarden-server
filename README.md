This is bitwarden server to do self hosting without any license restrictions.

### Requirements

- [.NET Core 3.1 SDK](https://www.microsoft.com/net/download/core)
- [SQL Server 2017](https://docs.microsoft.com/en-us/sql/index)

### Database
- Install docker bitwarden/mssql
```
docker run --name=mssql --network="postgres_default" -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=T0ps3cret123.' -p 1433:1433 -v /docker/mssql/data:/var/opt/mssql/data -d bitwarden/mssql
```
- Migrate database using srv/utils/setup project

### API
It is running on 4000 by default
```
cd src/Api
dotnet run
```
- web is proxed via Api to avoid any Cors issues (don't need nginx)
- Licensing Service removed
- Self Hosted Restriction removed

## Identity 
It is running on 33656 by default
```
cd src/identity
dotnet run
```

Identity has been modified to connect your oidc provider without going through internalSso

- email claim is required to match bitwarden user.
- organization identifier claim to match organization.

in Bitwarden Identity 
```   
  "globalSettings": {
      ....
    "oidc":{
      "authority":"http://localhost:5000",
      "client":"bitwarden",
      "clientSecret":"r@ndoms3cret!",
      "organizationIdentifier":"orgId",
      "scopes":{"email","custom"}
    },    
```
in Your oidc provider
```
    # identityserver4 example

        public BitwardenClient()
        {
            ClientId = "bitwarden";
            RequireClientSecret = true;
            RequirePkce = true;
            ClientSecrets = new List<Secret> { new Secret("r@ndoms3cret!".Sha256()) };
            AllowedScopes = new string[]
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                "custom" // to provide orgId claim
            };
            AllowedGrantTypes = GrantTypes.Code;
            Enabled = true;
            RedirectUris = new List<string> { "http://localhost:33656/signin-oidc" };
            RequireConsent = false;
        }    
```