using System;
using System.IO;
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
            var data = GetSingleProperty();

            var strBuilder = new StringBuilder();

            using (XmlReader reader = XmlReader.Create(new StringReader(data)))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "Element":
                                    var name = reader.GetAttribute("Name");
                                    strBuilder.Append($"{name},");
                                    break;
                                case "Group":
                                    var groupId = reader.GetAttribute("ID");
                                    strBuilder.Append($"{groupId}.");
                                    break;
                                case "Property":
                                    var propertyId = reader.GetAttribute("ID");
                                    var dataType = reader.GetAttribute("DataType");
                                    var value = reader.GetAttribute("Value");
                                    strBuilder.AppendLine($"{propertyId},{dataType},{value}");
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            //strBuilder.AppendFormat("Text Node: {0}\n",
                            //    reader.GetValueAsync().Result);
                            break;
                        case XmlNodeType.EndElement:
                            //strBuilder.AppendFormat("End Element {0}\n", reader.Name);
                            break;
                        default:
                            //strBuilder.AppendFormat("Other node {0} with value {1}\n",
                            //    reader.NodeType, reader.Value);
                            break;
                    }
                }
            }

            strBuilder.ToString().Should().Be("gDuctLineConfig,gDuctLineConfig.CircuitDiagram,STRING,CD-3542-00\r\n");
        }

        string GetSingleProperty()
        {
            return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<DATA>
	<Element Name=""gDuctLineConfig"" Type=""PvParameter"">
		<Group ID=""gDuctLineConfig"">
			<Property ID=""CircuitDiagram"" DataType=""STRING"" Value=""CD-3542-00"" />
		</Group>
	</Element>
</DATA>";
        }
    }
}