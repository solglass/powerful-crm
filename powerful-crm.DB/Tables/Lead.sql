CREATE TABLE [dbo].[Lead]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [FirstName] NVARCHAR(100) NOT NULL, 
    [LastName] NVARCHAR(100) NOT NULL, 
    [Login] NVARCHAR(100) NOT NULL, 
    [Password] NVARCHAR(1000) NOT NULL, 
    [Email] NVARCHAR(100) NOT NULL, 
    [Phone] NVARCHAR(20) NOT NULL, 
    [BirthDate] DATETIME NOT NULL,
    [IsDeleted] BIT NOT NULL,
)
