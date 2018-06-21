# Set the File Name
$XML_Path = "c:\AppSettings.config"
 
$xmlWriter = New-Object System.XMl.XmlTextWriter($XML_Path,$Null)
$xmlWriter.Formatting = 'Indented'
$xmlWriter.Indentation = 4
$XmlWriter.IndentChar = "`t"
$xmlWriter.WriteStartDocument()
$xmlWriter.WriteStartElement('appSettings')
$xmlWriter.WriteEndElement()
$xmlWriter.WriteEndDocument()
$xmlWriter.Flush()
$xmlWriter.Close()

# Create the Initial  Node
$xmlDoc = [System.Xml.XmlDocument](Get-Content $XML_Path);
$schemeCatalogNode = $xmlDoc.CreateElement("add")
$schemeCatalogNode.SetAttribute("key", "SchemeCatalog");
$schemeCatalogNode.SetAttribute("value", "[path]");
$xmlDoc.SelectSingleNode("//appSettings").AppendChild($schemeCatalogNode)
$xmlDoc.Save($XML_Path)
      