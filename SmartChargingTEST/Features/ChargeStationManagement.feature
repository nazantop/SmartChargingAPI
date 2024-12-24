Feature: Charge Station Management
  As a user
  I want to manage charge stations
  So that I can add, update, and remove charge stations within a group

  Scenario: Add a charge station to a group
    Given I have a group with name "Group A" and capacity 150
    When I add a charge station named "Station A" with capacity 50
    Then the station should be added successfully

  Scenario: Update a charge station's name
    Given I have a group with name "Group A" and capacity 150
    And the group has a charge station named "Station A"
    When I update the station's name to "Updated Station"
    Then the station's name should be "Updated Station"

  Scenario: Remove a charge station
    Given I have a group with name "Group A" and capacity 150
    And the group has a charge station named "Station A"
    When I remove the station
    Then the station should no longer exist

  Scenario: Delete a charge station and ensure all associated connectors are removed
    Given I have a group with name "Group A" and capacity 150
    And the group has a charge station named "Station A"
    And the charge station "Station A" has the following connectors:
      | Id | MaxCurrentAmps |
      | 1  | 10             |
      | 2  | 20             |
    When I remove the station
    Then the station should no longer exist
    And no connectors should remain in the group
