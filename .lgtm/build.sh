#!/usr/bin/env bash

nuget restore $LGTM_SRC/Zilon.Core/Zilon.Core/Zilon.Core.csproj -SolutionDirectory $LGTM_SRC/Zilon.Core
nuget restore $LGTM_SRC/Zilon.Core/Zilon.Bot.Players/Zilon.Bot.Players.csproj -SolutionDirectory $LGTM_SRC/Zilon.Core
msbuild $LGTM_SRC/Zilon.Core/Zilon.Core/Zilon.Core.csproj /t:Build /p:Configuration=Release /p:OutputPath=../../bin
msbuild $LGTM_SRC/Zilon.Core/Zilon.Bot.Players/Zilon.Bot.Players.csproj /t:Build /p:Configuration=Release /p:OutputPath=../../bin