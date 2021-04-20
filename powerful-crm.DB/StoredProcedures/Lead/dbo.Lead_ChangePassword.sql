create proc [dbo].[Lead_ChangePassword]
(
    @id int,
    @oldPassword nvarchar(1000),
    @newPassword nvarchar(1000)
)
as

begin
    update dbo.[Lead]
        set
            Password = @newPassword
            where Id = @id 
end