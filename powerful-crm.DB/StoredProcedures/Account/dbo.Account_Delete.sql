create proc [dbo].[Account_Delete] (
	@id int
) as
begin
	delete from [dbo].[Account]
	where Id = @id
end
