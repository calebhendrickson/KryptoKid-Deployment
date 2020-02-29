FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 
COPY KryptoKid/bin/BNB/Release/netcoreapp3.1/publish/ app/

ENTRYPOINT ["dotnet", "app/KryptoKid.dll"]
