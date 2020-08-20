FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build
WORKDIR /app
COPY ./Panier.Core/*.csproj ./Panier.Core/
#COPY ./YAPI.IntegrationTest/*.csproj ./YAPI.IntegrationTest/
#COPY ./Yapi.Sdk/*.csproj ./Yapi.Sdk/
#COPY ./Yapi.Sdk.Sample/*.csproj ./Yapi.Sdk.Sample/
COPY ./Panier/*.csproj ./Panier/
COPY *.sln .
RUN dotnet restore
COPY . .
#RUN dotnet test ./YAPI.IntegrationTest/*.csproj
RUN dotnet publish ./Panier/*.csproj -o /publish/
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /publish .
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT [ "dotnet","Panier.dll" ]

