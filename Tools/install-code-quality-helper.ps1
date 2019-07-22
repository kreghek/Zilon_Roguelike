$WorkingDirectory="$PSScriptRoot\working-directory"
$ResharperCTLDirectory="$WorkingDirectory\resharper-clt"

New-Item -ItemType Directory -Force -Path $WorkingDirectory

# Установка ReSharper CLT

# Наличие папки, где должен располагаться ReSharper CTL, означает, что он уже установлен.
# Это означает, что если при какой-либо ситуации будет создана папка, но не будет загружен ReSharper,
# то при анализе будут происходить ошибки. Возможно, нужно сделать более качественную проверку ReSharper.
if (!(Test-Path -Path $ResharperCTLDirectory))
{
	New-Item -ItemType Directory -Force -Path $ResharperCTLDirectory
	New-Item -ItemType Directory -Force -Path "$WorkingDirectory\inspector-reports"
	$ResharperCTLZip="$ResharperCTLDirectory\resharper clt.zip"
	
	# Скачивание Resharper CLT
	Invoke-WebRequest https://download.jetbrains.com/resharper/ReSharperUltimate.2019.1.2/JetBrains.ReSharper.CommandLineTools.2019.1.2.zip -OutFile $ResharperCTLZip
	
	# Распаковка Resharper CLT
	Add-Type -AssemblyName System.IO.Compression.FileSystem
	function Unzip
	{
		param([string]$zipfile, [string]$outpath)

		[System.IO.Compression.ZipFile]::ExtractToDirectory($zipfile, $outpath)
	}

	Unzip $ResharperCTLZip $ResharperCTLDirectory
}

$GitHooksDirectory="..\.git\hooks"
$BackendGitHooksDirectory=".\Back GitHooks"

$GitHooks = Copy-Item $BackendGitHooksDirectory"\*" -Destination $GitHooksDirectory -Force -PassThru

# Конфигурируем хуки
$ProjectFullPath=(get-item $PSScriptRoot).parent.FullName
$Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False;
foreach($hook in $GitHooks)
{
	$Content = [IO.File]::ReadAllText($hook, $Utf8NoBomEncoding)
	
	$Content=$Content.replace('[project-path]', $ProjectFullPath)
	
	[IO.File]::WriteAllLines($hook, $Content, $Utf8NoBomEncoding)
}

echo 'Success'