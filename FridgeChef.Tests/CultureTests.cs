using Bunit;
using MudBlazor;
using MudBlazor.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FridgeChef.Tests
{
    public class CultureTests : MudTestContext
    {
        [Fact]
        public void Ukrainian_culture_accepts_comma()
        {
            var culture = new CultureInfo("uk-UA");

            var result = double.Parse("3,5", culture);

            Assert.Equal(3.5, result);
        }

        [Fact]
        public void English_culture_accepts_dot()
        {
            var culture = new CultureInfo("en-US");

            var result = double.Parse("3.5", culture);

            Assert.Equal(3.5, result);
        }

        [Fact]
        public void MudNumericField_accepts_comma_in_Ukrainian()
        {
            var culture = new CultureInfo("uk-UA");

            var field = Render<MudNumericField<double>>(parameters => parameters
                .Add(p => p.Culture, culture)
                .Add(p => p.Value, 1.0)
            );

            field.Find("input").Change("3,5");

            Assert.Equal(3.5, field.Instance.GetState(x => x.Value));
        }

        [Fact]
        public void MudNumericField_accepts_dot_in_English()
        {
            var culture = new CultureInfo("en-US");

            var field = Render<MudNumericField<double>>(parameters => parameters
                .Add(p => p.Culture, culture)
                .Add(p => p.Value, 1.0)
            );

            field.Find("input").Change("3.5");

            Assert.Equal(3.5, field.Instance.GetState(x => x.Value));
        }
    }
}
