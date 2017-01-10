CREATE PROCEDURE [dbo].[usp_GetFilePreUploadInfo]
	@ProjectId int, 
	@FolderId int,
	@FilesTable FileTableType READONLY,
	@AttachmentsTable FileTableType READONLY
AS
declare @Files as Table
(
	FileId bigint,
	FileInternalName uniqueidentifier,
	[FileName] nvarchar(255),
	BaseFileName nvarchar(255),
	DocNumber nvarchar(255),
	IsExistingFile bit,
    TempFile uniqueidentifier
	
)

declare @Attachments as Table
(
	ParentFile uniqueidentifier,
	[FileName] nvarchar(255),
	BaseFileName nvarchar(255)
)

--Select the ProjectId and BlobContainer
SELECT ProjectId, BlobContainer FROM Projects WHERE ProjectId = @ProjectId;

--Select the Folder Info
SELECT  FolderId,FolderInternalName,FullPath
FROM Folders
WHERE FolderId = @FolderId

--This query basically insert FilId, Etc into @Files table, TempFile is carried over so
--- Query for attachments can figure which attachment belongs to which file
INSERT INTO @Files(FileId, FileInternalName, FileName, BaseFileName, DocNumber, IsExistingFile, TempFile)
SELECT COALESCE(F.FileId, FT.FileId) AS FileId,
		COALESCE(F.FileInternalName, FT.FileInternalName) AS FileInternalName, 
	   FT.FileName, FT.BaseFileName,F.DocNumber,
	   (CASE WHEN F.FileId IS NULL THEN 0 ELSE 1 END)
			AS IsExistingFile,
		FT.FileInternalName			
FROM Files as F RIGHT JOIN 
	@FilesTable AS FT ON F.FileNameHash =  HASHBYTES('SHA1', LOWER(FT.BaseFileName)) 
			AND (F.ProjectId = @ProjectId and  F.FolderId=@FolderId)

--Now figure out actual attachment details, this query will choose the actual Parent fil
INSERT INTO @Attachments(FileName,BaseFileName,ParentFile)
SELECT FileName,BaseFileName,
		(SELECT FileInternalName FROM @Files WHERE TempFile = A.ParentFile) AS ParentFile
FROM @AttachmentsTable AS A

--Return FileInfo
SELECT FileId, FileInternalName, FileName, BaseFileName, DocNumber, IsExistingFile FROM @Files


--Return File Attacment information
SELECT F.FileId, F.FileInternalName As ParentFile, A.FileName,  A.BaseFileName,
				(CASE WHEN 
							(SELECT TOP 1 FA.FileId
								FROM FileAttachments AS FA 
								WHERE FA.FileId = F.FileId
								AND FA.FileNameHash =  HASHBYTES('SHA1', LOWER(A.BaseFileName))
								AND (FA.ProjectId = @ProjectId and  FA.FolderId=@FolderId)) IS NULL THEN 0 ELSE 1 END)
							AS IsExistingFile  
FROM @Attachments AS A 
	JOIN @Files AS F ON A.ParentFile = F.FileInternalName

	--LEFT JOIN FileAttachments AS FA ON FA.FileId = F.FileId 
		--		AND FA.FileNameHash =  HASHBYTES('SHA1', LOWER(A.BaseFileName)) 
			--	AND (FA.ProjectId = @ProjectId and  FA.FolderId=@FolderId)

					
RETURN 0