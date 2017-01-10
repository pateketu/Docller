CREATE PROCEDURE [dbo].[usp_GetTransmittalLite]
	@ProjectId as bigint,
	@TransmittalId as bigint
AS
	
	SELECT TransmittalId, TransmittalNumber, Subject, Message,
	 IsDraft, t.StatusId, StatusText, t.CreatedDate, u.FirstName,u.LastName,p.BlobContainer
	FROM Transmittals AS T 
	JOIN UserCache AS U ON T.CreatedBy = U.UserId
	JOIN Projects as p ON p.ProjectId = t.ProjectId
	LEFT JOIN Status as s ON T.StatusId = s.StatusId
	WHERE  TransmittalId = @TransmittalId AND t.ProjectId = @ProjectId

	SELECT U.UserId, U.FirstName, U.LastName, U.Email,TD.Cced, c.CompanyName
		 FROM TransmittalDistribution AS TD 
			JOIN UserCache AS U ON TD.UserId = U.UserId AND TransmittalId = @TransmittalId
			JOIN CompanyUsers as cu on cu.UserId = u.UserId
			JOIN Companies as c on c.CompanyId = cu.CompanyId
RETURN 0
