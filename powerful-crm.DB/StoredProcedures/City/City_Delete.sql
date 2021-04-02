create proc [dbo].[City_Delete]
( @id int)
as
begin
    DELETE from dbo.[City]
    where Id=@id
end