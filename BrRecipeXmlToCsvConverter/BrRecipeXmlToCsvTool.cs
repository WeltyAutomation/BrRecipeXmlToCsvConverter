using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace BrRecipeXmlToCsvConverter
{
    public static class BrRecipeXmlToCsvTool
    {
        public static string ConvertXmlToCsv(string xmlData)
        {
            var elementName = string.Empty;
            var groupNames = new Stack<string>();
            var strBuilder = new StringBuilder();

            //Add CSV Header line
            strBuilder.AppendLine("Parameter,Field,DataType,Value");

            using (XmlReader reader = XmlReader.Create(new StringReader(xmlData)))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "Element":
                                //This is the top of the tree
                                elementName = reader.GetAttribute("Name");
                                var type = reader.GetAttribute("Type");
                                strBuilder.AppendLine($"{elementName},$Type,STRING,{type}");

                                //Reset state
                                groupNames.Clear();
                                break;
                            case "Group":
                                ManageNestedDepth(reader.Depth, groupNames);
                                groupNames.Push(reader.GetAttribute("ID"));
                                break;
                            case "Property":
                                //This is the end of the line
                                var propertyId = reader.GetAttribute("ID");
                                var dataType = reader.GetAttribute("DataType");
                                var value = reader.GetAttribute("Value");

                                ManageNestedDepth(reader.Depth, groupNames);
                                var groupNamePath = BuildGroupNamePathToParameter(groupNames);

                                if (PropertyIsArrayElement(propertyId))
                                {
                                    strBuilder.AppendLine(
                                        $"{elementName},{groupNamePath}{propertyId},{dataType},{value}");
                                }
                                else
                                {
                                    strBuilder.AppendLine(
                                        $"{elementName},{groupNamePath}.{propertyId},{dataType},{value}");
                                }

                                break;
                        }
                    }
                }
            }

            return strBuilder.ToString().TrimEnd();
        }

        private static bool PropertyIsArrayElement(string propertyId)
        {
            return Regex.IsMatch(propertyId,@"\[\d+\]");
        }

        private static string BuildGroupNamePathToParameter(Stack<string> groupNames)
        {
            return string.Join(".", groupNames.Reverse())
                .Replace(".[", "[");
        }

        private static void ManageNestedDepth(int readerDepth, Stack<string> groupNames)
        {
            while (readerDepth - 2 < groupNames.Count)
            {
                //We are finding a new group and need to pop off the last entry which had parameters found already
                groupNames.Pop();
            }
        }
    }
}