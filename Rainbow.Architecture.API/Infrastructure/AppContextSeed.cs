using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Rainbow.Architecture.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rainbow.Architecture.API.Infrastructure
{
    public class AppContextSeed
    {
        public async Task SeedAsync(AppDbContext context, IWebHostEnvironment env, IOptions<AppSettings> settings, ILogger<AppContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(AppContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = settings.Value.UseCustomizationData;

                if (useCustomizationData)
                {
                    using (context)
                    {
                        var sqls = GetSqlFromFolder(env.ContentRootPath, logger);
                        if (sqls != null && sqls.Any())
                        {
                            var sql = string.Join(";\r\n", sqls);
                            await context.ExecuteAsync(sql, null, null, 60);
                        }
                    }
                }
            });
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<AppContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<DbException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }

        private IEnumerable<string> GetSqlFromFolder(string contentRootPath, ILogger<AppContextSeed> logger)
        {
            var sqlDirectory = new DirectoryInfo(Path.Combine(contentRootPath, "SeedData"));

            if (sqlDirectory.Exists)
            {
                var files = sqlDirectory.GetFiles()?.Where(x => x.Name.EndsWith(".sql", StringComparison.OrdinalIgnoreCase));
                if (files != null && files.Any())
                {
                    foreach (var file in files)
                    {
                        yield return File.ReadAllText(file.FullName);
                    }
                }
            }
        }

    }
}
