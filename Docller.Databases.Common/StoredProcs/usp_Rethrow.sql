CREATE PROCEDURE [dbo].[usp_Rethrow]
AS 
-- Return if there is no error information to retrieve.
	IF ERROR_NUMBER() IS NULL 
	  RETURN ;

DECLARE @ErrorMessage NVARCHAR(4000),
  @ErrorNumber INT,
  @ErrorSeverity INT,
  @ErrorState INT,
  @ErrorLine INT,
  @ErrorProcedure NVARCHAR(200) ;

    -- Assign variables to error-handling functions that 
    -- capture information for RAISERROR.
SELECT  @ErrorNumber = ERROR_NUMBER(), @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE(), @ErrorLine = ERROR_LINE(),
        @ErrorProcedure = ISNULL(ERROR_PROCEDURE(), '-') ;

    -- Building the message string that will contain original
    -- error information.
SELECT  @ErrorMessage = N'Error %d, Level %d, State %d, Procedure %s, Line %d, ' +
        'Message: ' + ERROR_MESSAGE() ;

    -- Raise an error: msg_str parameter of RAISERROR will contain
    -- the original error information.
RAISERROR (@ErrorMessage, @ErrorSeverity, 1, @ErrorNumber, -- parameter: original error number.
  @ErrorSeverity, -- parameter: original error severity.
  @ErrorState, -- parameter: original error state.
  @ErrorProcedure, -- parameter: original error procedure name.
  @ErrorLine-- parameter: original error line number.
        ) ;
