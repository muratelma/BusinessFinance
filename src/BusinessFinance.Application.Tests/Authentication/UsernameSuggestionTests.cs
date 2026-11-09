using BusinessFinance.Application.Authentication;

namespace BusinessFinance.Application.Tests.Authentication;

/// <summary>
/// Google ile ilk girişteki kullanıcı adı önerisi; kural
/// <c>BackfillUsernamesFromEmail</c> migration'ıyla aynıdır.
/// </summary>
public sealed class UsernameSuggestionTests
{
    [Theory]
    [InlineData("Ali@gmail.com", "ali")]
    [InlineData("mehmet.yilmaz-1@gmail.com", "mehmet.yilmaz-1")]
    [InlineData("ayşe_öz@gmail.com", "aye_z")]
    [InlineData("a+b@gmail.com", "kullaniciab")]
    [InlineData("çğ@gmail.com", "kullanici")]
    public void SuggestFromEmail_KeepsOnlyAllowedCharacters(string email, string expected)
    {
        var suggestion = Usernames.SuggestFromEmail(email);

        Assert.Equal(expected, suggestion);
        Assert.True(Usernames.IsValid(suggestion));
    }

    [Fact]
    public void SuggestFromEmail_CutsAtThirtyCharacters()
    {
        var suggestion = Usernames.SuggestFromEmail(new string('x', 40) + "@gmail.com");

        Assert.Equal(30, suggestion.Length);
    }

    [Fact]
    public void WithNumber_ShortensTheBaseToStayWithinTheLimit()
    {
        Assert.Equal("ali2", Usernames.WithNumber("ali", 2));
        var longName = new string('x', 30);
        var numbered = Usernames.WithNumber(longName, 12);
        Assert.Equal(30, numbered.Length);
        Assert.EndsWith("12", numbered, StringComparison.Ordinal);
    }
}
