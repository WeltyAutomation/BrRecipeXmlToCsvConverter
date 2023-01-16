using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using FluentAssertions;
using Xunit;

namespace BrRecipeXmlToCsvConverter.Tests
{
    public class BrRecipeXmlDataParsingTests
    {
        [Fact]
        public void WeCanParseSinglePropertyFromRecipeXmlData()
        {
            var body = @"	<Element Name=""gDuctLineConfig"" Type=""PvParameter"">
		<Group ID=""gDuctLineConfig"">
			<Property ID=""CircuitDiagram"" DataType=""STRING"" Value=""CD-XXXX-XX"" />
		</Group>
	</Element>";
            var xmlData = GetBrRecipeXmlHeader() + body + GetBrRecipeXmlFooter();

            var csvResults = BrRecipeXmlToCsvTool.ConvertXmlToCsv(xmlData);

            csvResults.Should().Be(@"Parameter,Field,DataType,Value
gDuctLineConfig,$Type,STRING,PvParameter
gDuctLineConfig,gDuctLineConfig.CircuitDiagram,STRING,CD-XXXX-XX");
        }

        [Fact]
        public void WeCanParseMultiplePropertiesFromRecipeXmlData()
        {
            var body = @"	<Element Name=""gDuctLineConfig"" Type=""PvParameter"">
		<Group ID=""gDuctLineConfig"">
			<Property ID=""CircuitDiagram"" DataType=""STRING"" Value=""CD-XXXX-XX"" />
			<Group ID=""Decoiler"">
				<Property ID=""AutoReverseProhibited"" DataType=""BOOL"" Value=""false"" />
			</Group>
		</Group>
	</Element>";
            var xmlData = GetBrRecipeXmlHeader() + body + GetBrRecipeXmlFooter();

            var csvResults = BrRecipeXmlToCsvTool.ConvertXmlToCsv(xmlData);

            csvResults.Should().Be(@"Parameter,Field,DataType,Value
gDuctLineConfig,$Type,STRING,PvParameter
gDuctLineConfig,gDuctLineConfig.CircuitDiagram,STRING,CD-XXXX-XX
gDuctLineConfig,gDuctLineConfig.Decoiler.AutoReverseProhibited,BOOL,false");
        }

        [Fact]
        public void WeCanParseMultipleElementsFromRecipeXmlData()
        {
            var body = @"	<Element Name=""gDuctLineConfig"" Type=""PvParameter"">
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
	</Element>";
            var xmlData = GetBrRecipeXmlHeader() + body + GetBrRecipeXmlFooter();
            
            var csvResults = BrRecipeXmlToCsvTool.ConvertXmlToCsv(xmlData);

            csvResults.Should().Be(@"Parameter,Field,DataType,Value
gDuctLineConfig,$Type,STRING,PvParameter
gDuctLineConfig,gDuctLineConfig.AuxDie[0].Enable,BOOL,false
gDuctLineConfig,gDuctLineConfig.AuxDie[0].Mode,USINT,0
gDuctLineConfig,gDuctLineConfig.AuxDie[1].Enable,BOOL,false
gDuctLineConfig,gDuctLineConfig.AuxDie[1].Mode,USINT,0
gDuctLineConfig,gDuctLineConfig.TemplateConfig,DINT,0
gMachineSettings,$Type,STRING,PvParameter
gMachineSettings,gMachineSettings.Processor.VeeFromLtDepthOffset,REAL,0.5625
gMachineSettings,gMachineSettings.InlinePlasma.CutoutLimits.MinDistanceFromEdge,REAL,0");
        }

        string GetBrRecipeXmlHeader()
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<DATA>\r\n";
        }

        string GetBrRecipeXmlFooter()
        {
            return "\r\n</DATA>";
        }
    }
}