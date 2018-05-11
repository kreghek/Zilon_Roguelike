Чтобы тесты заработали:
1. В bin положить файл конфигураций.
AppSettings.config

2. Указать путь к схемам.
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
  <add key="SchemeCatalog" value="[path]"/>
</appSettings>

[path] - например, C:\PROJECTS\Zilon\Zilon.Client\Assets\Resources\Schemes