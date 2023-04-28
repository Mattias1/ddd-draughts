FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS runtime

WORKDIR /app
COPY ./publish ./

RUN addgroup -S -g 1042 dkr-group && adduser -S -u 1042 dkr-user -G dkr-group
USER dkr-user

ENTRYPOINT ["dotnet", "Draughts.dll"]
