using FluentMigrator;

namespace OrderProcessingService.Infrastructure.Persistence.Migrations;

[Migration(-1, nameof(Initial))]
public class Initial : Migration
{
    public override void Up()
    {
    }

    public override void Down()
    {
        const string sql =
        """
        drop table orders;
        drop type order_state;
        drop table inbox;
        drop table outbox;
        """;

        Execute.Sql(sql);
    }
}