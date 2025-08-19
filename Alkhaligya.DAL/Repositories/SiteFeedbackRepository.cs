using Alkhaligya.DAL.Data.DbHelper;
using Alkhaligya.DAL.Models;
using Alkhaligya.DAL.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.DAL.Repositories
{
    public class SiteFeedbackRepository : Repository<SiteFeedback, int>, ISiteFeedbackRepository
    {
        public SiteFeedbackRepository(AlkhligyaContext context) : base(context)
        {
        }


        public override IQueryable<SiteFeedback> GetAll(bool tracking = true)
        {
            var query = _context.SiteFeedbacks;
            return tracking ? query : query.AsNoTracking();
        }
    }
}
