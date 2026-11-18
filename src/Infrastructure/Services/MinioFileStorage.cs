using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Services;

public sealed class MinioFileStorage : IFileStorage
{
    private readonly MinioStorageOptions options;
    private readonly IMinioClient? client;
    private readonly SemaphoreSlim bucketLock = new(1, 1);
    private bool bucketReady;

    public MinioFileStorage(IOptions<MinioStorageOptions> options)
    {
        this.options = options.Value;
        if (string.IsNullOrWhiteSpace(this.options.Endpoint))
            return;

        client = new MinioClient()
            .WithEndpoint(this.options.Endpoint)
            .WithCredentials(this.options.AccessKey, this.options.SecretKey)
            .WithSSL(this.options.UseSsl)
            .WithRegion("us-east-1")
            .Build();
    }

    public async Task<string> SaveAsync(Stream content, long length, string contentType, string key, CancellationToken ct = default)
    {
        var minio = Client();
        try
        {
            await EnsureBucketAsync(minio, ct);
            await minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(options.Bucket)
                .WithObject(key)
                .WithStreamData(content)
                .WithObjectSize(length)
                .WithContentType(contentType), ct);
            return key;
        }
        catch (Exception ex) when (ex is not DomainException and not OperationCanceledException)
        {
            throw new ExternalServiceException($"Không lưu được ảnh vào MinIO: {ex.Message}");
        }
    }

    public async Task<StoredFile?> OpenAsync(string key, CancellationToken ct = default)
    {
        var minio = Client();
        try
        {
            var stat = await minio.StatObjectAsync(new StatObjectArgs()
                .WithBucket(options.Bucket)
                .WithObject(key), ct);

            var buffer = new MemoryStream();
            await minio.GetObjectAsync(new GetObjectArgs()
                .WithBucket(options.Bucket)
                .WithObject(key)
                .WithCallbackStream(stream => stream.CopyTo(buffer)), ct);

            buffer.Position = 0;
            return new StoredFile(buffer, stat.ContentType);
        }
        catch (ObjectNotFoundException)
        {
            return null;
        }
        catch (BucketNotFoundException)
        {
            return null;
        }
        catch (Exception ex) when (ex is not DomainException and not OperationCanceledException)
        {
            throw new ExternalServiceException($"Không đọc được ảnh từ MinIO: {ex.Message}");
        }
    }

    private IMinioClient Client() =>
        client ?? throw new ExternalServiceException("Chưa cấu hình MinIO (Minio:Endpoint).", 503);

    private async Task EnsureBucketAsync(IMinioClient minio, CancellationToken ct)
    {
        if (bucketReady) return;

        await bucketLock.WaitAsync(ct);
        try
        {
            if (bucketReady) return;
            var exists = await minio.BucketExistsAsync(new BucketExistsArgs().WithBucket(options.Bucket), ct);
            if (!exists)
                await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(options.Bucket), ct);
            bucketReady = true;
        }
        finally
        {
            bucketLock.Release();
        }
    }
}
