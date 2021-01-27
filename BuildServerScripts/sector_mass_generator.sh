#!/bin/sh

set -e

mkdir -p $SECTORS_OUT

TOTAL_EXIT_CODE=0
for i in $(seq 1 100); do
  echo "======= $i ========="
  
  dotnet run -p Zilon.Core/Zilon.Core.MassSectorGenerator/Zilon.Core.MassSectorGenerator.csproj \
    --framework netcoreapp3.1 \
	--configuration Release \
	--runtime linux-x64 \
    -- out="$SECTORS_OUT/map-$i.bmp"
	
  EXIT_CODE=$?
  
  if [ $EXIT_CODE -eq 0 ]; then
    echo "Iteration successfull";
  else
    TOTAL_EXIT_CODE=1;
  fi
  
  echo "--------------------"
done

if [ $TOTAL_EXIT_CODE -eq 0 ]; then
  echo "Run succeeded, no failures occurred";
else
  echo "Unexpected exit code $TOTAL_EXIT_CODE";
  exit 1
fi