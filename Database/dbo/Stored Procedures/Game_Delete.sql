CREATE PROCEDURE [dbo].[Game_Delete]
	@id bigint
AS
	DELETE FROM [dbo].[Game]
	WHERE Id = @id