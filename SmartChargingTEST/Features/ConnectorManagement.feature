Feature: Connector Management
  As a user
  I want to manage connectors within charge stations
  So that I can add, update, and remove connectors while enforcing system rules

  Scenario: Add a connector to a charge station
    Given I have a group with name "Group A" and capacity 150
    And the group has a charge station named "Station B" with the following connectors:
      | Id | MaxCurrentAmps | Name |
      | 1  | 50             |  C   |
    When I add a connector with ID 2 and max current 50
    Then the connector should be added successfully

  Scenario: Update a connector's max current
    Given I have a group with name "Group A" and capacity 150
    And the group has a charge station named "Station B" with the following connectors:
      | Id | MaxCurrentAmps | Name |
      | 1  | 50             |  C   |
    When I update the connector's max current to 60
    Then the connector's max current should be 60

  Scenario: Remove a connector
    Given I have a group with name "Group A" and capacity 150
    And the group has a charge station named "Station B" with the following connectors:
      | Id | MaxCurrentAmps | Name |
      | 1  | 50             |  C   |
    When I remove the connector with ID 1
    Then the connector should no longer exist

  Scenario: Exceed group capacity with a connector
    Given I have a group with name "Group A" and capacity 100
    And the group has a charge station named "Station B" with the following connectors:
      | Id | MaxCurrentAmps | Name |
      | 1  | 50             |  C   |
    When I add a connector with ID 2 and max current 60
    Then the addition should fail