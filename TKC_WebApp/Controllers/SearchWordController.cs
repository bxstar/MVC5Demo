using iclickpro.BusinessLayer;
using iclickpro.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace TKC_WebApp.Controllers
{
    /// <summary>
    /// 宝贝找词拓词组词，控制器
    /// </summary>
    public class SearchWordController : TkcBaseController
    {
        /// <summary>
        /// 主页视图
        /// </summary>
        public ActionResult Index()
        {
            EntityUser session = Session["user"] as EntityUser;
            List<EntityItem> lstItem = new List<EntityItem>();
            if (session != null)
            {
                lstItem = CommonHandler.GetUserOnlineItems(session);
                ViewBag.UserName = session.fSubUserName;
            }
            else
            {
                ViewBag.UserName = "访客";
            }


            return View(lstItem);
        }

        /// <summary>
        /// 宝贝找词，跳转对应视图
        /// </summary>
        public ActionResult ByItem()
        {
            EntityUser session = Session["user"] as EntityUser;
            List<EntityItem> lstItem = new List<EntityItem>();
            if (session != null)
            {
                lstItem = CommonHandler.GetUserOnlineItems(session);
                ViewBag.UserName = session.fSubUserName;
            }
            else
            {
                ViewBag.UserName = "访客";
            }

            string itemUrl = Request["txtItemIdOrUrl"];
            if (itemUrl != null)
            {
                EntityItem itemOnline = CommonHandler.GetItemOnline(itemUrl);
                if (itemOnline != null)
                {
                    itemOnline.item_url = "http://item.taobao.com/item.htm?id=" + itemOnline.item_id;
                    ViewBag.ItemOnline = itemOnline;
                }
                else
                {
                    ViewBag.ItemOnline = new EntityItem() { item_url = "" };
                }
            }
            else
            {
                ViewBag.ItemOnline = new EntityItem() { item_url = "" };
            }

            return View(lstItem);
        }

        /// <summary>
        /// 宝贝找词，页面提交后跳转对应视图
        /// </summary>
        public ActionResult ToByItem()
        {

            string itemUrl = Request["txtItemIdOrUrl"];
            if (itemUrl != null)
            {
                EntityItem itemOnline = CommonHandler.GetItemOnline(itemUrl);
                if (itemOnline != null)
                {
                    itemOnline.item_url = ItemUrlPrefix + itemOnline.item_id;
                    ViewBag.ItemOnline = itemOnline;
                }
                else
                {
                    ViewBag.ItemOnline = new EntityItem() { item_url = "" };
                }
            }
            else
            {
                ViewBag.ItemOnline = new EntityItem() { item_url = "" };
            }

            List<EntityItem> lstItem = new List<EntityItem>();
            EntityUser session = Session["user"] as EntityUser;
            if (session != null)
            {
                lstItem = CommonHandler.GetUserOnlineItems(session);
                ViewBag.UserName = session.fSubUserName;
            }
            else
            {
                ViewBag.UserName = "访客";
            }


            return View("ByItem",lstItem);
        }

        /// <summary>
        /// 接口，返回宝贝找词的Json数据
        /// </summary>
        public JsonResult SearchWordByItem([DataSourceRequest]DataSourceRequest request,string itemUrl)
        {
            EntityItem itemOnline = CommonHandler.GetItemOnline(itemUrl);
            EntityUser session = Session["user"] as EntityUser;
            List<EntityWordData> lst = bllKeyword.GetItemKeywords(session, itemUrl);

            DataSourceResult result = lst.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 成交词拓展
        /// </summary>
        public ActionResult ByWord()
        {
            return View();
        }

        
        public JsonResult GetUserOnlineItems()
        {
            EntityUser session = Session["user"] as EntityUser;

            List<EntityItem> lstItem = CommonHandler.GetUserOnlineItems(session);

            return Json(lstItem, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetItemInfo()
        {
            string itemUrl = Request["itemUrl"];
            EntityItem itemOnline = CommonHandler.GetItemOnline(itemUrl);
            itemOnline.item_url = ItemUrlPrefix + itemOnline.item_id;
            return Json(itemOnline, JsonRequestBehavior.AllowGet);
        }
	}
}