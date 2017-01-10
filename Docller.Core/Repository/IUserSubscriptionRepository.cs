using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Docller.Core.Models;

namespace Docller.Core.Repository
{
    public interface IUserSubscriptionRepository: IRepository
    {
        void UpdateUser(User user);
    }
}
