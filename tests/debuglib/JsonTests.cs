using Xunit;
using Maestro.Debug;
using System.Text;

public sealed class JsonTests
{
	[Theory]
	[InlineData(null, "null")]
	[InlineData(true, "true")]
	[InlineData(false, "false")]
	[InlineData(0, "0")]
	[InlineData(1, "1")]
	[InlineData(-1, "-1")]
	[InlineData(99.5f, "99.5")]
	[InlineData("string", "\"string\"")]
	[InlineData("\"\\/\b\f\n\r\t", "\"\\\"\\\\/\\b\\f\\n\\r\\t\"")]
	public void SerializeValue(object value, string expectedJson)
	{
		var jsonValue = new JsonValue(value);
		var sb = new StringBuilder();
		Json.Serialize(jsonValue, sb);
		Assert.Equal(expectedJson, sb.ToString());
	}

	[Fact]
	public void SerializeComplex()
	{
		var smallObject = JsonValue.NewObject();
		smallObject["int"] = 7;
		smallObject["bool"] = false;
		smallObject["null"] = null;
		smallObject["string"] = "some text";

		var a = JsonValue.NewArray();
		a.Add("string");
		a.Add(false);
		a.Add(null);
		a.Add(0.25f);
		a.Add(smallObject);
		a.Add(JsonValue.NewArray());

		var o = JsonValue.NewObject();
		o["array"] = a;
		o["str"] = "asdad";
		o["empty"] = JsonValue.NewObject();

		var sb = new StringBuilder();
		Json.Serialize(o, sb);

		Assert.Equal(
			"{\"array\":[\"string\",false,null,0.25,{\"int\":7,\"bool\":false,\"null\":null,\"string\":\"some text\"},[]],\"str\":\"asdad\",\"empty\":{}}",
			sb.ToString()
		);
	}

	[Theory]
	[InlineData("null", null)]
	[InlineData("true", true)]
	[InlineData("false", false)]
	[InlineData("0", 0)]
	[InlineData("1", 1)]
	[InlineData("-1", -1)]
	[InlineData("99.5", 99.5f)]
	[InlineData("99.25", 99.25f)]
	[InlineData("99.125", 99.125f)]
	[InlineData("\"string\"", "string")]
	[InlineData("\"\\\"\\\\\\/\\b\\f\\n\\r\\t\"", "\"\\/\b\f\n\r\t")]
	public void DeserializeValue(string json, object expectedValue)
	{
		var success = Json.TryDeserialize(json, out var value);
		Assert.True(success);
		Assert.Equal(expectedValue, value.wrapped);
	}

	[Fact]
	public void DeserializeComplex()
	{
		Assert.True(Json.TryDeserialize(
			" { \"array\"  : [\"string\",  false,null,0.25,\n{\"int\":  7,  \"bool\":false,\"null\":null, \t\n   \"string\":\"some text\"},[]],   \n\"str\":\"asdad\", \"empty\":{}}",
			out var value
		));

		Assert.True(value.IsObject);

		Assert.True(value["array"].IsArray);
		Assert.Equal(6, value["array"].Count);
		Assert.Equal("string", value["array"][0].wrapped);
		Assert.Equal(false, value["array"][1].wrapped);
		Assert.Null(value["array"][2].wrapped);
		Assert.Equal(0.25f, value["array"][3].wrapped);

		Assert.True(value["array"][4].IsObject);
		Assert.Equal(7, value["array"][4]["int"].wrapped);
		Assert.Equal(false, value["array"][4]["bool"].wrapped);
		Assert.Null(value["array"][4]["null"].wrapped);
		Assert.Equal("some text", value["array"][4]["string"].wrapped);

		Assert.True(value["array"][5].IsArray);
		Assert.Equal(0, value["array"][5].Count);

		Assert.Equal("asdad", value["str"].wrapped);
		Assert.True(value["empty"].IsObject);
	}
}