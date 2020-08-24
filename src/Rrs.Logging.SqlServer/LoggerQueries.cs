using Rrs.Dapper.Fluent;
using System.Data;

namespace Rrs.Logging.SqlServer
{
    class LoggerQueries
    {
        private readonly string _logTable;
        private readonly int _maxEntries;

        public LoggerQueries(string logTable, int maxEntries)
        {
            _logTable = logTable;
            _maxEntries = maxEntries;
        }

        public void Create(IDbConnection c, LogEntry log)
        {
	        if (log.ObjectType.Length > 250) 
		        log.ObjectType = log.ObjectType.Substring(0, 249);

	        var command = $@"
insert into {_logTable} (SoftwareId, Level, Object, ObjectType, Created) values (@SoftwareId, @Level, @Object, @ObjectType, getdate())

delete from {_logTable}
where Id in (
	select Id
	from
	(
		select Id, row_number() over(order by fromtheashes, Id desc) n
		from (
			select case when Id < ident_current('{_logTable}') + 1 then 0 else 1 end fromtheashes, Id
			from {_logTable}
            where SoftwareId = @SoftwareId
		) t
	) t
	where n - {_maxEntries} > 0
) 

if (ident_current('{_logTable}')) > 1000000000
    dbcc checkident('{_logTable}', RESEED, 0)

";
            c.Sql(command).Parameters(log).Execute();
        }

        public void EnsureLogTableExists(IDbConnection c)
        {
            var command = $@"
if (select object_id('{_logTable}')) is null
begin
    create table {_logTable}
    (
	    Id int identity,
        SoftwareId uniqueidentifier not null,
        Level int not null,
	    Object nvarchar(max) null,
        ObjectType nvarchar(250) null,
	    Created datetime not null,
	    constraint PK_{_logTable}_Id primary key (Id)
    )

    create index IX_{_logTable}_SC on Log (SoftwareId, Created)
    create index IX_{_logTable}_SLC on Log (SoftwareId, Level, Created)
end
";
            c.Sql(command).Execute();
        }



    }
}
