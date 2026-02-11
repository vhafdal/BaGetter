using Microsoft.Extensions.Options;
using Xunit;

namespace BaGetter.Core.Tests;

public class BaGetterOptionsValidationTests
{
    [Fact]
    public void Validate_ShouldFail_WhenRegistrationPageSizeIsLessThanOne()
    {
        var validator = new ValidateBaGetterOptions<BaGetterOptions>(optionsName: null);
        var options = new BaGetterOptions
        {
            RegistrationPageSize = 0
        };

        var result = validator.Validate(Options.DefaultName, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(BaGetterOptions.RegistrationPageSize)));
    }
}
