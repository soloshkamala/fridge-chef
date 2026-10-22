using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FridgeChef.Models;

namespace FridgeChef.Tests
{
    public class ShoppingItemFormTests
    {
        private List<ValidationResult> Validate(ShoppingItemForm model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            Validator.TryValidateObject(
                model,
                context,
                results,
                true);

            return results;
        }

        [Fact]
        public void EmptyName_ShouldFailValidation()
        {
            var model = new ShoppingItemForm
            {
                Name = "",
                Quantity = 2,
                Unit = "кг",
                Category = "Овочі та фрукти"
            };

            var errors = Validate(model);

            Assert.Contains(errors, error =>
                error.ErrorMessage == "Введіть назву продукту");
        }

        [Fact]
        public void ZeroQuantity_ShouldFailValidation()
        {
            var model = new ShoppingItemForm
            {
                Name = "Морква",
                Quantity = 0,
                Unit = "кг",
                Category = "Овочі та фрукти"
            };

            var errors = Validate(model);

            Assert.Contains(errors, error =>
                error.ErrorMessage == "Кількість має бути більшою за 0");
        }

        [Fact]
        public void MissingQuantity_ShouldFailValidation()
        {
            var model = new ShoppingItemForm
            {
                Name = "Морква",
                Quantity = null,
                Unit = "кг",
                Category = "Овочі та фрукти"
            };

            var errors = Validate(model);

            Assert.Contains(errors, error =>
                error.ErrorMessage == "Вкажіть кількість");
        }

        [Fact]
        public void ValidProduct_ShouldPassValidation()
        {
            var model = new ShoppingItemForm
            {
                Name = "Морква",
                Quantity = 2,
                Unit = "кг",
                Category = "Овочі та фрукти"
            };

            var errors = Validate(model);

            Assert.Empty(errors);
        }
    }
}