# SQL Server in Docker

Having a Database in Docker is a great way to have an easy way to create environments for testing, since the instances can be copied off a base, modified as much as you want, and then torn down.
Having a local database is also great for development since there is no danger of other people interfering with your environment.

# Getting Started

The command to download the Docker image for SQL Server and run a container off it is:

    docker run -it \
        -e "ACCEPT_EULA=Y" \
        -e "SA_PASSWORD=LucyLem0n" \
        -p 1433:1433 \
        --name sql-server-2022 \
        mcr.microsoft.com/mssql/server:2022-latest

The backslash (\\) is used to nicely format the command arguments on separate lines for bash-like shells. For CMD you can use the caret symbol (^) instead of (/), and for PowerShell you can use the backtick symbol (`) instead.

Here's what the arguments do:

`-e "ACCEPT_EULA=Y"`, sets an environment variable read by SQL Server stating that you accept the end user license agreement

`-e "SA_PASSWORD=LucyLem0n"`, sets an environment variable for the system administrator password

`-p 1433:1433`, forwards the SQL Server port from inside the container to outside the container, without this you could not access SQL Server from your host computer

`--name sql-server-2022`, give the container instance a name (must be all lowercase)

`mcr.microsoft.com/mssql/server:2022-latest`, the image to download from Docker hub

When using `docker run` as above, the Docker CLI executes `docker pull` to download the image, `docker create` to create a container for the image, and finally `docker run` to run the container.

After this step is done, use the normal methods like SSMS to connect to the new instance that exists in `localhost` and use the username `sa` and the password provided in the command.

# Preparation for integration testing

After migrating/publishing changes to the database you created, commit the container into a **new image** to set a baseline for tests to run against.
This can be done with the following command:

    docker commit existing-container-name-or-hash-id new-image-name

The first time you wan too do this, the exact command would look like this:

    docker commit sql-server-2022 lucyslemonadestand-test

Note that the container and image names must be lowercase.

There are additional options for this command. See this link for a more in-depth breakdown: [https://www.dataset.com/blog/create-docker-image/](https://www.dataset.com/blog/create-docker-image/).

