#!/bin/sh

set -e
set -x

mkdir -p /test_sectorMapDrawer/maps

chmod u+x /test_sectorMapDrawer/Zilon.Core/Zilon.SectorMapDrawer/bin/Debug/Zilon.SectorMapDrawer.exe

for i in $(seq 1 1000); do
  mono --debug /test_sectorMapDrawer/Zilon.Core/Zilon.SectorMapDrawer/bin/Debug/Zilon.SectorMapDrawer.exe \
    scheme_catalog="/test_sectorMapDrawer/Zilon.Client/Assets/Resources/Schemes" \
    out="/test_sectorMapDrawer/maps/map-$i.bmp"
done