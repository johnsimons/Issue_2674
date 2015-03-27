This repo contains a VS solution to try to replicate the issue described in https://github.com/Particular/NServiceBus/issues/2674.

In this sample I decided to use a sql database so i do some async inserts into a table to ensure we have a distributed transaction.

## Setup
To run this solution you need a sql database named nservicebus with a table called `Table_1`, here is the tsql script:
```sql
CREATE TABLE [dbo].[Table_1](
	   [Name] [nvarchar](50) NULL) ON [PRIMARY]
```


