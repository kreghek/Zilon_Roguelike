#!/bin/sh

mkdir -p /test_sectorMapDrawer/maps

for i in $(seq 1 10); do
  diceSeed=$RANDOM

  ../Zilon.Core/Zilon.SectorMapDrawer/Zilon.SectorMapDrawer/bin/Release/Zilon.SectorMapDrawer.exe \
    dice_seed=$diceSeed \
    scheme_catalog="./Zilon.Client/Assets/Resources/Schemes" \
    out="/test_sectorMapDrawer/maps/map-$i.bmp"
done