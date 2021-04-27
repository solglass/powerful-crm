CREATE PROCEDURE [dbo].[Accounts_SelectByLeadId]
	@leadId int
AS
begin
	SELECT 
	a.Id,
	a.Currency,
	a.Name
	from [dbo].[Account] a 
	where a.LeadId=@leadId
end