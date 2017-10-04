FROM microsoft/dotnet

WORKDIR /app

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out

WORKDIR /app/HROSharingBot/out
ENTRYPOINT ["dotnet", "HROSharingBot.dll"]