Domain-driven design and Draughts
==================================
This is a little webapplication where people can come and play a game of Draughts (or Checkers if
you like).
The goals of this project are:
- To experiment and play with the DDD architecture
- To serve as an example of how to apply DDD in a real application
- To teach others what DDD is
- To play a fun game of Draughts


Documentation
--------------
The documentation is a part of the application. You can find the link to the
[documentation](http://localhost:52588/documentation) in the footer after you run it, or you can
look at the [source files](/Draughts/Application/Documentation/Views) of the docs.


Setup
------
### Dependencies
- Docker desktop:
  [docker.com/products/docker-desktop](https://www.docker.com/products/docker-desktop).

### Run Draughts
To run the application without IDE, you need some patience `;)` __WIP__.

### Setup development environment
Start the develop dependencies with `sudo ./run-dev.sh start`.
To initialise the database, run `dotnet build Draughts.Command/Draughts.Command.csproj`
and `dotnet run --project Draughts.Command/Draughts.Command.csproj data:essential data:dummy`.

You can then edit and run the application with your favourite IDE. It should open at
<http://localhost:52588>.

If you need to, you can access the databases with adminer at
[http://localhost:52580](http://localhost:52580/?server=draughts-db&username=root).
As credentials use _MySQL_, _draughts-db_, _root_, _root_. You can leave the database empty.
