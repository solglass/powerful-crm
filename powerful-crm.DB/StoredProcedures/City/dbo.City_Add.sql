CREATE PROCEDURE [dbo].[City_Add]
	(@name nvarchar(100))
AS
begin
	Insert into [dbo].[City]([Name])
	values(@name)
	select Scope_Identity()
end
