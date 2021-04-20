CREATE PROCEDURE [dbo].[Lead_GetCredentials](
	 @id int
)as
begin
	select
		l.Id,
		l.Login,
		l.Password
	from dbo.[Lead] l
	where l.Id = @id 
end