CREATE PROCEDURE [dbo].[usp_GetIssueSheet]
		@UserName nvarchar(400),
		@TransmittalId bigint
AS


SET NOCOUNT ON


--SET @TransmittalId = 1505

DECLARE @FileIds AS TABLE
(
	FileId bigint,
	TransmittalCreatedDate datetime

)

DECLARE @FileHistory AS TABLE
(
	FileId bigint,
	RevisionNumber int,
	TransmittalId bigint,
	StatusId bigint,
	StatusText nvarchar(200),
	TransmittalDate datetime
	
)

SELECT t.TransmittalId, t.TransmittalNumber, p.ProjectName, p.BlobContainer
FROM Transmittals as t JOIN Projects as p on t.ProjectId = p.ProjectId
WHERE t.TransmittalId = @TransmittalId

INSERT INTO @FileIds
SELECT FileId, T.CreatedDate
FROM TransmittedFiles as TF
JOIN Transmittals AS T ON TF.TransmittalId = T.TransmittalId
WHERE T.TransmittalId = @TransmittalId AND T.IsDraft = 0

SELECT Fids.FileId, F.FileName, F.Title, F.Revision FROM @FileIds AS FIds JOIN Files AS F ON FIds.FileId = F.FileId

WHILE EXISTS(SELECT FileId FROM @FileIds)
BEGIN
	
	DECLARE @FileId as bigint
	DECLARE @CutOffDate as datetime

	SELECT TOP 1 @FileId = FileId, @CutOffDate = TransmittalCreatedDate
	FROM @FileIds

	INSERT INTO @FileHistory
	SELECT Top 10 TF.FileId, TF.RevisionNumber, T.TransmittalId, S.StatusId, S.StatusText, T.CreatedDate
	FROM TransmittedFiles AS TF 
		JOIN Transmittals AS T ON TF.TransmittalId = T.TransmittalId
		JOIN Status as S ON T.StatusId = S.StatusId
	WHERE TF.FileId = @FileId AND T.IsDraft = 0 AND T.CreatedDate <= @CutOffDate
	ORDER BY T.CreatedDate DESc

	DELETE @FileIds
	WHERE FileId = @FileId
END

SELECT FH.TransmittalId,FH.FileId, COALESCE(F.FileName, FV.FileName) AS FileName,  COALESCE (F.Title, FV.Title) AS Title, COALESCE(F.Revision, FV.Revision) AS				Revision,FH.RevisionNumber, FH.StatusId, FH.StatusText, FH.TransmittalDate
FROM @FileHistory AS FH 
	LEFT JOIN Files AS F ON FH.FileId = F.FileId AND FH.RevisionNumber = F.CurrentRevision
	LEFT JOIN FileVersions AS FV ON FH.FileId = FV.FileId AND FH.RevisionNumber = FV.RevisionNumber
ORDER BY FileId ASC, RevisionNumber DESC


SELECT DISTINCT FH.TransmittalId, TD.UserId, U.FirstName, U.LastName, U.Email, CU.CompanyId, C.CompanyName
FROM @FileHistory AS FH 
	JOIN TransmittalDistribution AS TD ON FH.TransmittalId = TD.TransmittalId
	JOIN UserCache AS U ON TD.UserId = U.UserId
	JOIN CompanyUsers AS CU ON CU.UserId = U.UserId
	JOIN Companies AS C ON C.CompanyId = CU.CompanyId
ORDER BY FH.TransmittalId DESC


SELECT StatusId, StatusText, StatusLongText FROM Status