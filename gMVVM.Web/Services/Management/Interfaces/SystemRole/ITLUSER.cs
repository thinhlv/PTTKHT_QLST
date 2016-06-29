using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.Management.Interfaces.SystemRole
{
    [ServiceContract]
    public interface ITLUSER
    {
        [OperationContract]
        Boolean ContainsTLUSER(TL_USER data);

        [OperationContract]
        TL_USER CheckUserLogin(string userName, string password);

        [OperationContract]
        LoginData CheckLogin(string userName, string password, int indexConnect);

        [OperationContract]
        IEnumerable<TL_USER> GetByAllTLUSER();

        [OperationContract]
        TL_USER_InsResult InsertTLUSER(TL_USER_SearchResult data);

        [OperationContract]
        TL_USER_UpdResult UpdateTLUSER(TL_USER_SearchResult data);

        [OperationContract]
        TL_USER_DelResult DeleteTLUSER(string tlName);

        [OperationContract]
        TL_USER_AppResult ApproveTLUSER(TL_USER_SearchResult data);

        [OperationContract]
        IEnumerable<TL_USER_SearchResult> SearchTLUSER(TL_USER_SearchResult data, int top, bool level);

        [OperationContract]
        IEnumerable<TL_USER> GetByTopTLUSER(string _top, string _where, string _order);

        [OperationContract]
        String InsertAllTLUSER(ObservableCollection<TL_USER> lstUser);

        [OperationContract]
        String SendCodeTLUSER(string userName);

        [OperationContract]
        String ChangePassTLUSER(string userName, string code, string pass);

        [OperationContract]
        TL_USER_SearchResult GetUserInfomation(string user);

        [OperationContract]
        bool LoginAD(string user, string password);
    }

    [ServiceContract]
    public class LoginData
    {
        [DataMember]
        public TL_USER_SearchResult User { get; set; }

        [DataMember]
        public List<TL_SYSROLEDETAIL> Roles { get; set; }        
    }
}