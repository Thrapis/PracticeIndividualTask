CREATE OR ALTER PROCEDURE GetMaterialObjectByName @Name nvarchar(100)
AS
    SELECT * FROM MATERIAL_OBJECT WHERE NAME = @Name;
GO;

CREATE OR ALTER PROCEDURE GetConstituentComponentsByMaterialObjectName @MaterialObjectName nvarchar(100)
AS
    SELECT * FROM CONSTITUENT_COMPONENT WHERE MATERIAL_OBJECT_NAME = @MaterialObjectName;
GO;

CREATE OR ALTER PROCEDURE GetConstituentComponentTypesByMaterialObjectName @MaterialObjectName nvarchar(100)
AS
    SELECT CCT.* FROM CONSTITUENT_COMPONENT_TYPE CCT
        INNER JOIN CONSTITUENT_COMPONENT CC on CCT.ID = CC.CONSTITUENT_COMPONENT_TYPE_ID
    WHERE MATERIAL_OBJECT_NAME = @MaterialObjectName;
GO;

CREATE OR ALTER PROCEDURE GetConstituentComponentTypeById @Id int
AS
    SELECT * FROM CONSTITUENT_COMPONENT_TYPE WHERE ID = @Id;
GO;