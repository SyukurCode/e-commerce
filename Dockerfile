# ----------------------------------------
# STAGE 1: Build (in amd64 to avoid QEMU crash)
# ----------------------------------------
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
EXPOSE 80

RUN 

# Salin dan restore projek
COPY ["E-CommersAdelia.csproj", "."]
RUN dotnet restore "./E-CommersAdelia.csproj"

# Salin semua dan bina
COPY . .
RUN dotnet publish "./E-CommersAdelia.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ----------------------------------------
# STAGE 2: Runtime untuk Raspberry Pi (linux/arm64)
# ----------------------------------------
FROM --platform=linux/arm64 mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Salin dari build stage
COPY --from=build /app/publish .

# (Optional) Tukar permission untuk folder upload (jika perlu)
RUN chmod -R 777 wwwroot/Upload/Products || true

# Jalankan sebagai root atau APP_UID (jika anda set user tertentu)
# USER $APP_UID  ← hanya guna jika anda set nilai ini di luar
ENTRYPOINT ["dotnet", "E-CommersAdelia.dll"]
