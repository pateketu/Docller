using Docller.Core.Common;
using Docller.Core.Repository;
using StructureMap;

namespace Docller.Core.Services
{
    public class ServiceBase<T> where T: IRepository
    {
        private readonly T _repository;
        
        public ServiceBase(T repository)
        {
            this._repository = repository;
        }

        public T Repository
        {
            get { return this._repository; }
        }

        protected IDocllerContext Context
        {
            get
            {
                IDocllerContext context = ObjectFactory.TryGetInstance<IDocllerContext>();
                if(context == null)
                {
                    throw new NoDocllerContextException(this.GetType());
                }
                return context;
            }
        }
    }
}