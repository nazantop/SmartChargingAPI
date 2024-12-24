using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SmartChargingAPI.Repositories.Implementation;
using SmartChargingAPI.Repositories.Interfaces;
using TechTalk.SpecFlow;

[Binding]
public class CommonHooks
{
    public IConfiguration Configuration { get; private set; }
    public IServiceProvider ServiceProvider { get; private set; }

    public readonly IGroupRepository _groupRepository;
    public readonly IChargeStationRepository _chargeStationRepository;

    public CommonHooks()
    {
        var serviceCollection = new ServiceCollection();

        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.test.json", optional: false)
            .Build();

        serviceCollection.AddSingleton(Configuration);

        serviceCollection.AddSingleton<IMongoClient>(new MongoClient(Configuration["MongoDB:ConnectionString"]));

        serviceCollection.AddTransient<IGroupRepository, GroupRepository>();
        serviceCollection.AddTransient<IChargeStationRepository, ChargeStationRepository>();

        ServiceProvider = serviceCollection.BuildServiceProvider();

        _groupRepository = ServiceProvider.GetRequiredService<IGroupRepository>();
        _chargeStationRepository = ServiceProvider.GetRequiredService<IChargeStationRepository>();
    }

    [BeforeScenario]
    public async Task SetupAsync()
    {
        // Clean up any existing groups before running the test
        var existingGroups = await _groupRepository.GetAllAsync();
        foreach (var group in existingGroups)
        {
            await _groupRepository.RemoveAsync(Guid.Parse(group.Id));
        }
    }

    [AfterScenario]
    public async Task CleanupAsync()
    {
        // Remove all groups after running the test
        var existingGroups = await _groupRepository.GetAllAsync();
        foreach (var group in existingGroups)
        {
            await _groupRepository.RemoveAsync(Guid.Parse(group.Id));
        }
    }
}
