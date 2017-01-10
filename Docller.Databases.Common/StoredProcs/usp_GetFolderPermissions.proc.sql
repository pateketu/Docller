CREATE PROCEDURE [dbo].[usp_GetFolderPermissions]
	@UserName nvarchar(400), 
	@FolderId int
AS
	DECLARE @UserId int
	DECLARE @CompanyId int
	exec @UserId = usp_GetTenantUserId @UserName
	
	SELECT @CompanyId  = CompanyId 
	FROM CompanyUsers
	WHERE UserId = @UserId
	
	SElECT cfp.PermissionMask
	FROM CompanyFolderPermissions as cfp
	WHERE cfp.CompanyId = @CompanyId AND cfp.FolderId = @FolderId

RETURN 0