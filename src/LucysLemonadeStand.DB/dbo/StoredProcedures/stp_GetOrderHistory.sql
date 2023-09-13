CREATE PROCEDURE [dbo].[stp_GetOrderHistory]
	@Page int = 0,
	@PageSize int = 50
AS
BEGIN
	SELECT * 
	FROM dbo.OrderHistory
	ORDER BY Occurred DESC
	OFFSET @Page * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY;
END
