using Xunit;

namespace XmlToCsvConverter.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var data = GetSingleProperty();


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