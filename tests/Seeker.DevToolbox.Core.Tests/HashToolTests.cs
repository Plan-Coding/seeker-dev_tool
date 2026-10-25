using Seeker.DevToolbox.Core.Tools.Security;

namespace Seeker.DevToolbox.Core.Tests;

public class HashToolTests
{
    [Theory]
    [InlineData(HashKind.Md5, "900150983cd24fb0d6963f7d28e17f72")]
    [InlineData(HashKind.Sha1, "a9993e364706816aba3e25717850c26c9cd0d89d")]
    [InlineData(HashKind.Sha256, "ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad")]
    [InlineData(HashKind.Sha512, "ddaf35a193617abacc417349ae20413112e6fa4e89a97ea20a9eeee64b55d39a2192992a274fc1a836ba3c23a3feebbd454d4423643ce80e2a9ac94fa54ca49f")]
    public void Compute_KnownVectors(HashKind kind, string expected)
    {
        Assert.Equal(expected, HashTool.Compute("abc", kind));
    }

    [Fact]
    public void ComputeHmac_KnownVector()
    {
        const string expected = "f7bc83f430538424b13298e6aa6fb143ef4d59a14946175997479dbc2d1a3cd8";

        Assert.Equal(
            expected,
            HashTool.ComputeHmac("The quick brown fox jumps over the lazy dog", "key", HashKind.Sha256));
    }

    [Fact]
    public void Compute_EmptyString_KnownVector()
    {
        Assert.Equal(
            "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            HashTool.Compute(string.Empty, HashKind.Sha256));
    }
}
