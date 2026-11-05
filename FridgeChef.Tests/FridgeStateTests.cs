
using Bunit;
using FridgeChef.Models;
using FridgeChef.Pages;
using FridgeChef.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Text;

namespace FridgeChef.Tests
{
    public class FridgeStateTests : MudTestContext
    {
        private readonly FakeHttpHandler handler;

        public FridgeStateTests()
        {
            handler = new FakeHttpHandler();

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://localhost:5039/")
            };

            Services.AddSingleton(client);
            Services.AddScoped<FridgeService>();
            Services.AddScoped<FridgeItemsApiService>();
        }

        [Fact]
        public void Empty_fridge_shows_empty_state()
        {
            handler.ResponseJson = "[]";

            var page = Render<Fridge>();

            page.WaitForAssertion(() =>
                Assert.Contains(
                    "Ваш холодильник порожній",
                    page.Markup));
        }

        [Fact]
        public void Loading_fridge_shows_loading_state()
        {
            // HTTP-запит залишається незавершеним.
            handler.KeepPending = true;

            var page = Render<Fridge>();

            page.WaitForAssertion(() =>
                Assert.Contains(
                    "Завантаження...",
                    page.Markup));
        }

[Fact]
public void Error_fridge_shows_error_state()
{
    // Імітуємо помилку сервера
    handler.StatusCode = HttpStatusCode.InternalServerError;

    var page = Render<Fridge>();

    var service = Services.GetRequiredService<FridgeService>();

    page.WaitForAssertion(() =>
    {
        Assert.False(service.IsLoading);

        Assert.False(
            string.IsNullOrWhiteSpace(service.ErrorMessage)
        );

        Assert.Contains(
            service.ErrorMessage!,
            page.Markup
        );
    });
}

        [Fact]
        public void Data_fridge_shows_products_and_recipes_section()
        {
            // Сервер повертає один продукт.
            handler.ResponseJson = """
            [
                {
                    "id": 1,
                    "name": "Молоко",
                    "quantity": 1,
                    "unit": "л"
                }
            ]
            """;

            var page = Render<Fridge>();

            page.WaitForAssertion(() =>
            {
                Assert.Contains("Молоко", page.Markup);
                Assert.Contains(
                    "Підібрані рецепти",
                    page.Markup);
            });
        }

        [Fact]
        public void Search_shows_message_when_no_products_found()
        {
            handler.ResponseJson = "[]";

            var page = Render<Fridge>();

            var search = page.Find("input");
            search.Input("Ананас");

            page.WaitForAssertion(() =>
                Assert.Contains(
                    "Продуктів не знайдено",
                    page.Markup));
        }

        [Fact]
        public void AddToFridge_rejects_negative_quantity()
        {
            var service = new FridgeService();

            var milk = service.Ingredients
                .First(x => x.Name == "Молоко");

            service.AddToFridge(milk, -5, "мл");

            Assert.Empty(service.FridgeItems);
        }

        [Fact]
        public void AddToFridge_rejects_invalid_unit()
        {
            var service = new FridgeService();

            var milk = service.Ingredients
                .First(x => x.Name == "Молоко");

            service.AddToFridge(milk, 2, "шт");

            Assert.Empty(service.FridgeItems);
        }

        [Fact]
        public void AddIngredientModel_rejects_zero_quantity()
        {
            var model = new AddIngredientModel
            {
                Quantity = 0,
                Unit = "мл"
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            var isValid = Validator.TryValidateObject(
                model,
                context,
                results,
                true);

            Assert.False(isValid);

            Assert.Contains(results, result =>
                result.ErrorMessage ==
                "Кількість повинна бути більшою за нуль");
        }

        private sealed class FakeHttpHandler : HttpMessageHandler
        {
            public string ResponseJson { get; set; } = "[]";

            public HttpStatusCode StatusCode { get; set; } =
                HttpStatusCode.OK;

            public bool KeepPending { get; set; }

            private readonly TaskCompletionSource<HttpResponseMessage>
                pendingResponse = new(
                    TaskCreationOptions.RunContinuationsAsynchronously);

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                if (KeepPending)
                {
                    return pendingResponse.Task;
                }

                var response = new HttpResponseMessage(StatusCode)
                {
                    Content = new StringContent(
                        ResponseJson,
                        Encoding.UTF8,
                        "application/json")
                };

                return Task.FromResult(response);
            }
        }
    }
}
