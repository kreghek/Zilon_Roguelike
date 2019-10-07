#!/bin/sh

set -e
set -x

chmod u+x /test_mass_sector_generator/Zilon.Core/Zilon.Core.MassSectorGenerator/bin/Debug/Zilon.Core.MassSectorGenerator.exe

mono --debug /test_mass_sector_generator/Zilon.Core/Zilon.Core.MassSectorGenerator/bin/Debug/Zilon.Core.MassSectorGenerator.exe