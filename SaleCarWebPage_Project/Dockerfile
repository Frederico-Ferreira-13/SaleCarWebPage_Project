FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SaleCarWebPage_Project.csproj", "./"]
RUN dotnet restore "SaleCarWebPage_Project.csproj"
COPY . . 
WORKDIR "/src"
RUN dotnet publish "SaleCarWebPage_Project.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SaleCarWebPage_Project.dll"]