FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["EmployeeManagement.API/EmployeeManagement.API.csproj", "EmployeeManagement.API/"]
COPY ["EmployeeManagement.Services/EmployeeManagement.Services.csproj", "EmployeeManagement.Services/"]
COPY ["EmployeeManagement.Data/EmployeeManagement.Data.csproj", "EmployeeManagement.Data/"]
COPY ["EmployeeManagement.AI/EmployeeManagement.AI.csproj", "EmployeeManagement.AI/"]
COPY ["EmployeeManagement.Blockchain/EmployeeManagement.Blockchain.csproj", "EmployeeManagement.Blockchain/"]
COPY ["EmployeeManagement.IoT/EmployeeManagement.IoT.csproj", "EmployeeManagement.IoT/"]
RUN dotnet restore "EmployeeManagement.API/EmployeeManagement.API.csproj"
COPY . .
WORKDIR "/src/EmployeeManagement.API"
RUN dotnet build "EmployeeManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmployeeManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagement.API.dll"]