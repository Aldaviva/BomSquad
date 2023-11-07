using Bom.Squad;
using FluentAssertions;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace Test;

public class BomSquadTest {

    public BomSquadTest(ITestOutputHelper outputHelper) {
        outputHelper.WriteLine($".NET {(PlatformInfo.IsNetCore ? "Core" : "Framework")} {(PlatformInfo.Is64Bit ? "64-bit" : "32-bit")}");
    }

    [Fact]
    public void ReadUtf8NoBom() {
        byte[]       input    = { 0x68, 0x69 };
        const string expected = "hi";

        Decode(input, new UTF8Encoding(false, true)).Should().Be(expected);
        Decode(input, new UTF8Encoding(true, true)).Should().Be(expected);
        Decode(input, Encoding.UTF8).Should().Be(expected);
        Encoding.UTF8.GetString(input).Should().Be(expected);
    }

    [Fact]
    public void ReadUtf8Bom() {
        byte[]       input    = { 0xEF, 0xBB, 0xBF, 0x68, 0x69 };
        const string expected = "hi";

        Decode(input, new UTF8Encoding(false, true)).Should().Be(expected);
        Decode(input, new UTF8Encoding(true, true)).Should().Be(expected);
        Decode(input, Encoding.UTF8).Should().Be(expected);
    }

    [Fact]
    public void WriteUtf8NoBom() {
        byte[]       expected = { 0x68, 0x69 };
        const string input    = "hi";

        Encode(input, new UTF8Encoding(false, true)).Should().Equal(expected);
        BomSquad.DefuseUtf8Bom();
        Encode(input, Encoding.UTF8).Should().Equal(expected);
        Encoding.UTF8.GetBytes(input).Should().Equal(expected);
    }

    [Fact]
    public void WriteUtf8Bom() {
        byte[]       expected = { 0xEF, 0xBB, 0xBF, 0x68, 0x69 };
        const string input    = "hi";

        Encode(input, new UTF8Encoding(true, true)).Should().Equal(expected);
    }

    private static IEnumerable<byte> Encode(string input, Encoding encoding) {
        using MemoryStream memoryStream = new();
        using (StreamWriter writer = new(memoryStream, encoding)) {
            writer.Write(input);
        }

        return memoryStream.ToArray();
    }

    private static string Decode(byte[] input, Encoding encoding) {
        using MemoryStream memoryStream = new(input);
        using StreamReader streamReader = new(memoryStream, encoding);
        return streamReader.ReadToEnd();
    }

}