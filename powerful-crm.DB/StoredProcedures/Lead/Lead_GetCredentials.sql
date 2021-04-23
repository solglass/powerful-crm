CREATE PROCEDURE [dbo].[Lead_GetCredentials](
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