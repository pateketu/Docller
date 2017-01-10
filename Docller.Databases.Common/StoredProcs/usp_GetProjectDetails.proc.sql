CREATE PROCEDURE [dbo].[usp_GetProjectDetails]
	@UserName nvarchar(400), 
	@Projectd bigint
AS
	--DECLARE @UserId int
	--exec @UserId = usp_GetTenantUserId @UserName

	SELECT ProjectId, ProjectName, ProjectImage, BlobContainer
	FROM Projects 
	WHERE ProjectId = @Projectd

RETURN 0