set host=localhost
echo %1
IF "%1." NEQ "." (SET host=%1)
echo %host%
set PublishDir="\\%host%\d$\atc\bitwarden\api"
dotnet publish %~dp0 -c Release /p:PublishDir=%PublishDir%
:EOF