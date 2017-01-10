using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Common;
using Docller.Core.DB;
using Docller.Core.Models;
using Docller.Core.Repository.Accessors;
using Docller.Core.Repository.Collections;
using Docller.Core.Repository.Mappers;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace Docller.Core.Repository
{
    public class ProjectRepository : BaseRepository, IProjectRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionRepository"/> class.
        /// </summary>
        /// <param name="federation">The federation.</param>
        /// <param name="federationKey">The federation key.</param>
        /// <remarks></remarks>
        public ProjectRepository(FederationType federation, long federationKey)
            : base(federation, federationKey)
        {

        }
        public int Create(string userName, PermissionFlag permissionFlag, Project project, List<Status> defaultStatus)
        {
            Database db = this.GetDb();
            GenericParameterMapper parameterMapper = new GenericParameterMapper(db);
            
            StoredProcAccessor<Project> accessor = db.CreateStoredProcAccessor<Project>(StoredProcs.AddProject,
                                                                               parameterMapper);
            accessor.ExecuteNonQuery(project.ProjectId, project.ProjectName, project.ProjectCode, project.BlobContainer,
                                            project.ProjectImage, project.CustomerId, userName, permissionFlag,
                                            new StatusCollection (defaultStatus));

            if(parameterMapper.ReturnValue != null)
            {
                return parameterMapper.ReturnValue.Value;
            }
            return -1;
        }

        public Project GetProjectDetails(string userName, long projectId)
        {
            Database db = this.GetDb();
            StoredProcAccessor<Project> accessor = db.CreateStoredProcAccessor(StoredProcs.GetProjectDetails,
                                                                               new GenericParameterMapper(db),
                                                                               DefaultMappers.ForProjectDetails);
            return accessor.ExecuteSingle(userName, projectId);
        }

        public IEnumerable<Status> GetProjectStatuses(long projectId)
        {
            Database db = this.GetDb();
            StoredProcAccessor<Status> storedProcAccessor = new StoredProcAccessor<Status>(db,
                                                                                           StoredProcs.
                                                                                               GetProjectStatuses,
                                                                                           DefaultMappers.
                                                                                               ForProjectStatus,
                                                                                           new GenericParameterMapper(db));
            return storedProcAccessor.Execute(projectId);
        }

        public void UpdateProject(Project project)
        {
            Database db = GetDb();
            ModelParameterMapper<Project> modelParameterMapper = new ModelParameterMapper<Project>(db,project);
            SqlDataRepositoryHelper.ExecuteNonQuery(db, StoredProcs.UpdateProjectDetails, project, modelParameterMapper);

        }
    }
}
