CREATE PROCEDURE [dbo].[usp_GetMyTransmittlas]
	@UserName NVARCHAR (400), 
	@ProjectId bigint, 
	@ShowDraft bit, 
	@CreatedByMe bit, 
	@SendToMe bit, 
	@SendToMyCompany bit,
	@PageNumber int,
	@PageSize int
AS
	DECLARE @UserId bigint
	exec @UserId = usp_GetTenantUserId @UserName



	IF @CreatedByMe = 1 AND @ShowDraft = 1 -- My Drafts
	BEGIN
		SELECT t.TransmittalId, t.TransmittalNumber, t.Subject, t.Message, t.CreatedDate, s.StatusText, s.StatusLongText
		FROM Transmittals as t LEFT JOIN Status as s on t.StatusId = s.StatusId
		WHERE t.ProjectId = @ProjectId AND t.IsDraft = 1 AND t.CreatedBy = @UserId
		ORDER BY t.CreatedDate DESC
		RETURN 0
	END

	
	IF @CreatedByMe = 1 AND @ShowDraft = 0 -- My Send
	BEGIN
		SELECT t.TransmittalId, t.TransmittalNumber, t.Subject, t.Message,  t.CreatedDate,s.StatusText, s.StatusLongText 
		FROM Transmittals as t JOIN Status as s on t.StatusId = s.StatusId
		WHERE t.ProjectId = @ProjectId AND t.IsDraft = 0 AND t.CreatedBy = @UserId
		ORDER BY t.CreatedDate DESC
		RETURN 0
	END

	IF @SendToMe = 1 -- Send to me
	BEGIN
		SELECT t.TransmittalId, t.TransmittalNumber, t.Subject, t.Message,  t.CreatedDate, s.StatusText, s.StatusLongText, td.Cced
		FROM Transmittals as t 
		JOIN TransmittalDistribution as td on t.TransmittalId = td.TransmittalId AND td.UserId = @UserId
		JOIN Status as s on t.StatusId = s.StatusId
		WHERE t.ProjectId = @ProjectId AND t.IsDraft = 0
		ORDER BY t.CreatedDate DESC
		RETURN 0
	END


	IF @SendToMyCompany = 1 -- Send to my company
	BEGIN

		DECLARE @CompanyId bigint
		exec @CompanyId = usp_GetUserCompanyId @userName

		SELECT t.TransmittalId, t.TransmittalNumber, t.Subject, t.Message, t.CreatedDate, s.StatusText, s.StatusLongText, td.Cced
		FROM Transmittals as t 
		JOIN TransmittalDistribution as td on t.TransmittalId = td.TransmittalId
		JOIN CompanyUsers as cu on cu.UserId = td.UserId AND cu.CompanyId = @CompanyId
		JOIN Status as s on t.StatusId = s.StatusId
		WHERE t.ProjectId = @ProjectId AND t.IsDraft = 0
		ORDER BY t.CreatedDate DESC
		RETURN 0
	END

