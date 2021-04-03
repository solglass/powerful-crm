CREATE PROCEDURE [dbo].[Lead_SelectByBirthDate](
	@birthDate DATETIME
)
as
begin
	select
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
	where l.BirthDate=@birthDate
end