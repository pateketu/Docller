--sample exec RECREATE_TYPE @schema='dbo', @typ_nme='UserTableType', @sql='AS TABLE([bar] varchar(10) NOT NULL)'


CREATE PROCEDURE [dbo].[recompile_prog]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @v TABLE (RecID INT IDENTITY(1,1), spname sysname)
    -- retrieve the list of stored procedures
    INSERT INTO 
        @v(spname) 
    SELECT 
        '[' + s.[name] + '].[' + items.name + ']'     
    FROM 
        (SELECT sp.name, sp.schema_id, sp.is_ms_shipped FROM sys.procedures sp UNION SELECT so.name, so.SCHEMA_ID, so.is_ms_shipped FROM sys.objects so WHERE so.type_desc LIKE '%FUNCTION%') items
        INNER JOIN sys.schemas s ON s.schema_id = items.schema_id    
        WHERE is_ms_shipped = 0;

    -- counter variables
    DECLARE @cnt INT, @Tot INT;
    SELECT @cnt = 1;
    SELECT @Tot = COUNT(*) FROM @v;
    DECLARE @spname sysname
    -- start the loop
    WHILE @Cnt <= @Tot BEGIN    
        SELECT @spname = spname        
        FROM @v        
        WHERE RecID = @Cnt;
        --PRINT 'refreshing...' + @spname    
        BEGIN TRY        -- refresh the stored procedure        
            EXEC sp_refreshsqlmodule @spname    
        END TRY    
        BEGIN CATCH        
            PRINT 'Validation failed for : ' + @spname + ', Error:' + ERROR_MESSAGE();
        END CATCH    
        SET @Cnt = @cnt + 1;
    END;

END
GO
CREATE PROCEDURE [dbo].[RECREATE_TYPE]
    @schema     VARCHAR(100),       -- the schema name for the existing type
    @typ_nme    VARCHAR(128),       -- the type-name (without schema name)
    @sql        VARCHAR(MAX)        -- the SQL to create a type WITHOUT the "CREATE TYPE schema.typename" part
AS DECLARE
    @scid       BIGINT,
    @typ_id     BIGINT,
    @temp_nme   VARCHAR(1000),
    @msg        VARCHAR(200)
BEGIN
    -- find the existing type by schema and name
    SELECT @scid = [SCHEMA_ID] FROM sys.schemas WHERE UPPER(name) = UPPER(@schema);
    IF (@scid IS NULL) BEGIN
        SET @msg = 'Schema ''' + @schema + ''' not found.';
        RAISERROR (@msg, 1, 0);
    END;
    SELECT @typ_id = system_type_id FROM sys.types WHERE UPPER(name) = UPPER(@typ_nme);
    SET @temp_nme = @typ_nme + '_rcrt'; -- temporary name for the existing type

    -- if the type-to-be-recreated actually exists, then rename it (give it a temporary name)
    -- if it doesn't exist, then that's OK, too.
    IF (@typ_id IS NOT NULL) BEGIN
        exec sp_rename @objname=@typ_nme, @newname= @temp_nme, @objtype='USERDATATYPE'
    END;    

    -- now create the new type
    SET @sql = 'CREATE TYPE ' + @schema + '.' + @typ_nme + ' ' + @sql;
    exec sp_sqlexec @sql;

    -- if we are RE-creating a type (as opposed to just creating a brand-spanking-new type)...
    IF (@typ_id IS NOT NULL) BEGIN
        exec recompile_prog;    -- then recompile all stored procs (that may have used the type)
        exec sp_droptype @typename=@temp_nme;   -- and drop the temporary type which is now no longer referenced
    END;    
END

GO
