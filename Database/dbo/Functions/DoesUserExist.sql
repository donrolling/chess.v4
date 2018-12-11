Create FUNCTION [dbo].[DoesUserExist]
(
	@emailAddress varchar(255)
)
RETURNS Bit
AS
BEGIN
	If Exists(
		Select * FROM [dbo].[User] WHERE Email = @emailAddress
	) begin
		return 1
	end 
	return 0
END