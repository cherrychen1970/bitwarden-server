:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
set name=%1
::dotnet ef migrations add %name% --context DatabaseContext
dotnet ef migrations add %name% --context SqliteDatabaseContext

