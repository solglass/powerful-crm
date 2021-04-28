declare @DbVersion int

select top 1 @DbVersion = version
from dbo.DbVersion order by id desc

if @DbVersion > 2 set noexec on
begin transaction
--modify indexes
DROP INDEX [NonClusteredIndex-Login] ON [dbo].[Lead]
CREATE NONCLUSTERED INDEX [NonClusteredIndex-Login] ON [dbo].[Lead]
(
	[Login] ASC
)
INCLUDE([Id]) 
WHERE ([IsDeleted]=(0))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON ) ON [PRIMARY]
GO
-- drop tables
alter table [dbo].[Lead] drop column [RoleId] 
go
drop table [dbo].[Account]
go
--drop procedures
drop procedure [dbo].[Account_Add]
go
drop procedure [dbo].[Account_Delete]
go
drop procedure [dbo].[Lead_UpdateRole]
go
drop procedure [dbo].[Account_SelectById]
go
drop procedure [dbo].[Account_SelectByLeadId]
go
--modify procedures
ALTER PROCEDURE [dbo].[Lead_AddUpdate]
	@Id int null,
	@firstName nvarchar(100),	
	@lastName nvarchar(100),
	@login nvarchar(100) null,
	@password nvarchar(1000) null,
	@email nvarchar(100),
	@phone nvarchar(20),
	@cityId int,
	@birthDate datetime
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
					@birthDate) as s (Id,FirstName,LastName,Login,Password,Email,Phone,CityId,BirthDate) --Таблица источник
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
                 Insert (FirstName,LastName,Login,Password,Email,Phone,CityId,BirthDate,IsDeleted)
				values(s.FirstName,s.LastName,s.Login,s.Password,s.Email,s.Phone,s.CityId,s.BirthDate,0)
		OUTPUT Inserted.Id
				; --Не забываем про точку с запятой
end
go
ALTER PROCEDURE [dbo].[Lead_GetCredentials](
	 @id int,
	 @login nvarchar(100)
)as
begin
	select
		l.Id,
		l.Login,
		l.Password
	from dbo.[Lead] l
	where l.Id = @id or l.Login=@login
end

update dbo.DbVersion set [Version] = 1
commit

set noexec off