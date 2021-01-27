#!/bin/sh

set -e

mkdir -p /home/runner/work/Zilon_Roguelike/Zilon_Roguelike/test_mass_sector_generator/maps

$TOTAL_EXIT_CODE=0
for i in $(seq 1 100); do
  echo "======= $i ========="
  
  # https://stackoverflow.com/questions/22009364/is-there-a-try-catch-command-in-bash
  { # try
	
	set +e # disable fail if some of command is failed
	
    dotnet run -p Zilon.Core/Zilon.Core.MassSectorGenerator/Zilon.Core.MassSectorGenerator.csproj \
      --framework netcoreapp3.1 \
		--configuration Release \
		--runtime linux-x64 \
      -- out="/home/runner/work/Zilon_Roguelike/Zilon_Roguelike/test_mass_sector_generator/maps/map-$i.bmp"
		
	set -e
	
  } ||  { # catch
    if [ $? -eq 0 ]
    then
      echo "Iteration successfull"
    else
      $TOTAL_EXIT_CODE=$?
    fi
  }
    
    echo "--------------------"
done

if [ $TOTAL_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
else
  echo "Unexpected exit code $TOTAL_EXIT_CODE";
  exit 1
fi