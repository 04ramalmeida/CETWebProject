# Aqua (Web)

## About
This project serves as a prototype for a web application meant for a fictional water company "Aqua". Built with ASP.NET Core 5 and following the MVC paradigm, it uses Entity Framework for interacting with a SQL Server database and uses ASP.NET Core Identity for user authentication and authorization.


This app has different features for different roles:
  - **Administrators**
      - Manage user accounts.
      - Review and approve or deny customer account requests.
  - **Employees**
      - View customer accounts and invoices.
      - Issue or delete invoices.
      - Access and manage water meter information.
      - Add, edit or delete water consumption readings.
      - Deliver water meters and process customer requests.
  - **Customers**
      - Manage their profile, including updating personal information and profile pictures.
      - View notifications and invoices, and make payments.
      - Monitor water meter details and consumption readings.
      - Submit water consumption readings.

## Setup

### Windows & Visual Studio 2022

1. [Install Visual Studio](https://visualstudio.microsoft.com/vs/)
 
2. Select the following pre-requisites for the Web App when installing Visual Studio
![Select ASP.NET and web development, .NET desktop development](https://i.imgur.com/1ICPHFT.png)
![Select Data storage and processing](https://i.imgur.com/SVYoZ9t.png)
Under Individual components:
![Select .NET 5.0 Runtime](https://i.imgur.com/SJgxx7N.png)
![Select Data sources for SQL Server support and SQL Server Express 2019 LocalDB](https://i.imgur.com/ROUBzh6.png)
3. [Clone the repository using Visual Studio](https://learn.microsoft.com/en-us/visualstudio/version-control/git-clone-repository?view=vs-2022)
4. Once you open the project, create a appsettings.json file based on the example file.
5. 
[Mobile app](https://github.com/04ramalmeida/AquaApp)
