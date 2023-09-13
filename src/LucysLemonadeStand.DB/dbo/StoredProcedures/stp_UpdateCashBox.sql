CREATE PROCEDURE [dbo].[stp_UpdateCashBox]
	@AmountToAdd DECIMAL(10, 2)
AS
BEGIN
	IF @AmountToAdd < 0 AND NOT EXISTS (SELECT TOP 1 1 FROM dbo.CashBox cb WHERE cb.CashOnHand >= (@AmountToAdd * -1))
	BEGIN
		;THROW 50009, 'Not enough cash in cash box for a withdrawal.', 1
	END

	UPDATE dbo.CashBox
	SET CashOnHand = CashOnHand + @AmountToAdd
	
	RETURN 0;
END
