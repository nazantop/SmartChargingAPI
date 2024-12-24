using SmartChargingAPI.Models;
using TechTalk.SpecFlow;
using SmartChargingAPI.IServices;
using SmartChargingAPI.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using SmartChargingAPI.Services;

[Binding]
[Scope(Feature = "Charge Station Management")]
public class ChargeStationManagementSteps
{
    private readonly IGroupService _groupService;
    private readonly IChargeStationService _chargeStationService;
    private readonly IConnectorService _connectorService;

    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;

    private Group _currentGroup;
    private ChargeStation _currentStation;
    private bool _operationResult;

    public ChargeStationManagementSteps(CommonHooks hooks)
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
    public async Task GivenIHaveAGroupWithNameAndCapacity(string name, int capacity)
    {
        var result = await _groupService.AddGroup(new Group(name, capacity));
        _currentGroup = result?.Data;
    }

    [When(@"I add a charge station named ""(.*)"" with capacity (.*)")]
    public async Task WhenIAddAChargeStationNamedWithCapacity(string stationName, int capacity)
    {
        _currentStation = new ChargeStation(stationName);
        var result = await _chargeStationService.AddChargeStation(Guid.Parse(_currentGroup.Id), _currentStation);
        _operationResult = result.IsSuccess;
    }

    [Then(@"the station should be added successfully")]
    public void ThenTheStationShouldBeAddedSuccessfully()
    {
        Assert.IsTrue(_operationResult, "The station was not added successfully.");
    }

    [Given(@"the group has a charge station named ""(.*)""")]
    public async Task GivenTheGroupHasAChargeStationNamed(string stationName)
    {
        _currentStation = new ChargeStation(stationName);
        await _chargeStationService.AddChargeStation(Guid.Parse(_currentGroup.Id), _currentStation);
    }

    [When(@"I update the station's name to ""(.*)""")]
    public async Task WhenIUpdateTheStationSNameTo(string newName)
    {
        var chargeStation = new ChargeStation(newName);
        var result = await _chargeStationService.UpdateChargeStation(Guid.Parse(_currentStation.Id), chargeStation);
        _operationResult = result.IsSuccess;
        if (_operationResult)
        {
            _currentStation.Name = newName;
        }
    }

    [Then(@"the station's name should be ""(.*)""")]
    public void ThenTheStationSNameShouldBe(string expectedName)
    {
        Assert.AreEqual(expectedName, _currentStation.Name, "The station's name does not match.");
    }

    [When(@"I remove the station")]
    public async Task WhenIRemoveTheStation()
    {
        var result = await _chargeStationService.RemoveChargeStation(Guid.Parse(_currentStation.Id));
        _operationResult = result.IsSuccess;
    }

    [Then(@"the station should no longer exist")]
    public async Task ThenTheStationShouldNoLongerExist()
    {
        var group = await _groupService.GetGroupById(Guid.Parse(_currentGroup.Id));
        var stationExists = group?.Data?.ChargeStations.Any(s => s.Id == _currentStation.Id);
        Assert.IsFalse(stationExists, "The station still exists in the group.");
    }

    [Given(@"the charge station ""(.*)"" has the following connectors:")]
    public async Task GivenTheChargeStationHasTheFollowingConnectors(string stationName, Table table)
    {
        var group = await _groupService.GetGroupById(Guid.Parse(_currentGroup.Id));
        var station = group?.Data?.ChargeStations.FirstOrDefault(s => s.Name == stationName);
        Assert.IsNotNull(station, $"Charge station {stationName} does not exist in the group.");

        foreach (var row in table.Rows)
        {
            var maxCurrentAmps = int.Parse(row["MaxCurrentAmps"]);
            var connector = new Connector{
                MaxCurrentAmps = maxCurrentAmps
            };
            await _connectorService.AddConnector(Guid.Parse(station.Id), connector);
        }
    }

    [Then(@"no connectors should remain in the group")]
    public async Task ThenNoConnectorsShouldRemainInTheGroup()
    {
        var group = await _groupService.GetGroupById(Guid.Parse(_currentGroup.Id));
        var connectorsExist = group?.Data?.ChargeStations.Any(s => s.Connectors.Any());
        Assert.IsFalse(connectorsExist, "Some connectors still exist in the group.");
    }
}