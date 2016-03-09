use [Genesis-Server]
------------------------------------
-- :: SCRIPT TO RESET DATABASE :: --
-- :: Assuming LCM DB Exists   :: --
------------------------------------

---------------------------------
-- [ Step 1.) Clean Database ] --
---------------------------------

--Delete System Users
truncate table [Genesis-Server].[dbo].[tbl_users]

--Delete System Branches
truncate table [Genesis-Server].[dbo].[tbl_branch]

--Delete System Clients
truncate table [Genesis-Server].[dbo].[tbl_client]

--Delte System Suppliers
truncate table [Genesis-Server].[dbo].[tbl_supplier]

--Delete System Products
truncate table [Genesis-Server].[dbo].[tbl_product]

--Delete System Product Categories
truncate table [Genesis-Server].[dbo].[tbl_productCategory]

--Delete Documents (Purchase / Sales)
truncate table [Genesis-Server].[dbo].[tbl_document]

--Delete Payments (Payments for Purchases)
truncate table [Genesis-Server].[dbo].[tbl_payment]

--Delete Payment Details (List of Purchases Paid)
truncate table [Genesis-Server].[dbo].[tbl_paymentDetails]

--Delete Product Price History
truncate table [Genesis-Server].[dbo].[tbl_productPriceHistory]

--Delete Receivables (Payments for Sales)
truncate table [Genesis-Server].[dbo].[tbl_receivable]

--Delete Receivable Details (List of Sales Paid)
truncate table [dbo].[tbl_receivableDetails]

--Delete Transactions (purchase / Sale / refund / return)
truncate table [Genesis-Server].[dbo].[tbl_transaction]


----------------------------------
-- [ Step 2.) Insert Branches ] --
----------------------------------

Insert Into [dbo].[tbl_branch] ([branchCode],[branchName],[branchAddress],[dateCreated],[createdBy])
values  ('UniWay','Uni Way Construction Supply','',GETDATE(),1),
		('UniGood','Uni Good Construction Supply','San Roque Arbol, Lubao, Pampanga',GETDATE(),1),
		('LCM','Angeles LCM Trading Corporation','',GETDATE(),1)

---------------------------------------------------------------
-- [ Step 3.) Insert Users ] (default password is 123456)    --
---------------------------------------------------------------

Insert Into [dbo].[tbl_users] ([userName],[passWord],[emailAddress],[firstName],[middleName],[lastName],[branchId],[dateCreated],[status])
values  ('cperez','e10adc3949ba59abbe56e057f20f883e','','','','','1',GETDATE(),'1'),
		('csabado','e10adc3949ba59abbe56e057f20f883e','','','','','2',GETDATE(),'1'),
		('dmadayag','e10adc3949ba59abbe56e057f20f883e','','','','','3',GETDATE(),'1'),
		('adminuniway','e10adc3949ba59abbe56e057f20f883e','','','','','1',GETDATE(),'1'),
		('adminunigood','e10adc3949ba59abbe56e057f20f883e','','','','','2',GETDATE(),'1'),
		('adminlcm','e10adc3949ba59abbe56e057f20f883e','','','','','3',GETDATE(),'1')
---------------------------------------------------------------
-- [ Step 4.) Insert Branch Clients - From InStock DB       ] --
---------------------------------------------------------------

Insert Into [Genesis-Server].[dbo].[tbl_client] ([clientCode],[clientName],[branchId],[status],[dateCreated],[createdBy])
select [ClientCode],[ClientName],[BranchClientsDetail_BranchNameDetail],[IsActive],GETDATE(),'1' 
from [InStock].[dbo].[BranchClientsDetails]

---------------------------------------------------------------
-- [ Step 5.) Insert Branch Suppliers - From InStock DB     ] --
---------------------------------------------------------------

Insert Into [Genesis-Server].[dbo].[tbl_supplier] ([supplierCode],[supplierName],[branchId],[status],[dateCreated],[createdBy])
select [SupplierCode],[SupplierName],[BranchSuppliersDetail_BranchNameDetail],[IsActive],GETDATE(),'1' 
from [InStock].[dbo].[BranchSuppliersDetails]

---------------------------------------------------------------
-- [ Step 6.) Inser Product Categories - From InStock DB   ] --
---------------------------------------------------------------

Insert Into [Genesis-Server].[dbo].[tbl_productCategory] ([categoryId],[categoryCode],[categoryName],[dateCreated],[createdBy])
select [Id],[CategoryNumber],[CategoryName],GETDATE(),'1' 
from [InStock].[dbo].[CategoryMasters]

---------------------------------------------------------------
-- [ Step 7.) Inser Products - From InStock DB             ] --
---------------------------------------------------------------

Insert Into [Genesis-Server].[dbo].[tbl_product] ([productCode],[productDescription],[categoryId],[reorderLevel],[UOM],[branchId],[unitPrice],[beginning],[incoming],[outgoing],[ending],[dateCreated],[createdBy])
select [StockCode],[StockDescription],[BranchStocksDetail_CategoryMaster],[ReorderQuantity],[UOM],[BranchStocksDetail_BranchNameDetail],[UnitPrice],[StartInventory],[IncomingCount],[OutgoingCount],[EndingInventory],GETDATE(),'1' 
from [InStock].[dbo].[BranchStocksDetails]

