# Issue Tracker - Ticket System

## Basic premise
Ticket system with individual user cookie authentication. Possibility to create projects restricted only for assigned users. Dashboard with project summary.
Creating issues (tickets) with descriptions, comments, attachments. Issues have their own members who can change priority and status of it.

### Technologies
* ASP.NET Core MVC
* Entity Framework Core
* Clean architecture
* CQRS + MediatR
* Bootstrap 5
* Unit and integration testing (Xunit) - TDD

### Features
* ✅ Registration + email confirmation + password recovery
* ✅ Users roles administration
* ✅ Integration testing enviroment setup (factory + helpers methods)
* ✅ CRUD actions for Project entity
* ✅ Assigning members to Projects
* 🔜 CRUD actions for Issue entity
* 🔜 Assigning members to Issues
* 🔜 Working with Issues (commenting, changing status etc.)
* 🔜 Projects dashboard

> I hold off with development of this project for some time. I wasn't happy with the quality of my code for the frontend part.
> I decided that it would be benefitial for me and my growth as a developer to first improve my knowledge and skill in web technologies like HTML/CSS/JS.

## How to run
Please clone this repository and open in Visual Studio 2022.
### Configuration
#### DB config
If you're on Windows and have *localdb* installed you should be good to go. Otherwise, please change `"ConnectionStrings":"DefaultConnection"` in `appsettings.json` to point to MS SQL server of your choice.
#### SendGrid config
Application is using [SendGrid API](https://docs.sendgrid.com/). Setting it up in `appsettings.json` with your credentials is required for Registration/Password recovery to work properly.
