CREATE PROCEDURE [dbo].[usp_GetFileHistory]
	@FileId bigint
AS

SELECT FileId, [FileName], FileExtension
FROM Files WHERE FileId=@FileId

SELECT t.TransmittalId, t.TransmittalNumber, t.Subject, t.CreatedDate, s.StatusText, u.FirstName, u.LastName
FROM Files as f LEFT JOIN TransmittedFiles as tf on f.FileId = tf.FileId and tf.RevisionNumber = f.CurrentRevision
JOIN Transmittals as t on tf.TransmittalId = t.TransmittalId
JOIN [Status] as s on t.StatusId = s.StatusId
JOIN UserCache as u on t.CreatedBy = u.UserId
WHERE f.FileId = @FileId 
ORDER BY t.CreatedDate DESC


SELECT fv.FileName, fv.Title, fv.CreatedDate, fv.RevisionNumber, fv.Revision, fv.VersionPath, s.StatusText, u.FirstName, u.LastName,
		tf.TransmittalId, fa.FileName as AttachmentName, fa.FileExtension as AttachmentFileExtension --, t.TransmittalNumber, t.Subject, t.CreatedDate as TransmittalCreatedDate, tu.FirstName, tu.LastName
FROM FileVersions as fv 
JOIN UserCache as u ON fv.CreatedBy = u.UserId
LEFT JOIN FileAttachments as fa ON fv.FileId = fa.FileId AND fv.RevisionNumber = fa.RevisionNumber
LEFT JOIN TransmittedFiles as tf on fv.fileId = tf.FileId and fv.RevisionNumber  = tf.RevisionNumber
LEFT JOIN Status as s on fv.StatusId = s.StatusId
--LEFT JOIN Transmittals as t on tf.TransmittalId = t.TransmittalId
--LEFT JOIN Users as tu on t.CreatedBy = tu.UserId

WHERE fv.FileId=@FileId
ORDER BY fv.CreatedDate DESC, fv.Revision





RETURN 0
