Feature: Group Management
  As a user
  I want to manage groups
  So that I can create, update, retrieve, and delete groups while enforcing system rules

  Scenario: Create a new group successfully
    Given I have a group with name "Group A" and capacity 100
    When I add the group
    Then the group should be added successfully

  Scenario: Retrieve all groups
    Given I have the following groups:
      | Name       | Capacity |
      | Group A    | 100      |
      | Group B    | 200      |
    When I retrieve all groups
    Then I should see the following groups:
      | Name       | Capacity |
      | Group A    | 100      |
      | Group B    | 200      |

  Scenario: Update a group's name and capacity
    Given I have a group with name "Group A" and capacity 100
    And I add the group
    When I update the group to have name "Updated Group" and capacity 150
    Then the group should have name "Updated Group" and capacity 150

  Scenario: Update a group's capacity below its current usage
    Given I have a group with name "Group A" and capacity 100
    And I add the group
    And the group has a charge station named "Station A" with the following connectors:
      | MaxCurrentAmps | Name |
      | 50             |  C   |
    And the group has a charge station named "Station B" with the following connectors:
      | MaxCurrentAmps | Name |
      | 50             |  D   |
    When I update the group to have capacity 90
    Then the update should fail

  Scenario: Delete a group and ensure all associated charge stations and connectors are removed
  Given I have a group with name "Group A" and capacity 150
  And I add the group
  And the group has a charge station named "Station A" with the following connectors:
    | MaxCurrentAmps | Name |
    | 50             |  C   |
  And the group has a charge station named "Station B" with the following connectors:
    | MaxCurrentAmps | Name |
    | 50             |  D   |
  When I delete the group
  Then the group should no longer exist
  And no charge stations or connectors should remain
