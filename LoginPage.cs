using Microsoft.Playwright;
using System.Threading.Tasks;

namespace PlaywrightEbizPOM
{
    public class LoginPage
    {
        private readonly IPage _page;

        public LoginPage(IPage page)
        {
            _page = page;
        }

        public async Task LoginAsync(string email, string password)
        {
            await _page.GotoAsync("https://qaerequisition2.e-bizsoft.net/Login.aspx");
            await _page.GetByPlaceholder("Email Address").ClickAsync();
            await _page.GetByPlaceholder("Email Address").FillAsync(email);
            await _page.GetByPlaceholder("Password").ClickAsync();
            await _page.GetByPlaceholder("Password").FillAsync(password);
            await _page.GetByRole(AriaRole.Button, new() { Name = "Sign In" }).ClickAsync();
        }
    }
}
