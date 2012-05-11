# First some common params, delivered by the nuget package installer
param($installPath, $toolsPath, $package, $project)

# Get project path
$path = [System.IO.Path]
$projectpath = $path::GetDirectoryName($project.FileName)

# Copy content from old web.config -> web.config.config
$webconfig = $path::Combine($projectpath, "web.config")
$webconfigconfig = $path::Combine($projectpath, "web.config.config")
copy $webconfig $webconfigconfig

# Update the PreBuildEvent
$project.Properties.Item("PreBuildEvent").Value = "SubstituteApp `$(ConfigurationName) SubstituteApp.config.xml `$(ProjectDir)"

# Get the build project of type [Microsoft.Build.Evaluation.Project]
$buildProject = Get-Project $project.ProjectName | % {
            $path = $_.FullName
            @([Microsoft.Build.Evaluation.ProjectCollection]::GlobalProjectCollection.GetLoadedProjects($path))[0]
        }

# Find the Content node for "web.config.substitute.xml"
$webconfigsubstitutenode = $buildProject.Xml.ItemGroups | foreach {$_.Items} | Where-Object {$_.Include -match "web.config.substitute.xml"}

# Add a dependency to "web.config.config" for "web.config.substitute.xml"
$webconfigsubstitutenode.AddMetaData("DependentUpon", "web.config.config")

# Persists the changes in project
$project.Save() 