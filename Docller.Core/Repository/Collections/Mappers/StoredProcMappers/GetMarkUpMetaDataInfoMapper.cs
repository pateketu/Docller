using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Docller.Core.Models;
using Docller.Core.Repository.Mappers;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository.Collections.Mappers.StoredProcMappers
{
    public class GetMarkUpMetaDataInfoMapper:IResultSetMapper<MarkUpFile>
    {
        public IEnumerable<MarkUpFile> MapSet(IDataReader reader)
        {
            MarkUpFile markUpFile = null;
                
            using (reader)
            {
                if (reader.Read())
                {
                    bool existingFile = reader.GetInt32(0) == 1;
                    bool createdByCurrentUser = reader.GetInt32(1) == 1;
                    long attachmentId = reader.GetInt64(2);
                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            markUpFile = DefaultMappers.ForMarkUp.MapRow(reader);
                            markUpFile.IsExistingFile = existingFile;
                            markUpFile.CreatedByCurrentUser = createdByCurrentUser;
                            markUpFile.FileAttachmentId = attachmentId;

                        }
                    }
                    if (markUpFile == null)
                    {
                        markUpFile = new MarkUpFile()
                            {
                                IsExistingFile = existingFile,
                                CreatedByCurrentUser = createdByCurrentUser,
                                FileAttachmentId = attachmentId
                            };
                    }
                }
            }
            yield return markUpFile;
        }
    }
}
