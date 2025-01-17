namespace PlaywrightEbizPOM
{
    using Microsoft.Playwright;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class BaseClass
    {
        protected IPlaywright? Playwright;
        protected IBrowser? Browser;
        protected IBrowserContext? Context;
        protected IPage? Page;

        private readonly string _videoPath = Path.Combine(Directory.GetCurrentDirectory(), "videos");
        private readonly string _screenshotPath = Path.Combine(Directory.GetCurrentDirectory(), "screenshots");
        private readonly string _tracePath = Path.Combine(Directory.GetCurrentDirectory(), "traces");

        [SetUp]
        public async Task SetUp()
        {
            try
            {
                // Ensure directories exist
                Directory.CreateDirectory(_videoPath);
                Directory.CreateDirectory(_screenshotPath);
                Directory.CreateDirectory(_tracePath);

                // Initialize Playwright and Browser
                Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = false, // Set to true for headless mode
                    SlowMo = 50,      // Optional: Adds a delay between actions
                    Args = new[] { "--no-startup-window" }
                });

                // Configure Browser Context with video recording and tracing
                Context = await Browser.NewContextAsync(new BrowserNewContextOptions
                {
                    RecordVideoDir = _videoPath,
                    RecordVideoSize = new RecordVideoSize { Width = 1280, Height = 720 }
                });

                // Create a new page
                Page = await Context.NewPageAsync();

                // Start tracing
                await Context.Tracing.StartAsync(new TracingStartOptions
                {
                    Screenshots = true,
                    Snapshots = true,
                    Sources = true
                });
            }
            catch (Exception ex)
            {
                Assert.Fail($"Failed to initialize Playwright: {ex.Message}");
            }
        }

        [TearDown]
        public async Task TearDown()
        {
            try
            {
                // Capture screenshot on failure
                if (Page != null && TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    var screenshotFile = Path.Combine(_screenshotPath, $"{TestContext.CurrentContext.Test.Name}.png");
                    await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotFile });
                    TestContext.AddTestAttachment(screenshotFile, "Failure Screenshot");
                }

                // Save tracing data
                if (Context != null)
                {
                    var traceFile = Path.Combine(_tracePath, $"{TestContext.CurrentContext.Test.Name}.zip");
                    await Context.Tracing.StopAsync(new TracingStopOptions { Path = traceFile });
                    TestContext.AddTestAttachment(traceFile, "Trace File");
                }

                // Close the Page
                if (Page != null)
                {
                    await Page.CloseAsync();
                    Page = null;
                }

                // Close the Browser Context
                if (Context != null)
                {
                    await Context.CloseAsync();
                    Context = null;
                }

                // Close the Browser
                if (Browser != null)
                {
                    await Browser.CloseAsync();
                    Browser = null;
                }

                // Dispose Playwright
                if (Playwright != null)
                {
                    Playwright.Dispose();
                    Playwright = null;
                }
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"TearDown encountered an issue: {ex.Message}");
            }
        }
    }
}
