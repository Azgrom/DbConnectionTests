using System.Data.Common;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace DbConnectionTests;

public static class TestDefinitions
{
    private const string MySqlServerAlias   = "localhost";
    private const string MySqlDatabaseAlias = "testContainersDb";

    public static IContainer BuildMySqlTestContainer()
    {
        const string testMySqlString     = "testmysqlimage";
        var          commonDirectoryPath = CommonDirectoryPath.GetSolutionDirectory();
        var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(commonDirectoryPath, "Tests/DbConnectionTests")
            .WithDockerfile("Dockerfile")
            .WithDeleteIfExists(true)
            .WithName(testMySqlString)
            .Build();

        var createImageTask = imageFromDockerfileBuilder.CreateAsync();
        createImageTask.Wait();

        return new ContainerBuilder()
            .WithImage(imageFromDockerfileBuilder)
            .WithEnvironment("MYSQL_ROOT_PASSWORD",        "toor")
            .WithEnvironment("MYSQL_DATABASE",             MySqlDatabaseAlias)
            .WithEnvironment("MYSQL_ALLOW_EMPTY_PASSWORD", "yes")
            .WithPortBinding(3306, 3306)
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(3306))
            .WithName("MySqlTestContainer")
            .Build();
    }

    public static string DefineTestDbConnectionString() =>
        new DbConnectionStringBuilder
        {
            { "Server", MySqlServerAlias },
            { "Database", MySqlDatabaseAlias },
            { "User ID", "root" },
            { "Password", "toor" }
        }.ConnectionString;
}
