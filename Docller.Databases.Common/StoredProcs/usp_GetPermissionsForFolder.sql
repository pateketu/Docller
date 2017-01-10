CREATE PROCEDURE [dbo].[usp_GetPermissionsForFolder]
	@UserName nvarchar(400), 
	@ProjectId bigint,
	@FolderId bigint
AS
	DECLARE @IgnoreCompanyId int
	exec @IgnoreCompanyId = usp_GetUserCompanyId @UserName

	SELECT c.CompanyId as EntityId, c.CompanyName, cfp.PermissionMask 
	FROM CompanyFolderPermissions as cfp
	JOIN Companies as c on cfp.CompanyId = c.CompanyId and cfp.FolderId = @FolderId	
	WHERE c.CompanyId != @IgnoreCompanyId
	ORDER BY CompanyName

RETURN 0
