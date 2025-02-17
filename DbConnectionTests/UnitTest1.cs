using System.Data;
using DotNet.Testcontainers.Containers;
using MySql.Data.MySqlClient;
using Xunit.Abstractions;

namespace DbConnectionTests;

public class UnitTest1 : IAsyncLifetime
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly string            _testConnectionString = TestDefinitions.DefineTestDbConnectionString();
    private readonly IContainer        _mySqlDbContainer    = TestDefinitions.BuildMySqlTestContainer();
    public UnitTest1(ITestOutputHelper testOutputHelper) { _testOutputHelper = testOutputHelper; }

    [Fact]
    public void Test1()
    {
        using var cnn = new MySqlConnection(_testConnectionString);
        using var cmd = cnn.CreateCommand() ??
                        throw new ArgumentNullException(nameof(cnn.CreateCommand), @"Cannot accept null command");

        cmd.CommandType = CommandType.Text;
        cmd.CommandText = MySqlTestQueries.HealthCheckPingDatabase;

        cnn.Open();

        using var dataReader = cmd.ExecuteReader();
        using var dataTable  = dataReader.GetSchemaTable() ?? throw new NullReferenceException();

        foreach (var dataRow in dataTable.Rows.Cast<DataRow>())
        {
            _testOutputHelper.WriteLine(dataRow.ToString());
        }

        Thread.Sleep(TimeSpan.FromSeconds(5));
    }

    public async Task InitializeAsync()
    {
        await _mySqlDbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _mySqlDbContainer.StopAsync();
        await _mySqlDbContainer.DisposeAsync();
    }
}