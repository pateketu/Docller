CREATE PROCEDURE [dbo].[usp_GetPermissionsForProject]
	@ProjectId as int
AS	 

SELECT uc.UserId as EntityId, uc.FirstName, uc.LastName, uc.Email, c.CompanyName, pu.PermissionMask 
FROM ProjectUsers as pu
JOIN UserCache as uc on pu.UserId = uc.UserId
JOIN CompanyUsers as cu on cu.UserId=uc.UserId
JOIN Companies as c on cu.CompanyId = c.CompanyId
WHERE ProjectId=@ProjectId
ORDER BY CompanyName
