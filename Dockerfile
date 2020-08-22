FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build
WORKDIR /app
COPY ./Panier.Core/*.csproj ./Panier.Core/
COPY ./Panier.Business/*.csproj ./Panier.Business/
COPY ./Panier.Entities/*.csproj ./Panier.Entities/
COPY ./Panier.UnitTests/*.csproj ./Panier.UnitTests/
COPY ./Panier.DataAccess/*.csproj ./Panier.DataAccess/
COPY ./Panier/*.csproj ./Panier/
COPY *.sln .
RUN dotnet restore
COPY . .
RUN dotnet test ./Panier.UnitTests/*.csproj
RUN dotnet publish ./Panier/*.csproj -o /publish/
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build /publish .
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT [ "dotnet","Panier.dll" ]

