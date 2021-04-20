CREATE PROCEDURE [dbo].[Lead_GetCredentials](
	 @id int,
	 @login nvarchar(100)
)as
begin
	select
		l.Id,
		l.Login,
		l.Password
	from dbo.[Lead] l
	where @id is not null and l.Id = @id or @login is not null and l.Login=@login
end