using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using WebApplication1.Common;
using WebApplication1.Model;
using WebApplication1.Model.Dto;
using Zhongyu.Data.Extensions;

namespace WebApplication1.Models
{
    public class DapperService
    {
        public static MySqlConnection MySqlConnection()
        {
            string mysqlConnectionStr = ConfigurationManager.ConnectionStrings["MySqlConnString"].ToString();
            var connection = new MySqlConnection(mysqlConnectionStr);
            new MySqlCommand().CommandTimeout = 0;
            connection.Open();
            return connection;
        }

        public static class SqlHelp
        {
            #region 用户管理

            //获取所有用户
            public static List<UsersDto> GetUsers()
            {
                List<UsersDto> user;
                using (IDbConnection conn = MySqlConnection())
                {
                    user = conn.Query<UsersDto>("select * from sysuser", commandType: CommandType.Text,
                        commandTimeout: 0).ToList();
                }

                return user;
            }

            //获取所有角色
            public static List<RolesDto> GetRoles()
            {
                List<RolesDto> role;
                using (IDbConnection conn = MySqlConnection())
                {
                    role = conn.Query<RolesDto>("select * from role", commandType: CommandType.Text, commandTimeout: 0)
                        .ToList();
                }

                return role;
            }

            //
            public static List<UserRoles> GetUserRoles()
            {
                List<UserRoles> userroles;
                using (IDbConnection conn = MySqlConnection())
                {
                    userroles = conn.Query<UserRoles>("select * from userrole", commandType: CommandType.Text,
                        commandTimeout: 0).ToList();
                }

                return userroles;
            }

            //新增用户
            public static int InsertUsers(UsersDto model, string role)
            {
                using (IDbConnection conn = MySqlConnection())
                {
                    int result = 0;
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        result = conn.Execute(
                            "Insert into sysuser values (@Id,@LoginName,@Password,@Name,@Phone,@Enable)", model);
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            foreach (var rid in role.Split(','))
                            {
                                if (!string.IsNullOrWhiteSpace(rid))
                                {
                                    conn.Execute("Insert into userrole values (@UserId,@RoleId)",
                                        new UserRoles { RoleId = rid, UserId = model.Id });
                                    //RoleUser.Insert(});
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }

                    return result;
                }
            }

            //用户搜索
            public static List<UsersDto> SearchUser(string username)
            {
                List<UsersDto> user;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@Name", "%" + username + "%");
                    user = conn.Query<UsersDto>("select* from sysuser where Name like @Name", p,
                        commandType: CommandType.Text, commandTimeout: 0).ToList();

                }

                return user;
            }

            //获取一个用户
            public static UsersDto GetUserById(string Id)
            {
                UsersDto user;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@Id", Id);
                    user = conn.Query<UsersDto>("select* from sysuser where Id=@Id", p, commandType: CommandType.Text,
                        commandTimeout: 0).FirstOrDefault();
                }

                return user;
            }

            //更新用户
            public static int UpdateUserById(UsersDto user, string role)
            {
                // UsersDto users;
                using (IDbConnection conn = MySqlConnection())
                {
                    int result = 0;
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        result = conn.Execute(
                            "update sysuser set Id=@Id,LoginName=@LoginName,Password=@Password,Name=@Name,Phone=@Phone,Enable=@Enable where Id=@Id",
                            user);
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            var p = new DynamicParameters();
                            p.Add("@Id", user.Id);
                            conn.Execute("delete from userrole where UserId=@Id", p);
                            foreach (var rid in role.Split(','))
                            {
                                if (!string.IsNullOrWhiteSpace(rid))
                                {
                                    conn.Execute("Insert into userrole values (@UserId,@RoleId)",
                                        new UserRoles { RoleId = rid, UserId = user.Id });

                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }

                    return result;



                }
            }

            //删除用户
            public static int DeleteUserById(string userid)
            {
                // UsersDto users;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@Id", userid);
                    return conn.Execute("delete from userrole where UserId=@Id", p) +
                           conn.Execute("delete from sysuser where Id=@Id", p);
                }
            }

            //不修改密码
            public static int UpdateUserNoPasswordById(UsersDto user, string role)
            {

                // UsersDto users;
                using (IDbConnection conn = MySqlConnection())
                {
                    int result = 0;
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        result = conn.Execute(
                            "update sysuser set Id=@Id,LoginName=@LoginName,Name=@Name,Phone=@Phone,Enable=@Enable where Id=@Id",
                            user);
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            var p = new DynamicParameters();
                            p.Add("@Id", user.Id);
                            conn.Execute("delete from userrole where UserId=@Id", p);
                            foreach (var rid in role.Split(','))
                            {
                                if (!string.IsNullOrWhiteSpace(rid))
                                {
                                    conn.Execute("Insert into userrole values (@UserId,@RoleId)",
                                        new UserRoles { RoleId = rid, UserId = user.Id });

                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }

                    return result;
                }

            }

            #endregion

            /// <summary>
            /// 获取登陆用户
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static LoginUser UserLogin(string username, string password)
            {
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@UserName", username);
                    p.Add("@PassWord", password);
                    return conn.Query<LoginUser>(
                        "select * from sysuser where LoginName=@UserName and Password=@PassWord",
                        p, commandType: CommandType.Text, commandTimeout: 0).ToList().FirstOrDefault();
                }
            }

            /// <summary>
            /// 总成查询
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<AssembleSearchDto> AssemblySearch(AssembleSearchModel model)
            {

                List<AssembleSearchDto> list;
                using (IDbConnection conn = MySqlConnection())
                {


                    var p = new DynamicParameters();

                    p.Add("@_DateTimeStart", model.StarTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Area);
                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list = conn.Query<AssembleSearchDto>("SP_Data_All_Barcode_QueryByDate", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                }

                return list;
            }

            /// <summary>
            /// 零件基础数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<PartDataDto> PartDataBase(PartDataModel model)
            {

                List<PartDataDto> list;
                using (IDbConnection conn = MySqlConnection())
                {


                    var p = new DynamicParameters();

                    p.Add("@_DateTimeStart", model.StartTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Line);
                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list = conn.Query<PartDataDto>("SP_DataPart_ExportByDate", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                }

                return list;
            }

            //扫描量
            public static List<ResScnRateDto> ScanRate(PartDataModel model)
            {
                List<ResScnRateDto> listend = new List<ResScnRateDto>();
                ResScanRateDto list = new ResScanRateDto();
                if (model.EndTime.Month - model.StartTime.Month > 0)
                {
                    CommonHelp.StarMon = model.StartTime.ToString("yyyy MMMM dd") + "~" +
                                         model.EndTime.ToString("yyyy MMMM dd");
                    CommonHelp.EndMon = CommonHelp.list.Where(p => p.Area.Equals(model.Line)).FirstOrDefault()
                        .DesCribe;
                }
                else
                {
                    CommonHelp.StarMon = model.StartTime.Month + "月";
                    CommonHelp.EndMon = CommonHelp.list.Where(p => p.Area.Equals(model.Line)).FirstOrDefault()
                        .DesCribe;
                }

                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StartTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Line);
                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list.assemlist = conn.Query<ResAssemblyRateDto>("SP_ScanerRatezc", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list.partlist = conn.Query<ResPartDto>("SP_ScanerRatepart", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    var tt = from left in list.assemlist
                             join right in list.partlist
                                 on left.Figure_No_up equals right.Figure_No
                             select new ResScnRateDto
                             {
                                 Figure_No_up = left.Figure_No_up,
                                 sum_up = left.sum_up,
                                 upstation = left.upstation,
                                 Figure_No_down = left.Figure_No_down,
                                 sum_down = left.sum_down,
                                 DOWNSTATION = left.DOWNSTATION,
                                 DOWNRATE = Convert.ToString(left.DOWNRATE) + "%",
                                 cartype = right.cartype,
                                 Partname = right.Partname,
                                 Part_Sum = right.Part_Sum,
                                 PartFigureNo = right.PartFigureNo
                             };
                    listend = tt.ToList();

                    foreach (var itmes in listend)
                    {
                        itmes.Rate = Convert.ToString(itmes.sum_up / itmes.Part_Sum * 100) + "%";
                    }


                }

                return listend;
            }

            /// <summary>
            /// 直通率基础数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ThroughRateDto> ThroughRateData(ThroughRateModel model)
            {
                List<ThroughRateDto> list;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StartTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Line);
                    list = conn.Query<ThroughRateDto>("SP_FirstPassYied", p, commandType: CommandType.StoredProcedure,
                        commandTimeout: 0).ToList();
                }

                return list;
            }

            // 需求与押车
            public static ResNeedDeleyListDto NeedDeley(ThroughRateModel model)
            {

                ResNeedDeleyListDto list = new ResNeedDeleyListDto();
                using (IDbConnection conn = MySqlConnection())
                {

                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StartTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Line);

                    list.ListNeed = conn.Query<ResNeedDeleyDto>("SP_RequireSum", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list.ListDeley = conn.Query<ResNeedDeleyDto>("SP_YaCheSum", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();

                }

                return list;
            }

            /// <summary>
            /// 总成模糊查询
            /// </summary>
            /// <param name="code"></param>
            /// <param name="area"></param>
            /// <returns></returns>
            public static List<AssembleSearchDto> DarkSearch(string code, string area)
            {

                List<AssembleSearchDto> list;
                using (IDbConnection conn = MySqlConnection())
                {


                    var p = new DynamicParameters();

                    p.Add("@_barcode_zc", code);
                    p.Add("@_Area", area);

                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list = conn.Query<AssembleSearchDto>("SP_Data_All_Barcode_QueryByBarcodeZc", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                }

                return list;
            }

            //精确查询
            public static List<AssembleSearchDto> DataSourceSearch(string code, string area)
            {

                List<AssembleSearchDto> list;
                using (IDbConnection conn = MySqlConnection())
                {


                    var p = new DynamicParameters();

                    p.Add("@_CSN", code);
                    p.Add("@_Area", area);

                    // var list1= conn.Query("SP_Data_All_Barcode_QueryByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                    list = conn.Query<AssembleSearchDto>("SP_Data_All_Barcode_QueryByCSN", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 0).ToList();
                }

                return list;
            }

            /// <summary>
            /// QC查询
            /// </summary>
            /// <param name="Area"></param>
            /// <returns></returns>
            public static List<QCSelectDto> QCSelect(string Area)
            {
                List<QCSelectDto> list = null;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_LINE", Area);
                    try
                    {
                        list = conn.Query<QCSelectDto>("SP_QC_PASS", p, commandType: CommandType.StoredProcedure,
                            commandTimeout: 0).ToList();
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return list;

            }

            /// <summary>
            /// 根据时间段获取部件原始数据
            /// </summary>
            public static List<ByDataDto> PartByData(ByDataModel model)
            {

                List<ByDataDto> list;
                using (IDbConnection conn = MySqlConnection())
                {
                    // TimeZone.CurrentTimeZone.ToLocalTime(model.StartDateTime);
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StartDateTime);
                    p.Add("@_DateTimeEnd", model.EndDateTime);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);
                    list = conn.Query<ByDataDto>("SP_DataPart_QueryByDate", p, commandType: CommandType.StoredProcedure)
                        .ToList();
                }

                return list;
            }

            /// <summary>
            /// 获得部件excel表原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<PartToExcelDto> PartyByDataToExcel(AssembleSearchModel model)
            {

                List<PartToExcelDto> list;
                using (IDbConnection conn = MySqlConnection())
                {
                    // model.EndTime = model.StarTime.AddDays(1);
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StarTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Area);
                    //var list2= conn.Query("SP_DataPart_ExportByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 240).ToList();
                    list = conn.Query<PartToExcelDto>("SP_DataPart_ExportByDate1", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 240).ToList();
                }

                return list;
            }

            /// <summary>
            /// 获得扭矩excel表原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<TorqToExcelDto> TorqByDataToExcel(AssembleSearchModel model)
            {
                List<TorqToExcelDto> list;
                using (IDbConnection conn = MySqlConnection())
                {

                    //model.EndTime = model.StarTime.AddDays(1);
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StarTime);
                    p.Add("@_DateTimeEnd", model.EndTime);
                    p.Add("@_Area", model.Area);
                    //var list6= conn.Query("SP_DataTorq_ExportByDate", p, commandType: CommandType.StoredProcedure, commandTimeout: 240).ToList();
                    list = conn.Query<TorqToExcelDto>("SP_DataTorq_ExportByDate", p,
                        commandType: CommandType.StoredProcedure, commandTimeout: 240).ToList();
                }

                return list;
            }

            /// <summary>
            /// 根据条码查询部件原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ByCodeDto> PartByCode(ByCodeModel model)
            {
                List<ByCodeDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_Barcode_zc", model.BarCode);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);
                    dataTable = conn.Query<ByCodeDto>("SP_DataPart_QueryByBarcodeZC", p,
                        commandType: CommandType.StoredProcedure).ToList();
                }

                return dataTable;
            }

            public static List<PartDetailToExcelDto> PartToExcel(ByCodeModel model)
            {
                List<PartDetailToExcelDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_Barcode_zc", model.BarCode);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);
                    dataTable = conn.Query<PartDetailToExcelDto>("SP_DataPart_QueryByBarcodeZC", p,
                        commandType: CommandType.StoredProcedure).ToList();
                }

                return dataTable;
            }

            /// <summary>
            /// 根据时间段查询扭矩原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ByDataDto> TorqByData(ByDataModel model)
            {
                List<ByDataDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_DateTimeStart", model.StartDateTime);
                    p.Add("@_DateTimeEnd", model.EndDateTime);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);

                    dataTable = conn
                        .Query<ByDataDto>("SP_DataTorq_QueryByDate", p, commandType: CommandType.StoredProcedure)
                        .ToList();


                }

                return dataTable;
            }

            /// <summary>
            /// 根据部件编号得到总成信息原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ResAssemblyBarCode> GetAssemblyByPartCode(string partcode, string area)
            {
                List<ResAssemblyBarCode> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_barcode_part", partcode);
                    p.Add("@_Area", area);
                    //  var tt= conn.Query<ResAssemblyBarCode>("SP_PART_ZC", p, commandType: CommandType.StoredProcedure).ToList();
                    //  var tt= conn.Query("SP_PART_ZC", p, commandType: CommandType.StoredProcedure).ToList();
                    dataTable = conn.Query<ResAssemblyBarCode>("SP_Data_All_Barcode_QueryByBarcodePart", p,
                        commandType: CommandType.StoredProcedure).ToList();
                }

                return dataTable;
            }

            /// <summary>
            /// 根据条码查询扭矩原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ByCodeDto> TorqByCode(ByCodeModel model)
            {

                List<ByCodeDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_Barcode_zc", model.BarCode);
                    p.Add("@_Station", model.Station);
                    p.Add("@_Area", model.Area);
                    dataTable = conn.Query<ByCodeDto>("SP_DataTorq_QueryByBarcodeZC", p,
                        commandType: CommandType.StoredProcedure).ToList();
                }

                return dataTable;
            }

            /// <summary>
            /// 节拍查询
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<BeatDto> BeatChart(ChartModel model)
            {

                List<BeatDto> dataTable;
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@_DateTime", model.DateTime);
                    p.Add("@_Area", model.Area);
                    dataTable = conn.Query<BeatDto>("SP_GETCOUNTBYTIME", p, commandType: CommandType.StoredProcedure)
                        .ToList();
                }

                return dataTable;
            }

            /// <summary>
            /// 图标显示原始数据
            /// </summary>
            /// <param name="model"></param>
            /// <returns></returns>
            public static List<ChartDataDto> GetChart(ChartModel model)
            {
                List<ChartDataDto> ResList = new List<ChartDataDto>();
                using (IDbConnection conn = MySqlConnection())
                {
                    MySqlParameter[] p = new MySqlParameter[3]
                    {
                        new MySqlParameter("@_DateTime", model.DateTime),
                        new MySqlParameter("@_QueryKind", model.QueryKind), new MySqlParameter("@_Area", model.Area)
                    };
                    // List<dynamic>
                    DataTable dataTable = ExecuteDataTable("SP_Web_TaktTimeCount", CommandType.StoredProcedure, p);
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        ResList.Add(new ChartDataDto()
                        {
                            Num = Convert.ToInt32(dataTable.Rows[0][dataTable.Columns[i].ColumnName]),
                            ChartDate = dataTable.Columns[i].ColumnName.Substring(0, 10),
                            Light = Convert.ToInt32(dataTable.Rows[0][dataTable.Columns[i].ColumnName]) + 10,
                            Hight = Convert.ToInt32(dataTable.Rows[0][dataTable.Columns[i].ColumnName]) + 70
                        });
                    }
                }

                return ResList;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="commandtype"></param>
            /// <param name="parameters"></param>
            /// <returns></returns>
            public static DataTable ExecuteDataTable(string sql, CommandType commandtype, MySqlParameter[] parameters)
            {
                DataTable data = new DataTable(); //实例化datatable，用于装载查询结果集  
                using (MySqlConnection con = MySqlConnection())
                {
                    using (MySqlCommand cmd = new MySqlCommand(sql, con))
                    {
                        cmd.CommandType = commandtype; //设置command的commandType为指定的Commandtype  
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
                        adapter.Fill(data); //填充datatable  

                    }
                }

                return data;
            }
        }

        //角色服务类
        public static class Roles
        {
            //插入
            public static int Insert(RolesDto model)
            {
                int result;
                using (IDbConnection conn = MySqlConnection())
                {
                    result = conn.Execute("Insert into role values (@Id,@Name)", model);
                }

                return result;
            }

            //编辑
            public static int Update(RolesDto model)
            {
                // UsersDto users;
                using (IDbConnection conn = MySqlConnection())
                {
                    return conn.Execute("update role set Name=@Name where Id=@Id", model);
                }
            }

            //获取角色
            public static RolesDto GetById(string Id)
            {
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@Id", Id);
                    return conn.Query<RolesDto>("select* from role where Id=@Id", p, commandType: CommandType.Text,
                        commandTimeout: 0).FirstOrDefault();
                }
            }
            public static int Delete(string Id)
            {
                // UsersDto users;

                var s = 0;
                using (IDbConnection conn = MySqlConnection())
                {
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        var p = new DynamicParameters();
                        p.Add("@Id", Id);

                        s = conn.Execute("delete from role where Id=@Id", p) +
                            conn.Execute("delete from rolefunction where RoleId=@Id", p);
                            
                        transaction.Commit();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }

                return s;
            }
        }

        //删除



        //用户角色管理
        public static class RoleUser
        {
            //插入
            public static int Insert(UserRoles model)
            {
                int result;
                using (IDbConnection conn = MySqlConnection())
                {
                    try
                    {
                        result = conn.Execute("Insert into userrole values (@UserId,@RoleId)", model);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                }

                return result;
            }

            public static int Delete(string id)
            {
                int result;
                using (IDbConnection conn = MySqlConnection())
                {
                    try
                    {
                        var p = new DynamicParameters();
                        p.Add("@Id", id);
                        return conn.Execute("delete from userrole where Id=@Id", p);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }

                }


            }

        }

        //功能管理
        public static class Function
        {
            //查询所有功能
            public static List<ResFunctionDto> GetFunction()
            {
                using (IDbConnection conn = MySqlConnection())
                {
                    return conn.Query<ResFunctionDto>("select * from function", commandType: CommandType.Text,
                        commandTimeout: 0).ToList();
                }
            }
        }

        //子菜单功能管理
        public static class SonFunction
        {
            public static List<SonFunctionDto> GetSonFunction()
            {
                using (IDbConnection conn = MySqlConnection())
                {
                    return conn.Query<SonFunctionDto>("select * from sonfunction", commandType: CommandType.Text,
                        commandTimeout: 0).ToList();
                }
            }

            public static SonFunctionDto GetById(string Id)
            {
                try
                {
                    using (IDbConnection conn = MySqlConnection())
                    {
                        var p = new DynamicParameters();
                        p.Add("@Id", Id);
                        var tt = conn.Query<SonFunctionDto>("select * from sonfunction where SonFunctionId=@Id", p,
                            commandType: CommandType.Text, commandTimeout: 0).FirstOrDefault();
                        return conn.Query<SonFunctionDto>("select * from sonfunction where SonFunctionId=@Id", p,
                            commandType: CommandType.Text, commandTimeout: 0).FirstOrDefault();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }

        }

        //角色权限管理
        public static class RoleFunction
        {
            //查询
            public static List<RoleFunctionDto> Entity()
            {
                using (IDbConnection conn = MySqlConnection())
                {
                    return conn.Query<RoleFunctionDto>("select * from rolefunction", commandType: CommandType.Text,
                        commandTimeout: 0).ToList();
                }
            }

            //新增
            public static int insert(RoleFunctionDto model)
            {
                string fatherId = "";
                using (IDbConnection conn = MySqlConnection())
                {
                    int result = 0;
                    IDbTransaction transaction = conn.BeginTransaction();
                    try
                    {
                        if (!model.RoleId.IsNullOrEmpty())
                        {
                            delete(model.RoleId);
                            if (!string.IsNullOrWhiteSpace(model.FunctionID))
                            {
                                foreach (var rid in model.FunctionID.Split(','))
                                {
                                    if (!string.IsNullOrWhiteSpace(rid))
                                    {
                                        if (rid.Length > 1)
                                        {
                                            if (SonFunction.GetById(rid).FatherId != fatherId)
                                            {
                                                conn.Execute(
                                                    "Insert into rolefunction values (@RoleId,@FunctionId)",
                                                    new RoleFunctionDto
                                                    {
                                                        RoleId = model.RoleId,
                                                        FunctionID = SonFunction.GetById(rid).FatherId
                                                    });
                                                fatherId = SonFunction.GetById(rid).FatherId;
                                            }

                                        }

                                        conn.Execute("Insert into rolefunction values (@RoleId,@FunctionId)",
                                            new RoleFunctionDto
                                            {
                                                RoleId = model.RoleId,
                                                FunctionID = rid
                                            });
                                        //RoleUser.Insert(});
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }

                    return result;
                }
            }

            //删除
            public static int delete(string id)
            {
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@Id", id);
                    return conn.Execute("delete from rolefunction where RoleId=@Id", p);

                }
            }
        }

        public static class SysUser
        {
            //获取当前用户
            public static SysUserDto GetById(string Id)
            {
                using (IDbConnection conn = MySqlConnection())
                {
                    var p = new DynamicParameters();
                    p.Add("@Id", Id);
                    return conn.Query<SysUserDto>("select* from sysuser where Id=@Id", p,
                        commandType: CommandType.Text,
                        commandTimeout: 0).FirstOrDefault();
                }
            }

            public static int ChangePassword(string uid, string password)
            {
                using (IDbConnection conn = MySqlConnection())
                {
                    SysUserDto item = GetById(uid);
                    item.Password = password;
                    UsersDto user = new UsersDto()
                    {
                        Name = item.Name,
                        Enable = item.Enable,
                        Id = item.Id,
                        Password = item.Password,
                        LoginName = item.LoginName,
                        Phone = item.Phone
                    };



                    return conn.Execute(
                        "update sysuser set Id=@Id,LoginName=@LoginName,Password=@Password,Name=@Name,Phone=@Phone,Enable=@Enable where Id=@Id",
                        user);
                }

            }

        }
    }
}
