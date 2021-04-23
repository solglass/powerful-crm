declare @DbVersion int

select top 1 @DbVersion = version
from dbo.DbVersion order by id desc

if @DbVersion is not null set noexec on

INSERT INTO dbo.[Role] VALUES
	(1, 'Client'),
	(2, 'Administrator')
GO

INSERT INTO dbo.DbVersion VALUES
	(1)
GO
set noexec off