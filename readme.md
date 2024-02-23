# Active Directory API HEP-BEJUNE

This API, developed by HEP-BEJUNE, serves as a proof of concept to demonstrate the feasibility of interfacing with an Active Directory (AD) to retrieve information and perform CRUD operations on users and groups. This lightweight API provides endpoints for listing user groups, users within a group, and adding a user to a group.

## Features

- **List User Groups**: Retrieve all groups a specified user belongs to.
- **List Group Members**: Get a list of all users within a specified group.
- **Add User to Group**: Add a specified user to a specified group, with authentication.

## Endpoints

### GET /api/ActiveDirectory/Users

- **Description**: List all groups a specified user belongs to.
- **Parameters**:
  - `username`: The SAM account name of the user.
- **Authentication**: Not required.

### GET /api/ActiveDirectory/Groups/{groupName}/Users

- **Description**: List all users in a specified group.
- **Parameters**:
  - `groupName`: The SAM account name of the group.
- **Authentication**: Not required.

### POST /api/ActiveDirectory/Groups/{groupName}/AddUser

- **Description**: Add a user to a specified group.
- **Parameters**:
  - `groupName`: The SAM account name of the group (in URL).
  - `username`: The SAM account name of the user (in request body).
- **Authentication**: Required. Credentials must be configured in the API.


## Disclaimer

This API is a proof of concept and is intended for demonstration purposes only. Please evaluate and test thoroughly before using in a production environment.