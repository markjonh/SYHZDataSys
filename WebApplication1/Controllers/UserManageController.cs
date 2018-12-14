using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zhongyu.Data.Extensions;
using Zhongyu.Data;
using WebApplication1.Model.Dto;
using static WebApplication1.Models.DapperService;
using WebApplication1.Model;

namespace WebApplication1.Controllers
{
    public class UserManageController : Controller
    {
        // GET: UserManage
        public ActionResult Index(string name)
        {
            List<UsersDto> modes = new List<UsersDto>();
            List<string> userid = new List<string>();
            List<string> roleid = new List<string>();

            ViewBag.name = name;
            var query = SqlHelp.GetUsers();
            if (!name.IsNullOrEmpty())
            {
                query = SqlHelp.SearchUser(name);
            }
            //query.ForEach(p => { p.Roles = (from role in SqlHelp.GetRoles()
            //        join userroles in SqlHelp.GetUserRoles() on role.Id equals userroles.RoleId
            //        join users in query on userroles.UserId equals users.Id
            //        select new RolesDto
            //        {

            //            Name = role.Name,
            //            Id = role.Id

            //        }).ToList();
            //});
            SqlHelp.GetUserRoles().ForEach(
                p1 =>
                {
                    userid.Add(p1.UserId);
                });
            userid.Distinct();
            query.ForEach(p =>
                {
                    List<RolesDto> roles = new List<RolesDto>();
                    if (userid.Contains(p.Id))
                    {
                        //把获取这个用户下的所有角色id
                        roleid.Clear();
                        foreach (var userRolese in SqlHelp.GetUserRoles().Where(p3 => p3.UserId.Equals(p.Id)))
                        {
                            roleid.Add(userRolese.RoleId);
                            //roleid
                        }
                        //roles.Clear();
                        roleid.ForEach(p4 =>
                            {
                                roles.Add(SqlHelp.GetRoles().Where(p5 => p5.Id.Equals(p4)).FirstOrDefault());
                            }

                        );
                        p.Roles = roles;
                    }

                }

            );

            return View(query);
        }
        //新增
        public ActionResult Create()
        {
            ViewBag.Role = SqlHelp.GetRoles();
            return View();
        }

        [HttpPost]
        public JsonResult Create(UsersDto item, FormCollection coll)
        {
            OperationResult r = new OperationResult();
            try
            {
                List<UsersDto> list = SqlHelp.GetUsers();
                if (list.Exists(p => p.LoginName.Equals(item.LoginName)))
                {
                    r.ResultType = OperationResultType.ValidError;
                    r.Message = "该登录名已存在";
                }
                else
                {
                    string role = coll["role"] == null ? "" : coll["role"].ToString();
                    item.Password = Zhongyu.Common.EncryptHelper.MD5Encrypt(item.Password, 32);
                    item.Id = Guid.NewGuid().ToString();
                    SqlHelp.InsertUsers(item, role);
                    r.ResultType = OperationResultType.Success;
                }
            }
            catch (Exception ex)
            {
                r.Message = "新增失败，请检查所填数据是否正确。";
                r.ResultType = OperationResultType.Error;
            }

            return Json(r);
        }


        public ActionResult Edit(string id)
        {
            ViewBag.Role = SqlHelp.GetRoles();
            UsersDto item = SqlHelp.GetUserById(id);
            List<string> userid = new List<string>();
            List<string> roleid = new List<string>();
            SqlHelp.GetUserRoles().ForEach(
                p1 =>
                {
                    userid.Add(p1.UserId);
                });
            List<RolesDto> roles = new List<RolesDto>();
            if (userid.Contains(item.Id))
            {
                //把获取这个用户下的所有角色id
                roleid.Clear();
                foreach (var userRolese in SqlHelp.GetUserRoles().Where(p3 => p3.UserId.Equals(item.Id)))
                {
                    roleid.Add(userRolese.RoleId);
                    //roleid
                }

                //roles.Clear();
                roleid.ForEach(p4 => { roles.Add(SqlHelp.GetRoles().Where(p5 => p5.Id.Equals(p4)).FirstOrDefault()); }

                );
            }
            item.Roles = roles;
            return View(item);
        }

        [HttpPost]
        public JsonResult Edit(UsersDto item, FormCollection coll)
        {
            OperationResult r = new OperationResult();
            try
            {
                if (SqlHelp.GetUsers().Exists(p => !p.Id.Equals(item.Id) && p.LoginName.Equals(item.LoginName)))
                {
                    r.ResultType = OperationResultType.ValidError;
                    r.Message = "该登录名已存在";
                }
                else
                {
                    string role = coll["role"] == null ? "" : coll["role"].ToString();
                    if (!item.Password.IsNullOrWhiteSpace())
                    {
                        item.Password = Zhongyu.Common.EncryptHelper.MD5Encrypt(item.Password, 32);
                    }
                    if (!item.Password.IsNullOrWhiteSpace())
                    {
                        SqlHelp.UpdateUserById(item, role);
                    }
                    else
                    {
                        SqlHelp.UpdateUserNoPasswordById(item, role);
                    }
                    r.ResultType = OperationResultType.Success;
                }

            }
            catch (Exception ex)
            {
                r.Message = "失败，请检查数据。";
                r.ResultType = OperationResultType.Error;
            }

            return Json(r);
        }

        public JsonResult Delete(string id)
        {
            OperationResult r = new OperationResult();
            try
            {
                int i = SqlHelp.DeleteUserById(id);
                r.ResultType = OperationResultType.Success;
            }
            catch (Exception ex)
            {
                r.Message = "失败，请检查数据。";
                r.ResultType = OperationResultType.Error;
            }  
            return Json(r);
        }

    }
}