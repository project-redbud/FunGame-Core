using FunGame.Core.Api;
using FunGame.Core.Library.Constant;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace FunGame.Core.Tests.Infrastructure;

public class CalculationTests
{
    [Fact]
    public void Round_RoundsAwayFromZeroAndSnapsTinyValuesToZero()
    {
        Assert.Equal(1d, Calculation.Round(.5, 0), 3);
        Assert.Equal(2d, Calculation.Round(1.5, 0), 3);
        Assert.Equal(-2d, Calculation.Round(-1.5, 0), 3);
        Assert.Equal(0d, Calculation.Round(1e-12, 4), 3);
    }

    [Fact]
    public void Round2DigitsAnd4Digits_KeepExpectedPrecision()
    {
        Assert.Equal(1.23, Calculation.Round2Digits(1.2345), 4);
        Assert.Equal(-1.24, Calculation.Round2Digits(-1.2355), 4);
        Assert.Equal(1.2345, Calculation.Round4Digits(1.23454), 5);
    }

    [Fact]
    public void PercentageCheck_ClampsIntoUnitRange()
    {
        Assert.Equal(1d, Calculation.PercentageCheck(1.5), 3);
        Assert.Equal(0d, Calculation.PercentageCheck(-1), 3);
        Assert.Equal(.3, Calculation.PercentageCheck(.3), 3);
    }

    [Fact]
    public void IsApproximatelyZero_UsesEpsilon()
    {
        Assert.True(Calculation.IsApproximatelyZero(1e-12));
        Assert.False(Calculation.IsApproximatelyZero(1e-8));
        Assert.True(Calculation.IsApproximatelyZero(5e-4, 1e-3));
    }
}

public class NetworkUtilityTests
{
    [Theory]
    [InlineData("127.0.0.1", true)]
    [InlineData("255.255.255.255", true)]
    [InlineData("256.1.1.1", false)]
    [InlineData("localhost", false)]
    public void IsIP_MatchesIpv4Literal(string value, bool expected)
    {
        Assert.Equal(expected, NetworkUtility.IsIP(value));
    }

    [Theory]
    [InlineData("user@example.com", true)]
    [InlineData("user@example.com.cn", true)]
    [InlineData("user.name@example.com", true)]
    [InlineData("a.b.c@example.co.uk", true)]
    [InlineData("user_name@s.example.com", true)]
    [InlineData(".user@example.com", false)]
    [InlineData("user.@example.com", false)]
    [InlineData("user..name@example.com", false)]
    [InlineData("user@@example.com", false)]
    [InlineData("example.com", false)]
    public void IsEmail_MatchesMailPattern(string value, bool expected)
    {
        Assert.Equal(expected, NetworkUtility.IsEmail(value));
    }

    [Theory]
    [InlineData("角色名1", true)]
    [InlineData("user_1", false)]
    [InlineData("", false)]
    public void IsUserName_AllowsChineseAndAlphanumeric(string value, bool expected)
    {
        Assert.Equal(expected, NetworkUtility.IsUserName(value));
    }

    [Theory]
    [InlineData("纯中文", true)]
    [InlineData("中文abc", false)]
    public void IsChineseName_RequiresPureChinese(string value, bool expected)
    {
        Assert.Equal(expected, NetworkUtility.IsChineseName(value));
    }

    [Fact]
    public void GetUserNameLength_CountsChineseAsTwo()
    {
        Assert.Equal(2, NetworkUtility.GetUserNameLength("ab"));
        Assert.Equal(2, NetworkUtility.GetUserNameLength("中"));
        Assert.Equal(6, NetworkUtility.GetUserNameLength("ab中文"));
    }
}

public class EncryptionTests
{
    [Fact]
    public void Md5AndSha256_MatchKnownVectors()
    {
        Assert.Equal("900150983cd24fb0d6963f7d28e17f72", Encryption.MD5("abc"));
        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", Encryption.Sha256("abc"));
    }

    [Fact]
    public void Hmac_IsDeterministicAndKeyDependent()
    {
        string first = Encryption.HmacSha512("消息", "密钥");
        string second = Encryption.HmacSha512("消息", "密钥");
        string other = Encryption.HmacSha512("消息", "别的密钥");

        Assert.Equal(first, second);
        Assert.NotEqual(first, other);
        Assert.Equal(128, first.Length);
        Assert.Equal(64, Encryption.HmacSha256("消息", "密钥").Length);
    }

    [Fact]
    public void GenerateRandomString_HonoursLength()
    {
        string value = Encryption.GenerateRandomString(16);

        Assert.Equal(16, value.Length);
        Assert.NotEqual(value, Encryption.GenerateRandomString(16));
    }

    [Fact]
    public void FileSha256_HashesFileContent()
    {
        string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");
        File.WriteAllText(path, "abc");
        try
        {
            Assert.Equal(Encryption.Sha256("abc"), Encryption.FileSha256(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}

public class VerificationTests
{
    [Theory]
    [InlineData(VerifyCodeType.MixVerifyCode, 6)]
    [InlineData(VerifyCodeType.LetterVerifyCode, 8)]
    [InlineData(VerifyCodeType.NumberVerifyCode, 4)]
    public void CreateVerifyCode_ReturnsRequestedLength(VerifyCodeType type, int length)
    {
        string code = Verification.CreateVerifyCode(type, length);

        Assert.Equal(length, code.Length);
    }

    [Fact]
    public void CreateVerifyCode_LetterTypeUsesLettersOnly()
    {
        string code = Verification.CreateVerifyCode(VerifyCodeType.LetterVerifyCode, 16);

        Assert.All(code, value => Assert.True(char.IsLetter(value)));
    }
}

public class DateTimeUtilityTests
{
    [Fact]
    public void GetDateTime_DateOnlyDropsTime()
    {
        Assert.Equal(DateTime.Today, DateTimeUtility.GetDateTime(TimeType.LongDateOnly));
        Assert.Equal(DateTime.Today, DateTimeUtility.GetDateTime(TimeType.ShortDateOnly));
    }

    [Fact]
    public void GetDateTimeToString_UsesRequestedFormat()
    {
        Assert.Equal(DateTime.Now.Year.ToString(), DateTimeUtility.GetDateTimeToString(TimeType.Year4));
        Assert.Equal(10, DateTimeUtility.GetDateTimeToString("yyyy-MM-dd").Length);
    }

    [Fact]
    public void GetDateTime_ParsesOrFallsBackToNow()
    {
        Assert.Equal(new DateTime(2026, 1, 2, 3, 4, 5), DateTimeUtility.GetDateTime("2026-01-02 03:04:05"));
        Assert.True(DateTimeUtility.GetDateTime("不是时间").Year >= 2026);
    }

    [Fact]
    public void GetTradableTime_SwitchesAtFifteenOClock()
    {
        Assert.Equal(new DateTime(2026, 1, 2, 15, 0, 0), DateTimeUtility.GetTradableTime(new DateTime(2026, 1, 1, 10, 0, 0)));
        Assert.Equal(new DateTime(2026, 1, 3, 9, 0, 0), DateTimeUtility.GetTradableTime(new DateTime(2026, 1, 1, 16, 0, 0)));
    }

    [Fact]
    public void GetNowShortTime_HasNoMilliseconds()
    {
        Assert.DoesNotContain(".", DateTimeUtility.GetNowShortTime());
    }
}

public class LinqExtensionTests
{
    [Fact]
    public void GetPage_ReturnsRequestedSlice()
    {
        int[] source = [1, 2, 3, 4, 5];

        Assert.Equal(new[] { 1, 2 }, source.GetPage(1, 2));
        Assert.Equal(new[] { 3, 4 }, source.GetPage(2, 2));
        Assert.Equal(new[] { 5 }, source.GetPage(3, 2));
    }

    [Fact]
    public void MaxPage_HandlesEmptyAndInvalidPageSize()
    {
        Assert.Equal(3, new[] { 1, 2, 3, 4, 5 }.MaxPage(2));
        Assert.Equal(5, new[] { 1, 2, 3, 4, 5 }.MaxPage(0));
        Assert.Equal(1, Array.Empty<int>().MaxPage(2));
    }
}
