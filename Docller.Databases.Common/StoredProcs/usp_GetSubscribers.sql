CREATE PROCEDURE [dbo].[usp_GetSubscribers]
	@ProjectId bigint	
AS

	declare @SubUsers AS Table
	(
		UserId bigint,
		[FirstName] [nvarchar](255) NULL,
		[LastName] [nvarchar](255) NULL,
		[Email] [nvarchar](400) NOT NULL
	)

	IF @ProjectId = 0
	BEGIN
		INSERT INTO @SubUsers
		SELECT u.UserId, u.FirstName, u.LastName, u.Email
		FROM UserCache as u 
		JOIN ProjectUsers as pu ON u.UserId = pu.UserId
	END
	ELSE
	BEGIN
		INSERT INTO @SubUsers
		SELECT u.UserId, u.FirstName, u.LastName, u.Email
		FROM UserCache as u 
		JOIN ProjectUsers as pu ON u.UserId = pu.UserId AND pu.ProjectId = @ProjectId
	END
	-- select companies first
	SELECT distinct c.CompanyName, c.CompanyAlias, c.CompanyId
	FROM @SubUsers AS subs 
	JOIN CompanyUsers as cu on subs.UserId = cu.UserId
	JOIN Companies as c on cu.CompanyId = c.CompanyId

	SELECT subs.UserId, subs.FirstName, subs.LastName, subs.Email, cu.CompanyId
	FROM @SubUsers AS subs 
	JOIN CompanyUsers as cu on subs.UserId = cu.UserId
	

RETURN 0