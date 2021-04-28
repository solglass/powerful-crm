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