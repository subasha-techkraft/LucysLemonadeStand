CREATE PROCEDURE [dbo].[stp_ReadCashBox]
AS
BEGIN
	SELECT CashOnHand FROM dbo.CashBox;
END	
