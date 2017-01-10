CREATE PROCEDURE [dbo].[usp_GetProjectPermissions]
	@UserName nvarchar(400), 
	@Projectd int
AS
	DECLARE @UserId int
	exec @UserId = usp_GetTenantUserId @UserName

	SElECT pu.PermissionMask
	FROM ProjectUsers as pu
	WHERE pu.ProjectId = @Projectd AND UserId = @UserId

	 
RETURN 0