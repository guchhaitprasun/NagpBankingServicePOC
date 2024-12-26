# Nagp Banking Service POC


## Table of Contents
- [Requirment](#requirment)
- [Assumptions](#assumptions)
- [Technologies Used](#technologies-used)
- [Project Setups](#project-setup)
- [Project Execution & Debugging](#project-execution)


## Requirment
Develop the services mentioned below and implement the inter-services communication as per the requirement. 
1.	**Account Service:** The Account Service will have the following features:
    - Get Account Statement API. 
    - Create Account API and on account creation will publish event like event_account_created. This event will be consumed by notification service 2 only (Rabbit MQ use topic exchange). 
    - Place request for PDF generation statement (call PDF generation service to place the request. Use RabbitMQ to communicate)
    
    Account Details may contain Account Number, Account Type (Saving/Current), Balance, etc. Account Statement may contain a list of transactions. The transaction will be from Account Number, to Account Number, Transaction Type (Debit/Credit), Amount, Date Time.

2.	**PDF generation Service:** The PDF generation service will have the following features:
    - Consumes request for PDF generation (consumes PDF generation requests placed in RabbitMQ) – It also uses account statement API from Account Service to get the statement details for PDF generation –(use gRPC for this.)
    - Publish event after PDF generation. This event will be consumed by both notification service 1 and notification service 2 (Rabbit MQ use fanout exchange) 

3.	**Notification service 1**
    - Consume the events raised by the Account service. The consumer needs to be connected to the rabbit MQ broker.

4.	**Notification service 2** 
    - Consume the events raised by the Account service for account creation and pdf generation event raised by Pdf generation Service. The consumer needs to be connected to the rabbit MQ broker. 


## Assumptions
While designing the solution the key focus area is to understand the concepts of **Services & Middleware**. Though the implementation follows the best practices but, following are the assumptions while developing the solution. 

1. Application is not ment for and is not redy for production. 
2. Intent of this project is to understand the fundamentals of Services and middleware
3. PDF generation & integration is functionally required but based on the premise of the assignment it is not implmented as it required a seperate research and integration effort. 

## Technologies Used
- **Backend:**
  - ASP.NET Core Web API
  - Entity Framework Core
  - RabbitMQ
  - gRCP
- **Database:**
  - SQL Server

## Project Setup

### Prerequisites
- Visual Studio 2022
- .NET Core SDK
- SQL Server 2019
- RabbitMQ Server


### Folder Details

run the `NagpBankingServicePOC.sln` file using Visual Studio and it will open with the following projects

- **`Account/AccountService`**: .Net WebAPIs with gRCP Server & RabbitMQ Publisher Implementation.
- **`Document/DocumentService`**: Console application with RabbitMQ Publisher and Consumer along with gRCP Client Implemnattion.
- **`Notification/NotificationService1`**: Console application with RabbitMQ  Consumer Implemnattion.
- **`Notification/NotificationService2`**: Console application with RabbitMQ  Consumer Implemnattion.
- **`Shared/SharedProject`**: Class Library for Shared DTOs enums and other resources.

### Configuration
- **Databse Configuration** 
    - make sure SQL Server and SSMS 19 is installed in your machine
    - open `appsettings.json` and set Databse name of your choice under `ConnectionStrings -> Default Connection` section.
    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=<your-server-name>;Database=<database-name>;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    ``` 
    - open **Package Manager Console** and run the following command, this with create a new database on your local database server using Entity framework. 
     ```bash
    Update-Database
    ```
- **RabbitMQ Configuration**
    - make sure RabbitMQ server instance is running on your machine 
    - pen appsettings.json and set credentials and queue for your Rabbit MQ Instance.
    ```json
    "RabbitMQ": {
        "HostName": "<your-rabbitmq-host>", //for locally running istance use localhost
        "UserName": "<server-username>", //default username is guest,
        "Password": "<server-password>" //default password is guest,
        "Exchange": "nagp_topic_exchange", // exchange name for topic queue, 
        "AccountsQueue"   : "account_queue", 
        "DocumentsQueue"   : "document_queue"  
    }
    ```
    - for **Document Service** set credentials under `Program.cs`
    - for **Notification Service 1** set credentials under `Program.cs`
    - for **Notification Service 2** set credentials under `Program.cs`'
- **gRCP Configuaration**
    - when running locally the API service will run on port `44336`, if this has been changes then the same needs to be updated in `DocumentService\gRCP\Client\AccountRcpClient.cs` line no 19
    

## Project Execution
After doing all the configuration run all the projects except shared project from Visual Studio.