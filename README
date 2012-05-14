SubstituteApp
=============

Substitution of any config, text or code file before compilation.
Support of different substitutions based on build flavor (i.e. Debug, Release etc.)

Perfect for generation of complete sites including all neccesary files.
Enables you to put environment specific values under source control.

------------------------------------------------------------
Usage
------------------------------------------------------------
1.  	Create a file with the same name as the file you want to make substitutions in.
		But add an additional extension, same file extension as the file you want to generate. 
		I.e. "web.config.config", "robots.txt.txt" or "crossdomain.xml.xml"

2.	a	Create a file with your substitutions. Name it whatever you want, but
		it will contain XML-data so a XML-file would be just fine. I.e. "substitute_web.config.xml"
	b	Format the XML-file as the "example.xml" in this project. (or see XML-Format section below)

3.		Add the "SubstituteApp.exe" to you project. The "Libraries"-folder could be a suitable place.

4.		Add a "SubstituteApp.config.xml" to your project and populate it with 

5.		Add a line to your pre-build event, in your project.
		I.e. '$(SolutionDir)Libraries\SubstituteApp\SubstituteApp.exe $(ConfigurationName) SubstituteApp.config.xml $(ProjectDir)'
		The example shows when the "SubstituteApp.exe" is located in folder "\Libraries\SubstituteApp\"

6.		Exclude the result file from Source Control (NOT exclude from project).
		In VS2010 using TFS:	- Mark the file by clicking on it
								          - Go to: File->Source Control->Exclude 'YourFile' from Source Control
								          - NOTE: Your can't exclude a file from source control if it has a dependency to a not excluded file.
								            Remove dependency (edit .csproj file) or exclude the parent file too.

------------------------------------------------------------
Parameters
------------------------------------------------------------
1.		Build flavor name (use "$(ConfigurationName)")
2.		Path to SubstituteApp.config.xml
3.		Path to your project (use "$(ProjectDir)")
(4-).	If path (3.) contains whitespaces the app will believe additional parameters are sent in. 
      These will be concaternated in the app with a whitespace between.

------------------------------------------------------------
What happens
------------------------------------------------------------
The app search the template file for keys and substitutes them with the value defined 
in the substitution-xml-file.
The app also remove the readonly attribute of the target file. In order to be able to write to it.

------------------------------------------------------------
XML-Format - Config
------------------------------------------------------------
- Root node: 'substitutes'
- Child nodes: 'substitute' containing three nodes. 
  Optional attribute 'name' is added for nuget transformation support.
	- 'templatefile'
	- 'substitutefile'
	- 'resultfile'
	each containing the relative path (from projectroot) to given file.

------------------------------------------------------------
XML-Format - Substitution
------------------------------------------------------------
- Root node: 'substitution'
- One 'configuration' node for each build flavor you want to use. Attribute 'name'.
- X number of 'item' nodes under each 'configuration' node.
	- Attribute: 'key', the key to find and substitute.
	- InnerXml: the value of found key after substitution
- Each 'configuration' node needs the same amount of 'item' nodes. Else the substitution will fail.
- From version 1.2.0.0 it is possible to have subtrees instead of values in the substitute-xml.
  The subtree may however lose the xml-formatting.

- Example:
<?xml version="1.0" encoding="utf-8" ?>
<substitution>
  <configuration name="Debug">
    <item key="Key1">Value1</item>
    <item key="Key2">Value2</item>
  </configuration>
  <configuration name="Release">
    <item key="Key1">Value3</item>
    <item key="Key2">Value4</item>
  </configuration>
</substitution>

------------------------------------------------------------
Errors
------------------------------------------------------------
The app will fail if...

1.	...it isn't invoked with 3 (or more) parameters.
	An error will be found in the build log.

2.	...the substitution file doesn't have a configuration node for the given configuration.
	An error will be found in the build log and the the same error will be written to the result file.

3.	...the substitution file doesn't have the same amount of item nodes whitin each configuration node.
	An error will be found in the build log and the the same error will be written to the result file.