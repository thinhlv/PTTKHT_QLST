using gMVVM.Web.Services.Management.Interfaces.SystemRole;
using gMVVM.Web.Services.Management.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Net.Mail;
using gMVVM.Web.Services.Management;
using gMVVM.Web.Services.Management.Functions;
using gMVVM.Web.Services.Management.Interfaces.AssCommon;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Collections;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using gMVVM.Web.Services.Management.Interfaces.QuanLySoThu;

namespace gMVVM.Web.Services.Management.Implement
{
    public class ImplementInterface : ITLUSER, ITLSYSROLE, ITLMENU, IBranch, IDEPARTMENT, IARGUMENT, ISYS_PARAMETERS, IZOO_BaoCaoTonKho, IZOO_PhieuNhapThuoc
       
    {
        public const string formatDate = "dd/MM/yyyy HH:mm:ss";
      
        #region [Public Functions]

        public int GetDayOfYear(int year)
        {
            return (new DateTime(year + 1, 1, 1) - new DateTime(year, 1, 1)).Days;
        }

        public string SetAutoId(string id, int maxlength)
        {
            while (id.Length < maxlength)
            {
                id = "0" + id;
            }
            return id;
        }        

        #endregion

        #region[TL_USER]

        public TL_USER CheckUserLogin(string userName, string password)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = (from user in dataContext.TL_USERs where user.TLNANME == userName && user.Password == password select user).Single();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public LoginData CheckLogin(string userName, string password, int indexConnect)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    string isCheckPass = "0";
                    try
                    {
                        isCheckPass = System.Web.Configuration.WebConfigurationManager.AppSettings["IsCheckPass"].ToString();
                    }
                    catch (Exception)
                    {
                    }
                    var currentUser = (from user in dataContext.TL_USERs
                                       join br in dataContext.CM_BRANCHes on user.TLSUBBRID equals br.BRANCH_ID
                                       where user.TLNANME == userName && user.AUTH_STATUS == "A" && (user.Password == password || isCheckPass == "0")
                                       select new TL_USER_SearchResult
                                       {
                                           TLID = user.TLID,
                                           TLNANME = user.TLNANME,
                                           TLFullName = user.TLFullName,
                                           TLSUBBRID = user.TLSUBBRID,
                                           BRANCH_CODE = br.BRANCH_CODE,
                                           SECUR_CODE = user.SECUR_CODE,
                                           RoleName = user.RoleName,
                                           PHONE = user.PHONE,
                                           Password = user.Password,
                                           MARKER_ID = user.MARKER_ID,
                                           ISFIRSTTIME = user.ISFIRSTTIME,
                                           ISAPPROVE = user.ISAPPROVE,
                                           EMAIL = user.EMAIL,
                                           BRANCH_TYPE = user.BRANCH_TYPE,
                                           BRANCH_NAME = user.BRANCH_NAME,
                                           Birthday = user.Birthday,
                                           AUTH_STATUS = user.AUTH_STATUS,
                                           AUTH_ID = user.AUTH_ID,
                                           APPROVE_DT = user.APPROVE_DT,
                                           ADDRESS = user.ADDRESS,
                                           TAX_NO = br.TAX_NO
                                       }).SingleOrDefault();
                    if (currentUser == null) return null;

                    //User
                    LoginData loginData = new LoginData();
                    loginData.User = currentUser;

                    //Role                    
                    var currentRole = (from item in dataContext.TL_SYSROLEDETAILs where item.ROLE_ID.Equals(currentUser.RoleName) select item).ToList();
                    loginData.Roles = currentRole;

                    //connection = lstConnect[indexConnect];

                    return loginData;

                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TL_USER_SearchResult ConvertDataUser(TL_USER data)
        {
            TL_USER_SearchResult dtResult = new TL_USER_SearchResult() 
            {
                TLID = data.TLID,
                TLNANME = data.TLNANME,
                TLFullName = data.TLFullName,
                TLSUBBRID = data.TLSUBBRID,
                SECUR_CODE = data.SECUR_CODE,
                RoleName = data.RoleName,
                PHONE = data.PHONE,
                Password = data.Password,
                MARKER_ID = data.MARKER_ID,
                ISFIRSTTIME = data.ISFIRSTTIME,
                ISAPPROVE = data.ISAPPROVE,
                EMAIL = data.EMAIL,
                BRANCH_TYPE = data.BRANCH_TYPE,
                BRANCH_NAME = data.BRANCH_NAME,
                Birthday = data.Birthday,
                AUTH_STATUS = data.AUTH_STATUS,
                AUTH_ID = data.AUTH_ID,
                APPROVE_DT = data.APPROVE_DT,
                ADDRESS = data.ADDRESS
            };
            return dtResult;
        }

        public IEnumerable<TL_USER> GetByAllTLUSER()
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.TL_USERs.ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool ContainsTLUSER(TL_USER data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var obj = (
                        from item in dataContext.TL_USERs
                        where item.TLNANME == data.TLNANME
                        select item
                        );

                    return (obj != null && obj.Count() > 0);
                }
            }
            catch (Exception)
            {                
                return false;
            }
        }

        public IEnumerable<TL_USER_SearchResult> SearchTLUSER(TL_USER_SearchResult data, int top, bool level)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<TL_USER_SearchResult> result = dataContext.TL_USER_Search(data.TLID, data.TLNANME, data.Password, data.TLFullName, data.TLSUBBRID, data.BRANCH_NAME, data.BRANCH_TYPE, data.RoleName, data.EMAIL, data.ADDRESS, data.PHONE, data.AUTH_STATUS,
                       data.MARKER_ID, data.AUTH_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate), data.ISAPPROVE, data.Birthday == null ? "" : data.Birthday.Value.ToString(formatDate), data.ISFIRSTTIME, data.SECUR_CODE, top, level ? "UNIT" : "ALL").ToList();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public TL_USER_InsResult InsertTLUSER(TL_USER_SearchResult data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    TL_USER_InsResult result = dataContext.TL_USER_Ins(data.TLNANME, data.Password, data.TLFullName, data.TLSUBBRID, data.BRANCH_NAME, data.BRANCH_TYPE, data.RoleName, data.EMAIL,data.ADDRESS, data.PHONE, data.AUTH_STATUS,
                       data.MARKER_ID, data.AUTH_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate), data.ISAPPROVE, data.Birthday == null ? "" : data.Birthday.Value.ToString(formatDate), data.ISFIRSTTIME, data.SECUR_CODE).Single();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new TL_USER_InsResult() { Result = "-1", ErrorDesc = ex.Message, TLID = "" };
            }
        }

        public TL_USER_UpdResult UpdateTLUSER(TL_USER_SearchResult data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    TL_USER_UpdResult result = dataContext.TL_USER_Upd(data.TLNANME, data.TLID, data.Password, data.TLFullName, data.TLSUBBRID, data.BRANCH_NAME, data.BRANCH_TYPE, data.RoleName, data.EMAIL, data.ADDRESS, data.PHONE, data.AUTH_STATUS,
                       data.MARKER_ID, data.AUTH_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate), data.ISAPPROVE, data.Birthday == null ? "" : data.Birthday.Value.ToString(formatDate), data.ISFIRSTTIME, data.SECUR_CODE).Single();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new TL_USER_UpdResult() { Result = "-1", ErrorDesc = ex.Message, TLID = "" };
            }
        }

        public TL_USER_DelResult DeleteTLUSER(string tlName)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    TL_USER_DelResult result = dataContext.TL_USER_Del(tlName).Single();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new TL_USER_DelResult() { Result = "-1", ErrorDesc = ex.Message };
            }
        }

        public TL_USER_AppResult ApproveTLUSER(TL_USER_SearchResult data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    TL_USER_AppResult result = dataContext.TL_USER_App(data.TLNANME, data.AUTH_STATUS, data.AUTH_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)).Single();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new TL_USER_AppResult() { Result = "-1", ErrorDesc = ex.Message };
            }
        }

        public IEnumerable<TL_USER> GetByTopTLUSER(string _top, string _where, string _order)
        {
            try
            {
                using (var data = new AssetDataContext())
                {
                    string query = "Select * from [TL_USER]";
                    if (!_top.Equals(""))
                        query = " Select top " + _top + " * from [TL_USER] ";
                    if (!_where.Equals(""))
                        query += " where " + _where + " ";
                    if (!_order.Equals(""))
                        query += " order by " + _order;

                    IEnumerable<TL_USER> results = data.ExecuteQuery<TL_USER>(@query);

                    return results.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string InsertAllTLUSER(ObservableCollection<TL_USER> lstUser)
        {
            try
            {                
                using (var dataContext = new AssetDataContext())
                {                    
                    dataContext.TL_USERs.InsertAllOnSubmit(lstUser);
                    dataContext.SubmitChanges();
                    
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {                
                return ex.Message;
            }
        }

        public string ChangePassTLUSER(string userName, string code, string pass)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var currentUser = (from item in dataContext.TL_USERs where item.TLNANME.Equals(userName) select item).Single();
                    if (currentUser == null) return "NotExisted";
                    if (currentUser.SECUR_CODE != code) return "CodeFail";

                    currentUser.Password = pass;
                    currentUser.SECUR_CODE = "";
                    
                    dataContext.SubmitChanges();
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SendCodeTLUSER(string userName)
        {
            try
            {                
                using (var dataContext = new AssetDataContext())
                {
                    var currentUser = (from item in dataContext.TL_USERs where item.TLNANME.Equals(userName) select item).SingleOrDefault();
                    if (currentUser == null) return "NotExisted";

                    currentUser.SECUR_CODE = System.Guid.NewGuid().ToString("N").Substring(0, 10);
                    string body = "Xin chào " + currentUser.TLFullName + ",\n\n\t"
                        + " Mã dùng để đổi mật khẩu (Code): " + currentUser.SECUR_CODE + "\n\t"
                        + " p/s: Code chỉ dùng để thay đổi mật khẩu 1 lần duy nhất.\n\n"
                        + " Trân Trọng!\n"
                        + "-----------------\n"
                        + " Công Ty TNHH Hoàn Cầu - GSOFT";

                    Mail(currentUser.EMAIL, body, "[gEduBanking] - Password Code");
                    dataContext.SubmitChanges();
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void Mail(string EmailTo, string MailBody, string MailSubject)
        {
            string eMail = "cs@gsoft.com.vn";
            string Pass = "Gsoft239@#(";
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gsoft.com.vn");
            mail.From = new MailAddress("GSOFT<" + eMail + ">");
            mail.To.Add(EmailTo); //nhap dia chi mail gui den
            mail.Subject = MailSubject;
            mail.Body = MailBody;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(eMail, Pass);
            SmtpServer.EnableSsl = false;
            SmtpServer.Send(mail);                
        }

        public TL_USER_SearchResult GetUserInfomation(string user)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    //string servername = System.Web.Configuration.WebConfigurationManager.AppSettings["serverName"].ToString();
                    //string servernameUser = System.Web.Configuration.WebConfigurationManager.AppSettings["servernameUser"].ToString();
                    //string UserAD = System.Web.Configuration.WebConfigurationManager.AppSettings["UserAD"].ToString();
                    //string PasswordAD = System.Web.Configuration.WebConfigurationManager.AppSettings["PasswordAD"].ToString();

                    string CheckLDAP = System.Web.Configuration.WebConfigurationManager.AppSettings["CheckLDAP"].ToString();

                    if (CheckLDAP != "1") return new TL_USER_SearchResult() { TLNANME = user, TLFullName = "" };

                    //DirectoryEntry entry = new DirectoryEntry(servername, UserAD, PasswordAD);
                    DirectoryEntry entry = System.Web.HttpContext.Current.Session["ENTRY"] as DirectoryEntry;
                    DirectorySearcher ds = new DirectorySearcher(entry);
                    ds.Filter = "(samaccountname=" + user + ")";

                    ds.PropertiesToLoad.Add("mail");
                    ds.PropertiesToLoad.Add("displayname");
                    ds.PropertiesToLoad.Add("givenName");
                    ds.PropertiesToLoad.Add("sn");
                    SearchResult results = ds.FindOne();
                    //DirectoryEntry ac = (DirectoryEntry)results.GetDirectoryEntry();                    
                    //PrincipalContext ctx = new PrincipalContext(ContextType.Domain, servernameUser, UserAD, PasswordAD);
                    //UserPrincipal userInfo = UserPrincipal.FindByIdentity(ctx, user);
                    //DirectoryEntry deUser = userInfo.GetUnderlyingObject() as DirectoryEntry;
                    //DirectoryEntry deUserContainer = deUser.Parent;
                    //var a = deUserContainer.Properties["distinguishedName"].Value;

                    string lstName = results.Properties["givenName"].Count > 0 ? results.Properties["givenName"][0].ToString() : "";
                    string firstName = results.Properties["sn"].Count > 0 ? results.Properties["sn"][0].ToString() : "";

                    TL_USER_SearchResult dataReturn = new TL_USER_SearchResult()
                    {
                        //TLNANME = user,
                       // APPROVE_DT = results.Properties["whencreated"].Count > 0 ? Convert.ToDateTime(results.Properties["whencreated"][0].ToString()) : DateTime.Now,
                        TLFullName = (firstName + " " + lstName).Trim(),
                        EMAIL = results.Properties["mail"].Count > 0 ? results.Properties["mail"][0].ToString() : "",
                        //EMAIL = userInfo.EmailAddress,
                      //  PHONE = results.Properties["telephonenumber"].Count > 0 ? results.Properties["telephonenumber"][0].ToString() : "",
                     //   ADDRESS = (results.Properties["streetaddress"].Count > 0 ? (results.Properties["streetaddress"][0].ToString() + ", ") : "") +
                      //  (results.Properties["l"].Count > 0 ? (results.Properties["l"][0].ToString() + ", ") : "") +
                      //  (results.Properties["co"].Count > 0 ? results.Properties["co"][0].ToString() : "")

                    };
                    return dataReturn;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool LoginAD(string user, string password)
        {
            bool authenticated = false;
            DirectoryEntry entry;
            try
            {
                string CheckLDAP = System.Web.Configuration.WebConfigurationManager.AppSettings["CheckLDAP"].ToString();
                if (CheckLDAP != "1") return true;

                string servername = System.Web.Configuration.WebConfigurationManager.AppSettings["serverName"].ToString();
                entry = new DirectoryEntry(servername, user, password);//(servername, user, password);               
                object nativeObject = entry.NativeObject;
                authenticated = true;
            }
            catch (Exception)
            {
                return false;
            }

            if (System.Web.HttpContext.Current.Session["ENTRY"] == null)
                System.Web.HttpContext.Current.Session.Add("ENTRY", entry);
            else
                System.Web.HttpContext.Current.Session["ENTRY"] = entry;

            return authenticated;
        }

        #endregion                

        #region[TL_SYSROLE]

        public IEnumerable<TL_SYSROLE> GetByAllTLSYSROLE()
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.TL_SYSROLEs.ToList();
                    return result;
                }
            }
            catch (Exception)
            {                
                return null;
            }
        }
        public string InsertTLSYSROLE(TL_SYSROLE data, List<TL_SYSROLEDETAIL> lstRoleDetail)
        {
            try
            {                
                using (var dataContext = new AssetDataContext())
                {
                    //Kiem tra ton tai
                    var item = (from it in dataContext.TL_SYSROLEs where it.ROLE_ID.Equals(data.ROLE_ID) select it).SingleOrDefault();
                    if (item != null) return ListMessage.Existed;

                    //Insert SYSROLE
                    dataContext.TL_SYSROLEs.InsertOnSubmit(data);
                    
                    //Insert SYSROLEDETAIL
                    dataContext.TL_SYSROLEDETAILs.InsertAllOnSubmit(lstRoleDetail);

                    dataContext.SubmitChanges();

                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public Boolean UpdateTLSYSROLE(TL_SYSROLE data, List<TL_SYSROLEDETAIL> lstUpdate, List<TL_SYSROLEDETAIL> lstInsert, List<string> lstDelete)
        {
            try
            {                
                using (var dataContext = new AssetDataContext())
                {
                    //Update TL_SYSROLE
                    var resultRole = (from item1 in dataContext.TL_SYSROLEs where item1.ROLE_ID == data.ROLE_ID select item1).Single();
                    MyHelper.Copy<TL_SYSROLE>(resultRole, data);

                    //Update TL_SYSROLEDETAIL
                    //Update
                    foreach (var roleDetail in lstUpdate)
                    {
                        var resuleItemUpdate = (from item in dataContext.TL_SYSROLEDETAILs
                                                where item.ROLE_ID.Equals(data.ROLE_ID) && item.MENU_ID.Equals(roleDetail.MENU_ID)
                                                select item).Single();
                        MyHelper.Copy<TL_SYSROLEDETAIL>(resuleItemUpdate, roleDetail);
                    }
                    //Insert
                    dataContext.TL_SYSROLEDETAILs.InsertAllOnSubmit(lstInsert);

                    //Delete
                    var lstDeleteDetail = (from item in dataContext.TL_SYSROLEDETAILs where lstDelete.Contains(item.MENU_ID) && item.ROLE_ID.Equals(data.ROLE_ID) select item);
                    dataContext.TL_SYSROLEDETAILs.DeleteAllOnSubmit(lstDeleteDetail);

                    dataContext.SubmitChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public Boolean DeleteTLSYSROLE(TL_SYSROLE data)
        {
            try
            {                
                using (var dataContext = new AssetDataContext())
                {
                    var item = (from item1 in dataContext.TL_SYSROLEs where item1.ROLE_ID.Equals(data.ROLE_ID) select item1).Single();
                    dataContext.TL_SYSROLEs.DeleteOnSubmit(item);
                    var lstItemRoleDetail = (from it in dataContext.TL_SYSROLEDETAILs where it.ROLE_ID.Equals(data.ROLE_ID) select it);
                    dataContext.TL_SYSROLEDETAILs.DeleteAllOnSubmit(lstItemRoleDetail);

                    dataContext.SubmitChanges();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public IEnumerable<TL_SYSROLE> GetByTopTLSYSROLE(string _top, string _where, string _order)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    string query = "Select * from [TL_SYSROLE]";
                    if (!_top.Equals(""))
                        query = " Select top " + _top + " * from [TL_SYSROLE] ";
                    if (!_where.Equals(""))
                        query += " where " + _where + " ";
                    if (!_order.Equals(""))
                        query += " order by " + _order;

                    IEnumerable<TL_SYSROLE> results = dataContext.ExecuteQuery<TL_SYSROLE>(@query);

                    return results.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IEnumerable<TL_SYSROLEDETAIL> GetByTopTLROLEDETAIL(string _top, string _where, string _order)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    string query = "Select * from [TL_SYSROLEDETAIL]";
                    if (!_top.Equals(""))
                        query = " Select top " + _top + " * from [TL_SYSROLEDETAIL] ";
                    if (!_where.Equals(""))
                        query += " where " + _where + " ";
                    if (!_order.Equals(""))
                        query += " order by " + _order;

                    IEnumerable<TL_SYSROLEDETAIL> results = dataContext.ExecuteQuery<TL_SYSROLEDETAIL>(@query);

                    return results.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region[TL_MENU]

        public IEnumerable<TL_MENU> GetByAllTLMENU()
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.TL_MENUs.ToList();
                    return result;
                }
            }
            catch (Exception)
            {                
                return null;
            }
        }

        public string InsertTLMENU(TL_MENU data)
        {
            try
            {               
                using (var dataContext = new AssetDataContext())
                {
                    dataContext.TL_MENUs.InsertOnSubmit(data);
                    dataContext.SubmitChanges();

                    string query = "Select Top 1 MENU_ID From [TL_MENU] Order by MENU_ID DESC";
                    IEnumerable<int> id = dataContext.ExecuteQuery<int>(@query);
                    query = id.First().ToString();
                    return query;
                }
            }
            catch (Exception)
            {                
                return "";
            }
        }

        public bool UpdateTLMENU(TL_MENU data)
        {
            try
            {                
                using (var dataContext = new AssetDataContext())
                {
                    var item = (from item1 in dataContext.TL_MENUs where item1.MENU_ID.Equals(data.MENU_ID) select item1).Single();
                    MyHelper.Copy<TL_MENU>(item, data);
                    dataContext.SubmitChanges();
                    return true;
                }
            }
            catch (Exception)
            {                
                return false;
            }
        }

        public bool DeleteTLMENU(TL_MENU data)
        {
            try
            {                
                using (var dataContext = new AssetDataContext())
                {
                    var item = (from item1 in dataContext.TL_MENUs where item1.MENU_ID.Equals(data.MENU_ID) select item1).Single();
                    dataContext.TL_MENUs.DeleteOnSubmit(item);

                    dataContext.SubmitChanges();

                    string query = "Update TL_MENU Set MENU_PARENT = '' where MENU_PARENT = '" + item.MENU_ID + "' "
                        + " DELETE TL_SYSROLEDETAIL Where MENU_ID='" + item.MENU_ID + "' ";
                    dataContext.ExecuteQuery<int>(@query);
                    return true;
                }
            }
            catch (Exception)
            {                
                return false;
            }
        }

        public IEnumerable<TL_MENU> GetByTopTLMENU(string _top, string _where, string _order)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    string query = "Select * from [TL_MENU]";
                    if (!_top.Equals(""))
                        query = " Select top " + _top + " * from [TL_MENU] ";
                    if (!_where.Equals(""))
                        query += " where " + _where + " ";
                    if (!_order.Equals(""))
                        query += " order by " + _order;

                    IEnumerable<TL_MENU> results = dataContext.ExecuteQuery<TL_MENU>(@query);

                    return results.ToList();
                }
            }
            catch (Exception)
            {               
                return null;
            }
        }

        public IEnumerable<TL_MENU> GetParentTLMENU()
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    string query = "Select * from [TL_MENU] where MENU_PARENT = '' ";                    

                    IEnumerable<TL_MENU> results = dataContext.ExecuteQuery<TL_MENU>(@query);

                    return results.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion        


        #region [BRANCH]

        public IEnumerable<CM_BRANCH> GetByAllBranch()
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.CM_BRANCHes.ToList();
                    return result;
                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        public string InsertBranch(CM_BRANCH data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    //Check existed BRANCH_CODE
                    var existItem = (from item in dataContext.CM_BRANCHes where item.BRANCH_CODE.Equals(data.BRANCH_CODE) select item).FirstOrDefault();
                    if(existItem != null) return ListMessage.Existed;

                    //Check existed TAX_NO
                    if (data.TAX_NO != null && data.TAX_NO != "")
                    {
                        var existTax = (from item in dataContext.CM_BRANCHes where item.TAX_NO.Equals(data.TAX_NO) select item).FirstOrDefault();
                        if (existTax != null) return "Mã số thuế đã tồn tại";
                    }
                    
                    //Phat sinh ma
                    SYS_CODEMASTER autoId = (from item in dataContext.SYS_CODEMASTERs
                                             where item.Prefix.Equals("BRN")
                                             select item).SingleOrDefault();
                    if (autoId != null)
                    {
                        autoId.CurValue = autoId.CurValue + 1;
                        data.BRANCH_ID = "BRN" + autoId.CurValue.Value.ToString("000000000000");
                    }
                    else
                    {
                        data.BRANCH_ID = "BRN000000000001";
                        dataContext.SYS_CODEMASTERs.InsertOnSubmit(new SYS_CODEMASTER() { Prefix = "BRN", CurValue = 2, Active = '1' });
                    }

                    dataContext.CM_BRANCHes.InsertOnSubmit(data);
                    dataContext.SubmitChanges();
                    return data.BRANCH_ID;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string UpdateBranch(CM_BRANCH data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    //Check existed branch_code
                    var existItem = (from item in dataContext.CM_BRANCHes where item.BRANCH_CODE.Equals(data.BRANCH_CODE) && !item.BRANCH_ID.Equals(data.BRANCH_ID) select item).FirstOrDefault();
                    if (existItem != null) return ListMessage.Existed;

                    //Check existed tax_no
                    //Check existed TAX_NO
                    if (data.TAX_NO != null && data.TAX_NO != "")
                    {
                        var existTaxNo = (from item in dataContext.CM_BRANCHes where item.TAX_NO.Equals(data.TAX_NO) && !item.BRANCH_ID.Equals(data.BRANCH_ID) select item).FirstOrDefault();
                        if (existTaxNo != null) return "Mã số thuế đã tồn tại";
                    }
                    var result = (from item in dataContext.CM_BRANCHes
                                  where item.BRANCH_ID.Equals(data.BRANCH_ID)
                                  select item).SingleOrDefault();
                    if (result == null) return ListMessage.NotExisted;

                    MyHelper.Copy(result, data);

                    dataContext.SubmitChanges();
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string DeleteBranch(string id)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = (from item in dataContext.CM_BRANCHes
                                  where item.BRANCH_ID.Equals(id)
                                  select item).SingleOrDefault();
                    if (result == null) return ListMessage.NotExisted;

                    dataContext.CM_BRANCHes.DeleteOnSubmit(result);
                    dataContext.SubmitChanges();
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public IEnumerable<CM_BRANCH> GetByTopBranch(string _top, string _where, string _order)
        {
            try
            {
                using (var data = new AssetDataContext())
                {
                    string query = "Select * from [CM_BRANCH]";
                    if (!_top.Equals(""))
                        query = " Select top " + _top + " * from [CM_BRANCH] ";
                    if (!_where.Equals(""))
                        query += " where " + _where + " ";
                    if (!_order.Equals(""))
                        query += " order by " + _order;

                    IEnumerable<CM_BRANCH> results = data.ExecuteQuery<CM_BRANCH>(@query);

                    return results.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ApproveBranch(CM_BRANCH data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {

                    //update LNTYPE
                    var result = (from current in dataContext.CM_BRANCHes where current.BRANCH_ID == data.BRANCH_ID select current).Single();
                    
                    result.AUTH_STATUS ="A";
                    result.CHECKER_ID = data.CHECKER_ID;      
                    result.APPROVE_DT = data.APPROVE_DT;
                    dataContext.SubmitChanges();
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public IEnumerable<CM_BRANCH_GETALLCHILDResult> GetByParentBranchID(string parentId)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<CM_BRANCH_GETALLCHILDResult> result = dataContext.CM_BRANCH_GETALLCHILD(parentId).ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public BranchLevel CM_BRANCH_Level(string branchId)
        {
            try
            {
                using (var dataContext = new BranchLevelContext())
                {
                    IMultipleResults results = dataContext.GetDetail(branchId);
                    BranchLevel data = new BranchLevel();
                    data.RegionList = results.GetResult<CM_BRANCH>().ToList();
                    data.BranchList = results.GetResult<CM_BRANCH>().ToList();
                    data.TransList = results.GetResult<CM_BRANCH>().ToList();                    

                    return data;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<CM_BRANCH_SearchResult> BranchSearch(CM_BRANCH_SearchResult data, string branchLogin, int? top)
        {
            try
            {
                using (var dataContext = new BranchLevelContext())
                {
                    IEnumerable<CM_BRANCH_SearchResult> result = dataContext.CM_BRANCH_Search(data.BRANCH_ID, data.FATHER_ID, data.F_BRANCH_CODE, data.BRANCH_CODE,
                        data.BRANCH_NAME, data.REGION_ID, "", data.BRANCH_TYPE, data.ADDR, data.TEL, data.NOTES, data.REGION_ID, data.MAKER_ID, data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate),
                        data.AUTH_STATUS, data.CHECKER_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(), top, branchLogin).ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion


        #region [DEPARTMENT]

        public IEnumerable<CM_DEPARTMENT_SearchResult> SearchDEPARTMENT(CM_DEPARTMENT_SearchResult data, int top)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<CM_DEPARTMENT_SearchResult> result = dataContext.CM_DEPARTMENT_Search(data.DEP_ID, data.DEP_CODE, data.DEP_NAME, data.DAO_CODE, data.DAO_NAME, data.BRANCH_ID, data.GROUP_ID, data.TEL, data.NOTES, data.RECORD_STATUS,
                        data.MAKER_ID, data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate),data.AUTH_STATUS, data.CHECKER_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate), top).ToList();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public CM_DEPARTMENT_InsResult InsertDEPARTMENT(CM_DEPARTMENT_SearchResult data)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    CM_DEPARTMENT_InsResult result = dataContext.CM_DEPARTMENT_Ins(data.DEP_CODE, data.DEP_NAME, data.DAO_CODE, data.DAO_NAME, data.BRANCH_ID, data.GROUP_ID, data.TEL, data.NOTES, data.RECORD_STATUS,
                        data.MAKER_ID, data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate), data.AUTH_STATUS, data.CHECKER_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new CM_DEPARTMENT_InsResult() { Result = "-1", ErrorDesc = ex.Message, DEP_ID = "" };
            }
        }

        public CM_DEPARTMENT_UpdResult UpdateDEPARTMENT(CM_DEPARTMENT_SearchResult data)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    CM_DEPARTMENT_UpdResult result = dataContext.CM_DEPARTMENT_Upd(data.DEP_ID, data.DEP_CODE, data.DEP_NAME, data.DAO_CODE, data.DAO_NAME, data.BRANCH_ID, data.GROUP_ID, data.TEL, data.NOTES, data.RECORD_STATUS,
                        data.MAKER_ID, data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate),data.AUTH_STATUS, data.CHECKER_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new CM_DEPARTMENT_UpdResult() { Result = "-1", ErrorDesc = ex.Message, DEP_ID = "" };
            }
        }

        public CM_DEPARTMENT_DelResult DeleteDEPARTMENT(string id)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    CM_DEPARTMENT_DelResult result = dataContext.CM_DEPARTMENT_Del(id).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new CM_DEPARTMENT_DelResult() { Result = "-1", ErrorDesc = ex.Message };
            }
        }

        public CM_DEPARTMENT_AppResult ApproveDEPARTMENT(CM_DEPARTMENT_SearchResult data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    CM_DEPARTMENT_AppResult result = dataContext.CM_DEPARTMENT_App(data.DEP_ID, data.AUTH_STATUS, data.CHECKER_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)).FirstOrDefault();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new CM_DEPARTMENT_AppResult() { Result = "-1", ErrorDesc = ex.Message };
            }
        }

        public IEnumerable<CM_DEPARTMENT_LstResult> CM_DEPARTMENT_Lst()
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.CM_DEPARTMENT_Lst();
                    return result.ToList();
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        
        #endregion

        #region [ARGUMENT]

        public IEnumerable<SYS_PARAMETER> GetByAllArgument()
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.SYS_PARAMETERs.ToList();
                    return result;
                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        public string InsertArgument(SYS_PARAMETER data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    //Check existed
                    var existItem = (from item in dataContext.SYS_PARAMETERs where item.ParaKey.Equals(data.ParaKey) select item).FirstOrDefault();
                    if (existItem != null) return ListMessage.Existed;                    

                    dataContext.SYS_PARAMETERs.InsertOnSubmit(data);
                    dataContext.SubmitChanges();
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string UpdateArgument(SYS_PARAMETER data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    //Check existed
                    var existItem = (from item in dataContext.SYS_PARAMETERs where item.ParaKey.Equals(data.ParaKey) && !item.ID.Equals(data.ID) select item).FirstOrDefault();
                    if (existItem != null) return ListMessage.Existed;
                    var result = (from item in dataContext.SYS_PARAMETERs
                                  where item.ParaKey.Equals(data.ParaKey)
                                  select item).SingleOrDefault();
                    if (result == null) return ListMessage.NotExisted;

                    MyHelper.Copy(result, data);

                    dataContext.SubmitChanges();
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string DeleteArgument(string id)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = (from item in dataContext.SYS_PARAMETERs
                                  where item.ParaKey.Equals(id)
                                  select item).SingleOrDefault();
                    if (result == null) return ListMessage.NotExisted;

                    dataContext.SYS_PARAMETERs.DeleteOnSubmit(result);
                    dataContext.SubmitChanges();
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public IEnumerable<SYS_PARAMETER> GetByTopArgument(string _top, string _where, string _order)
        {
            try
            {
                using (var data = new AssetDataContext())
                {
                    string query = "Select * from [SYS_PARAMETERS]";
                    if (!_top.Equals(""))
                        query = " Select top " + _top + " * from [SYS_PARAMETERS] ";
                    if (!_where.Equals(""))
                        query += " where " + _where + " ";
                    if (!_order.Equals(""))
                        query += " order by " + _order;

                    IEnumerable<SYS_PARAMETER> results = data.ExecuteQuery<SYS_PARAMETER>(@query);

                    return results.ToList();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ApproveArgument(SYS_PARAMETER data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {

                    //update LNTYPE
                    var result = (from current in dataContext.SYS_PARAMETERs where current.ParaKey == data.ParaKey select current).Single();

                    result.AUTH_STATUS = "A";
                    result.CHECKER_ID = data.CHECKER_ID;
                    result.APPROVE_DT = data.APPROVE_DT;

                    dataContext.SubmitChanges();
                    return ListMessage.True;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion

        #region SYS_PARAMETERS

        public SYS_PARAMETERS_ByIdResult GetParameterById(string paraKey)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    SYS_PARAMETERS_ByIdResult result = dataContext.SYS_PARAMETERS_ById(paraKey).Single();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region IZOO_BaoCaoTonKho
        public IEnumerable<ZOO_BAOCAOTONKHO_SearchResult> LayBaoCaoTonKho(string maThuoc, string tenThuoc, DateTime ngayHetHang)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<ZOO_BAOCAOTONKHO_SearchResult> result = dataContext.ZOO_BAOCAOTONKHO_Search(maThuoc, tenThuoc, ngayHetHang == null ? DateTime.Now.ToString(formatDate) : ngayHetHang.ToString(formatDate), 0).ToList();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region IZOO_PhieuNhapThuoc
        public ZOO_PHIEUNHAPTHUOC_InsResult LuuPhieuNhap(ZOO_PHIEUNHAPTHUOC data)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    ZOO_PHIEUNHAPTHUOC_InsResult result = dataContext.ZOO_PHIEUNHAPTHUOC_Ins(
                        data.MaThuoc, 
                        data.SoLuong, 
                        data.NgayNhap == null ? "" : data.NgayNhap.Value.ToString(formatDate), 
                        data.NOTES, 
                        data.RECORD_STATUS, 
                        data.MAKER_ID, 
                        data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate), 
                        data.AUTH_STATUS, 
                        data.CHECKER_ID, 
                        data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)
                        ).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new ZOO_PHIEUNHAPTHUOC_InsResult() { Result = "-1", ErrorDesc = ex.Message, MaPhieuNhap = "" };
            }
        }

        public IEnumerable<ZOO_PHIEUNHAPTHUOC_SearchResult> TimPhieuNhap(string maphieu, string tenthuoc, DateTime ngaynhap, int top)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<ZOO_PHIEUNHAPTHUOC_SearchResult> result = dataContext.ZOO_PHIEUNHAPTHUOC_Search(maphieu, tenthuoc, ngaynhap == null ? "": ngaynhap.ToString(formatDate), top).ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<ZOO_THUOC> DanhSachThuoc()
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.ZOO_THUOCs.ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion


        /// <summary>
        /// Goi Store thong bang parameter
        /// </summary>
        /// <typeparam name="T">Doi tuong store tra ve</typeparam>
        /// <param name="parameters">Doi so truyen vao store</param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteStoredProcedure<T>(List<gParam> parameters)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    Type genericType = typeof(T);

                    string commandsthing = genericType.Name.Replace("Result", " ");
                    int count = parameters.Count();
                    string[] param = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        if (i > 0) commandsthing += ", ";
                        commandsthing += parameters[i].Name + "={" + i.ToString() + "} ";
                        param[i] = parameters[i].Value == null ? "" : parameters[i].Value;
                    }                                     

                    return dataContext.ExecuteQuery<T>(commandsthing, param).ToList<T>();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class gParam
    {
        public gParam(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    
}