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

    [Fact]
    public void Validate_ShouldFail_WhenMaxPackageSizeIsZero()
    {
        var validator = new ValidateBaGetterOptions<BaGetterOptions>(optionsName: null);
        var options = new BaGetterOptions
        {
            MaxPackageSizeGiB = 0
        };

        var result = validator.Validate(Options.DefaultName, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(BaGetterOptions.MaxPackageSizeGiB)));
    }

    [Fact]
    public void Validate_ShouldFail_WhenPathBaseDoesNotStartWithSlash()
    {
        var validator = new ValidateBaGetterOptions<BaGetterOptions>(optionsName: null);
        var options = new BaGetterOptions
        {
            PathBase = "bagetter"
        };

        var result = validator.Validate(Options.DefaultName, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(BaGetterOptions.PathBase)));
    }

    [Fact]
    public void Validate_ShouldFail_WhenCorsAllowsAnyOriginAndCredentials()
    {
        var validator = new ValidateBaGetterOptions<BaGetterOptions>(optionsName: null);
        var options = new BaGetterOptions
        {
            Cors = new CorsPolicyOptions
            {
                AllowAnyOrigin = true,
                AllowCredentials = true
            }
        };

        var result = validator.Validate(Options.DefaultName, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(BaGetterOptions.Cors)));
    }

    [Fact]
    public void Validate_ShouldFail_WhenCorsRestrictedWithoutOrigins()
    {
        var validator = new ValidateBaGetterOptions<BaGetterOptions>(optionsName: null);
        var options = new BaGetterOptions
        {
            Cors = new CorsPolicyOptions
            {
                AllowAnyOrigin = false,
                AllowedOrigins = System.Array.Empty<string>()
            }
        };

        var result = validator.Validate(Options.DefaultName, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(BaGetterOptions.Cors)));
    }

    [Fact]
    public void Validate_ShouldFail_WhenHstsEnabledButSecurityHeadersDisabled()
    {
        var validator = new ValidateBaGetterOptions<BaGetterOptions>(optionsName: null);
        var options = new BaGetterOptions
        {
            SecurityHeaders = new SecurityHeadersOptions
            {
                Enabled = false,
                EnableHsts = true
            }
        };

        var result = validator.Validate(Options.DefaultName, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(BaGetterOptions.SecurityHeaders)));
    }
}
