CREATE PROCEDURE [dbo].[Lead_HardDelete]
(
	@id int
)
as
begin
	Delete [dbo].[Lead] where Id=@id
end
