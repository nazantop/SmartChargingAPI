using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;
using SmartChargingAPI.Models;
using SmartChargingAPI.Repositories.Interfaces;
using SmartChargingAPI.Services;
using TechTalk.SpecFlow;

[Binding]
public class GroupManagementSteps
{
    private IConfiguration _configuration;
    private IMongoClient _mongoClient;
    private IMongoDatabase _database;
    private GroupService _groupService;
    private ChargeStationService _chargeStationService;
    private ConnectorService _connectorService;
    private Group _currentGroup;
    private List<Group> _retrievedGroups;
    private bool _operationResult;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;

    public GroupManagementSteps(CommonHooks hooks)
    {
        _groupRepository = hooks._groupRepository;
        _chargeStationRepository = hooks._chargeStationRepository;

        _groupService = new GroupService(_groupRepository, new Mock<ILogger<GroupService>>().Object);
        _chargeStationService = new ChargeStationService(
            _groupRepository,
            _chargeStationRepository,
            _groupService,
            new Mock<ILogger<ChargeStationService>>().Object);
        _connectorService = new ConnectorService(
            _groupRepository,
            _chargeStationRepository,
            new Mock<ILogger<ConnectorService>>().Object);
    }


    [Given(@"I have a group with name ""(.*)"" and capacity (.*)")]
    public void GivenIHaveAGroupWithNameAndCapacity(string name, int capacity)
    {
        _currentGroup = new Group(name, capacity);
    }

    [When(@"I add the group")]
    public async Task WhenIAddTheGroup()
    {
        var group = await _groupService.AddGroup(new Group(_currentGroup.Name, _currentGroup.CapacityAmps));
        _currentGroup = group.Data;
        _operationResult = _currentGroup != null;
    }

    [Then(@"the group should be added successfully")]
    public void ThenTheGroupShouldBeAddedSuccessfully()
    {
        Assert.IsTrue(_operationResult, "The group was not added successfully.");
    }

    [Given(@"I have the following groups:")]
    public async Task GivenIHaveTheFollowingGroups(Table table)
    {
        foreach (var row in table.Rows)
        {
            var name = row["Name"];
            var capacity = int.Parse(row["Capacity"]);
            await _groupService.AddGroup( new Group(name, capacity));
        }
    }

    [When(@"I retrieve all groups")]
    public async Task WhenIRetrieveAllGroups()
    {
        var result = await _groupService.GetGroups();
        _retrievedGroups = result.Data;
    }

    [Then(@"I should see the following groups:")]
    public void ThenIShouldSeeTheFollowingGroups(Table table)
    {
        foreach (var row in table.Rows)
        {
            var name = row["Name"];
            var capacity = int.Parse(row["Capacity"]);

            Assert.IsTrue(_retrievedGroups.Any(g => g.Name == name && g.CapacityAmps == capacity),
                $"Group with name {name} and capacity {capacity} was not found.");
        }
    }

    [When(@"I update the group to have name ""(.*)"" and capacity (.*)")]
    public async Task WhenIUpdateTheGroupToHaveNameAndCapacity(string newName, int newCapacity)
    {
        var result = await _groupService.UpdateGroup(new Guid(_currentGroup.Id), new Group(newName,newCapacity));
        _operationResult = result.IsSuccess;
    }

    [Then(@"the group should have name ""(.*)"" and capacity (.*)")]
    public async Task ThenTheGroupShouldHaveNameAndCapacity(string expectedName, int expectedCapacity)
    {
        var group = await _groupService.GetGroupById(new Guid(_currentGroup.Id));
        Assert.IsNotNull(group.Data, "The group does not exist.");
        Assert.AreEqual(expectedName, group?.Data?.Name, "The group name does not match.");
        Assert.AreEqual(expectedCapacity, group?.Data.CapacityAmps, "The group capacity does not match.");
    }

    [When(@"I update the group to have capacity (.*)")]
    public async Task WhenIUpdateTheGroupToHaveCapacity(int newCapacity)
    {
        _currentGroup.CapacityAmps = newCapacity;
        var result = await _groupService.UpdateGroup(new Guid(_currentGroup.Id), _currentGroup);
        _operationResult = result.IsSuccess;
    }

    [Then(@"the update should fail")]
    public void ThenTheUpdateShouldFail()
    {
        Assert.IsFalse(_operationResult, "The update should have failed but it succeeded.");
    }

    [When(@"I delete the group")]
    public async Task WhenIDeleteTheGroup()
    {
        var result = await _groupService.RemoveGroup(new Guid(_currentGroup.Id));
        _operationResult = result.IsSuccess;
    }

    [Then(@"the group should no longer exist")]
    public async Task ThenTheGroupShouldNoLongerExist()
    {
        var group = await _groupService.GetGroupById(new Guid(_currentGroup.Id));
        Assert.IsNull(group.Data, "The group still exists.");
    }

    [Then(@"no charge stations or connectors should remain")]
    public async Task ThenNoChargeStationsOrConnectorsShouldRemain()
    {
        var groups = await _groupService.GetGroups();
        Assert.IsFalse(groups?.Data?.Any(g => g.ChargeStations.Any()));
        Assert.IsFalse(groups?.Data?.SelectMany(g => g.ChargeStations).Any(cs => cs.Connectors.Any()));
    }

    [Given(@"the group has the following charge stations:")]
    public async Task GivenTheGroupHasTheFollowingChargeStations(Table table)
    {
        foreach (var row in table.Rows)
        {
            var station = new ChargeStation(row["Name"]);

            await _chargeStationService.AddChargeStation(new Guid(_currentGroup.Id), station);

            if (int.TryParse(row["Capacity"], out int capacity))
            {
                var numberOfConnectors = capacity / 10; 
                for (int i = 1; i <= numberOfConnectors; i++)
                {
                    var connector = new Connector(i.ToString(), 10);
                    await _connectorService.AddConnector(new Guid(station.Id), connector);
                }
            }
        }
    }


    [Given(@"the charge station ""(.*)"" has the following connectors:")]
    public async Task GivenTheChargeStationHasTheFollowingConnectors(string stationName, Table table)
    {
        var group = await _groupService.GetGroupById(new Guid(_currentGroup.Id));
        var station = group?.Data?.ChargeStations.FirstOrDefault(s => s.Name == stationName);

        foreach (var row in table.Rows)
        {
            var connector = new Connector(row["Id"], int.Parse(row["MaxCurrentAmps"]));
            await _connectorService.AddConnector(new Guid(station.Id), connector);
        }
    }
    

    [Given(@"I add the group")]
    public async Task GivenIAddTheGroup()
    {
        var result = await _groupService.AddGroup(_currentGroup);
        _currentGroup = result.Data;
        Assert.IsNotNull(_currentGroup, "The group was not added successfully.");
    }   
}