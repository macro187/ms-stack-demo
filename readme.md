Disclaimer
==========

This thing is junk and is only here for archival purposes.



Introduction
============

This is an anonymised version of a .NET stack demo application I did in 2014 for
a job interview.

At the time, I had never used ASP.NET MVC and hadn't touched Entity Framework
in over 4 years.

The criteria was to build an ASP.NET MVC application from scratch with no tool assistance.

I built this thing in a week using Vim, GNU Make, command line C# compiler and
NuGet, the Cassini development web server, and (I think) SQL Server Express.

After all this trouble, the company made me a shitty job offer which I declined.



Description
===========

`Demo.System` contains string functions and argument guards.

`Demo.EntityFramework` contains hackery to redirect EF from public properties to
private backing properties.

`Demo.Courses` is a UI and persistence ignorant
[domain](https://en.wikipedia.org/wiki/Domain-driven_design), consisting of a
couple of model types and a repository interface and abstract base class.

`Demo.Database` contains an EF database context and a database-backed
implementation of the repository.

`Demo.Web.Controllers` is the compiled bits of the ASP.NET MVC application.

`Demo.Web.Application` is the non-compiled bits of the ASP.NET MVC web
application.

Things ending in `.Test` are unit tests.

The remainder are stubs for NuGet packages.

The thing builds with GNU Make and my
[Makery](https://github.com/macro187/makery) build scripts.

