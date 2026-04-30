namespace DotnetPing.Tests;

public class JsonUtilityTest
{
    [Fact]
    public void SerializeCompact_WritesSingleLineJson()
    {
        // Arrange
        var value = new { Name = "dotnet-ping", Codes = new[] { 200, 204 } };

        // Act
        var json = JsonUtility.SerializeCompact(value);

        // Assert
        Assert.Equal("""{"Name":"dotnet-ping","Codes":[200,204]}""", json);
    }

    [Fact]
    public void Serialize_UsesDefaultJsonSerializerOptions()
    {
        // Arrange
        var value = new { Name = "dotnet-ping" };

        // Act
        var json = JsonUtility.Serialize(value);

        // Assert
        Assert.Equal("""{"Name":"dotnet-ping"}""", json);
    }
}
