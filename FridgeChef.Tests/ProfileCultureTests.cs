using System.Globalization;
using Xunit;

namespace FridgeChef.Tests;

public class ProfileCultureTests
{
    [Fact]
    public void Number_ShouldUseComma_ForUkrainianCulture()
    {
        var culture = new CultureInfo("uk-UA");

        var result = 3.5.ToString("0.0", culture);

        Assert.Equal("3,5", result);
    }

    [Fact]
    public void Number_ShouldUseDot_ForEnglishCulture()
    {
        var culture = new CultureInfo("en-US");

        var result = 3.5.ToString("0.0", culture);

        Assert.Equal("3.5", result);
    }
}