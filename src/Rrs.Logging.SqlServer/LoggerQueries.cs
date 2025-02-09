﻿using Rrs.Dapper.Fluent;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Rrs.Logging.SqlServer
{
    class LoggerQueries
    {
        private readonly string _logTable;
        private readonly int _maxEntries;
        private readonly int _retentionDays;
        private readonly double _tolerance;
        private readonly string _insertCommand;
        private readonly string _deleteCommand;
        private readonly string _existsCommand;

        public LoggerQueries(string logTable, int maxEntries, int retentionDays, double tolerance)
        {
            _logTable = logTable;
            _maxEntries = maxEntries;
            _retentionDays = retentionDays;
            _tolerance = tolerance;

            _insertCommand = $"insert into {_logTable} (SoftwareId, Level, Object, ObjectType, Created) values (@SoftwareId, @Level, @Object, @ObjectType, getdate())";
            
            // check entries over max entries + tolerance percentage
            // delete by date to number of retention days
            // check max entries again in case all the logs are within that retention period
            // delete by max entries
            _deleteCommand = $@"
if (select min(Created) from {_logTable} where SoftwareId = @SoftwareId) < dateadd(day, -{_retentionDays}*{_tolerance}, getdate())
	delete from {_logTable}
	where	SoftwareId = @SoftwareId and 
			Created < dateadd(day, -{_retentionDays}, getdate())

if (select count(*) from {_logTable} where  SoftwareId = @SoftwareId) > {_maxEntries} * {_tolerance}
	delete from Log
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

if ident_current('{_logTable}') > 1000000000
	dbcc checkident('{_logTable}', RESEED, 0)
";

            _existsCommand = $@"
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
        }

        public Task Delete(IDbConnection c, Guid softwareId) => c.Sql(_deleteCommand).Parameters(new { softwareId }).Timeout(0).ExecuteAsync();

        public Task Create(IDbConnection c, LogEntry log)
        {
	        if (log.ObjectType.Length > 250) 
		        log.ObjectType = log.ObjectType.Substring(0, 249);

            return c.Sql(_insertCommand).Parameters(log).ExecuteAsync();
        }

        public void EnsureLogTableExists(IDbConnection c) => c.Sql(_existsCommand).Execute();
    }
}
