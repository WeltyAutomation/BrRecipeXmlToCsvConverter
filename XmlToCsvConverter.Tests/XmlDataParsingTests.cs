using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using FluentAssertions;
using Xunit;

namespace XmlToCsvConverter.Tests
{
    public class XmlDataParsingTests
    {
        [Fact]
        public void WeCanParseSinglePropertyFromRecipeXmlData()
        {
            var xmlData = GetSingleProperty();

            var csvResults = ParseRecipleXmlDataToCsv(xmlData);

            csvResults.Should().Be("gDuctLineConfig,gDuctLineConfig.CircuitDiagram,STRING,CD-XXXX-XX");
        }

        [Fact]
        public void WeCanParseMultipleProeprtiesFromRecipeXmlData()
        {
            var xmlData = GetMultipleProperties();

            var csvResults = ParseRecipleXmlDataToCsv(xmlData);

            csvResults.Should().Be("gDuctLineConfig,gDuctLineConfig.CircuitDiagram,STRING,CD-XXXX-XX\r\ngDuctLineConfig,gDuctLineConfig.Decoiler.AutoReverseProhibited,BOOL,false");
        }

        private static string ParseRecipleXmlDataToCsv(string data)
        {
            var elementName = string.Empty;
            var groupNames = new List<String>();
            var strBuilder = new StringBuilder();

            using (XmlReader reader = XmlReader.Create(new StringReader(data)))
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
                                groupNames.Clear();
                                break;
                            case "Group":
                                //We need to handle nesting
                                groupNames.Add(reader.GetAttribute("ID"));
                                break;
                            case "Property":
                                //This is the end of the line
                                var propertyId = reader.GetAttribute("ID");
                                var dataType = reader.GetAttribute("DataType");
                                var value = reader.GetAttribute("Value");

                                var groupName = string.Join(".", groupNames);

                                strBuilder.AppendLine($"{elementName},{groupName}.{propertyId},{dataType},{value}");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            return strBuilder.ToString().TrimEnd();
        }

        string GetSingleProperty()
        {
            return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<DATA>
	<Element Name=""gDuctLineConfig"" Type=""PvParameter"">
		<Group ID=""gDuctLineConfig"">
			<Property ID=""CircuitDiagram"" DataType=""STRING"" Value=""CD-XXXX-XX"" />
		</Group>
	</Element>
</DATA>";
        }

        string GetMultipleProperties()
        {
            return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<DATA>
	<Element Name=""gDuctLineConfig"" Type=""PvParameter"">
		<Group ID=""gDuctLineConfig"">
			<Property ID=""CircuitDiagram"" DataType=""STRING"" Value=""CD-XXXX-XX"" />
			<Group ID=""Decoiler"">
				<Property ID=""AutoReverseProhibited"" DataType=""BOOL"" Value=""false"" />
			</Group>
		</Group>
	</Element>
</DATA>";
        }
    }
}