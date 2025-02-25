FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HoopsVsBeans.sln", "./"]
COPY ["HoopsVsBeans/HoopsVsBeans.csproj", "HoopsVsBeans/"]

RUN dotnet restore "HoopsVsBeans/HoopsVsBeans.csproj"

COPY . .

WORKDIR "/src/HoopsVsBeans"
RUN dotnet publish "HoopsVsBeans.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app .

EXPOSE 8080

ENTRYPOINT ["dotnet", "HoopsVsBeans.dll"]
