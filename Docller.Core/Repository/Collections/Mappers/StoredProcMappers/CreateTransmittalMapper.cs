using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Infrastructure;
using Docller.Core.Models;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Mappers.StoredProcMappers
{
    public class CreateTransmittalMapper : IResultSetMapper<CreateTransmittalInfo>
    {
        public IEnumerable<CreateTransmittalInfo> MapSet(IDataReader reader)
        {
            CreateTransmittalInfo createTransmittalInfo = new CreateTransmittalInfo();
            TransmittalMapper transmittalMapper = new TransmittalMapper();
            createTransmittalInfo.Transmittal = transmittalMapper.MapSetTransmittal(reader);
            if (reader.NextResult())
            {
                IssueSheetMapper issueSheetMapper = new IssueSheetMapper();
                createTransmittalInfo.IssueSheetData = issueSheetMapper.MapSetSingle(reader);
            }
            yield return createTransmittalInfo;

        }
    }
}
