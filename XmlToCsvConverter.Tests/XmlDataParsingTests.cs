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

            var csvResults = ParseRecipeXmlDataToCsv(xmlData);

            csvResults.Should().Be("gDuctLineConfig,gDuctLineConfig.CircuitDiagram,STRING,CD-XXXX-XX");
        }

        [Fact]
        public void WeCanParseMultiplePropertiesFromRecipeXmlData()
        {
            var xmlData = GetMultipleProperties();

            var csvResults = ParseRecipeXmlDataToCsv(xmlData);

            csvResults.Should().Be(@"gDuctLineConfig,gDuctLineConfig.CircuitDiagram,STRING,CD-XXXX-XX
gDuctLineConfig,gDuctLineConfig.Decoiler.AutoReverseProhibited,BOOL,false");
        }

        [Fact]
        public void WeCanParseMultipleElementsFromRecipeXmlData()
        {
            var xmlData = GetMultipleElements();
            
            var csvResults = ParseRecipeXmlDataToCsv(xmlData);

            csvResults.Should().Be(@"gDuctLineConfig,gDuctLineConfig.AuxDie[0].Enable,BOOL,false
gDuctLineConfig,gDuctLineConfig.AuxDie[0].Mode,USINT,0
gDuctLineConfig,gDuctLineConfig.AuxDie[1].Enable,BOOL,false
gDuctLineConfig,gDuctLineConfig.AuxDie[1].Mode,USINT,0
gDuctLineConfig,gDuctLineConfig.TemplateConfig,DINT,0
gMachineSettings,gMachineSettings.Processor.VeeFromLtDepthOffset,REAL,0.5625
gMachineSettings,gMachineSettings.InlinePlasma.CutoutLimits.MinDistanceFromEdge,REAL,0");
        }

        private static string ParseRecipeXmlDataToCsv(string data)
        {
            var elementName = string.Empty;
            var groupNames = new Stack<String>();
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
                                while (reader.Depth -2 < groupNames.Count)
                                {
                                    //We are finding a new group and need to pop off the last entry which had parameters found already
                                    groupNames.Pop();
                                }
                                var groupNamePath = string.Join(".", groupNames.Reverse())
                                    .Replace(".[","[");

                                strBuilder.AppendLine($"{elementName},{groupNamePath}.{propertyId},{dataType},{value}");
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

        string GetMultipleElements()
        {
            return @"<?xml version=""1.0"" encoding=""UTF-8""?>
<DATA>
	<Element Name=""gDuctLineConfig"" Type=""PvParameter"">
		<Group ID=""gDuctLineConfig"">
			<Group ID=""AuxDie"">
				<Group ID=""[0]"">
					<Property ID=""Enable"" DataType=""BOOL"" Value=""false"" />
					<Property ID=""Mode"" DataType=""USINT"" Value=""0"" />
				</Group>
				<Group ID=""[1]"">
					<Property ID=""Enable"" DataType=""BOOL"" Value=""false"" />
					<Property ID=""Mode"" DataType=""USINT"" Value=""0"" />
				</Group>
			</Group>
			<Property ID=""TemplateConfig"" DataType=""DINT"" Value=""0"" />
		</Group>
	</Element>
	<Element Name=""gMachineSettings"" Type=""PvParameter"">
		<Group ID=""gMachineSettings"">
			<Group ID=""Processor"">
				<Property ID=""VeeFromLtDepthOffset"" DataType=""REAL"" Value=""0.5625"" />
			</Group>
			<Group ID=""InlinePlasma"">
				<Group ID=""CutoutLimits"">
					<Property ID=""MinDistanceFromEdge"" DataType=""REAL"" Value=""0"" />
				</Group>
			</Group>
		</Group>
	</Element>
</DATA>";
        }
    }
}