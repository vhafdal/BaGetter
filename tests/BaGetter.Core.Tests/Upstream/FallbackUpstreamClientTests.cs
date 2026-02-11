using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.Versioning;
using Xunit;

namespace BaGetter.Core.Tests.Upstream;

public class FallbackUpstreamClientTests
{
    [Fact]
    public async Task DownloadPackageOrNullAsync_UsesNextMirrorWhenFirstReturnsNull()
    {
        var first = new Mock<IUpstreamClient>();
        var second = new Mock<IUpstreamClient>();
        var expected = new MemoryStream(new byte[] { 1, 2, 3 });

        first.Setup(x => x.DownloadPackageOrNullAsync("a", It.IsAny<NuGetVersion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stream)null);
        second.Setup(x => x.DownloadPackageOrNullAsync("a", It.IsAny<NuGetVersion>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var target = new FallbackUpstreamClient(
            [first.Object, second.Object],
            Mock.Of<ILogger<FallbackUpstreamClient>>());

        var result = await target.DownloadPackageOrNullAsync("a", new NuGetVersion("1.0.0"), CancellationToken.None);

        Assert.Same(expected, result);
    }

    [Fact]
    public async Task ListPackageVersionsAsync_UsesFirstNonEmptyResult()
    {
        var first = new Mock<IUpstreamClient>();
        var second = new Mock<IUpstreamClient>();
        var expected = new List<NuGetVersion> { new("1.0.0") };

        first.Setup(x => x.ListPackageVersionsAsync("a", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NuGetVersion>());
        second.Setup(x => x.ListPackageVersionsAsync("a", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var target = new FallbackUpstreamClient(
            [first.Object, second.Object],
            Mock.Of<ILogger<FallbackUpstreamClient>>());

        var result = await target.ListPackageVersionsAsync("a", CancellationToken.None);

        Assert.Same(expected, result);
    }
}
