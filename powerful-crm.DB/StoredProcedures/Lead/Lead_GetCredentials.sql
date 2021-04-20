CREATE PROCEDURE [dbo].[Lead_GetCredentials](
	@login nvarchar(100)
)as
begin
	select
		l.Id,
		l.Login,
		l.Password
	from dbo.[Lead] l
	where l.Login = @login 
end