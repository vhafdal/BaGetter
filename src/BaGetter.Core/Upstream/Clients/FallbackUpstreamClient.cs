using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace BaGetter.Core;

/// <summary>
/// Tries multiple upstream clients in order until one returns a result.
/// </summary>
public sealed class FallbackUpstreamClient : IUpstreamClient, IDisposable
{
    private readonly IReadOnlyList<IUpstreamClient> _clients;
    private readonly ILogger<FallbackUpstreamClient> _logger;

    public FallbackUpstreamClient(
        IReadOnlyList<IUpstreamClient> clients,
        ILogger<FallbackUpstreamClient> logger)
    {
        if (clients == null || clients.Count == 0)
        {
            throw new ArgumentException("At least one upstream client is required.", nameof(clients));
        }

        _clients = clients;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<NuGetVersion>> ListPackageVersionsAsync(string id, CancellationToken cancellationToken)
    {
        foreach (var client in _clients)
        {
            try
            {
                var versions = await client.ListPackageVersionsAsync(id, cancellationToken);
                if (versions.Count > 0)
                {
                    return versions;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Mirror failed while listing versions for {PackageId}", id);
            }
        }

        return Array.Empty<NuGetVersion>();
    }

    public async Task<IReadOnlyList<Package>> ListPackagesAsync(string id, CancellationToken cancellationToken)
    {
        foreach (var client in _clients)
        {
            try
            {
                var packages = await client.ListPackagesAsync(id, cancellationToken);
                if (packages.Count > 0)
                {
                    return packages;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Mirror failed while listing metadata for {PackageId}", id);
            }
        }

        return Array.Empty<Package>();
    }

    public async Task<Stream> DownloadPackageOrNullAsync(string id, NuGetVersion version, CancellationToken cancellationToken)
    {
        foreach (var client in _clients)
        {
            try
            {
                var stream = await client.DownloadPackageOrNullAsync(id, version, cancellationToken);
                if (stream != null)
                {
                    return stream;
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Mirror failed while downloading {PackageId} {PackageVersion}", id, version);
            }
        }

        return null;
    }

    public void Dispose()
    {
        foreach (var disposable in _clients.OfType<IDisposable>())
        {
            disposable.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
