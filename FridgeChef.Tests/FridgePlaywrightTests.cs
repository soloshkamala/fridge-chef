
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit;
using Xunit;

namespace FridgeChef.Tests;

public class FridgePlaywrightTests : PageTest
{
    private const string FridgeUrl = "http://localhost:5050/fridge";
    private const string HomeUrl = "http://localhost:5050/";

    private const string TestProduct = "Молоко";
    private const string InitialQuantity = "17";
    private const string UpdatedQuantity = "23";

    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = 1400,
                Height = 900
            }
        };
    }

    [Fact]
    public async Task Fridge_updates_automatically_via_SignalR()
    {
        var firstPage = Page;
        IPage? secondPage = null;
        bool productMayExist = false;

        try
        {
            await firstPage.GotoAsync(HomeUrl);

            Console.WriteLine("URL: " + firstPage.Url);

            await firstPage.PauseAsync();

            await OpenFridgeAsync(firstPage);

            var firstCard = GetProductCard(firstPage);

            secondPage = await Context.RunAndWaitForPageAsync(
                async () =>
                {
                    await firstPage.EvaluateAsync(
                        "() => window.open('/fridge', '_blank')");
                });

            await WaitForFridgeAsync(secondPage);

            var secondCard = GetProductCard(secondPage);

            Assert.Equal(0, await firstCard.CountAsync());
            Assert.Equal(0, await secondCard.CountAsync());

            var search = secondPage.Locator(
                ".ingredient-search input");

            await search.FillAsync(TestProduct);

            var searchResult = secondPage
                .Locator(".search-result-item")
                .Filter(new LocatorFilterOptions
                {
                    HasTextString = TestProduct
                });

            await Assertions.Expect(searchResult)
                .ToBeVisibleAsync(new()
                {
                    Timeout = 15000
                });

            await searchResult.ClickAsync();

            var dialog = secondPage.Locator(".mud-dialog");

            await Assertions.Expect(dialog)
                .ToBeVisibleAsync();

            var quantityInput = dialog.Locator("input").First;

            await quantityInput.FillAsync(InitialQuantity);
            await quantityInput.PressAsync("Tab");

            productMayExist = true;

            await dialog.Locator("button[type='submit']")
                .ClickAsync();

            await Assertions.Expect(secondCard)
                .ToBeVisibleAsync(new()
                {
                    Timeout = 15000
                });

            await Assertions.Expect(secondCard)
                .ToContainTextAsync(InitialQuantity);

            await Assertions.Expect(firstCard)
                .ToBeVisibleAsync(new()
                {
                    Timeout = 15000
                });

            await Assertions.Expect(firstCard)
                .ToContainTextAsync(
                    InitialQuantity,
                    new() { Timeout = 15000 });

            Console.WriteLine("SIGNALR ADD: PASSED");

            await secondCard
                .Locator("button.mud-icon-button")
                .Nth(0)
                .ClickAsync();

            await Assertions.Expect(dialog)
                .ToBeVisibleAsync();

            quantityInput = dialog.Locator("input").First;

            await quantityInput.FillAsync(UpdatedQuantity);
            await quantityInput.PressAsync("Tab");

            await dialog.Locator("button[type='submit']")
                .ClickAsync();

            await Assertions.Expect(secondCard)
                .ToContainTextAsync(
                    UpdatedQuantity,
                    new() { Timeout = 15000 });

            await Assertions.Expect(firstCard)
                .ToContainTextAsync(
                    UpdatedQuantity,
                    new() { Timeout = 15000 });

            Console.WriteLine("SIGNALR UPDATE: PASSED");

            await secondCard
                .Locator("button.mud-icon-button")
                .Nth(1)
                .ClickAsync();

            await Assertions.Expect(secondCard)
                .ToHaveCountAsync(
                    0,
                    new() { Timeout = 15000 });

            await Assertions.Expect(firstCard)
                .ToHaveCountAsync(
                    0,
                    new() { Timeout = 15000 });

            productMayExist = false;

            Console.WriteLine("SIGNALR DELETE: PASSED");
        }
        finally
        {
            if (productMayExist && secondPage != null)
            {
                try
                {
                    await secondPage.ReloadAsync();
                    await WaitForFridgeAsync(secondPage);

                    var card = GetProductCard(secondPage);

                    if (await card.CountAsync() > 0)
                    {
                        await card
                            .Locator("button.mud-icon-button")
                            .Nth(1)
                            .ClickAsync();

                        await Assertions.Expect(card)
                            .ToHaveCountAsync(
                                0,
                                new() { Timeout = 15000 });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        "CLEANUP ERROR: " + ex.Message);
                }
            }

            if (secondPage != null)
            {
                await secondPage.CloseAsync();
            }
        }
    }

    private static ILocator GetProductCard(IPage page)
    {
        return page
            .Locator(".ingredient-wrapper")
            .Filter(new LocatorFilterOptions
            {
                HasTextString = TestProduct
            });
    }

    private static async Task OpenFridgeAsync(IPage page)
    {
        await page.GotoAsync(FridgeUrl);
        await WaitForFridgeAsync(page);
    }

    private static async Task WaitForFridgeAsync(IPage page)
    {
        await Assertions.Expect(page)
            .ToHaveURLAsync(
                FridgeUrl,
                new() { Timeout = 15000 });

        await Assertions.Expect(
            page.Locator(".status-card .mud-progress-circular"))
            .ToHaveCountAsync(
                0,
                new() { Timeout = 15000 });

        await Assertions.Expect(
            page.Locator(".ingredient-search input"))
            .ToBeVisibleAsync();

        var errors = await page
            .Locator(".error-card")
            .AllTextContentsAsync();

        if (errors.Count > 0)
        {
            throw new Exception(
                "Fridge loading failed: " +
                string.Join("; ", errors));
        }
    }
}
