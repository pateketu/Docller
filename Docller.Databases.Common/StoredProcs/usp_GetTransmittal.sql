CREATE PROCEDURE [dbo].[usp_GetTransmittal]
	@ProjectId as bigint,
	@TransmittalId as bigint
AS
	
	SELECT TransmittalId, TransmittalNumber, Subject, Message,
	 IsDraft, t.StatusId, StatusText, t.CreatedDate, u.FirstName,u.LastName, p.BlobContainer
	FROM Transmittals AS T 
	JOIN UserCache AS U ON T.CreatedBy = U.UserId
	JOIN Projects as p on p.ProjectId = t.ProjectId
	LEFT JOIN Status as s ON T.StatusId = s.StatusId
	WHERE  TransmittalId = @TransmittalId AND t.ProjectId = @ProjectId AND IsQuickShare = 0

	IF @@ROWCOUNT > 0
	BEGIN
		 SELECT U.UserId, U.FirstName, U.LastName, U.Email,TD.Cced, c.CompanyName
		 FROM TransmittalDistribution AS TD 
			JOIN UserCache AS U ON TD.UserId = U.UserId AND TransmittalId = @TransmittalId
			JOIN CompanyUsers as cu on cu.UserId = u.UserId
			JOIN Companies as c on c.CompanyId = cu.CompanyId
		
		DECLARE @NumOfFiles as int

		SELECT @NumOfFiles = COUNT(FileId) FROM TransmittedFiles WHERE TransmittalId = @TransmittalId

		SELECT F.FileId, F.FileInternalName, F.FileName,F.Title, F.Revision,F.CurrentRevision, S.StatusText, fo.FullPath
		FROM TransmittedFiles AS TF 
			JOIN Files AS F ON TF.FileId = F.FileId 
				AND TF.RevisionNumber = F.CurrentRevision
				AND TF.TransmittalId = @TransmittalId
			JOIN Folders as fo ON fo.FolderId = f.FolderId
			LEFT JOIN [Status] as s ON F.StatusId = s.StatusId

		IF @@ROWCOUNT != @NumOfFiles
		BEGIN
			SELECT FV.FileId, F.FileInternalName, FV.VersionPath, FV.FileName,FV.Title, FV.Revision, FV.RevisionNumber, S.StatusText, fo.FullPath
			FROM TransmittedFiles AS TF 
				JOIN FileVersions AS FV ON TF.FileId = FV.FileId 
						AND TF.RevisionNumber = FV.RevisionNumber AND TF.TransmittalId = @TransmittalId
				JOIN Files as f on f.FileId = fv.FileId
				JOIN Folders as fo on fo.FolderId = fv.FolderId
			LEFT JOIN [Status] as s ON FV.StatusId = s.StatusId
		END


 
	END

RETURN 0
