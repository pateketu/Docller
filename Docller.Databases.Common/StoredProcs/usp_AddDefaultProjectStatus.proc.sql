CREATE PROCEDURE [dbo].[usp_AddDefaultProjectStatus]
	@CustomerId as bigint,
	@ProjectId as bigint,
	@Status as StringTableType READONLY

AS
	INSERT INTO Status(CustomerId, ProjectId, StatusId, StatusText)
	SELECT @CustomerId,@ProjectId,Id,StringValue 
		FROM @Status
RETURN 0