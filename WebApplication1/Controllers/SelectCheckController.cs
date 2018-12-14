using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using WebApplication1.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;

namespace WebApplication1.Controllers
{
    public class SelectCheckController : Controller
    {
        private static readonly string arialFontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),"arialuni.ttf");//arial unicode MS是完整的unicode字型。
        private static readonly string Querystring = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),"SIMHEI.TTF");//標楷體
        //可用Arial或標楷體，自己選一個
        static BaseFont baseFont = BaseFont.CreateFont(Querystring, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);



        // GET: SelectCheck
        public ActionResult fcjQueryZC(SelectCheckModels SCM)
        {
            string _LineCode = "fcj_";
            string _opxx = SCM.opxx;
            string _StartDateTime = SCM.StartDateTime;
            string _EndDateTime = SCM.EndDateTime;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            if (_opxx == null) _opxx = "op80";
            List<SelectListItem> opxxSelect = new List<SelectListItem>();
            opxxSelect.Add(new SelectListItem { Value = "op80", Text = "OP80 拧紧工位OP80", Selected = (_opxx == "op80") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op90", Text = "OP90 拧紧工位OP90", Selected = (_opxx == "op90") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op100", Text = "OP100 拧紧工位OP100", Selected = (_opxx == "op100") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op110", Text = "OP110 拧紧工位OP110", Selected = (_opxx == "op110") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "All", Text = "All 所有数据", Selected = (_opxx == "All") ? true : false });
            ViewBag.opxxSelect = opxxSelect;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
                }
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                }
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }



            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "') ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "`  WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "' ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }







        // GET: SelectCheck
        public ActionResult hqQueryZC(SelectCheckModels SCM)
        {
            string _LineCode = "hq_";
            string _opxx = SCM.opxx;
            string _StartDateTime = SCM.StartDateTime;
            string _EndDateTime = SCM.EndDateTime;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            if (_opxx == null) _opxx = "op30";
            List<SelectListItem> opxxSelect = new List<SelectListItem>();
            opxxSelect.Add(new SelectListItem { Value = "op30", Text = "OP30 拧紧工位OP30", Selected = (_opxx == "op30") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op30_2", Text = "OP30_2 拧紧工位OP30_2", Selected = (_opxx == "op30_2") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op50", Text = "OP50 拧紧工位OP50", Selected = (_opxx == "op50") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op80", Text = "OP80 拧紧工位OP80", Selected = (_opxx == "op80") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op85", Text = "OP85 拧紧工位OP85", Selected = (_opxx == "op85") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op100", Text = "OP100 拧紧工位OP100", Selected = (_opxx == "op100") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op110", Text = "OP110 拧紧工位OP110", Selected = (_opxx == "op110") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op120", Text = "OP120 拧紧工位OP120", Selected = (_opxx == "op120") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op140", Text = "OP140 拧紧工位OP140", Selected = (_opxx == "op140") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "All", Text = "All 所有数据", Selected = (_opxx == "All") ? true : false });
            ViewBag.opxxSelect = opxxSelect;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";

                }
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                }
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }



            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_zc,'" + _BarCode + "') ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "`  WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "' ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult qxQueryZC(SelectCheckModels SCM)
        {
            string _LineCode = "qx_";
            string _opxx = SCM.opxx;
            string _StartDateTime = SCM.StartDateTime;
            string _EndDateTime = SCM.EndDateTime;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            if (_opxx == null) _opxx = "op80";
            List<SelectListItem> opxxSelect = new List<SelectListItem>();
            opxxSelect.Add(new SelectListItem { Value = "op80", Text = "OP80 拧紧工位OP80", Selected = (_opxx == "op80") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op90", Text = "OP90 拧紧工位OP90", Selected = (_opxx == "op90") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op100", Text = "OP100 拧紧工位OP100", Selected = (_opxx == "op100") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op110", Text = "OP110 拧紧工位OP110", Selected = (_opxx == "op110") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "All", Text = "All 所有数据", Selected = (_opxx == "All") ? true : false });
            ViewBag.opxxSelect = opxxSelect;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";

                }
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                }
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }



            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_zc,'" + _BarCode + "') ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "`  WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "' ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult fcjQueryPart(SelectCheckModels SCM)
        {
            string _LineCode = "fcj_";
            string _opxx = SCM.opxx;
            string _StartDateTime = SCM.StartDateTime;
            string _EndDateTime = SCM.EndDateTime;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            if (_opxx == null) _opxx = "op10";
            List<SelectListItem> opxxSelect = new List<SelectListItem>();
            opxxSelect.Add(new SelectListItem { Value = "op10", Text = "OP10 拧紧工位OP10", Selected = (_opxx == "op10") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op20", Text = "OP20 拧紧工位OP20", Selected = (_opxx == "op20") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op30", Text = "OP30 拧紧工位OP30", Selected = (_opxx == "op30") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op50", Text = "OP50 拧紧工位OP50", Selected = (_opxx == "op50") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op60", Text = "OP60 拧紧工位OP60", Selected = (_opxx == "op60") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op120", Text = "OP120 拧紧工位OP120", Selected = (_opxx == "op120") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "All", Text = "All 所有数据", Selected = (_opxx == "All") ? true : false });
            ViewBag.opxxSelect = opxxSelect;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_zc,'" + _BarCode + "')";

                }
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                }
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }

            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_zc,'" + _BarCode + "') ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "`  WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "' ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";

            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult hqQueryPart(SelectCheckModels SCM)
        {
            string _LineCode = "hq_";
            string _opxx = SCM.opxx;
            string _StartDateTime = SCM.StartDateTime;
            string _EndDateTime = SCM.EndDateTime;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            if (_opxx == null) _opxx = "op10";
            List<SelectListItem> opxxSelect = new List<SelectListItem>();
            opxxSelect.Add(new SelectListItem { Value = "op10", Text = "OP10 拧紧工位OP10", Selected = (_opxx == "op10") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op20", Text = "OP20 拧紧工位OP20", Selected = (_opxx == "op20") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op40", Text = "OP40 拧紧工位OP40", Selected = (_opxx == "op40") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op60", Text = "OP60 拧紧工位OP60", Selected = (_opxx == "op60") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op90", Text = "OP90 拧紧工位OP90", Selected = (_opxx == "op90") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op110_1", Text = "OP110 拧紧工位OP110", Selected = (_opxx == "op110_1") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op120_1", Text = "OP120 拧紧工位OP120", Selected = (_opxx == "op120_1") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op130", Text = "OP130 拧紧工位OP130", Selected = (_opxx == "op130") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "All", Text = "All 所有数据", Selected = (_opxx == "All") ? true : false });
            ViewBag.opxxSelect = opxxSelect;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";

                }
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                }
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }

            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_zc,'" + _BarCode + "') ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "`  WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "' ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";

            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult qxQueryPart(SelectCheckModels SCM)
        {
            string _LineCode = "qx_";
            string _opxx = SCM.opxx;
            string _StartDateTime = SCM.StartDateTime;
            string _EndDateTime = SCM.EndDateTime;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            if (_opxx == null) _opxx = "op10";
            List<SelectListItem> opxxSelect = new List<SelectListItem>();
            opxxSelect.Add(new SelectListItem { Value = "op10", Text = "OP10 拧紧工位OP10", Selected = (_opxx == "op10") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op20", Text = "OP20 拧紧工位OP20", Selected = (_opxx == "op20") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op40", Text = "OP40 拧紧工位OP40", Selected = (_opxx == "op40") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op60", Text = "OP60 拧紧工位OP60", Selected = (_opxx == "op60") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op90", Text = "OP90 拧紧工位OP90", Selected = (_opxx == "op90") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op110_1", Text = "OP110 拧紧工位OP110", Selected = (_opxx == "op110_1") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op120_1", Text = "OP120 拧紧工位OP120", Selected = (_opxx == "op120_1") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op130", Text = "OP130 拧紧工位OP130", Selected = (_opxx == "op130") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "All", Text = "All 所有数据", Selected = (_opxx == "All") ? true : false });
            ViewBag.opxxSelect = opxxSelect;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";

                }
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                }
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }

            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE  INSTR(barcode_zc,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_zc,'" + _BarCode + "') ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "`  WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "' ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";

            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult fcjQuerySubPart(SelectCheckModels SCM)
        {
            string _LineCode = "fcj_";
            string _opxx = SCM.opxx;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            if (_opxx == null) _opxx = "op10";
            List<SelectListItem> opxxSelect = new List<SelectListItem>();
            opxxSelect.Add(new SelectListItem { Value = "op10", Text = "OP10 拧紧工位OP10", Selected = (_opxx == "op10") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op20", Text = "OP20 拧紧工位OP20", Selected = (_opxx == "op20") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op30", Text = "OP30 拧紧工位OP30", Selected = (_opxx == "op30") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op50", Text = "OP50 拧紧工位OP50", Selected = (_opxx == "op50") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op60", Text = "OP60 拧紧工位OP60", Selected = (_opxx == "op60") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op120", Text = "OP120 拧紧工位OP120", Selected = (_opxx == "op120") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "All", Text = "All 所有数据", Selected = (_opxx == "All") ? true : false });
            ViewBag.opxxSelect = opxxSelect;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_part,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_part,'" + _BarCode + "')";

                }
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }

            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_part,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_part, '" + _BarCode + "') ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult hqQuerySubPart(SelectCheckModels SCM)
        {
            string _LineCode = "hq_";
            string _opxx = SCM.opxx;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            if (_opxx == null) _opxx = "op10";
            List<SelectListItem> opxxSelect = new List<SelectListItem>();
            opxxSelect.Add(new SelectListItem { Value = "op10", Text = "OP10 拧紧工位OP10", Selected = (_opxx == "op10") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op20", Text = "OP20 拧紧工位OP20", Selected = (_opxx == "op20") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op40", Text = "OP40 拧紧工位OP40", Selected = (_opxx == "op40") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op60", Text = "OP60 拧紧工位OP60", Selected = (_opxx == "op60") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op90", Text = "OP90 拧紧工位OP90", Selected = (_opxx == "op90") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op110_1", Text = "OP110 拧紧工位OP110", Selected = (_opxx == "op110_1") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op120_1", Text = "OP120 拧紧工位OP120", Selected = (_opxx == "op120_1") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op130", Text = "OP130 拧紧工位OP130", Selected = (_opxx == "op130") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "All", Text = "All 所有数据", Selected = (_opxx == "All") ? true : false });
            ViewBag.opxxSelect = opxxSelect;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_part,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_part,'" + _BarCode + "')";

                }
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }

            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_part,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_part, '" + _BarCode + "') ORDER BY ID) a ";
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult qxQuerySubPart(SelectCheckModels SCM)
        {
            string _LineCode = "qx_";
            string _opxx = SCM.opxx;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            if (_opxx == null) _opxx = "op10";
            List<SelectListItem> opxxSelect = new List<SelectListItem>();
            opxxSelect.Add(new SelectListItem { Value = "op10", Text = "OP10 拧紧工位OP10", Selected = (_opxx == "op10") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op20", Text = "OP20 拧紧工位OP20", Selected = (_opxx == "op20") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op30", Text = "OP30 拧紧工位OP30", Selected = (_opxx == "op30") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op50", Text = "OP50 拧紧工位OP50", Selected = (_opxx == "op50") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op60", Text = "OP60 拧紧工位OP60", Selected = (_opxx == "op60") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "op120", Text = "OP120 拧紧工位OP120", Selected = (_opxx == "op120") ? true : false });
            opxxSelect.Add(new SelectListItem { Value = "All", Text = "All 所有数据", Selected = (_opxx == "All") ? true : false });
            ViewBag.opxxSelect = opxxSelect;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                if (_opxx == "All")
                {
                    querycount = "SELECT COUNT(*) FROM";
                    querycount = querycount + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        querycount = querycount + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_part,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { querycount = querycount + " UNION ALL "; }
                    }
                    querycount = querycount + ") a ";
                }
                else
                {
                    querycount = " SELECT COUNT(*) FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_part,'" + _BarCode + "')";

                }
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }

            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage.ToString() + ";";
                query = query + "SET @perpage =" + _CountPerPage.ToString() + ";";
                query = query + "SELECT * FROM";
                if (_opxx == "All")
                {
                    query = query + "(";
                    for (int i = 0; i < opxxSelect.Count() - 1; i++)
                    {
                        query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + opxxSelect[i].Value + "`.* FROM `" + _LineCode + opxxSelect[i].Value + "` WHERE INSTR(barcode_part,'" + _BarCode + "')";
                        if (i < opxxSelect.Count() - 2) { query = query + " UNION ALL "; }
                    }
                    query = query + ") a ";
                }
                else
                {
                    query = query + "(SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + _opxx + "`.* FROM `" + _LineCode + _opxx + "` WHERE INSTR(barcode_part, '" + _BarCode + "') ORDER BY ID) a "; 
                }
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult fcjQueryQC(SelectCheckModels SCM)
        {
            string _LineCode = "fcj_";
            string _StartDateTime = SCM.StartDateTime;
            string _EndDateTime = SCM.EndDateTime;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode !=null)
            {
                querycount = " SELECT COUNT(*) FROM `" + _LineCode + "qc_pass` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                querycount = " SELECT COUNT(*) FROM `" + _LineCode + "qc_pass` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }

            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage + ";";
                query = query + "SET @perpage =" + _CountPerPage + ";";
                query = query + "SELECT * FROM";
                query = query + "(";
                query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + "qc_pass`.* FROM `" + _LineCode + "qc_pass` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
                query = query + ") a ";
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage + ";";
                query = query + "SET @perpage =" + _CountPerPage + ";";
                query = query + "SELECT * FROM";
                query = query + "(";
                query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + "qc_pass`.* FROM `" + _LineCode + "qc_pass` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                query = query + ") a ";
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult hqQueryQC(SelectCheckModels SCM)
        {
            string _LineCode = "hq_";
            string _StartDateTime = SCM.StartDateTime;
            string _EndDateTime = SCM.EndDateTime;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                querycount = " SELECT COUNT(*) FROM `" + _LineCode + "qc_pass` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                querycount = " SELECT COUNT(*) FROM `" + _LineCode + "qc_pass` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }

            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage + ";";
                query = query + "SET @perpage =" + _CountPerPage + ";";
                query = query + "SELECT * FROM";
                query = query + "(";
                query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + "qc_pass`.* FROM `" + _LineCode + "qc_pass` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
                query = query + ") a ";
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage + ";";
                query = query + "SET @perpage =" + _CountPerPage + ";";
                query = query + "SELECT * FROM";
                query = query + "(";
                query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + "qc_pass`.* FROM `" + _LineCode + "qc_pass` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                query = query + ") a ";
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult qxQueryQC(SelectCheckModels SCM)
        {
            string _LineCode = "qx_";
            string _StartDateTime = SCM.StartDateTime;
            string _EndDateTime = SCM.EndDateTime;
            string _BarCode = SCM.BarCode;
            int _CountPerPage = SCM.CountPerPage;
            int _CurrentPage = SCM.CurrentPage;

            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            var querycount = "";
            if (_BarCode != null)
            {
                querycount = " SELECT COUNT(*) FROM `" + _LineCode + "qc_pass` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                querycount = " SELECT COUNT(*) FROM `" + _LineCode + "qc_pass` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
            }
            if (querycount != "")
            {
                ViewBag.TotleCount = (sql.ExecuteDataTable(querycount)).Rows[0][0].ToString();
            }

            var query = "";
            if (_BarCode != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage + ";";
                query = query + "SET @perpage =" + _CountPerPage + ";";
                query = query + "SELECT * FROM";
                query = query + "(";
                query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + "qc_pass`.* FROM `" + _LineCode + "qc_pass` WHERE INSTR(barcode_zc,'" + _BarCode + "')";
                query = query + ") a ";
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            else if (_StartDateTime != null && _EndDateTime != null)
            {
                query = "SET @rownum = 0;";
                query = query + "SET @page = " + _CurrentPage + ";";
                query = query + "SET @perpage =" + _CountPerPage + ";";
                query = query + "SELECT * FROM";
                query = query + "(";
                query = query + "SELECT @rownum:= @rownum + 1 AS rownum,`" + _LineCode + "qc_pass`.* FROM `" + _LineCode + "qc_pass` WHERE DATETIME>'" + _StartDateTime + "' AND  DATETIME<='" + _EndDateTime + "'";
                query = query + ") a ";
                query = query + " WHERE a.rownum > (@page - 1) * @perpage AND a.rownum <= @page * @perpage;";
            }
            if (query != "")
            {
                ViewBag.dt = sql.ExecuteDataTable(query);
            }
            else
            {
                ViewBag.dt = new DataTable();
            }

            return View();
        }

        // GET: SelectCheck
        public ActionResult fcjQueryChart(SelectCheckModels SCM)
        {
            return View();
        }

        // GET: SelectCheck
        public ActionResult hqQueryChart(SelectCheckModels SCM)
        {
            return View();
        }

        // GET: SelectCheck
        public ActionResult qxQueryChart(SelectCheckModels SCM)
        {
            return View();
        }


        /// <summary>
        ///下載PDF檔案
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadPdf(string BarcodeStr,string Area)
        {
            byte[] pdfFile = this.ConvertHtmlTextToPDF(BarcodeStr,Area);
            return new BinaryContentResult(pdfFile, "application/pdf");
           
        }

        /// <summary>
        /// 將Html文字 輸出到PDF檔裡
        /// </summary>
        /// <param name="BarcodeStr"></param>
        /// <returns></returns>
        public byte[] ConvertHtmlTextToPDF(string BarcodeStr, string Area)
        {
            if (string.IsNullOrEmpty(BarcodeStr))
            {
                return null;
            }

            MemoryStream outputStream = new MemoryStream(); //要把PDF寫到哪個串流
            byte[] data = Encoding.UTF8.GetBytes(BarcodeStr); //字串轉成byte[]
            MemoryStream msInput = new MemoryStream(data);

            Document doc = new Document(); //要寫PDF的文件，建構子沒填的話預設直式A4
            PdfWriter writer = PdfWriter.GetInstance(doc, outputStream);
            //指定文件預設開檔時的縮放為100%
            PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
            //開啟Document文件 
            doc.Open();

            Font font = new Font(baseFont, 15);
            var table1 = new PdfPTable(12);     //创建表格实例4列
            int[] a = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };        //设置列宽比例
            table1.SetWidths(a);
            table1.TotalWidth = 500;
            table1.LockedWidth = true;
            //电子流程卡标题
            Paragraph para = new Paragraph("电子流程卡查询", font);
            PdfPCell cell = new PdfPCell(para);
            cell.MinimumHeight = 30;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Colspan = 12;
            cell.BackgroundColor = BaseColor.GRAY;
            table1.AddCell(cell);
          
            SqlHelper sql = new SqlHelper(System.Configuration.ConfigurationManager.ConnectionStrings["MySqlConnString"].ConnectionString);
            MySqlParameter[] parameters = new MySqlParameter[2] {
                                            new MySqlParameter("Barcode", MySqlDbType.VarChar),
                                            new MySqlParameter("Line", MySqlDbType.VarChar), };
            parameters[0].Value = BarcodeStr;
            parameters[1].Value = Area;
            DataTable dt = sql.ExecuteDataTable("SP_AssemblyInfo", CommandType.StoredProcedure, parameters);
            if (dt.Rows.Count > 0)
            {
                #region 总成信息
                string figure, zc, sn, code, car, group;

                figure = dt.Rows[0]["figure_no"].ToString();
                car = dt.Rows[0]["cartype"].ToString();
                char[] ch = new char[1] { '&' };
                string[] bar = BarcodeStr.Split(ch);
                sn = bar[0];
                code = bar[1];
                group = car + dt.Rows[0]["production_name"].ToString();
                zc = group + figure.Substring(figure.Count() - 4);

                //总成信息标题
                font = new Font(baseFont, 8);
                para = new Paragraph("总成信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 15;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                para = new Paragraph("图号/零件号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(figure, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 6;
                table1.AddCell(cell);

                para = new Paragraph("物料编号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("/", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 3;
                table1.AddCell(cell);

                para = new Paragraph("总成名称", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(zc, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 7;
                table1.AddCell(cell);

                para = new Paragraph("质量状态", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("正常", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("SN", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(sn, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 10;
                table1.AddCell(cell);

                para = new Paragraph("出厂编号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(code, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 7;
                table1.AddCell(cell);

                para = new Paragraph("物料系列", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph(car, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("当前群组", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph(group, font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 7;
                table1.AddCell(cell);

                para = new Paragraph("生产方式", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("正常", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);
                #endregion

                #region 扫描信息
                font = new Font(baseFont, 8);
                para = new Paragraph("扫描信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                para = new Paragraph("序号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("时间", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("扫描点", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 5;
                table1.AddCell(cell);

                para = new Paragraph("员工", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("质量", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("生产状态", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                dt = sql.ExecuteDataTable("SP_Sannerinfo", CommandType.StoredProcedure, parameters);

                for (int index = 0; index < dt.Rows.Count; index++)
                {
                    para = new Paragraph((index + 1).ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["DateTime"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["station"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 5;
                    table1.AddCell(cell);

                    para = new Paragraph("/", font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    string quality = "";
                    if (dt.Rows[index]["scan_status"].ToString() == "1" || dt.Rows[index]["scan_status"].ToString() == "OK") quality = "正常"; else quality = "异常";

                    para = new Paragraph(quality, font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph("正常", font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    table1.AddCell(cell);
                }
                #endregion

                #region 装配信息
                //扫描信息标题
                font = new Font(baseFont, 8);
                para = new Paragraph("装配信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                para = new Paragraph("序号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("应装部件", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("实装部件", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("部件追溯码", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("追溯", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("差异件", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("拆散", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("装配时间", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                dt = sql.ExecuteDataTable("SP_PartsInfo", CommandType.StoredProcedure, parameters);

                for (int index = 0; index < dt.Rows.Count; index++)
                {
                    para = new Paragraph((index + 1).ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    string partkey = dt.Rows[index]["part_key"].ToString();
                    string pk = "";
                    if (partkey.Count() > 4) pk = partkey.Substring(partkey.Count() - 4); else pk = partkey;

                    para = new Paragraph(dt.Rows[index]["cartype"].ToString() + dt.Rows[index]["order_part_name"].ToString() + pk, font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["cartype"].ToString() + dt.Rows[index]["act_part_name"].ToString() + pk, font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["barcode_part"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    table1.AddCell(cell);

                    string infostatus = "";
                    if (dt.Rows[index]["part_sign"].ToString() == "1") { infostatus = "精追"; }
                    else if (dt.Rows[index]["part_sign"].ToString() == "2") { infostatus = "批追"; }
                    else { infostatus = "无"; }

                    para = new Paragraph(infostatus, font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph("否", font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph("无", font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["datetime"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    table1.AddCell(cell);
                }
                #endregion

                #region 制造参数信息
                //制造参数信息
                font = new Font(baseFont, 8);
                para = new Paragraph("制造参数信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);

                para = new Paragraph("序号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("应采集制造参数", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("编号", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("实采集制造参数", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                para = new Paragraph("拧紧值", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("角度值", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("结果", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("拆散", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table1.AddCell(cell);

                para = new Paragraph("时间", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 17;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 2;
                table1.AddCell(cell);

                dt = sql.ExecuteDataTable("SP_ManufacturerInfo", CommandType.StoredProcedure, parameters);

                for (int index = 0; index < dt.Rows.Count; index++)
                {
                    para = new Paragraph((index + 1).ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["cartype"].ToString() + dt.Rows[index]["order_nut_name"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["code_no"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["cartype"].ToString() + dt.Rows[index]["act_nut_name"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["torque"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["angle"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    string ok_nok = "";
                    if (dt.Rows[index]["ok_nok"].ToString() == "1") { ok_nok = "OK"; } else { ok_nok = "NOK"; }

                    para = new Paragraph(ok_nok, font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph("/", font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    table1.AddCell(cell);

                    para = new Paragraph(dt.Rows[index]["datetime"].ToString(), font);
                    cell = new PdfPCell(para);
                    cell.MinimumHeight = 17;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    cell.Colspan = 2;
                    table1.AddCell(cell);
                }
                #endregion
            }
            else
            {
                font = new Font(baseFont, 8);
                para = new Paragraph("未查询到该信息", font);
                cell = new PdfPCell(para);
                cell.MinimumHeight = 30;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 12;
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                table1.AddCell(cell);
            }
            doc.Add(table1);

            doc.Close();
            msInput.Close();
            outputStream.Close();
            //回傳PDF檔案 
            return outputStream.ToArray();

        }

        public class SqlHelper
        {
            public string SQLName = "MySql";
            private string connectionString;

            /// <summary>  
            /// 设定数据库访问字符串  
            /// </summary>  
            public string ConnectionString
            {
                set { connectionString = value; }
            }

            /// <summary>  
            /// 构造函数  
            /// </summary>  
            /// <param name="connectionString">数据库访问字符串</param>  
            public SqlHelper(string connectionString)
            {
                this.connectionString = connectionString;
            }

            /// <summary>
            /// 判断数据库是否可以联接
            /// </summary>
            /// <returns>返回联接结果</returns>
            public bool isConnectted()
            {
                MySqlConnection mySqlConnection = new MySqlConnection(connectionString);
                try
                {
                    mySqlConnection.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    mySqlConnection.Close();
                }
            }

            /// <summary>  
            /// 执行一个查询，并返回查询结果  
            /// </summary>  
            /// <param name="sql">要执行的sql语句</param>  
            /// <param name="commandType">要执行的查询语句的类型，如存储过程或者sql文本命令</param>  
            /// <returns>返回查询结果集</returns>  
            public DataTable ExecuteDataTable(string sql, CommandType commandType)
            {
                return ExecuteDataTable(sql, commandType, null);
            }

            /// <summary>  
            /// 执行一个查询，并返回结果集  
            /// </summary>  
            /// <param name="sql">要执行的sql文本命令</param>  
            /// <returns>返回查询的结果集</returns>  
            public DataTable ExecuteDataTable(string sql)
            {
                return ExecuteDataTable(sql, CommandType.Text, null);
            }


            /// <summary>  
            /// 执行一个查询，并返回查询结果  
            /// </summary>  
            /// <param name="sql">要执行的sql语句</param>  
            /// <param name="commandtype">要执行查询语句的类型，如存储过程或者sql文本命令</param>  
            /// <param name="parameters">Transact-SQL语句或者存储过程参数数组</param>  
            /// <returns></returns>  
            public DataTable ExecuteDataTable(string sql, CommandType commandtype, MySqlParameter[] parameters)
            {
                DataTable data = new DataTable(); //实例化datatable，用于装载查询结果集  
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.CommandType = commandtype;//设置command的commandType为指定的Commandtype  
                                                      //如果同时传入了参数，则添加这些参数  
                        if (parameters != null)
                        {
                            foreach (MySqlParameter parameter in parameters)
                            {
                                cmd.Parameters.Add(parameter);
                            }
                        }

                        //通过包含查询sql的sqlcommand实例来实例化sqldataadapter  
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(data);//填充datatable  

                    }
                }
                return data;
            }

            /// <summary>  
            /// 返回一个SqlDataReader对象的实例  
            /// </summary>  
            /// <param name="sql">要执行的SQl查询命令</param>  
            /// <returns></returns>  
            public MySqlDataReader ExecuteReader(string sql)
            {
                return ExecuteReader(sql, CommandType.Text, null);
            }

            /// <summary>  
            ///   
            /// </summary>  
            /// <param name="sql">要执行的sql语句</param>  
            /// <param name="commandType">要执行查询语句的类型，如存储过程或者SQl文本命令</param>  
            /// <returns></returns>  
            public MySqlDataReader ExecuteReader(string sql, CommandType commandType)
            {
                return ExecuteReader(sql, commandType, null);
            }

            /// <summary>  
            /// 返回一个sqldatareader对象的实例  
            /// </summary>  
            /// <param name="sql"></param>  
            /// <param name="commandType"></param>  
            /// <param name="parameters"></param>  
            /// <returns></returns>  
            public MySqlDataReader ExecuteReader(string sql, CommandType commandType, MySqlParameter[] parameters)
            {
                MySqlConnection con = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(sql, con);

                if (parameters != null)
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameters);
                    }
                }
                con.Open();
                //CommandBehavior.CloseConnection参数指示关闭reader对象时关闭与其关联的Connection对象  
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }

            /// <summary>  
            /// 执行一个查询，返回结果集的首行首列。忽略其他行，其他列  
            /// </summary>  
            /// <param name="sql">要执行的SQl命令</param>  
            /// <returns></returns>  
            public Object ExecuteScalar(string sql)
            {
                return ExecuteScalar(sql, CommandType.Text, null);
            }

            /// <summary>  
            ///   
            /// </summary>  
            /// <param name="sql"></param>  
            /// <param name="commandType"></param>  
            /// <returns></returns>  
            public Object ExecuteScalar(string sql, CommandType commandType)
            {
                return ExecuteScalar(sql, commandType, null);
            }


            /// <summary>  
            ///   
            /// </summary>  
            /// <param name="sql"></param>  
            /// <param name="commandType">参数类型</param>  
            /// <param name="parameters"></param>  
            /// <returns></returns>  
            public Object ExecuteScalar(string sql, CommandType commandType, MySqlParameter[] parameters)
            {
                Object result = null;
                MySqlConnection con = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (MySqlParameter parapmeter in parameters)
                    {
                        cmd.Parameters.Add(parapmeter);
                    }
                }

                con.Open();
                result = cmd.ExecuteScalar();
                con.Close();
                return result;
            }

            /// <summary>  
            /// 对数据库进行增删改的操作  
            /// </summary>  
            /// <param name="sql">要执行的sql命令</param>  
            /// <returns></returns>  
            public int ExecuteNonQuery(string sql)
            {
                return ExecuteNonQuery(sql, CommandType.Text, null);
            }

            /// <summary>  
            /// 数据库进行增删改的操作  
            /// </summary>  
            /// <param name="sql">对数据库进行操作的sql命令</param>  
            /// <param name="commandType">要执行查询语句的类型，如存储过程或者sql文本命令</param>  
            /// <returns></returns>  
            public int ExecuteNonQuery(string sql, CommandType commandType)
            {
                return ExecuteNonQuery(sql, commandType, null);
            }

            /// <summary>  
            /// 对数据库进行增删改的操作  
            /// </summary>  
            /// <param name="sql">要执行的sql语句</param>  
            /// <param name="commandType">要执行的查询语句类型，如存储过程或者sql文本命令</param>  
            /// <param name="parameters">Transact-SQL语句或者存储过程的参数数组</param>  
            /// <returns></returns>  
            public int ExecuteNonQuery(string sql, CommandType commandType, MySqlParameter[] parameters)
            {
                int count = 0;
                MySqlConnection con = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(sql, con);
                cmd.CommandType = commandType;
                if (parameters != null)
                {
                    foreach (MySqlParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }

                con.Open();
                count = cmd.ExecuteNonQuery();
                con.Close();
                return count;
            }

            /// <summary>  
            /// 返回当前连接的数据库中所有用户创建的数据库  
            /// </summary>  
            /// <returns></returns>  
            public DataTable GetTables()
            {
                DataTable table = null;
                using (MySqlConnection con = new MySqlConnection(connectionString))
                {
                    con.Open();
                    table = con.GetSchema("Tables");

                }
                return table;
            }
        }
    }








}