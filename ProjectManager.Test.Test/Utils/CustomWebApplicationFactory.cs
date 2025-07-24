using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;

public class CustomWebApplicationFactory
    : WebApplicationFactory<ProjectManager.Presentation.Program>
{
    private MongoDbRunner _runner;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _runner = MongoDbRunner.Start();

        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMongoClient));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddSingleton<IMongoClient>(new MongoClient(_runner.ConnectionString));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _runner?.Dispose();
        }
    }
}
