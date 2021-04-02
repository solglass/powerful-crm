CREATE PROCEDURE [dbo].[Lead_AddUpdate]
	@Id int null,
	@firstName nvarchar(100),	
	@lastName nvarchar(100),
	@login nvarchar(100) null,
	@password nvarchar(1000) null,
	@email nvarchar(100),
	@phone nvarchar(20),
	@cityId int null,
	@birthDate datetime
AS
begin
MERGE [dbo].[Lead] as ls --Целевая таблица
        USING (select @Id,
					@firstName,	
					@lastName,
					@login,
					@password,
					@email,
					@phone,
					@cityId,
					@birthDate) as s (Id,FirstName,LastName,Login,Password,Email,Phone,CityId,BirthDate) --Таблица источник
        ON (ls.Id = s.Id) --Условие объединения
        WHEN MATCHED THEN --Если истина (UPDATE)
                 Update
			set
				FirstName = s.FirstName,
				LastName = s.LastName,
				Email = s.Email,
				Phone = s.Phone,
				BirthDate = s.BirthDate
        WHEN NOT MATCHED THEN --Если НЕ истина (INSERT)
                 Insert (FirstName,LastName,Login,Password,Email,Phone,CityId,BirthDate,IsDeleted)
				values(s.FirstName,s.LastName,s.Login,s.Password,s.Email,s.Phone,s.CityId,s.BirthDate,0)
		OUTPUT Inserted.Id
				; --Не забываем про точку с запятой
end