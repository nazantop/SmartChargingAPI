using SmartChargingAPI.Models;
using TechTalk.SpecFlow;
using SmartChargingAPI.IServices;
using SmartChargingAPI.Services;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Moq;
using Microsoft.Extensions.Logging;
using SmartChargingAPI.Repositories.Interfaces;

[Binding]
[Scope(Feature = "Connector Management")]
public class ConnectorManagementSteps
{
    private readonly IGroupService _groupService;
    private readonly IChargeStationService _chargeStationService;
    private readonly IConnectorService _connectorService;
    private readonly IGroupRepository _groupRepository;
    private readonly IChargeStationRepository _chargeStationRepository;

    private Group? _currentGroup;
    private ChargeStation? _currentStation;
    private Connector? _currentConnector;
    private bool _operationResult;

    public ConnectorManagementSteps(CommonHooks hooks)
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
        var groupList = new List<Group> { new Group(name, capacity) };
        var result = await _groupService.AddGroup(groupList);
        _currentGroup = result?.Data.FirstOrDefault();
    }

    [Given(@"the group has a charge station named ""(.*)"" with the following connectors:")]
    public async Task GivenTheGroupHasAChargeStationNamedWithConnectors(string stationName, Table table)
    {
       var connectors = table.Rows.Select(row => new Connector(row["Name"],int.Parse(row["MaxCurrentAmps"]))).ToList();

        _currentStation = new ChargeStation(stationName)
        {
            Connectors = connectors
        };

        var result = await _chargeStationService.AddChargeStation(Guid.Parse(_currentGroup.Id), _currentStation);
        _currentStation = result.Data;
        _operationResult = result.Data == null ? false : true;
        _currentConnector = connectors.FirstOrDefault();
    }

    [When(@"I add a connector with ID (.*) and max current (.*)")]
    public async Task WhenIAddAConnectorWithIDAndMaxCurrent(int id, int maxCurrent)
    {
        var connector = new Connector(id.ToString(), maxCurrent);
        var result = await _connectorService.AddConnector(Guid.Parse(_currentStation.Id), connector);
        _currentConnector = result.Data;
        _operationResult = _currentConnector != null;
    }

    [Then(@"the connector should be added successfully")]
    public void ThenTheConnectorShouldBeAddedSuccessfully()
    {
        Assert.IsTrue(_operationResult, "The connector was not added successfully.");
    }

    [Given(@"the station has a connector with ID (.*) and max current (.*)")]
    public async Task GivenTheStationHasAConnectorWithIDAndMaxCurrent(int id, int maxCurrent)
    {
        var connector = new Connector(id.ToString(), maxCurrent);
        var result = await _connectorService.AddConnector(Guid.Parse(_currentStation.Id), connector);
        _currentConnector = result?.Data;
        Assert.IsNotNull(_currentConnector, "The connector was not added to the station.");
    }

    [When(@"I update the connector's max current to (.*)")]
    public async Task WhenIUpdateTheConnectorSMaxCurrentTo(int newMaxCurrent)
    {
        _currentConnector.MaxCurrentAmps = newMaxCurrent;
        var result = await _connectorService.UpdateConnector(Guid.Parse(_currentStation.Id), _currentConnector);
        _operationResult = result.IsSuccess;
    }

    [Then(@"the connector's max current should be (.*)")]
    public void ThenTheConnectorSMaxCurrentShouldBe(int expectedMaxCurrent)
    {
        Assert.AreEqual(expectedMaxCurrent, _currentConnector.MaxCurrentAmps, "The connector's max current does not match.");
    }

    [When(@"I remove the connector with ID (.*)")]
    public async Task WhenIRemoveTheConnectorWithID(int id)
    {
        var result = await _connectorService.RemoveConnector(Guid.Parse(_currentStation.Id), id);
        _operationResult = result.IsSuccess;
    }

    [Then(@"the connector should no longer exist")]
    public async Task ThenTheConnectorShouldNoLongerExist()
    {
        var group = await _groupService.GetGroupById(Guid.Parse(_currentGroup.Id));
        var station = group?.Data?.ChargeStations.FirstOrDefault(s => s.Id == _currentStation.Id);
        var connectorExists = station?.Connectors.Any(c => c.Id == _currentConnector.Id);
        Assert.IsFalse(connectorExists, "The connector still exists.");
    }

    [Then(@"the addition should fail")]
    public void ThenTheAdditionShouldFail()
    {
        Assert.IsFalse(_operationResult, "The addition should have failed but it succeeded.");
    }
}
