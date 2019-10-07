#!/bin/sh

set -e
set -x

mkdir -p /test_sectorMapDrawer/maps

chmod u+x /test_sectorMapDrawer/Zilon.Core/Zilon.SectorMapDrawer/bin/Release/Zilon.SectorMapDrawer.exe

for i in $(seq 1 10); do
  diceSeed="$(od -An -N2 -i /dev/random)"

  mono --debug /test_sectorMapDrawer/Zilon.Core/Zilon.SectorMapDrawer/bin/Release/Zilon.SectorMapDrawer.exe \
    dice_seed="$(diceSeed)" \
    scheme_catalog="/test_sectorMapDrawer/Zilon.Client/Assets/Resources/Schemes" \
    out="/test_sectorMapDrawer/maps/map-$i.bmp"
done