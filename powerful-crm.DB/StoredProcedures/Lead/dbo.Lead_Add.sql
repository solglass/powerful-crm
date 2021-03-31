CREATE PROCEDURE [dbo].[Lead_Add]
	@firstName nvarchar(100),	
	@lastName nvarchar(100),
	@login nvarchar(100),
	@password nvarchar(1000),
	@email nvarchar(100),
	@phone nvarchar(20),
	@birthDate datetime
AS
begin
	Insert into [dbo].[Lead](FirstName,LastName,Login,Password,Email,Phone,BirthDate,IsDeleted)
	values(@firstName,@lastName,@login,@password,@email,@phone,@birthDate,0)
	select Scope_Identity()
end
