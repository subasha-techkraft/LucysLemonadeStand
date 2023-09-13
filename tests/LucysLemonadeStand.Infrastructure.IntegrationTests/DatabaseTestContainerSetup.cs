using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System;

namespace LucysLemonadeStand.Infrastructure.IntegrationTests;
public class DatabaseTestContainerSetup : IAsyncLifetime
{
    private readonly MsSqlTestcontainer _dbContainer;

    public string ConnectionString { get => _dbContainer.ConnectionString; }

    public DatabaseTestContainerSetup()
    {        
        //Make sure that no instance of this image is running on port 1433 at the time of the test, it may cause issues.

        //note that this way of writing this setup is obselete in TestContainers version 3.0.0.
        //I tried using that setup but found that it made configuration very confusing or impossible.
        //I couldn't find a way to set the password.
        //I think the assumption they made is that everyone will use a program that will seed the database on startup if it's empty.
        //This way the container can be a totally blank MS SQL server container and therefore the credentials and database name are not important.
        //That's not easy to handle without something like Entity Framework migrations and is also slower than running a pre-built database Docker image.
        //As a result I'm using an older version that I got to work using older documentation.
        //Luckily, it's fairly simple code.
        _dbContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration()
            {
                Database = "LucysLemonadeStandDB",
                //Username = "sa", //this is assumed, and trying to change it may cause an exception.
                //This may have been supported in older versions
                Password = "LucyLem0n",
            })
            .WithImage("lucyslemonadestand-test")
            .Build();
        //The above setup and build will take care of creating a container off the given image,
        //choosing a random unused port,
        //waiting for the container to be fully up and running and available on that port,
        //and creating the connection string.
        //Multiple instances of this can be created at the same time and should not conflict with each other.

        //I found this article on Testcontainers and it includes some info on how to configure it for when the test container is itself inside a pipeline: https://semaphoreci.com/blog/integration-tests#docker-compose
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}
