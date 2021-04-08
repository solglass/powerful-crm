CREATE PROCEDURE [dbo].[City_SelectById]
	@id int
AS
begin
	SELECT 
	c.Id,
	c.Name
	from [dbo].[City] c 
	where c.Id=@id
end