using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Docller.Core.Repository
{
    public enum RepositoryStatus
    {
        Unknown = -1,
        Success = 0,
        ExistingFolder=105,
        ExistingFile =106,
        VersionPathNull = 107,
        RequiredFieldsMissing=108,
        UserPartOfAnotherCompany=109

    }
}
