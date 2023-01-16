using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System;
using System.Linq;

namespace BrRecipeXmlToCsvConverter
{
    public static class BrRecipeXmlToCsvTool
    {
        public static string ConvertXmlToCsv(string xmlData)
        {
            var elementName = string.Empty;
            var groupNames = new Stack<String>();
            var strBuilder = new StringBuilder();

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

                                //Reset state
                                groupNames.Clear();
                                break;
                            case "Group":
                                //We need to handle nesting
                                if (reader.Depth < groupNames.Count + 2)
                                {
                                    //We are finding a new group and need to pop off the last entry which had parameters found already
                                    groupNames.Pop();
                                }
                                groupNames.Push(reader.GetAttribute("ID"));
                                break;
                            case "Property":
                                //This is the end of the line
                                var propertyId = reader.GetAttribute("ID");
                                var dataType = reader.GetAttribute("DataType");
                                var value = reader.GetAttribute("Value");

                                //Check property depth and pop off any extra group names in the stack
                                while (reader.Depth - 2 < groupNames.Count)
                                {
                                    //We are finding a new group and need to pop off the last entry which had parameters found already
                                    groupNames.Pop();
                                }
                                var groupNamePath = string.Join(".", groupNames.Reverse())
                                    .Replace(".[", "[");

                                strBuilder.AppendLine($"{elementName},{groupNamePath}.{propertyId},{dataType},{value}");
                                break;
                        }
                    }
                }
            }

            return strBuilder.ToString().TrimEnd();
        }
    }
}