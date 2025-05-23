# LaWashSmartParking Solution

This document describes the solution structure and provides instructions to run the **API** and **Simulator** projects.

## Prerequisites

- **.NET SDK**: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (recommended) or [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0).
- **IDE/Editor**: Visual Studio 2022 (or newer), Visual Studio Code, or JetBrains Rider.
- **Database**: Local or remote SQL Server (if using EF Core in the API).

## Setup and Execution Steps

### 1. Clone the Repository

```bash
git clone https://github.com/ingjalexandercastro/LaWashSmartParking.git
cd LaWashSmartParking
```

### 2. Restore Dependencies

```bash
cd LaWashSmartParking.API
dotnet restore

cd ../LaWashSmartParking.Simulator
dotnet restore
``` 

### 3. Configure Connections

- **API**: Edit `LaWashSmartParking.API/appsettings.json` and set `ConnectionStrings:DefaultConnection`.
- **Simulator**: Check in `LaWashSmartParking.Simulator/appsettings.json` that `SimulatorSettings:ApiBaseUrl` points to `http://localhost:5209`.

### 4. Run the API

```bash
cd LaWashSmartParking.API
dotnet run --urls "http://localhost:5209"
``` 

### 5. Run the Simulator

```bash
cd LaWashSmartParking.Simulator
dotnet run
```
