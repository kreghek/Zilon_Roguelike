dotnet publish Zilon.Core/Zilon.Core/Zilon.Core.csproj -c Release -o Zilon.Client/Assets/Plugins/Zilon.Core -f netstandard2.0 --verbosity minimal
dotnet publish Zilon.Core/Zilon.Bot.Players/Zilon.Bot.Players.csproj -c Release -o Zilon.Client/Assets/Plugins/Zilon.Core -f netstandard2.0 --verbosity minimal
pause