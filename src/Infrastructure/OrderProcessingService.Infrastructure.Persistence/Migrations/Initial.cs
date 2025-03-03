using FluentMigrator;

namespace OrderProcessingService.Infrastructure.Persistence.Migrations;

[Migration(13489, nameof(Initial))]
public class Initial : Migration
{
    public override void Up()
    {
        const string sql = """
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
        (
            order_id          bigint primary key        not null,
        
            order_state       order_state               not null,
            order_created_at  timestamp with time zone  not null,
            order_updated_at  timestamp with time zone  not null 
        );
        
        create table outbox
        (
            message_id           bigint    primary key generated always as identity,
            
            message_type         text      not null,
            message_key          bytea     not null,
            message_value        bytea     not null,
            message_created_at   timestamp with time zone not null,
            message_processed_at timestamp with time zone
        );
        
        create unique index inbox_key_value_idx on outbox (message_key, message_value);
        
        create table inbox
        (
            message_id           bigint    primary key generated always as identity,
            
            message_key          bytea     not null,
            message_value        bytea     not null,
            message_created_at   timestamp with time zone not null,
            message_processed_at timestamp with time zone
        );
        
        create unique index outbox_key_value_idx on inbox (message_key, message_value);
        """;

        Execute.Sql(sql);
    }

    public override void Down()
    {
        const string sql = """
        drop table orders;
        drop type order_state;
        drop table inbox;
        drop table outbox;
        """;

        Execute.Sql(sql);
    }
}