using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SubstituteApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if(args.Length < 3)
            {
                Console.WriteLine("Missing arguments, when invoking substitute application.");
                Console.WriteLine("Arguments required:");
                Console.WriteLine("- Configuration");
                //Console.WriteLine("- ResultFileName");
                Console.WriteLine("- SubstitutionConfigurationFileName");
                Console.WriteLine("- ProjectPath");
                Environment.Exit(-1);
                return;
            }
            var configuration = args[0];

            var projectPath = string.Empty;

            // If given project path contains spaces
            for (var i = 2; i < args.Length; i++)
            {
                if (!string.IsNullOrEmpty(projectPath))
                {
                    projectPath += " ";
                }
                projectPath += args[i];
            }

            var substituteHolderList = GetSubstituteHolders(args[1], projectPath);

            var hasError = false;

            foreach (var substituteHolder in substituteHolderList)
            {

                CheckIfFileExistsAndCreateItIfNot(substituteHolder.ResultFilePath);
                // Remove ReadOnly attribute from resultfile
                RemoveReadOnly(substituteHolder.ResultFilePath);

                IEnumerable<XElement> configurationElements = GetConfigurationElements(substituteHolder.SubstituteFilePath);

                // error check
                if(!AllConfigurationHasSameNumberOfItems(configurationElements))
                {
                    var error = "Not all configurations has the same number of substitutions. Check " +
                                 substituteHolder.SubstituteFileName;
                    Console.WriteLine(error);
                    SetContent(substituteHolder.ResultFilePath, error);
                    hasError = true;
                    continue;
                }

                var configurationNode = configurationElements.FirstOrDefault(n => n.Attribute("name").Value == configuration);

                if (configurationNode == null)
                {
                    var error = "No substitutions found for configuration. Check " + substituteHolder.SubstituteFileName;
                    Console.WriteLine(error);
                    SetContent(substituteHolder.ResultFilePath, error);
                    hasError = true;
                    continue;
                }

                var text = string.Empty;
                using (var streamReader = new StreamReader(substituteHolder.TemplateFilePath))
                {
                    text = streamReader.ReadToEnd();
                }

                var result = Substitute(text, configurationNode);

                RemoveReadOnly(substituteHolder.ResultFilePath);

                Console.WriteLine("Substituting...");
                Console.WriteLine("Configuration:" + configuration);
                Console.WriteLine("Contents from:" + substituteHolder.TemplateFilePath);
                Console.WriteLine("Substitutions in:" + substituteHolder.SubstituteFilePath);
                Console.WriteLine("Writes to:" + substituteHolder.ResultFilePath);

                SetContent(substituteHolder.ResultFilePath, result);
                
            }

            if(hasError)
            {
                Console.WriteLine("There were some errors!");
                Environment.Exit(-1);
            }
            else
            {
                Console.WriteLine("Done");
            }

        }

        private static void CheckIfFileExistsAndCreateItIfNot(string filePath)
        {
            if(!File.Exists(filePath))
            {
                var fileStream = File.Create(filePath);
                fileStream.Dispose();
            }
        }

        private static bool AllConfigurationHasSameNumberOfItems(IEnumerable<XElement> configurationElements)
        {
            var length = -1;
            if (configurationElements.Count() > 1)
            {
                foreach (var xElement in configurationElements)
                {
                    if (length != -1 && length != xElement.Elements("item").Count())
                    {
                        return false;
                    }
                    length = xElement.Elements("item").Count();
                }
            }
            return true;
        }

        private static IEnumerable<XElement> GetConfigurationElements(string substitutionsFilePath)
        {
            var substitutionXml = XDocument.Load(File.OpenRead(substitutionsFilePath));

            return substitutionXml.Root.Elements("configuration");
        }

        private static void RemoveReadOnly(string resultFilePath)
        {
            if (GetFileAttribute(resultFilePath, FileAttributes.ReadOnly))
            {
                RemoveFileAttribute(resultFilePath, FileAttributes.ReadOnly);
            }
        }

        private static void SetContent(string filePath, string content)
        {
            using (var outfile = new StreamWriter(filePath))
            {
                outfile.Write(content);
            }
        }

        private static string Substitute(string fileContent, XElement node)
        {
            foreach (var item in node.Elements("item"))
            {
                if (item.Attribute("key") == null)
                {
                    Console.WriteLine("Item missing key attribute.");
                    throw new Exception();
                }

                fileContent = fileContent.Replace(item.Attribute("key").Value, WriteSubTree(item));
            }
            return fileContent;
        }

        private static string WriteSubTree(XElement node)
        {
            // Default - only value substitution
            if (node.Elements().Count() == 0)
            {
                return node.Value;
            }

            // If node has subtree - write each subnode
            var returnString = string.Empty;
            foreach (var element in node.Elements())
            {
                returnString += element.ToString();
            }
            return returnString;
        }

        private static bool GetFileAttribute(string filePath, FileAttributes attributes)
        {
            return (File.GetAttributes(filePath) & attributes) == attributes;
        }

        private static void RemoveFileAttribute(string filePath, FileAttributes attributes)
        {
            File.SetAttributes(filePath, File.GetAttributes(filePath) & ~attributes);
        }

        private static IEnumerable<SubstituteHolder> GetSubstituteHolders(string substituteConfigurationFileName, string projectPath)
        {
            var substituteHolderList = new List<SubstituteHolder>();

            var substitutionElements = GetSubstitutionElements(projectPath + substituteConfigurationFileName);
            foreach (var substitutionElement in substitutionElements)
            {
                substituteHolderList.Add(new SubstituteHolder
                                             {
                                                 SubstituteFileName = substitutionElement.Element("substitutefile").Value,
                                                 SubstituteFilePath = projectPath + substitutionElement.Element("substitutefile").Value,
                                                 ResultFilePath = projectPath + substitutionElement.Element("resultfile").Value,
                                                 TemplateFilePath = projectPath + substitutionElement.Element("templatefile").Value,
                                             });
            }


            return substituteHolderList;
        }

        private static IEnumerable<XElement> GetSubstitutionElements(string substituteConfigurationFilePath)
        {
            var substitutionXml = XDocument.Load(File.OpenRead(substituteConfigurationFilePath));

            return substitutionXml.Root.Elements("substitute");
        }
    }
}
