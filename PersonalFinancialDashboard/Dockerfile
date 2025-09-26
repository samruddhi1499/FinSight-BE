# Stage 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY PersonalFinancialDashboard.csproj ./
RUN dotnet restore "./PersonalFinancialDashboard.csproj"

# Copy all source code and build
COPY . .
RUN dotnet build "PersonalFinancialDashboard.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "PersonalFinancialDashboard.csproj" -c Release -o /app/publish

# Stage 2: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published output from build
COPY --from=build /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Run the app
ENTRYPOINT ["dotnet", "PersonalFinancialDashboard.dll"]
