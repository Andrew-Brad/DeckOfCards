FROM microsoft/aspnetcore:2
MAINTAINER Andrew Brad
ARG source=/bin/Publish/
WORKDIR /app
ENV ASPNETCORE_URLS http://+:80
ENV ASPNETCORE_ENVIRONMENT Development
EXPOSE 80
COPY $source .
ENTRYPOINT ["dotnet", "ApiKickstart.WebApi.dll"]


