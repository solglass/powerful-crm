create proc [dbo].[Lead_DeleteOrRecover]
(
    @id int,
    @isDeleted bit
)
as
begin
    update dbo.[Lead]
        set IsDeleted=@isDeleted
    where Id=@id
end