CREATE OR ALTER PROCEDURE CreateAccount @Login nvarchar(50), @Password nvarchar(MAX),
                        @FullName nvarchar(100), @AccessLevel int, @Photo VARBINARY(MAX)
AS
    INSERT INTO ACCOUNT VALUES
        (@Login, CONVERT(VARCHAR(32), HashBytes('MD5', @Password), 2), @FullName, @AccessLevel, @Photo);
GO;

CREATE OR ALTER PROCEDURE Authenticate @Login nvarchar(50), @Password nvarchar(MAX)
AS
    SELECT LOGIN, FULL_NAME, ACCESS_LEVEL, PHOTO FROM ACCOUNT
    WHERE LOGIN = @Login AND
          HASH_PASSWORD = CONVERT(VARCHAR(32), HashBytes('MD5', @Password), 2);
GO;

CREATE OR ALTER PROCEDURE GetUserInfoByLogin @Login nvarchar(50)
AS
    SELECT LOGIN, FULL_NAME, ACCESS_LEVEL, PHOTO FROM ACCOUNT
    WHERE LOGIN = @Login;
GO;