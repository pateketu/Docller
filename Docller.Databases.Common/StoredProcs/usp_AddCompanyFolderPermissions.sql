CREATE PROCEDURE [dbo].[usp_AddCompanyFolderPermissions]
	@CustomerId int,
	@PermissionMask int,
	@IsFlatStructure bit,
	@Folders FolderTableType Readonly,
	@Companies CompanyTableType Readonly
AS
DECLARE @AllFolders as FolderTableType;
DECLARE @AllCompanies as CompanyTableType

INSERT INTO @AllCompanies (CompanyId)
SELECT CompanyId FROM @Companies;

IF @IsFlatStructure = 0
BEGIN
	WITH FolderStructure(CustomerId, ParentFolderId, FolderId, FolderName,FolderInternalName, FullPath, Level)
	AS
	(
		SELECT CustomerId, ParentFolderId, FolderId, FolderName, FolderInternalName, FullPath, 0 AS Level
		FROM Folders 
		WHERE FolderId IN (SELECT FolderId FROM @Folders)
		UNION ALL
		SELECT F.CustomerId, F.ParentFolderId, F.FolderId, F.FolderName, F.FolderInternalName, F.FullPath, Level + 1
		FROM Folders AS F
		JOIN FolderStructure as FS
		ON F.FolderId = FS.ParentFolderId

	)
	--We can't run While right after WITH
	INSERT INTO @AllFolders (FolderId)
	SELECT FolderId FROM FolderStructure 
END
ELSE
BEGIN
	INSERT INTO @AllFolders (FolderId)
	SELECT FolderId FROM @Folders
END

WHILE (SELECT COUNT(CompanyId) FROM @AllCompanies) > 0
BEGIN
	DECLARE @CompanyId bigint
	SELECT TOP 1 @CompanyId = CompanyId FROM @AllCompanies

	/*To avoid Race condition in Merge see
	http://weblogs.sqlteam.com/dang/archive/2009/01/31/UPSERT-Race-Condition-With-MERGE.aspx */

	MERGE CompanyFolderPermissions WITH (HOLDLOCK) as cfp 
	USING @AllFolders as fs ON cfp.FolderId = fs.FolderId 
							AND cfp.CompanyId = @CompanyId							
	WHEN MATCHED AND @PermissionMask != 1 THEN --Only update the permission if it is default
		 UPDATE SET cfp.PermissionMask = @PermissionMask 
	WHEN NOT MATCHED BY TARGET THEN
		INSERT (CustomerId, CompanyId, FolderId, PermissionMask)
		VALUES(@CustomerId, @CompanyId, fs.FolderId, @PermissionMask);									 


	/*IF EXISTS(SELECT CompanyId FROM CompanyFolderPermissions WHERE CompanyId = @CompanyId)
	BEGIN
		
	END
	ELSE
	BEGIN
		INSERT INTO CompanyFolderPermissions (CustomerId, CompanyId, FolderId, PermissionMask)
		SELECT @CustomerId, @CompanyId, fs.FolderId, @PermissionMask
		FROM CompanyFolderPermissions as cfp
		JOIN @AllFolders as FS ON cfp.FolderId = FS.FolderId AND  cfp.CompanyId = @CompanyId
		WHERE cfp.PermissionMask < @PermissionMask		

	END
	*/
	DELETE FROM @AllCompanies WHERE CompanyId = @CompanyId
	
END

RETURN 0
