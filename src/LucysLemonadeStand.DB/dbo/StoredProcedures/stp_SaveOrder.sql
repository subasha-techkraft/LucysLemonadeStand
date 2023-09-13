CREATE PROCEDURE [dbo].[stp_SaveOrder]
	@Type INT,
	@Cups INT,
	@CashGiven DECIMAL(10, 2),
	@Change DECIMAL(10, 2),
	@Occurred DATETIMEOFFSET,
	@OrderID INT OUT
AS
BEGIN

	BEGIN TRANSACTION;

	INSERT INTO dbo.OrderHistory ([Cups], [CashGiven], [Change], [Occurred])
	VALUES (@Cups, @CashGiven, @Change, @Occurred);

	SET @OrderID = @@IDENTITY;
	
	IF @Type = 0 --purchase by customer
	BEGIN
		UPDATE dbo.CashBox
		SET CashOnHand = CashOnHand + @CashGiven - @Change;

		UPDATE dbo.Pitcher
		SET Cups = Cups - @Cups;
	END

	ELSE IF @Type = 1 --refill from mom
	BEGIN
		UPDATE dbo.CashBox
		SET CashOnHand = CashOnHand - @CashGiven + @Change;

		UPDATE dbo.Pitcher
		SET Cups = Cups + @Cups;
	END

	COMMIT;
END
