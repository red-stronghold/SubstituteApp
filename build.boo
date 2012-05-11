import System.IO

solution_file = "SubstituteApp.sln"
project_dir   = "SubstituteApp"
build_folder  = "build/"
nuget_folder  = "nuget/"
build_config  = env('build.config')
build_output  = env('build.output')

target default, (compile):
   pass
   
target artifact, (compile, copy_files):
   pass
   
target nuget, (compile, copy_files, nuget_push):
   pass

target compile:
   msbuild(file: solution_file, configuration: build_config, version: "4")

target nuget_pack:
    rmdir(nuget_folder)
    mkdir(nuget_folder)

    exec("Libraries/Phantom/lib/nuget/NuGet.exe", "pack substituteapp.nuspec /o ${nuget_folder}")

target nuget_push, (nuget_pack):
    exec("Libraries/Phantom/lib/nuget/NuGet.exe", "push ${nuget_folder}SubstituteApp.2.1.2.nupkg")


target prepare_folders:
   exec("attrib", "-R ${project_dir}/** /S /D")
   rmdir(build_folder)
   mkdir(build_folder)

desc "Copy files"
target copy_files, (prepare_folders):
  print "Copying files to package"

  with FileList(project_dir):
    .Include("bin/${build_config}/SubstituteApp.exe")
    .Include("SubstituteApp.config.xml")
    .Include("web.config.config")
    .Include("web.config.substitute.xml")
    .Include("*.ps1")
    .ForEach def(file):
      file.CopyToDirectory(build_folder)
