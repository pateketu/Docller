CREATE PROCEDURE [dbo].[usp_GetProjectStatuses]
	@ProjectId bigint
AS
	SELECT ProjectId, StatusId, StatusText, StatusLongText
	FROM Status
	WHERE ProjectId = @ProjectId
RETURN 0