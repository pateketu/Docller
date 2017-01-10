CREATE PROCEDURE [dbo].[usp_RenameFolder]
	@ProjectId INT,
	@FolderId INT,
	@ParentFolderId INT,
	@FolderName NVARCHAR(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @ParentFolderId = 0
		SET @ParentFolderId = NULL

	IF EXISTS (SELECT FolderId FROM Folders
				WHERE FolderNameHash = HASHBYTES('SHA1', LOWER(@FolderName)) 
				AND ProjectId = @ProjectId 
				AND (ParentFolderId = @ParentFolderId OR @ParentFolderId IS NULL))
	BEGIN
		RETURN 105
	END
	ELSE
	BEGIN
		UPDATE Folders 
		SET FolderName = @FolderName 
		WHERE FolderId = @FolderId

		return 0
	END
END
