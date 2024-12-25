# SmartChargingAPI Version 23

SmartChargingAPI is a RESTful service designed to manage charging stations, connectors, and groups efficiently. 
It utilizes **MongoDB** as the backend database and is implemented using **.NET 8**.

---

## Features

- **Group Management**: Create, retrieve, update, and delete groups.
- **Charge Station Management**: Manage charge stations within groups.
- **Connector Management**: Handle connectors associated with charge stations.
- **MongoDB Integration**: Seamless integration with MongoDB Atlas for cloud-based data storage. Do pre installations needed.

---


## Notes

- This project uses **MongoDB Atlas**. Ensure your connection string is valid (string in appsettings.json file is valid).
- Swagger documentation is available for API exploration.
- Some data in database already inserted for you to test.

---

## Prerequisites

### Tools Required
- **.NET 8 SDK**:

### Default MongoDB Configuration

No local MongoDB setup is required for this connection. 

---
## Project Structure

### Directories
- **SmartChargingAPI**: Main application code.
- **SmartChargingTEST**: Unit and integration tests.
---

## Running the Application

### Steps to Run
1. Navigate to the `SmartChargingAPI` directory:
    ```bash
    cd SmartChargingAPI
    ```
2. Restore dependencies:
    ```bash
    dotnet restore
    ```
3. Run the application:
    ```bash
    dotnet run
    ```
4. Access the API documentation at:
    ```
    http://localhost:5043/swagger/index.html
    ```

---

## Testing the Application

### Running Tests
1. Navigate to the `SmartChargingTEST` directory:
    ```bash
    cd SmartChargingTEST
    ```
2. Run all tests:
    ```bash
    dotnet test
    ```

Tests include:
- Integration Tests (using MongoDB Atlas, with test isolation for each scenario)

---

## API Endpoints

### Group Management

#### Create Group
- **Endpoint**: `POST /api/groups`
- **Request Body**:
    ```json
    [
      {
        "name": "Group Name",
        "capacityAmps": 100
      }
    ]
    ```
- **Response**: `200 OK`

#### Get All Groups
- **Endpoint**: `GET /api/groups`
- **Response**: `200 OK`

#### Get Group by ID
- **Endpoint**: `GET /api/groups/{groupId}`
- **Response**: `200 OK`

#### Update Group
- **Endpoint**: `PUT /api/groups/{groupId}`
- **Request Body**:
    ```json
    {
      "name": "Updated Group Name",
      "capacityAmps": 150
    }
    ```
- **Response**: `200 OK`

#### Delete Group
- **Endpoint**: `DELETE /api/groups/{groupId}`
- **Response**: `200 OK`

### Charge Station Management

#### Create Charge Station
- **Endpoint**: `POST /api/charge-stations/{groupId}`
- **Request Body**:
    ```json
    {
      "name": "Station Name",
      "connectors": [
        {
          "maxCurrentAmps": 50
        },
        {
          "maxCurrentAmps": 30
        }
      ]
    }
    ```
- **Response**: `200 OK`

#### Get Charge Station by ID
- **Endpoint**: `GET /api/charge-stations/{stationId}`
- **Response**: `200 OK`

#### Update Charge Station
- **Endpoint**: `PUT /api/charge-stations/{stationId}`
- **Request Body**:
    ```json
    {
      "name": "Updated Station Name"
    }
    ```
- **Response**: `200 OK`

#### Delete Charge Station
- **Endpoint**: `DELETE /api/charge-stations/{stationId}`
- **Response**: `200 OK`

### Connector Management

#### Add Connector
- **Endpoint**: `POST /api/stations/{stationId}/connectors`
- **Request Body**:
    ```json
    {
      "maxCurrentAmps": 50
    }
    ```
- **Response**: `200 OK`

#### Update Connector
- **Endpoint**: `PUT /api/stations/{stationId}/connectors/{connectorId}`
- **Request Body**:
    ```json
    {
      "maxCurrentAmps": 60
    }
    ```
- **Response**: `200 OK`

#### Delete Connector
- **Endpoint**: `DELETE /api/stations/{stationId}/connectors/{connectorId}`
- **Response**: `200 OK`
