CREATE PROCEDURE [dbo].[Lead_SelectById]
	@id int
AS
begin
	SELECT 
	[Id],
	[FirstName],
	[LastName],
	[Login],
	[Password],
	[Email],
	[Phone],
	[BirthDate],
	[IsDeleted]
	from [dbo].[Lead]
end