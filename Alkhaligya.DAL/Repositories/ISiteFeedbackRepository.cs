using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Repositories
{
    public interface ISiteFeedbackRepository : IRepository<SiteFeedback, int>
    {
        IQueryable<SiteFeedback> GetAll(bool tracking = true);
    }
}
