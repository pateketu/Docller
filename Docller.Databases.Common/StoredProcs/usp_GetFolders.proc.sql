CREATE PROCEDURE [dbo].[usp_GetFolders]
@UserName NVARCHAR (400), @ProjectId bigint, @MaxLevel INT=NULL, @ParentFolderId BIGINT= NULL
WITH EXECUTE AS CALLER
AS

DECLARE @CompanyId int
exec @CompanyId = usp_GetUserCompanyId @UserName;

WITH ProjectFolderStructure(CustomerId, ParentFolderId, FolderId, FolderName,FolderInternalName, FullPath, Level)
AS
(
	SELECT CustomerId, ParentFolderId, FolderId, FolderName, FolderInternalName, FullPath, 0 AS Level
	FROM Folders 
	WHERE ParentFolderId IS NULL AND ProjectId = @ProjectId
	UNION ALL
	SELECT F.CustomerId, F.ParentFolderId, F.FolderId, F.FolderName, F.FolderInternalName, F.FullPath, Level + 1
	FROM Folders AS F 
	JOIN ProjectFolderStructure as PFS
	ON F.ParentFolderId = PFS.FolderId AND F.ProjectId = @ProjectId	

)

SELECT @ProjectId AS ProjectId, PFS.FolderId, CASE 
											  WHEN ParentFolderId IS NULL then 0
											  ELSE ParentFolderId END AS ParentFolderID, 
		PFS.Level, FolderName, FolderInternalName, FullPath, CFP.PermissionMask
	FROM ProjectFolderStructure AS PFS
	JOIN CompanyFolderPermissions as CFP ON PFS.FolderId=CFP.FolderId 
	AND CFP.CompanyId = @CompanyId
	WHERE (PFS.Level <= @MaxLevel OR @MaxLevel IS NULL) AND (PFS.ParentFolderId = @ParentFolderId OR @ParentFolderId IS NULL)
	Order By  PFS.Level, FolderName 