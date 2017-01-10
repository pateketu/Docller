CREATE PROCEDURE [dbo].[usp_DownloadSharedFiles]
	@ProjectId as bigint,
	@TransmittalId as bigint,
	@downloadBy nvarchar(400)
AS

DECLARE @NumOfFiles as int


SELECT TransmittalId, Subject, p.BlobContainer
 FROM Transmittals as t JOIN Projects as p on t.ProjectId = p.ProjectId
 WHERE t.TransmittalId=@TransmittalId AND t.IsQuickShare=1

 
IF @@ROWCOUNT > 0
BEGIN

	SELECT @NumOfFiles = COUNT(FileId) FROM TransmittedFiles WHERE TransmittalId = @TransmittalId

	SELECT F.FileId, F.FileInternalName, F.FileName,
	F.CurrentRevision, F.FileSize, fo.FullPath
			FROM TransmittedFiles AS TF 
				JOIN Files AS F ON TF.FileId = F.FileId 
					AND TF.RevisionNumber = F.CurrentRevision
					AND TF.TransmittalId = @TransmittalId
				JOIN Folders as fo ON fo.FolderId = f.FolderId
			
	IF @@ROWCOUNT != @NumOfFiles
			BEGIN
				SELECT FV.FileId, F.FileInternalName, FV.VersionPath, FV.FileName,FV.RevisionNumber, FV.FileSize, fo.FullPath
				FROM TransmittedFiles AS TF 
					JOIN FileVersions AS FV ON TF.FileId = FV.FileId 
							AND TF.RevisionNumber = FV.RevisionNumber AND TF.TransmittalId = @TransmittalId
					JOIN Files as f on f.FileId = fv.FileId
					JOIN Folders as fo on fo.FolderId = fv.FolderId
	END

END				