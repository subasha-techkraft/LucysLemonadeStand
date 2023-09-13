DELETE FROM dbo.CashBox;

INSERT INTO dbo.CashBox ([CashOnHand]) VALUES (20.0);

DELETE FROM dbo.Pitcher;

INSERT INTO dbo.Pitcher ([Cups]) VALUES (8);

DELETE FROM dbo.[PriceEntries];

INSERT INTO dbo.[PriceEntries] ([Item], [Price]) VALUES 
('Cup', 0.50),
('Refill of 8 cups', .40 * 8);