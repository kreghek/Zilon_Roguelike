#!/bin/sh

set -e
set -x

mkdir -p /test_sectorMapDrawer/maps

for i in $(seq 1 10); do
  diceSeed=$RANDOM

  /test_sectorMapDrawer/Zilon.Core/Zilon.SectorMapDrawer/Zilon.SectorMapDrawer/bin/Release/Zilon.SectorMapDrawer.exe \
    dice_seed=$diceSeed \
    scheme_catalog="/test_sectorMapDrawer/Zilon.Client/Assets/Resources/Schemes" \
    out="/test_sectorMapDrawer/maps/map-$i.bmp"
done