# Поместить в Resources
# Скрипт выбирает все новые иконки. И создаёт файлы схем на основе существующей с именованиями новых иконок.

$schemePath='..\Zilon.Client\Assets\Resources\Schemes\Props\Equipments'
$visualPath='..\Zilon.Client\Assets\Resources\VisualProps'
$newPropFilePath='.\new-props.txt'
$protoScheme='short-sword'
$outDropFile='drop.txt'

foreach($propSid in Get-Content $newPropFilePath) {
    copy-item -Path "$schemePath\$protoScheme.json" -Destination "$schemePath\$propSid.json"
	(Get-Content "$schemePath\$propSid.json" -Encoding UTF8).replace('Короткий меч', "$propSid").replace('Short sword', "$propSid") | Set-Content "$schemePath\$propSid.json"
	
	copy-item -Path "$visualPath\$protoScheme.prefab" -Destination "$visualPath\$propSid.prefab"
	
	Add-Content -Path ".\$outDropFile" -Value "{ `"SchemeSid`": `"$propSid`", `"Weight`": 10},"
}