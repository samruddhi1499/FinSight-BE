FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY PersonalFinancialDashboard/PersonalFinancialDashboard.csproj ./PersonalFinancialDashboard.csproj
RUN dotnet restore "./PersonalFinancialDashboard.csproj"

# Copy all source code into the build directory
COPY PersonalFinancialDashboard/. ./PersonalFinancialDashboard/
WORKDIR ./PersonalFinancialDashboard

# Build and publish
RUN dotnet build "PersonalFinancialDashboard.csproj" -c Release -o /app/build
RUN dotnet publish "PersonalFinancialDashboard.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ARG PORT=5000
ENV ASPNETCORE_URLS=http://*:${PORT}


ENTRYPOINT ["dotnet", "PersonalFinancialDashboard.dll"]
