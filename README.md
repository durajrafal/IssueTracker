# Issue Tracker - Ticket System
![Tests](https://github.com/durajrafal/IssueTracker/actions/workflows/ci_cd.yml/badge.svg)
[![Live Demo](https://img.shields.io/badge/Live%20Demo-Visit-darkgreen)](https://issuetrackerdemoduraj.azurewebsites.net/)
[![LinkedIn](https://img.shields.io/badge/-LinkedIn-blue?style=flat-square&logo=Linkedin&logoColor=white&link=https://www.linkedin.com/in/rafal-duraj/)](https://www.linkedin.com/in/rafal-duraj/)

## Basic premise
Ticket system with individual user cookie authentication. Possibility to create projects restricted only for assigned users. Dashboard with project summary.
Creating issues (tickets) with descriptions, comments, attachments. Issues have their own members who can change priority and status of it. 

**Live demo is accessible [here](https://issuetrackerdemoduraj.azurewebsites.net/).**

### Technologies
* ASP.NET Core MVC
* Entity Framework Core
* Clean architecture
* CQRS + MediatR
* Unit and integration testing (Xunit) - TDD
* FluentValidation
* FluentAssertions
* Bootstrap 5
* Typescript

### Features
* ✅ Registration + email confirmation + password recovery
* ✅ Users roles administration
* ✅ Integration testing enviroment setup (factory + helpers methods)
* ✅ CRUD actions for Project entity
* ✅ Assigning members to Projects
* ✅ Project access only for members
* ✅ CRUD actions for Issue entity
* ✅ Auditing changes for entites
* ✅ Assigning members to Issues
* 🔜 User notification
* 🔜 Working with Issues (commenting, attachments, task lists etc.)
* 🔜 Projects dashboard

> ### Note
> My main focus when I started this project was backend. However I decided to implement also frontend to make it more enjoyable to explore. I chose razor views as a simple default solution. In some places I wanted to have more interactive functionality so I used Typescript and created API endpoints to fulfill this. That's why I ended up with some mix-up of both kind of endpoints. I'm aware that better and cleaner solution would be to create a REST API and have separate frontend project. I've made this sacrifice so I could focus mostly on backend before learning any of frontend frameworks, while still having some decent GUI.

## How to run locally
Please clone this repository and open in Visual Studio 2022.
### Configuration
#### DB config
If you're on Windows and have *localdb* installed you should be good to go. Otherwise, please change `"ConnectionStrings":"DefaultConnection"` in `appsettings.json` to point to MS SQL server of your choice.
#### SendGrid config
Application is using [SendGrid API](https://docs.sendgrid.com/). Setting it up in `appsettings.json` with your credentials is required for Registration/Password recovery to work properly.
