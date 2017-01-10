CREATE PROCEDURE [dbo].[usp_UpdateFiles]
	@FilesTable FileTableType READONLY
AS

declare @Files as Table
(
	FileId bigint,
	FileInternalName uniqueidentifier,
	[FileName] nvarchar(255),
	IsExistingFile bit	
)

INSERT INTO @Files (FileId, FileInternalName, [FileName], IsExistingFile)
SELECT F.FileId, FT.FileInternalName, FT.FileName, 1
FROM Files as F JOIN 
	@FilesTable AS FT ON (F.FileNameHash =  HASHBYTES('SHA1', LOWER(FT.BaseFileName))  
			AND (F.ProjectId = FT.ProjectId and  F.FolderId=FT.FolderId))
WHERE F.FileId != FT.FileId



IF @@ROWCOUNT > 0
BEGIN
	SELECT FileId, FileInternalName, [FileName], IsExistingFile
	FROM @Files
	RETURN 106 -- Existing files found
END
ELSE
BEGIN

	UPDATE Files 
	SET FileName = FT.FileName,
	Title=FT.Title,
	DocNumber = FT.DocNumber,
	Revision =  COALESCE(FT.Revision, F.Revision),
	BaseFileName = FT.BaseFileName,
	StatusId = COALESCE(FT.StatusId, F.StatusId)
	FROM Files AS F	
	JOIN @FilesTable AS FT ON F.FileId = FT.FileId


END

RETURN 0
