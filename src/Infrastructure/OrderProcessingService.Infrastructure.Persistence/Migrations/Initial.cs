using FluentMigrator;

namespace OrderProcessingService.Infrastructure.Persistence.Migrations;

[Migration(1192837, nameof(Initial))]
public class Initial : Migration
{
    public override void Up()
    {
        const string sql =
        """
        create type order_state as enum
        (
            'created',
            'pending_approval',
            'approved',
            'packing',
            'packed',
            'in_delivery',
            'delivered',
            'cancelled'
        );
        
        create table orders
        (x
            order_id          bigint primary key        not null,

            order_state       order_state               not null,
            order_created_at  timestamp with time zone  not null,
            order_updated_at  timestamp with time zone  not null 
        );
        """;

        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql =
        """
        drop table orders;
        drop type order_state;
        """;

        Execute.Sql(sql);
    }
}