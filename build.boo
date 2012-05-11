solution_file = "SubstituteApp.sln"
configuration = "release"

deploy_location = "//cn-testweb/Websites/cloudnine.nuget.test.cloudnine.se/Packages"
deploy_location_command = deploy_location.Replace("/", "\\")

target default, (compile, copy_files, nuget_deploy):
  pass

desc "Compiles the solution"
target compile:
  msbuild(file: solution_file, configuration: configuration, version: "4")
	

desc "Copy files"
target copy_files:
  print "Copying files to package"

  rmdir("build/")
  mkdir("build/")

  with FileList("SubstituteApp"):
    .Include("bin/**/SubstituteApp.exe")
    .Include("SubstituteApp.config.xml")
    .Include("web.config.config")
    .Include("web.config.substitute.xml")
    .Include("*.ps1")
    .ForEach def(file):
      file.CopyToDirectory("build/")

  print "Removing read-only"
  exec("attrib", "-R build/*.* /S /D")


target nupack:
    rmdir("nuget/")
    mkdir("nuget/")

	#nuget_pack(nuspecfile: "SubstituteApp.nuspec", outputDirectory: "nuget/")
    exec("Libraries/Phantom/lib/nuget/NuGet.exe", "pack substituteapp.nuspec /o nuget/")


target nuget_deploy, (nupack):
    print "Removing read-only"
    exec("attrib", "-R ${deploy_location_command}/*.* /S /D")

    print "Deploying files"

    with FileList("nuget"):
        .Include("*.nupkg")
        .ForEach def(file):
            file.CopyToDirectory(deploy_location)
  

    print "Removing read-only again"
    exec("attrib", "-R ${deploy_location_command}/*.* /S /D")
