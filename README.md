# Tracky .NET
Tracky .NET is a solution showcasing side-by-side ASP.NET MVC and ASP.NET WebAPI, Entity Framework, NHibernate, Structure Map, NUnit,
and general architectural happiness. It is centered around a domain for recorded music data (Artists, Songs, Albums, Genres, Playlists,
etc.).

The project was started for two primary reasons.
* To learn Entity Framework in an intimate way, using a code-first approach to build a domain from scratch, scaffold controllers
  with asynchronous controller actions, and provide unit and integration tests proving their behavior
* To practice side-by-side implementation of two frameworks I have worked with a lot in recent years, but in separate capacities:
  ASP.NET MVC and ASP.NET Web API
  
As I progressed through the project, it quickly expanded in scope, as my own personal preferences for solution architecture
began to creep in. In its initial published form (March 2017), the solution is focused primarily laying a solid architectural
foundation for these technologies (MVC and Web API), including comprehensive tests for both MVC and API controllers.

These controllers were all created via scaffolding, using asynchronous actions, and Entity Framework. MVC controllers were
created via Visual Studio's **MVC 5 Controller with views, using Entity Framework** option, while API controllers were created
via the **Web API 2 Controller with actions, using Entity Framework** option. In both cases the option for using async
controller actions was used. These controllers remain mostly in their native states, though they have been modified slightly
to facilitate dependency injection of the EF data context (any any other services), provide basic sorting for Index and GetAll-style
actions, and some additional indirection logic to allow proper mocking for unit testing MVC saves. This last part signifies
an effort that was part of the first reason for starting the project; though it would likely be painful, I wanted to go through
the experience of unit-testing scaffolded Entity Framework controllers, before writing integration tests for them, and before
refactoring them to decouple the data context completely in favor of a repository pattern, and domain services to handle all
repository interactions.

Thusly, the initial commit of this project is heavy on architecture, and very light on front-end development. However, I've
set the stage for more of my past (and future) Web work to be factored into this project. Here are some of the things I plan
to do next.
* Add a full NHibernate implementation against the existing database that was created with Entity Framework migrations. This
  will include a full set of repositories specific to NHibernate, which should ultimately return all the same data that 
  Entity Framework does.
* Add a CascadeDeleteTests fixture to the integration tests project, and further refine the cascading deletes that happen
  with EF's default database migration built from my model.
* Move in front-end architectural best practices (Compass/SASS/CSS, JavaScript, etc.) and begin to showcase more code and features
  from my past development that would make sense in a best-practice Web solution.
  
If you've landed on this project and found it in any way helpful, many cheers and thanks!
