using iclickpro.Model;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Kendo.Mvc.Extensions;

namespace TKC_WebApp.Controllers
{
    /// <summary>
    /// 创意分析，控制器
    /// </summary>
    public class CreativeAnalysisController : TkcBaseController
    {
        // GET: CreativeAnalysis
        public ActionResult Index()
        {
            EntityUser session = Session["user"] as EntityUser;

            if (session == null)
            {
                ViewBag.UserName = "访客";
            }
            else
            {
                ViewBag.UserName = session.fSubUserName;
            }

            List<SelectListItem> lstCat = new List<SelectListItem>();
            lstCat.Add(new SelectListItem() { Text = "全部类目", Value = "" });
            string strTaoCiOneLevelCatsCache = wsProxyTaoCi.GetTaoCiOneLevelCatsCache(null);

            string[] arrTaoCiOneLevelCat = strTaoCiOneLevelCatsCache.Split(',');
            foreach (var item in arrTaoCiOneLevelCat)
            {
                string[] arrCatInfo = item.Split('ÿ');
                lstCat.Add(new SelectListItem() { Text = arrCatInfo[1], Value = arrCatInfo[0] });
            }
            
            ViewBag.CatList = lstCat;

            return View();
        }

        public ActionResult IndexPub()
        {
            EntityUser session = Session["user"] as EntityUser;

            if (session == null)
            {
                ViewBag.UserName = "访客";
            }
            else
            {
                ViewBag.UserName = session.fSubUserName;
            }

            List<SelectListItem> lstCat = new List<SelectListItem>();
            lstCat.Add(new SelectListItem() { Text = "全部类目", Value = "" });
            string strTaoCiOneLevelCatsCache = wsProxyTaoCi.GetTaoCiOneLevelCatsCache(null);

            string[] arrTaoCiOneLevelCat = strTaoCiOneLevelCatsCache.Split(',');
            foreach (var item in arrTaoCiOneLevelCat)
            {
                string[] arrCatInfo = item.Split('ÿ');
                lstCat.Add(new SelectListItem() { Text = arrCatInfo[1], Value = arrCatInfo[0] });
            }

            ViewBag.CatList = lstCat;

            return View();
        }

        public JsonResult GetCreativeData([DataSourceRequest]DataSourceRequest request, long? minClick, string catId)
        {
            List<EntityCreativeAnalysis> lstCreative = bllCreative.GetCreativeAnalysisFromDB(minClick == null ? 0 : minClick.Value, catId);

            //List<EntityCreativeAnalysis> lstCreative = bllCreative.GetCreativeAnalysisFromDB(minClick, catName);

            DataSourceResult result = lstCreative.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}