CREATE PROCEDURE [dbo].[Account_Add](
    @name nvarchar(100),
    @currency int,
    @leadId int
	)
AS
begin
	Insert into [dbo].[Account]([Name],[Currency],[LeadId])
	values(@name, @currency, @leadId)
	select Scope_Identity()
end