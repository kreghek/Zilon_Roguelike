#!/bin/sh

set -e

mkdir -p /test_mass_sector_generator/maps

chmod u+x /test_mass_sector_generator/Zilon.Core/Zilon.Core.MassSectorGenerator/bin/Debug/Zilon.Core.MassSectorGenerator.exe

for i in $(seq 1 100); do
  echo "======= $i ========="
  mono --debug /test_mass_sector_generator/Zilon.Core/Zilon.Core.MassSectorGenerator/bin/Debug/Zilon.Core.MassSectorGenerator.exe \
    out="/test_mass_sector_generator/maps/map-$i.bmp"
  echo "--------------------"
done