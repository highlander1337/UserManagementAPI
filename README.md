# UserManagementAPI

## Phase 1 - Creating and enhancing code with copilot

* Prompt copilot to set up folder structure architecture
* Prompt copilot to produce initial CRUD operation routes
* Prompt copilot to update User models to use a request/response dto strategy
* Prompt the copilot to add Swagger for program documentation.cs
* Add packages manually using nuget since copilot has issues to use the right version
* Update the code manually, since it's hard to get Copilot to use the right design patterns for the request/response DTO strategy.

## Phase 2 - Identify bugs

* Prompt copilot to add validation to ensure only valid user data is processed.
* Prompt copilot to add Exception Handle Middleware to handle internal server issues.
* Prompt copilot to add simple correlation id structure to Exception Handle Middleware for tracking.
* Prompt copilot to add pagination in get operation to enhance performance