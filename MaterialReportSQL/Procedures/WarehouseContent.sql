CREATE OR ALTER PROCEDURE AddContentToWarehouse @MaterialObjectName nvarchar(100), @CommissioningDate datetime
AS
    INSERT INTO CONTENT_OF_WAREHOUSE VALUES (NEXT VALUE FOR ContentWarehouseSequence, @MaterialObjectName, @CommissioningDate);
GO;

CREATE OR ALTER PROCEDURE GetContentFromWarehouseById @InventoryId int
AS
    SELECT * FROM CONTENT_OF_WAREHOUSE WHERE INVENTORY_ID = @InventoryId;
GO;

CREATE OR ALTER PROCEDURE GetBrokenContentFromWarehouseById @InventoryId int
AS
    SELECT * FROM BROKEN_CONTENT_OF_WAREHOUSE WHERE INVENTORY_ID = @InventoryId;
GO;

CREATE OR ALTER PROCEDURE GetDecommissionedContentFromWarehouseById @InventoryId int
AS
    SELECT * FROM DECOMMISSIONED_CONTENT_OF_WAREHOUSE WHERE INVENTORY_ID = @InventoryId;
GO;

CREATE OR ALTER PROCEDURE GetContentOfWarehouseByIdRange @Part int
AS
    SELECT * FROM CONTENT_OF_WAREHOUSE ORDER BY INVENTORY_ID
        OFFSET @Part * 20 ROWS FETCH NEXT 20 ROWS ONLY;
GO;

CREATE OR ALTER PROCEDURE GetContentPartsCount
AS
    SELECT COUNT(*) / 20 FROM CONTENT_OF_WAREHOUSE;
GO;

CREATE OR ALTER PROCEDURE GetBrokenContentOfWarehouseByIdRange @Part int
AS
    SELECT * FROM BROKEN_CONTENT_OF_WAREHOUSE ORDER BY INVENTORY_ID
        OFFSET @Part * 20 ROWS FETCH NEXT 20 ROWS ONLY;
GO;

CREATE OR ALTER PROCEDURE GetBrokenContentPartsCount
AS
    SELECT COUNT(*) / 20 FROM BROKEN_CONTENT_OF_WAREHOUSE;
GO;

CREATE OR ALTER PROCEDURE GetDecommissionedContentOfWarehouseByIdRange @Part int
AS
    SELECT * FROM DECOMMISSIONED_CONTENT_OF_WAREHOUSE ORDER BY INVENTORY_ID
        OFFSET @Part * 20 ROWS FETCH NEXT 20 ROWS ONLY;
GO;

CREATE OR ALTER PROCEDURE GetDecommissionedContentPartsCount
AS
    SELECT COUNT(*) / 20 FROM DECOMMISSIONED_CONTENT_OF_WAREHOUSE;
GO;

CREATE OR ALTER PROCEDURE FromSimpleToBroken @InventoryId int, @RestorationCoast float(53), @Deprecation float(53)
AS
    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO BROKEN_CONTENT_OF_WAREHOUSE
            SELECT INVENTORY_ID, MATERIAL_OBJECT_NAME, COMMISSIONING_DATE, @RestorationCoast, @Deprecation
            FROM CONTENT_OF_WAREHOUSE WHERE INVENTORY_ID = @InventoryId;

        DELETE FROM CONTENT_OF_WAREHOUSE WHERE INVENTORY_ID = @InventoryId;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        PRINT ERROR_MESSAGE();
        ROLLBACK TRANSACTION;
    END CATCH;
GO;


CREATE OR ALTER PROCEDURE FromBrokenToSimple @InventoryId int
AS
    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO CONTENT_OF_WAREHOUSE
            SELECT INVENTORY_ID, MATERIAL_OBJECT_NAME, COMMISSIONING_DATE
            FROM BROKEN_CONTENT_OF_WAREHOUSE WHERE INVENTORY_ID = @InventoryId;

        DELETE FROM BROKEN_CONTENT_OF_WAREHOUSE WHERE INVENTORY_ID = @InventoryId;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        PRINT ERROR_MESSAGE();
        ROLLBACK TRANSACTION;
    END CATCH;
GO;


CREATE OR ALTER PROCEDURE FromBrokenToDecommissioned @InventoryId int, @DecommissioningDate datetime
AS
    BEGIN TRANSACTION;

    BEGIN TRY
        INSERT INTO DECOMMISSIONED_CONTENT_OF_WAREHOUSE
            SELECT INVENTORY_ID, MATERIAL_OBJECT_NAME, COMMISSIONING_DATE, RESTORATION_COAST, DEPRECATION, @DecommissioningDate
            FROM BROKEN_CONTENT_OF_WAREHOUSE WHERE INVENTORY_ID = @InventoryId;

        DELETE FROM BROKEN_CONTENT_OF_WAREHOUSE WHERE INVENTORY_ID = @InventoryId;

        COMMIT TRANSACTION;
    END TRY

    BEGIN CATCH
        PRINT ERROR_MESSAGE();
        ROLLBACK TRANSACTION;
    END CATCH;
GO;