CREATE PROCEDURE [Membership].[Client_Delete]
	@id bigint
AS
	DELETE FROM [Membership].[Client]
	WHERE Id = @id