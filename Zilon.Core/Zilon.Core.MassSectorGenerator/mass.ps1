# Разместить в папке с собранным генератором. Например, в Debug\bin
Set-PSDebug -Trace 1
For ($i=0; $i -le 100; $i++)
{
  echo "======= $i ========="
  $outPath "[путь к папки с изображениями]\map-"+$i+".bmp"
  echo $outPath
  .\Zilon.Core.MassSectorGenerator.exe -out=$outPath
  echo "-------------------"
}