# Project Setup

## Prerequisites:
1. Visual Studio (for now). This is to build the project, manually migrate the database to Docker, and run tests.
   All this can be done with scripts but it's outside the scope of this L&L for now.
1. Docker. Download Docker desktop from the website and make sure it's running.

## Setup
For help with Docker commands required for setup, see the [SQL Server Docker readme file](docker/SQL-Server-Docker.md).

1. Create a sql server Docker container.
   Set the `sa` password to `LucyLem0n`.
1. Connect to the container and create a database called LucysLemonadeStandDB.
1. Migrate/Publish the LucysLemonadeStand.DB project to the LucysLemonadeStandDB database you just created.
1. Set a user-secret for "ConnectionStrings:Default" that contains the connection string to the database.
   Instructions for this can be found in appsettings.json. 
   With this step done you can run the Lucy's Lemonade Stand API. 
1. Commit the container to a new image called `lucyslemonadestand-test`.
   With this step done, you can run integration tests that use Docker.