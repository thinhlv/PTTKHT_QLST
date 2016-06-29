
using gMVVM.Web.Services.Management.Functions;
using gMVVM.Web.Services.Management.Implement;
using gMVVM.Web.Services.Management.Interfaces;
using gMVVM.Web.Services.Management.Interfaces.AssCommon;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Transactions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;
using System.IO;

namespace gMVVM.Web.Services.Management
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(
        RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class gMVVMService : ImplementInterface, ICM_ALLCODE, ICM_DEPT_GROUP
    {
        #region CM_ALLCODE

        public IEnumerable<CM_ALLCODE_ByIdResult> CM_ALLCODE_ById(string CDNAME, string CDTYPE)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.CM_ALLCODE_ById(CDNAME, CDTYPE);

                    return result.ToList();

                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public IEnumerable<CM_ALLCODE_By_Type_IdResult> CM_ALLCODE_By_Type_Id(string CDTYPE)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.CM_ALLCODE_By_Type_Id(CDTYPE);

                    return result.ToList();

                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public CM_ALLCODE_InsResult CM_ALLCODE_Ins(CM_ALLCODE_SearchResult data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    CM_ALLCODE_InsResult result = dataContext.CM_ALLCODE_Ins(data.CDNAME, data.CDVAL, data.CONTENT, data.CDTYPE, data.LSTODR).Single();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return new CM_ALLCODE_InsResult() { Result = "-1", ID = -1, ErrorDesc = ex.Message };
            }
        }

        public CM_ALLCODE_UpdResult CM_ALLCODE_Upd(CM_ALLCODE_SearchResult data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    CM_ALLCODE_UpdResult result = dataContext.CM_ALLCODE_Upd(data.ID, data.CDNAME, data.CDVAL, data.CONTENT, data.CDTYPE, data.LSTODR).Single();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return new CM_ALLCODE_UpdResult() { Result = "-1", ID = -1, ErrorDesc = ex.Message };
            }
        }

        public CM_ALLCODE_DelResult CM_ALLCODE_Del(int id)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    CM_ALLCODE_DelResult result = dataContext.CM_ALLCODE_Del(id).Single();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return new CM_ALLCODE_DelResult() { Result = "-1", ErrorDesc = ex.Message };
            }
        }

        public IEnumerable<CM_ALLCODE_SearchResult> CM_ALLCODE_Search(CM_ALLCODE_SearchResult data, int top)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.CM_ALLCODE_Search(data.ID, data.CDNAME, data.CDVAL, data.CONTENT, data.CDTYPE, data.LSTODR, 200);
                    return result.ToList();
                }

            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region CM_DEPT_GROUP
        public CM_DEPT_GROUP_InsResult CM_DEPT_GROUP_Ins(CM_DEPT_GROUP_SearchResult data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {

                    CM_DEPT_GROUP_InsResult result = dataContext.CM_DEPT_GROUP_Ins(data.GROUP_CODE, data.GROUP_NAME, data.NOTES,
                        data.RECORD_STATUS, data.MAKER_ID, data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate), data.AUTH_STATUS, data.CHECKER_ID,
                        data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)).Single();
                    return result;

                }

            }
            catch (Exception ex)
            {
                return new CM_DEPT_GROUP_InsResult() { Result = "-1", GROUP_ID = "", ErrorDesc = ex.Message };
            }
        }

        public CM_DEPT_GROUP_UpdResult CM_DEPT_GROUP_Upd(CM_DEPT_GROUP_SearchResult data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    CM_DEPT_GROUP_UpdResult result = dataContext.CM_DEPT_GROUP_Upd(data.GROUP_ID, data.GROUP_CODE, data.GROUP_NAME, data.NOTES,
                        data.RECORD_STATUS, data.MAKER_ID, data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate), data.AUTH_STATUS, data.CHECKER_ID,
                        data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)).Single();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return new CM_DEPT_GROUP_UpdResult() { Result = "-1", GROUP_ID = "", ErrorDesc = ex.Message };
            }
        }

        public CM_DEPT_GROUP_DelResult CM_DEPT_GROUP_Del(string id)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    CM_DEPT_GROUP_DelResult result = dataContext.CM_DEPT_GROUP_Del(id).Single();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return new CM_DEPT_GROUP_DelResult() { Result = "-1", ErrorDesc = ex.Message };
            }
        }

        public IEnumerable<CM_DEPT_GROUP_SearchResult> SearchCM_DEPT_GROUP(CM_DEPT_GROUP_SearchResult data, int top)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<CM_DEPT_GROUP_SearchResult> result = dataContext.CM_DEPT_GROUP_Search(data.GROUP_ID, data.GROUP_CODE, data.GROUP_NAME, data.NOTES, data.RECORD_STATUS, data.AUTH_STATUS,
                        data.MAKER_ID, data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate), data.CHECKER_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate), top).ToList();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public CM_DEPT_GROUP_ApprResult CM_DEPT_GROUP_Appr(CM_DEPT_GROUP_SearchResult data)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    CM_DEPT_GROUP_ApprResult result = dataContext.CM_DEPT_GROUP_Appr(data.GROUP_ID, data.AUTH_STATUS, data.CHECKER_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)).Single();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new CM_DEPT_GROUP_ApprResult() { Result = "-1", ErrorDesc = ex.Message };
            }
        }

        #endregion
    }

}