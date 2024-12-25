using TechTalk.SpecFlow;
using Microsoft.Extensions.Logging;
using Moq;
using SmartChargingAPI.Models;
using SmartChargingAPI.Repositories.Interfaces;
using SmartChargingAPI.Services;
using NUnit.Framework;
using SmartChargingAPI.IServices;

[Binding]
[Scope(Feature = "Group Management")]
public class GroupManagementSteps
{
    private readonly IGroupService _groupService;
    private readonly IChargeStationService _chargeStationService;
    private readonly IConnectorService _connectorService;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;

    private Group _currentGroup;
    private List<Group> _retrievedGroups;
    private bool _operationResult;

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
    [Given(@"I add the group")]
    public async Task WhenIAddTheGroup()
    {
        var groupList = new List<Group> { _currentGroup };
        var result = await _groupService.AddGroup(groupList);
        _currentGroup = result.Data.FirstOrDefault();
        _operationResult = _currentGroup != null;
    }

    [Given(@"the group has a charge station named ""(.*)"" with the following connectors:")]
    public async Task WhenIAddAChargeStationNamedWithTheFollowingConnectors(string stationName, Table table)
    {
        var connectors = table.Rows.Select(row => new Connector(row["Name"],int.Parse(row["MaxCurrentAmps"]))).ToList();
   
        var station = new ChargeStation(stationName)
        {
            Connectors = connectors
        };

        var result = await _chargeStationService.AddChargeStation(Guid.Parse(_currentGroup.Id), station);
        _operationResult = result.IsSuccess;
    }

    [Then(@"the group should be added successfully")]
    public void ThenTheGroupShouldBeAddedSuccessfully()
    {
        Assert.IsTrue(_operationResult, "The group was not added successfully.");
    }

    [Given(@"I have the following groups:")]
    public async Task GivenIHaveTheFollowingGroups(Table table)
    {
        var groupList = table.Rows.Select(row => new Group(row["Name"], int.Parse(row["Capacity"]))).ToList();
        await _groupService.AddGroup(groupList);
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
        var result = await _groupService.UpdateGroup(new Guid(_currentGroup.Id), new Group(newName, newCapacity));
        _operationResult = result.IsSuccess;
    }

    [Then(@"the group should have name ""(.*)"" and capacity (.*)")]
    public async Task ThenTheGroupShouldHaveNameAndCapacity(string expectedName, int expectedCapacity)
    {
        var group = await _groupService.GetGroupById(new Guid(_currentGroup.Id));
        Assert.AreEqual(expectedName, group.Data.Name);
        Assert.AreEqual(expectedCapacity, group.Data.CapacityAmps);
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
        Assert.IsFalse(_operationResult, "The update should have failed but succeeded.");
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
        Assert.IsNull(group.Data);
    }

    [Then(@"no charge stations or connectors should remain")]
    public async Task ThenNoChargeStationsOrConnectorsShouldRemain()
    {
        var groups = await _groupService.GetGroups();
        Assert.IsFalse(groups.Data.Any(g => g.ChargeStations.Any()));
    }
    
} 