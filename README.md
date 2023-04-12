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
- Dotnet 7 SDK
- Docker and docker-compose
- Node.js 18 and npm 9

### Run Draughts
To run a production build of draughts, you need some patience `;)` __WIP__.

### Setup development environment
Optional: Create _appsettings.env.json_ files to override existing settings.
The json files exist in the _Draughts_, _Draughts.IntegrationTest_ and _Draughts.Command_ projects.

Start the develop dependencies with `sudo ./run-dev.sh start`.
To build the frontend run `npm install && npm run build:dev`.
To initialise the database, run
```
cd Draughts.Command/ \
  && dotnet build Draughts.Command.csproj \
  && dotnet run --project Draughts.Command.csproj data:essential data:dummy \
  ;  cd ../
```

You can then run the application with something like `cd Draughts && dotnet run -v n`. It should
open at <http://localhost:52588>.

If you need to, you can access the databases with adminer at
[http://localhost:52580](http://localhost:52580/?server=draughts-db&username=root).
As credentials use _MySQL_, _draughts-db_, _root_, _root_. You can leave the database empty.
