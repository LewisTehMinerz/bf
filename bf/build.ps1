$platforms = @("win-x86", "win-x64", "linux-x64", "osx-x64")

foreach ($platform in $platforms) {
    dotnet publish -r $platform -c Debug /p:PublishSingleFile=true
    7z a artifacts/bf-$platform.7z bin/Debug/netcoreapp3.0/$platform/publish/bf*
}

copy-item bin/Debug/netcoreapp3.0/bf.dll artifacts/bf.dll