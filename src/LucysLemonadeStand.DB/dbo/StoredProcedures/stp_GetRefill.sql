CREATE PROCEDURE [dbo].[stp_GetRefill]
	@Cups int
AS
BEGIN
	DECLARE @Price DECIMAL(10, 2);
	SELECT @Price = Price FROM dbo.[PriceEntries] WHERE [Item] = 'Refill of 8 cups';
	
	IF NOT EXISTS (SELECT 1 FROM dbo.CashBox WHERE CashOnHand >= @Price)
	BEGIN
		;THROW 50004, 'There is not enough money to get a refill.', 1;
	END

	BEGIN TRANSACTION;


	INSERT INTO dbo.OrderHistory ([Type], [Cups], [CashGiven], [Change])
	VALUES (1, 8, @Price, 0);

	UPDATE dbo.CashBox
	SET CashOnHand = CashOnHand - @Price;

	UPDATE dbo.Pitcher
	SET Cups = Cups + 8;

	COMMIT;
END
