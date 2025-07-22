FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copiar os arquivos .csproj de todos os projetos referenciados
COPY ./ProjectManager.Presentation/*.csproj ./ProjectManager.Presentation/
COPY ./ProjectManager.Application/*.csproj ./ProjectManager.Application/
COPY ./ProjectManager.Infrastructure/*.csproj ./ProjectManager.Infrastructure/
COPY ./ProjectManager.Domain/*.csproj ./ProjectManager.Domain/

# Restaurar dependências
RUN dotnet restore ./ProjectManager.Presentation/ProjectManager.Presentation.csproj

# Copiar todo o código fonte (de todos os projetos)
COPY . .

# Publicar o projeto Presentation
RUN dotnet publish ./ProjectManager.Presentation/ProjectManager.Presentation.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "ProjectManager.Presentation.dll"]
