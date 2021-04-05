CREATE PROCEDURE [dbo].[Lead_Search]
@firstName NVARCHAR(100) = null,
@lastName NVARCHAR(100) = null,
@email NVARCHAR(100) = null,
@login NVARCHAR(100)=null,
@phone NVARCHAR(20)=null,
@birthDate DATETIME=null,
@CityName NVARCHAR(20)=null
as begin
SELECT 
	l.[Id],
	l.[FirstName],
	l.[LastName],
	l.[Login],
	l.[Password],
	l.[Email],
	l.[Phone],
	l.[BirthDate],
	l.[IsDeleted],
	c.Id,
	c.[Name]
	from [dbo].[Lead] l inner join [dbo].[City] c on c.Id=l.CityId 
  where l.IsDeleted = 0 and 
  (@firstName is not null and l.FirstName = @firstName or @firstName is null) and
  (@lastName is not null and l.LastName = @lastName or @lastName is null) and
  (@email is not null and l.Email = @email or @email is null)and
  (@login is not null and l.Login = @login or @login is null)and
  (@phone is not null and l.Phone = @phone or @phone is null)and
  (@CityName is not null and c.Name = @CityName or @CityName is null)and
  (@birthDate!=Convert(date, getdate())  and l.BirthDate = @birthDate or @birthDate=Convert(date, getdate()))
end