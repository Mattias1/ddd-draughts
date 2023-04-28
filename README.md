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


Setup development environment
------------------------------
### Dependencies
- Dotnet 7 SDK
- Node.js 18 and npm 9
- Docker and docker-compose

### Run Draughts
To run a production build of draughts, you need some patience `;)` __WIP__.

### Setup development environment
Optional: Create _appsettings.env.json_ files to override existing settings.
The json files exist in the _Draughts_, _Draughts.IntegrationTest_ and _Draughts.Command_ projects.

Start and initialise the develop dependencies with `sudo ./run-dev.sh start`.
To build the frontend run `npm install && npm run build:dev`.

You can then run the application with something like `cd Draughts && dotnet run -v n`. It should
open at [http://localhost:52588](http://localhost:52588).

If you need to, you can access the databases with adminer at
[http://localhost:52580](http://localhost:52580/?server=draughts-db&username=root).
As credentials use _MySQL_, _draughts-db_, _root_, _root_. You can leave the database empty.


Release instructions
---------------------
Build a release: `./build-release.sh`

Deploy with something like:
```
sudo docker container rm -f draughts \
  ; sudo docker image rm draughts \
  ; sudo docker load -i ./Draughts/publish/docker-image-draughts.tar \
  ; sudo docker run --name draughts -p 8000:8000 -v ${PWD}/Draughts/logs:/app/logs:z draughts
```
Note:
- Make sure the user 'dkr-user' with uid '1042' has write access to the logs dir.
- You should probably add something like `--restart always`,
  `-v some-dir/appsettings.env.json:/app/appsettings.env.json:ro` and
  `-v some-dir/my-cert.pfx:/app/cert.pfx:ro` to the docker run command.
