CREATE PROCEDURE [dbo].[usp_GetUserSubscriptionInfo]
	@UserName nvarchar(400),
	@needCompanyInfo bit
AS
	DECLARE @UserId int
	
	exec @UserId = usp_GetTenantUserId @UserName
	
	SELECT u.UserPermissions FROM UserCache as u WHERE UserId = @UserId

	SELECT p.ProjectId, p.ProjectName, p.ProjectImage, p.ProjectCode, p.BlobContainer, p.CreatedDate, pu.PermissionMask
	FROM Projects as p 
			JOIN ProjectUsers as pu on p.ProjectId = pu.ProjectId and pu.UserId = @UserId
	
	IF @needCompanyInfo = 1 
	BEGIN
		SELECT c.CompanyId, c.CompanyName, c.CompanyAlias		
		FROM Companies as c 
			 JOIN CompanyUsers as cu on c.CompanyId = cu.CompanyId		 
		WHERE cu.UserId = @UserId
	END
RETURN 0
