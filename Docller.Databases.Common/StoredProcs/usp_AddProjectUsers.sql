CREATE PROCEDURE [dbo].[usp_AddProjectUsers]
	@CustomerId bigint,
	@ProjectId bigint,
	@PermissionFlag int,
	@Users UserTableType READONLY
AS

	INSERT INTO ProjectUsers (CustomerId,ProjectId,UserId, PermissionMask)
		SELECT @CustomerId, @ProjectId,u.UserId, @PermissionFlag
		FROM @Users AS U LEFT JOIN ProjectUsers AS pu ON u.UserId = pu.UserId AND pu.ProjectId = @ProjectId 
		WHERE pu.ProjectId IS NULL
RETURN 0
