# ExchangeAPI


In order to run the project you need to add connection string to a SQL Database in appsettings.json file. Also please run the following script in your database.


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[transaction](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[ISO_currency] [char](3) NOT NULL,
	[amount] [numeric](7, 2) NOT NULL,
	[transaction_date] [datetime] NOT NULL,
 CONSTRAINT [PK_transaction] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
GO

Also, about the questions:

# We would like to know what do you think about using the user ID as the input endpoint. 
Following the convention of paths. UserID should be obtained from one of the headers or the body of the request not as part of the url of the endpoint.

# Also, how would you improve the transaction to ensure that the user who makes the purchase is the correct user?
Authentication should be done before hitting the transaction endpoint. Also, you can authenticate and get a token (json web token) which will be used to access the other endpoints. 

The endpoints are:

exchange/rate/{ISOCode}
exchange/purchase


