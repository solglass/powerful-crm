CREATE PROCEDURE [dbo].[Lead_Update]
(
	@id int,
	@firstName nvarchar(100),	
	@lastName nvarchar(100),
	@email nvarchar(100),
	@phone nvarchar(20),
	@birthDate datetime
)
AS
begin
	Update [dbo].[Lead]
	set
		FirstName = @firstName,
		LastName = @lastName,
		Email = @email,
		Phone = @phone,
		BirthDate = @birthDate
	where Id = @id
end
