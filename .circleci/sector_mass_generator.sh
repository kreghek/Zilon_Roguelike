#!/bin/sh

set -e

mkdir -p /test_mass_sector_generator/maps

chmod u+x /test_mass_sector_generator/Zilon.Core/Zilon.Core.MassSectorGenerator/bin/Debug/Zilon.Core.MassSectorGenerator.exe

TOTAL_EXIT_CODE=0
for i in $(seq 1 100); do
  echo "======= $i ========="
  
  mono --debug /test_mass_sector_generator/Zilon.Core/Zilon.Core.MassSectorGenerator/bin/Debug/Zilon.Core.MassSectorGenerator.exe \
    out="/test_mass_sector_generator/maps/map-$i.bmp"
	
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
  echo "Unexpected exit code $UNITY_EXIT_CODE";
  exit 1
fi