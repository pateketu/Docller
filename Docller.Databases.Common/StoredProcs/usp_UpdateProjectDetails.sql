CREATE PROCEDURE [dbo].[usp_UpdateProjectDetails]	
	@ProjectId bigint,
	@ProjectName nvarchar(255) 
	
AS
		UPDATE Projects
		SET ProjectName = @ProjectName --, ProjectCode = @ProjectCode, ProjectImage = @ProjectImage
		WHERE ProjectId = @ProjectId

RETURN 0
