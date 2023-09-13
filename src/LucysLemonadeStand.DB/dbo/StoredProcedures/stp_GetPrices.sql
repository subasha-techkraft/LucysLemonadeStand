CREATE PROCEDURE [dbo].[stp_GetPrices]
	@Item nvarchar(50) = NULL
AS
BEGIN
	IF @Item IS NULL
	BEGIN
		SELECT * FROM dbo.PriceEntries;
	END
	ELSE
	BEGIN
		SELECT * FROM dbo.PriceEntries
		WHERE [Item] = @Item;
	END

END
