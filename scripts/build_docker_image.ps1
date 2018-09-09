$invocation = (Get-Variable MyInvocation).Value
$directoryPath = Split-Path $invocation.MyCommand.Path
$PublishFolderName = "Publish"
$ProjectName = "ApiKickstart.WebApi"
$DockerTag = "latest"
$DockerImageName = "apikickstart:$DockerTag" #no special characters
$PathToProjectRelativeToSolution = "src/Web/ApiKickstart.WebApi/"

#Clean publish directory and rebuild in release mode
cd $directoryPath
cd ..
Get-ChildItem ./ -include $PublishFolderName -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
dotnet publish $PathToProjectRelativeToSolution/$ProjectName.csproj -c Release -o bin/$PublishFolderName

#Docker workflow
docker build $PathToProjectRelativeToSolution/ -t $DockerImageName --rm
docker save $DockerImageName --output ./$PathToProjectRelativeToSolution/bin/$PublishFolderName/$ProjectName



#Echo [io.path]::combine($directoryPath, '..')
#Get-ChildItem $directoryPath -include Publish -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }
#Echo $directoryPath


