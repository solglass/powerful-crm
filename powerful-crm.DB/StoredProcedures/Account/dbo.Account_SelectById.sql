CREATE PROCEDURE [dbo].[Account_SelectById]
	@id int
AS
begin
	SELECT 
	a.Id,
	a.Currency,
	a.Name,
	a.LeadId
	from [dbo].[Account] a
	where a.Id=@id
end