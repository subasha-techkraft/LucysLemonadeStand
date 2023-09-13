CREATE PROCEDURE [dbo].[stp_UpdatePitcher]
	@Cups int
AS
BEGIN
	UPDATE dbo.Pitcher
	SET Cups = Cups + @Cups;
END
	
