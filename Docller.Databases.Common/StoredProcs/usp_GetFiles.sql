CREATE PROCEDURE [dbo].[usp_GetFiles]
	@ProjectId bigint,
	@FolderId bigint,
	@userName nvarchar(400),
	@RestrictToTransmitted bit,
	@SortBy nvarchar(10),
	@SortDirection int,	
	@PageNumber int,
	@PageSize int
	
AS
	
		DECLARE @Files as Table
	(
		FileId bigint,
		FileInternalName uniqueidentifier,
		[FileName] nvarchar(255) NOT NULL,
		[FileExtension] [nvarchar](10)  NOT NULL,
		[ContentType] [nvarchar] (255) NOT NULL,
		[CurrentRevision] int NOT NULL,
		[FileSize] int NOT NULL,
		[DocNumber] [nvarchar](255) NULL,
		[Revision] [nvarchar](10) NULL,
		[Title] nvarchar(1000) NULL,
		[Notes] nvarchar(3000) NULL,
		[Status] nvarchar(200) NULL,
		[ThumbnailUrl] nvarchar(1000) NULL,
		[PreviewUrl] nvarchar(1000) NULL,
		[CreatedDate] datetime NOT NULL,
		[FullPath] [nvarchar](2000) NULL,
		[CreatedByUserName] nvarchar(400) NOT NULL,
		[CreatedByFirstName] nvarchar(400) NOT NULL,
		[CreatedByLastName] nvarchar(400) NOT NULL,
		[VersionCount] int NULL,
		[TransmittalCount] int NULL,
		[PreviewsGenerated] datetime NULL
	)

	--DECLARE @CompanyId bigint
	DECLARE @UserId bigint
	exec @UserId = usp_GetTenantUserId @UserName
	--exec @CompanyId = usp_GetUserCompanyId @UserName

	IF @RestrictToTransmitted = 0
	BEGIN
		
		INSERT INTO @Files
		SELECT F.FileId, F.FileInternalName, F.FileName, F.FileExtension, F.ContentType, F.CurrentRevision, F.FileSize, F.DocNumber,
				F.Revision, F.Title, F.Notes, S.StatusText,F.ThumbnailUrl, F.PreviewUrl, F.CreatedDate, FO.FullPath, 
				U.UserName, U.FirstName, U.LastName, (SELECT COUNT(FileId) + 1
														FROM FileVersions AS FV WHERE FV.FileId = F.FileId),
													(SELECT COUNT(TransmittalId) 
														FROM TransmittedFiles AS TF WHERE TF.FileId = F.FileId),
													PreviewsGenerated
		FROM Files AS F
			 JOIN Folders AS FO ON F.FolderId = FO.FolderId
			 JOIN UserCache AS U ON F.CreatedBy = U.UserId
			LEFT JOIN Status AS S ON F.StatusId = S.StatusId
			WHERE F.FolderId = @FolderId AND F.ProjectId = @ProjectId
			ORDER BY
				CASE WHEN @SortBy='Date' AND @SortDirection=0 THEN F.CreatedDate END ASC,
				CASE WHEN @SortBy='Date' AND @SortDirection=1 THEN F.CreatedDate END DESC,
				CASE WHEN @SortBy='FileName' AND @SortDirection=0 THEN F.FileName END ASC,
				CASE WHEN @SortBy='FileName' AND @SortDirection=1 THEN F.FileName END DESC,
				CASE WHEN @SortBy='Title' AND @SortDirection=0 THEN F.Title END ASC,
				CASE WHEN @SortBy='Title' AND @SortDirection=1 THEN F.Title END DESC,
				CASE WHEN @SortBy='Status' AND @SortDirection=0 THEN S.StatusText END ASC,
				CASE WHEN @SortBy='Status' AND @SortDirection=1 THEN S.StatusText END DESC
			OFFSET (@PageNumber-1)*@PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY
		
			SELECT COUNT(*) FROM Files
			WHERE FolderId = @FolderId AND ProjectId = @ProjectId
			
	END				
	ELSE
	BEGIN
		
		INSERT INTO @Files
		SELECT F.FileId, F.FileInternalName, F.FileName, F.FileExtension, F.ContentType, F.CurrentRevision, F.FileSize, F.DocNumber,
				F.Revision, F.Title, F.Notes, S.StatusText,F.ThumbnailUrl, F.PreviewUrl, F.CreatedDate, FO.FullPath, 
				U.UserName, U.FirstName, U.LastName, (SELECT COUNT(FileId) + 1
														FROM FileVersions AS FV WHERE FV.FileId = F.FileId),
													(1) AS HasTransmittal,
													F.PreviewsGenerated
		FROM TransmittedFiles as TF 
		JOIN Transmittals AS T ON TF.TransmittalId = T.TransmittalId AND T.IsDraft = 0
		JOIN TransmittalDistribution AS TD ON T.TransmittalId = TD.TransmittalId AND TD.UserId = @UserId
		JOIN Files AS F ON TF.FileId = F.FileId AND FolderId = @FolderId
		JOIN Folders AS FO ON FO.FolderId = @FolderId
		JOIN UserCache AS U ON F.CreatedBy = U.UserId
		JOIN Status AS S ON F.StatusId = S.StatusId
		ORDER BY
				CASE WHEN @SortBy='Date' AND @SortDirection=0 THEN F.CreatedDate END ASC,
				CASE WHEN @SortBy='Date' AND @SortDirection=1 THEN F.CreatedDate END DESC,
				CASE WHEN @SortBy='FileName' AND @SortDirection=0 THEN F.FileName END ASC,
				CASE WHEN @SortBy='FileName' AND @SortDirection=1 THEN F.FileName END DESC,
				CASE WHEN @SortBy='Title' AND @SortDirection=0 THEN F.Title END ASC,
				CASE WHEN @SortBy='Title' AND @SortDirection=1 THEN F.Title END DESC,
				CASE WHEN @SortBy='Status' AND @SortDirection=0 THEN S.StatusText END ASC,
				CASE WHEN @SortBy='Status' AND @SortDirection=1 THEN S.StatusText END DESC
			OFFSET (@PageNumber-1)*@PageSize ROWS
			FETCH NEXT @PageSize ROWS ONLY

			SELECT COUNt(TF.FileId)
			FROM TransmittedFiles as TF 
			JOIN Transmittals AS T ON TF.TransmittalId = T.TransmittalId AND T.IsDraft = 0
			JOIN TransmittalDistribution AS TD ON T.TransmittalId = TD.TransmittalId AND TD.UserId = 32
			JOIN Files AS F ON TF.FileId = F.FileId AND FolderId = 105
		
	END
	SELECT * FROM @Files
	
	
	-- Get Attachments
	SELECT FA.FileId, FA.FileName, FA.FileExtension, 
			FA.ContentType, FA.FileSize, FA.RevisionNumber, FA.VersionPath
	FROM FileAttachments AS FA 
		JOIN @Files AS F ON FA.FileId = F.FileId 
			AND FA.RevisionNumber = F.CurrentRevision


	
RETURN 0