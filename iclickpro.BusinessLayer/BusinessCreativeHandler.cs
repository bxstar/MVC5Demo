using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iclickpro.Model;

namespace iclickpro.BusinessLayer
{
    public class BusinessCreativeHandler
    {
        /// <summary>
        /// 数据库，获取所有创意分析数据
        /// </summary>
        /// <param name="minClick"></param>
        /// <param name="catId"></param>
        /// <returns></returns>
        public List<EntityCreativeAnalysis> GetCreativeAnalysisFromDB(long minClick, string catId)
        {
            //var query = DefaultDbContext.Current().CreativeAnalysis
            //    .Where(o => o.click > minClick && o.cat_id.StartsWith(catId)).OrderByDescending(o => o.ctr);

            var query = from u in DefaultDbContext.Current().CreativeAnalysis
                        where (u.click >minClick && u.cat_id.StartsWith(catId))
                        orderby u.ctr descending
                        select u ;

            string sql = query.ToString();

            List<EntityCreativeAnalysis> lstCreative = query.ToList();
            return lstCreative;
        }

    }
}
