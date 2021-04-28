declare @DbVersion int

select top 1 @DbVersion = version
from dbo.DbVersion order by id desc

if @DbVersion > 2 set noexec on
begin transaction
-- Create column RoleId
alter table [dbo].[Lead] add [RoleId] int null default 1
go
update [dbo].[Lead] set [dbo].[Lead].[RoleId] = 1
go
alter table [dbo].[Lead] alter column [RoleId] int not null
go
-- Create table Account
CREATE TABLE [dbo].[Account]
(
	[Id] INT identity(1,1) NOT NULL, 
    [Name] NVARCHAR(100) NOT NULL, 
    [Currency] INT NOT NULL, 
    [LeadId] INT NOT NULL

CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
([Id] ASC) WITH
(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
 ON [PRIMARY],

 CONSTRAINT [UQAccount5E55825B7B2276C4] UNIQUE NONCLUSTERED 
([Currency],[LeadId] ASC )
WITH 
(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
ON [PRIMARY],

)
GO

ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [Account_fk0] FOREIGN KEY([LeadId])
REFERENCES [dbo].[Lead] ([Id])
ON UPDATE NO ACTION
GO

CREATE NONCLUSTERED INDEX [NonClusteredIndex-LeadId] ON [dbo].[Account]
(
	[LeadId] ASC
)
INCLUDE([Id],[Name],[Currency]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
-- Fill Accounts
Insert into [dbo].[Account]([Name],[Currency],[LeadId])
	select 'powerful RUB', 1, Id from [dbo].[Lead] order by Id
go
-- Create procedures
CREATE PROCEDURE [dbo].[Account_Add](
    @name nvarchar(100),
    @currency int,
    @leadId int
	)
AS
begin
	Insert into [dbo].[Account]([Name],[Currency],[LeadId])
	values(@name, @currency, @leadId)
	select Scope_Identity()
end
go
create proc [dbo].[Account_Delete] (
	@id int
) as
begin
	delete from [dbo].[Account]
	where Id = @id
end
go
create proc [dbo].[Lead_UpdateRole](
    @leadId int,
    @roleId int
)
as

begin
    update dbo.[Lead]
        set
            roleId = @roleId
            where Id = @leadId 
end
go
CREATE PROCEDURE [dbo].[Account_SelectById]
	@id int
AS
begin
	SELECT 
	a.Id,
	a.Currency,
	a.Name,
	a.LeadId as [Id]
	from [dbo].[Account] a
	where a.Id=@id
end
go
CREATE PROCEDURE [dbo].[Account_SelectByLeadId]
	@leadId int
AS
begin
	SELECT 
	a.Id,
	a.Currency,
	a.Name,
	a.LeadId as [Id]
	from [dbo].[Account] a 
	where a.LeadId=@leadId
end
go
-- Modify procedures
alter PROCEDURE [dbo].[Lead_AddUpdate]
	@Id int null,
	@firstName nvarchar(100),	
	@lastName nvarchar(100),
	@login nvarchar(100) null,
	@password nvarchar(1000) null,
	@email nvarchar(100),
	@phone nvarchar(20),
	@cityId int,
	@birthDate datetime,
	@roleId int
AS
begin
MERGE [dbo].[Lead] as ls --Целевая таблица
        USING (select @Id,
					@firstName,	
					@lastName,
					@login,
					@password,
					@email,
					@phone,
					@cityId,
					@birthDate,
					@roleId) as s (Id,FirstName,LastName,Login,Password,Email,Phone,CityId,BirthDate, RoleId) --Таблица источник
        ON (ls.Id = s.Id) --Условие объединения
        WHEN MATCHED THEN --Если истина (UPDATE)
                 Update
			set
				FirstName = s.FirstName,
				LastName = s.LastName,
				Email = s.Email,
				Phone = s.Phone,
				CityId = s.CityId,
				BirthDate = s.BirthDate
        WHEN NOT MATCHED THEN --Если НЕ истина (INSERT)
                 Insert (FirstName,LastName,Login,Password,Email,Phone,CityId,BirthDate,IsDeleted, RoleId)
				values(s.FirstName,s.LastName,s.Login,s.Password,s.Email,s.Phone,s.CityId,s.BirthDate,0, s.RoleId)
		OUTPUT Inserted.Id
				; --Не забываем про точку с запятой
end
go
alter PROCEDURE [dbo].[Lead_GetCredentials](
	 @id int,
	 @login nvarchar(100)
)as
begin
	select
		l.Id,
		l.[Login],
		l.[Password],
		l.RoleId as Id
	from dbo.[Lead] l
	where l.Id = @id or l.Login=@login
end
go
-- modify indexes
DROP INDEX [NonClusteredIndex-Login] ON [dbo].[Lead]
CREATE NONCLUSTERED INDEX [NonClusteredIndex-Login] ON [dbo].[Lead]
(
	[Login] ASC
)
INCLUDE([Id], [Password],[RoleId]) 
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
GO
update dbo.DbVersion set [Version] = 2
commit
set noexec off