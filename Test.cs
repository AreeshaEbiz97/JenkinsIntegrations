
using PlaywrightEbizPOM;  // Ensure this namespace is included
using NUnit.Framework;
using System.Threading.Tasks;

[TestFixture]
public class Tests : BaseClass   
{
    [Test]
    public async Task LoginTest()
    {
        if (Page == null)
        {
            Assert.Fail("Page is not initialized.");
            return;
        }

        var loginPage = new LoginPage(Page);
        await loginPage.LoginAsync("areesha.ahmedebizsoft3@gmail.com", "Aa1234567");

         // Navigate using menu image and Home link add test details 
        await Page.Locator("#menuimage").ClickAsync();
        await Page.Locator("a:has-text('Home')").ClickAsync();

         var isProfileIconVisible = await Page.IsVisibleAsync("#profileIcon");
        Assert.IsTrue(isProfileIconVisible, "Login failed. Profile icon not visible.");
    }
}  