create proc [dbo].[Lead_UpdateRole](
    @leadId int,
    @roleId int
)
as

begin
    update dbo.[Lead]
        set
            roleId = @roleId
            where Id = @leadId 
end