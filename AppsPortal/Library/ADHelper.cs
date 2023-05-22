using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Web;

namespace AppsPortal.Library
{
    public class ADMethods : IDisposable
    {
        DirectoryEntry oDE = null;
        DirectoryEntry oDEC = null;
        DirectorySearcher oDS = null;
        SearchResultCollection oResults = null;
        DataSet oDs = null;
        DataSet oDsUser = null;
        DataTable oTb = null;
        DataRow oRwUser = null;
        DataRow oRwResult = null;
        DataRow oNewCustomersRow = null;

        #region Private Variables

        private string sADPath = "";
        private string sADPathPrefix = "";
        private string sADUser = "";
        private string sADPassword = "";
        private string sADServer = "";
        private string sCharactersToTrim = "";

        #endregion

        #region Enumerations

        public enum ADAccountOptions
        {
            UF_TEMP_DUPLICATE_ACCOUNT = 0x0100,
            UF_NORMAL_ACCOUNT = 0x0200,
            UF_INTERDOMAIN_TRUST_ACCOUNT = 0x0800,
            UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,
            UF_SERVER_TRUST_ACCOUNT = 0x2000,
            UF_DONT_EXPIRE_PASSWD = 0x10000,
            UF_SCRIPT = 0x0001,
            UF_ACCOUNTDISABLE = 0x0002,
            UF_HOMEDIR_REQUIRED = 0x0008,
            UF_LOCKOUT = 0x0010,
            UF_PASSWD_NOTREQD = 0x0020,
            UF_PASSWD_CANT_CHANGE = 0x0040,
            UF_ACCOUNT_LOCKOUT = 0X0010,
            UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080,
            UF_EXPIRE_USER_PASSWORD = 0x800000,
        }

        public enum GroupType : uint
        {
            UniversalGroup = 0x08,
            DomainLocalGroup = 0x04,
            GlobalGroup = 0x02,
            SecurityGroup = 0x80000000
        }

        public enum LoginResult
        {
            LOGIN_OK = 0,
            LOGIN_USER_DOESNT_EXIST,
            LOGIN_USER_ACCOUNT_INACTIVE
        }

        #endregion

        #region Methods

        public ADMethods(string path, string server, string username, string password)
        {
            //LDAP://10.240.224.203:389/OU=DA;OU=SYR,OU=Country,OU=Organisation,DC=UNHCR,DC=LOCAL
            sADPath = path;
            sADUser = username;
            sADPassword = password;
            sADServer = server;
        }

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (bDisposing)
            {

            }
            // Free your own state.
            // Set large fields to null.

            sADPath = null;
            sADUser = null;
            sADPassword = null;
            sADServer = null;
            sCharactersToTrim = null;

            oDE = null;
            oDEC = null;
            oDS = null;
            oResults = null;
            oDs = null;
            oDsUser = null;
            oTb = null;
            oRwUser = null;
            oRwResult = null;
            oNewCustomersRow = null;
        }

        //Use C# Destructor Syntax for Finalization Code.
        ~ADMethods()
        {
            //Simply call Dispose(false).
            Dispose(false);
        }

        #region Validate Methods

        /// <summary>
        /// This Method will verify if the User Account Exists
        /// By Matching both the Username and Password as well as
        /// checking if the Account is Active.
        /// </summary>
        /// <param name="sUserName">Username to Validate</param>
        /// <param name="sPassword">Password of the Username to Validate</param>
        /// <returns></returns>
        public ADMethods.LoginResult Login(string sUserName, string sPassword)
        {
            //Check if the Logon exists Based on the Username and Password
            if (IsUserValid(sUserName, sPassword))
            {
                oDE = GetUser(sUserName);
                if (oDE != null)
                {
                    //Check the Account Status
                    int iUserAccountControl = Convert.ToInt32(oDE.Properties["userAccountControl"][0]);
                    oDE.Close();

                    //If the Disabled Item does not Exist then the Account is Active
                    if (!IsAccountActive(iUserAccountControl))
                    {
                        return LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;
                    }
                    else
                    {
                        return LoginResult.LOGIN_OK;
                    }

                }
                else
                {
                    return LoginResult.LOGIN_USER_DOESNT_EXIST;
                }
            }
            else
            {
                return LoginResult.LOGIN_USER_DOESNT_EXIST;
            }
        }

        /// <summary>
        /// This will perfrom a logical operation on the iUserAccountControl values
        /// to see if the user Account is Enabled or Disabled.
        /// The Flag for Determining if the Account is active is a Bitwise value (Decimal = 2)
        /// </summary>
        /// <param name="iUserAccountControl"></param>
        /// <returns></returns>
        public bool IsAccountActive(int iUserAccountControl)
        {
            int iUserAccountControl_Disabled = Convert.ToInt32(ADAccountOptions.UF_ACCOUNTDISABLE);
            int iFlagExists = iUserAccountControl & iUserAccountControl_Disabled;

            //If a Match is Found, then the Disabled Flag Exists within the Control Flags
            if (iFlagExists > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// This will perfrom a logical operation on the sUserName values
        /// to see if the user Account is Enabled or Disabled.  
        /// The Flag for Determining if the Account is active is a Bitwise value (Decimal = 2)
        /// </summary>
        /// <param name="sUserName">Username to Validate</param>
        /// <returns></returns>
        public bool IsAccountActive(string sUserName)
        {
            oDE = GetUser(sUserName);
            if (oDE != null)
            {
                //to check of the Disabled option exists.
                int iUserAccountControl = Convert.ToInt32(oDE.Properties["userAccountControl"][0]);
                oDE.Close();

                //Check if the Disabled Item does not Exist then the Account is Active
                if (!IsAccountActive(iUserAccountControl))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This Method will Attempt to log in a User Based on the Username and Password
        /// to Ensure that they have been set up within the Active Directory.  
        /// This is the basic UserName and Password check.
        /// </summary>
        /// <param name="sUserName">Username to Validate</param>
        /// <param name="sPassword">Password of the Username to Validate</param>
        /// <returns></returns>
        public bool IsUserValid(string sUserName, string sPassword)
        {
            try
            {
                oDE = GetUser(sUserName, sPassword);
                oDE.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Search Methods
        /// <summary>
        /// This will return a DirectoryEntry Object if the User Exists
        /// </summary>
        /// <param name="sUserName">Username to Get</param>
        /// <returns></returns>
        public DirectoryEntry GetUser(string sUserName)
        {
            //Create an Instance of the DirectoryEntry
            oDE = GetDirectoryObject();

            //Create Instance fo the Direcory Searcher
            oDS = new DirectorySearcher();

            oDS.SearchRoot = oDE;
            //Set the Search Filter
            oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
            oDS.SearchScope = SearchScope.Subtree;
            oDS.PageSize = 10000;

            //Find the First Instance
            SearchResult oResults = oDS.FindOne();

            //If found then Return Directory Object, otherwise return Null
            if (oResults != null)
            {
                oDE = new DirectoryEntry(oResults.Path, sADUser, sADPassword, AuthenticationTypes.Secure);
                return oDE;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Override method which will perfrom query based on combination of Username and Password
        /// </summary>
        /// <param name="sUserName">Username to Get</param>
        /// <param name="sPassword">Password for the Username to Get</param>
        /// <returns></returns>
        public DirectoryEntry GetUser(string sUserName, string sPassword)
        {
            //Create an Instance of the DirectoryEntry
            oDE = GetDirectoryObject(sUserName, sPassword);

            //Create Instance fo the Direcory Searcher
            oDS = new DirectorySearcher();
            oDS.SearchRoot = oDE;

            //Set the Search Filter
            oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
            oDS.SearchScope = SearchScope.Subtree;
            oDS.PageSize = 10000;

            //Find the First Instance
            SearchResult oResults = oDS.FindOne();

            //If a Match is Found, Return Directory Object, Otherwise return Null
            if (oResults != null)
            {
                oDE = new DirectoryEntry(oResults.Path, sADUser, sADPassword, AuthenticationTypes.Secure);
                return oDE;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// This will take a Username and Query the AD for the User.  
        /// When found it will Transform the Results from the Property Collection into a Dataset.
        /// </summary>
        /// <param name="sUserName">Username to Get</param>
        /// <returns>Users Dataset</returns>
        public DataSet GetUserDataSet(string sUserName)
        {
            oDE = GetDirectoryObject();

            //Create Instance fo the Direcory Searcher
            oDS = new DirectorySearcher();
            oDS.SearchRoot = oDE;

            //Set the Search Filter
            oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
            oDS.SearchScope = SearchScope.Subtree;
            oDS.PageSize = 10000;

            //Find the First Instance
            SearchResult oResults = oDS.FindOne();

            //Create Empty User Dataset
            oDsUser = CreateUserDataSet();

            //If Record is not Null, Then Populate DataSet
            if (oResults != null)
            {
                oNewCustomersRow = oDsUser.Tables["User"].NewRow();
                oNewCustomersRow = PopulateUserDataSet(oResults, oDsUser.Tables["User"]);

                oDsUser.Tables["User"].Rows.Add(oNewCustomersRow);
            }
            oDE.Close();

            return oDsUser;

        }

        public class TempStaff
        {
            public string StaffName { get; set; }
            public string Email { get; set; }
            public string Title { get; set; }
            public string Ext { get; set; }
            public string Mobile { get; set; }
        }

        /// <summary>
        /// This Method will Return a Dataset of User Details Based on Criteria passed to the Query
        /// The criteria is in the LDAP format
        /// e.g.
        /// (sAMAccountName='Test Account Name')(sn='Test Surname')
        /// </summary>
        /// <param name="sCriteria">Criteria to use for Searching</param>
        /// <returns>Users Dataset</returns>
        public List<TempStaff> GetUsersDataSet(string sCriteria)
        {
            oDE = GetDirectoryObject();

            //Create Instance fo the Direcory Searcher
            oDS = new DirectorySearcher();
            oDS.SearchRoot = oDE;

            //Set the Search Filter
            oDS.Filter = sCriteria;
            oDS.SearchScope = SearchScope.Subtree;
            oDS.PageSize = 10000;

            //Find the First Instance


            //Create Empty User Dataset
            //oDsUser = CreateUserDataSet();
            SearchResult result;
            List<TempStaff> lstADUsers = new List<TempStaff>();
            //If Record is not Null, Then Populate DataSet

            oDS.PropertiesToLoad.Add("samaccountname");
            oDS.PropertiesToLoad.Add("mail");
            oDS.PropertiesToLoad.Add("usergroup");
            oDS.PropertiesToLoad.Add("displayname");//first name
            oDS.PropertiesToLoad.Add("telephoneNumber");
            oDS.PropertiesToLoad.Add("title");
            oDS.PropertiesToLoad.Add("mobile");


            oResults = oDS.FindAll();

            if (oResults != null)
            {
                TempStaff objSurveyUsers;
                for (int counter = 0; counter < oResults.Count; counter++)
                {
                    try
                    {
                        string UserNameEmailString = string.Empty;
                        result = oResults[counter];
                        if (result.Properties.Contains("samaccountname") && result.Properties.Contains("mail") && result.Properties.Contains("displayname"))
                        {
                            objSurveyUsers = new TempStaff();
                            try { objSurveyUsers.StaffName = (String)result.Properties["displayname"][0]; }
                            catch { }
                            try { objSurveyUsers.Email = result.Properties["mail"][0].ToString(); }
                            catch { }
                            try { objSurveyUsers.Title = (String)result.Properties["title"][0]; }
                            catch { }
                            try { objSurveyUsers.Ext = result.Properties["telephoneNumber"][0].ToString().Substring(result.Properties["telephoneNumber"][0].ToString().Length - 4); }
                            catch { }
                            try { objSurveyUsers.Mobile = (String)result.Properties["mobile"][0]; }
                            catch { }

                            lstADUsers.Add(objSurveyUsers);
                        }
                    }
                    catch { }
                }
            }

            oDE.Close();
            return lstADUsers;

        }

        #endregion

        #region User Account Methods

        /// <summary>
        /// This Method will set the Users Password based on the User Name
        /// </summary>
        /// <param name="sUserName">The Username to Set the New Password</param>
        /// <param name="sNewPassword">The New Password</param>
        /// <param name="sMessage">Any Messages catched by the Exception</param>
        public void SetUserPassword(string sUserName, string sNewPassword, out string sMessage)
        {
            try
            {
                //Get Reference to User
                string LDAPDomain = "/sAMAccountName=" + sUserName + ",CN=Users," + GetLDAPDomain();
                oDE = GetDirectoryObject(LDAPDomain);
                oDE.Invoke("SetPassword", new Object[] { sNewPassword });
                oDE.CommitChanges();
                oDE.Close();
                sMessage = "";
            }
            catch (Exception ex)
            {
                sMessage = ex.Message;
            }
        }

        /// <summary>
        /// This Method will set the Users Password based on Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directory Entry to Set the New Password</param>
        /// <param name="sPassword">The New Password</param>
        /// <param name="sMessage">Any Messages catched by the Exception</param>
        public void SetUserPassword(DirectoryEntry oDE, string sPassword, out string sMessage)
        {
            try
            {
                //Set The new Password
                oDE.Invoke("SetPassword", new Object[] { sPassword });
                sMessage = "";

                oDE.CommitChanges();
                oDE.Close();
            }
            catch (Exception ex)
            {
                sMessage = ex.InnerException.Message;
            }

        }

        /// <summary>
        /// This Method will Enable a User Account Based on the Username
        /// </summary>
        /// <param name="sUserName">The Username of the Account to Enable</param>
        public void EnableUserAccount(string sUserName)
        {
            //Get the Directory Entry fot the User and Enable the Password
            EnableUserAccount(GetUser(sUserName));
        }

        /// <summary>
        /// This Method will Enable a User Account Based on the Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directoy Entry Object of the Account to Enable</param>
        public void EnableUserAccount(DirectoryEntry oDE)
        {
            oDE.Properties["userAccountControl"][0] = ADMethods.ADAccountOptions.UF_NORMAL_ACCOUNT;
            oDE.CommitChanges();
            oDE.Close();
        }

        /// <summary>
        /// This Method will Force Expire a Users Password based on Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directoy Entry Object of the Account to Expire</param>
        public void ExpireUserPassword(DirectoryEntry oDE)
        {
            //Set the Password Last Set to 0, this will Expire the Password
            oDE.Properties["pwdLastSet"][0] = 0;
            oDE.CommitChanges();
            oDE.Close();
        }

        /// <summary>
        /// This Methoid will Disable the User Account based on the Username
        /// </summary>
        /// <param name="sUsername">The Username of the Account to Disable</param>
        public void DisableUserAccount(string sUserName)
        {
            DisableUserAccount(GetUser(sUserName));
        }

        /// <summary>
        /// This Methoid will Disable the User Account based on the Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directoy Entry Object of the Account to Disable</param>
        public void DisableUserAccount(DirectoryEntry oDE)
        {
            oDE.Properties["userAccountControl"][0] = ADMethods.ADAccountOptions.UF_NORMAL_ACCOUNT | ADMethods.ADAccountOptions.UF_DONT_EXPIRE_PASSWD | ADMethods.ADAccountOptions.UF_ACCOUNTDISABLE;
            oDE.CommitChanges();
            oDE.Close();
        }

        /// <summary>
        /// Moves a User Account to a New OU Path
        /// </summary>
        /// <param name="oDE">Directory Entry Object of the User to Move</param>
        /// <param name="sNewOUPath">The New Path</param>
        public void MoveUserAccount(DirectoryEntry oDE, string sNewOUPath)
        {
            DirectoryEntry myNewPath = null;
            //Define the new Path
            myNewPath = new DirectoryEntry("LDAP://" + sADServer + "/" + sNewOUPath, sADUser, sADPassword, AuthenticationTypes.Secure);

            oDE.MoveTo(myNewPath);
            oDE.CommitChanges();
            oDE.Close();
        }

        /// <summary>
        /// This Method checks whether and Account is Lockecd based on the Directory Entry Object
        /// </summary>
        /// <param name="oDE">Directory Entry Object of the Account to check</param>
        /// <returns></returns>
        public bool IsAccountLocked(DirectoryEntry oDE)
        {
            return Convert.ToBoolean(oDE.InvokeGet("IsAccountLocked"));
        }

        /// <summary>
        /// This Method will unlock a User Account based on the Directory Entry Object
        /// </summary>
        /// <param name="oDE">Directory Entry Object of the Account to unlock</param>
        public void UnlockUserAccount(DirectoryEntry oDE)
        {
            SetProperty(oDE, "lockoutTime", "0");
        }

        /// <summary>
        /// This Method checks whether and Account is Expired based on the Directory Entry Object
        /// </summary>
        /// <param name="oDE">Directory Entry Object of the Account to check</param>
        /// <returns></returns>
        public bool IsUserExpired(DirectoryEntry oDE)
        {
            int iDecimalValue = int.Parse(GetProperty(oDE, "userAccountControl"));
            string sBinaryValue = Convert.ToString(iDecimalValue, 2);

            //Reverse the Binary Value to get the Location for all 1's
            char[] str = sBinaryValue.ToCharArray();
            Array.Reverse(str);
            string sBinaryValueReversed = new string(str);

            //24th 1 is the Switch for the Expired Account
            if (sBinaryValueReversed.Length >= 24)
            {
                if (sBinaryValueReversed.Substring(24, 1) == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// This Method will Create a new User Directory Object
        /// </summary>
        /// <param name="sCN">The CN of the New User</param>
        /// <returns></returns>
        public DirectoryEntry CreateNewUser(string sCN)
        {
            //Set the LDAP Path so that the user will be Created under the Users Container
            string LDAPDomain = "/CN=Users," + GetLDAPDomain();

            oDE = GetDirectoryObject();
            oDEC = oDE.Children.Add("CN=" + sCN, "user");
            oDE.Close();
            return oDEC;
        }

        /// <summary>
        /// This Method will Create a new User Directory Object based on a Username and LDAP Domain
        /// </summary>
        /// <param name="sUserName">The Username of the New User</param>
        /// <param name="sLDAPDomain">The LDAP Domain for the New User</param>
        /// <returns></returns>
        public DirectoryEntry CreateNewUser(string sUserName, string sLDAPDomain)
        {
            //Set the LDAP qualification so that the user will be Created under the Users Container
            string LDAPDomain = "/CN=Users," + sLDAPDomain;
            oDE = new DirectoryEntry("LDAP://" + sADServer + "/" + sLDAPDomain, sADUser, sADPassword, AuthenticationTypes.Secure);

            oDEC = oDE.Children.Add("CN=" + sUserName, "user");
            oDE.Close();
            return oDEC;
        }

        /// <summary>
        /// This Method will Delete an AD User based on UserNaeme. Please be careful when using this
        /// The only way you can restore this object is by Tombstone which will not
        /// Restore every details on the Directory Entry object
        /// </summary>
        /// <param name="sUserName">The Username of the Account to Delete</param>
        /// <returns>True or False if the Delete was successfull</returns>
        public bool DeleteUser(string sUserName)
        {
            string sParentPath = GetUser(sUserName).Parent.Path;
            return DeleteUser(sUserName, sParentPath);
        }

        /// <summary>
        /// This Method will Delete an AD User based on Username and specifying the Path. Please be careful when using this
        /// The only way you can restore this object is by Tombstone which will not
        /// Restore every details on the Directory Entry object
        /// </summary>
        /// <param name="sUserName">The Username of the Account to Delete</param>
        /// <param name="sParentPath">The Path where the Useraccount is Located on LDAP</param>
        /// <returns></returns>
        public bool DeleteUser(string sUserName, string sParentPath)
        {
            try
            {
                oDE = new DirectoryEntry(sParentPath, sADUser, sADPassword, AuthenticationTypes.Secure);

                oDE.Children.Remove(GetUser(sUserName));

                oDE.CommitChanges();
                oDE.Close();
                return true;
            }
            catch
            {
                return false;
            }

        }

        #endregion

        #region Group Methods

        /// <summary>
        /// This Method will Create a New Active Directory Group
        /// </summary>
        /// <param name="sOULocation">OU Location of the New Group to be Created</param>
        /// <param name="sGroupName">The Group Name</param>
        /// <param name="sDescription">The Group Description</param>
        /// <param name="oGroupTypeInput">The Group Type</param>
        /// <param name="bSecurityGroup">True or False whether the Group is a Security Group or a Distribution Group</param>
        /// <returns></returns>
        public DirectoryEntry CreateNewGroup(string sOULocation, string sGroupName, string sDescription, GroupType oGroupTypeInput, bool bSecurityGroup)
        {
            GroupType oGroupType;

            oDE = new DirectoryEntry("LDAP://" + sADServer + "/" + sOULocation, sADUser, sADPassword, AuthenticationTypes.Secure);

            //Check if the Requested group is a Security Group or Distribution Group
            if (bSecurityGroup)
            {
                oGroupType = oGroupTypeInput | GroupType.SecurityGroup;
            }
            else
            {
                oGroupType = oGroupTypeInput;
            }
            int typeNum = (int)oGroupType;

            //Add Properties to the Group
            DirectoryEntry myGroup = oDE.Children.Add("cn=" + sGroupName, "group");
            myGroup.Properties["sAMAccountName"].Add(sGroupName);
            myGroup.Properties["description"].Add(sDescription);
            myGroup.Properties["groupType"].Add(typeNum);
            myGroup.CommitChanges();

            return myGroup;

        }

        /// <summary>
        /// This Method will add a User Based on the Distinguished Name to an AD Group
        /// </summary>
        /// <param name="sDN">The Users Distinguished Name</param>
        /// <param name="sGroupDN">The Groups Distinguished Name</param>
        public void AddUserToGroup(string sDN, string sGroupDN)
        {
            oDE = new DirectoryEntry("LDAP://" + sADServer + "/" + sGroupDN, sADUser, sADPassword, AuthenticationTypes.Secure);

            //Adds the User to the Group
            oDE.Properties["member"].Add(sDN);
            oDE.CommitChanges();
            oDE.Close();
        }

        /// <summary>
        /// This Method will remove a User Based on the Distinguished Name to an AD Group
        /// </summary>
        /// <param name="sDN">The Users Distinguished Name</param>
        /// <param name="sGroupDN">The Groups Distinguished Name</param>
        public void RemoveUserFromGroup(string sDN, string sGroupDN)
        {
            oDE = new DirectoryEntry("LDAP://" + sADServer + "/" + sGroupDN, sADUser, sADPassword, AuthenticationTypes.Secure);

            //Removes the User to the Group
            oDE.Properties["member"].Remove(sDN);
            oDE.CommitChanges();
            oDE.Close();
        }

        /// <summary>
        /// This Method will Validate whether the User is a Memeber of an AD Group
        /// </summary>
        /// <param name="sDN">The Users Distinguished Name</param>
        /// <param name="sGroupDN">The Groups Distinguished Name</param>
        /// <returns></returns>
        public bool IsUserGroupMember(string sDN, string sGroupDN)
        {
            oDE = new DirectoryEntry("LDAP://" + sADServer + "/" + sDN, sADUser, sADPassword, AuthenticationTypes.Secure);

            string sUserName = GetProperty(oDE, "sAMAccountName");

            ArrayList oUserGroups = GetUserGroups(sUserName);
            int iGroupsCount = oUserGroups.Count;

            if (iGroupsCount != 0)
            {
                for (int i = 0; i < iGroupsCount; i++)
                {
                    //Check is User is a Member of the AD Group
                    if (sGroupDN == oUserGroups[i].ToString())
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// This Method will return an ArrayList of a User
        /// AD Group Memberships
        /// </summary>
        /// <param name="sUserName">The Username to get Group Memberships</param>
        /// <returns></returns>
        public ArrayList GetUserGroups(string sUserName)
        {
            ArrayList oGroupMemberships = new ArrayList();
            return AttributeValuesMultiString("memberOf", sUserName, oGroupMemberships);
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// This will retreive the Specified Property Value from the Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directory Object to retrieve from</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public string GetProperty(DirectoryEntry oDE, string sPropertyName)
        {
            if (oDE.Properties.Contains(sPropertyName))
            {
                return oDE.Properties[sPropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// This will retreive the Specified Property Value if its an Array Type from the Directory Entry object
        /// </summary>
        /// <param name="oDE">The Directory Object to retrieve from</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public ArrayList GetProperty_Array(DirectoryEntry oDE, string sPropertyName)
        {
            ArrayList myItems = new ArrayList();
            if (oDE.Properties.Contains(sPropertyName))
            {
                for (int i = 0; i < oDE.Properties[sPropertyName].Count; i++)
                {
                    myItems.Add(oDE.Properties[sPropertyName][i].ToString());
                }
                return myItems;
            }
            else
            {
                return myItems;
            }
        }

        /// <summary>
        /// This will retreive the Specified Property Value if its a Byte Type from the Directory Entry object
        /// </summary>
        /// <param name="oDE">The Directory Object to retrieve from</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public byte[] GetProperty_Byte(DirectoryEntry oDE, string sPropertyName)
        {
            if (oDE.Properties.Contains(sPropertyName))
            {
                return (byte[])oDE.Properties[sPropertyName].Value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This is an Override that will Allow a Property to be Extracted Directly
        /// from a Search Result Object
        /// </summary>
        /// <param name="oSearchResult">The Search Result</param>
        /// <param name="sPropertyName">The Property to retrieve</param>
        /// <returns></returns>
        public string GetProperty(SearchResult oSearchResult, string sPropertyName)
        {
            if (oSearchResult.Properties.Contains(sPropertyName))
            {
                return oSearchResult.Properties[sPropertyName][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// This will Set the Property of the Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directory Object to Set to</param>
        /// <param name="sPropertyName">The Property Name</param>
        /// <param name="sPropertyValue">The Property Value</param>
        public void SetProperty(DirectoryEntry oDE, string sPropertyName, string sPropertyValue)
        {
            //Check if the Value is Valid
            if (sPropertyValue != string.Empty)
            {
                //Check if the Property Exists
                if (oDE.Properties.Contains(sPropertyName))
                {
                    oDE.Properties[sPropertyName].Value = sPropertyValue;
                    oDE.CommitChanges();
                    oDE.Close();
                }
                else
                {
                    oDE.Properties[sPropertyName].Add(sPropertyValue);
                    oDE.CommitChanges();
                    oDE.Close();
                }
            }
        }

        /// <summary>
        /// This will Set the Property of the Directory Entry Object
        /// </summary>
        /// <param name="oDE">The Directory Object to Set to</param>
        /// <param name="sPropertyName">The Property Name</param>
        /// <param name="bPropertyValue">The Property Value</param>
        public void SetProperty(DirectoryEntry oDE, string sPropertyName, byte[] bPropertyValue)
        {

            //Clear Binary Data if Exists
            oDE.Properties[sPropertyName].Clear();

            //Update Attribute with Binary Data from File
            oDE.Properties[sPropertyName].Add(bPropertyValue);
            oDE.CommitChanges();
            oDE.Dispose();

        }

        /// <summary>
        /// This will Set the Property of the Directory Entry Object if its an Array Type
        /// </summary>
        /// <param name="oDE">The Directory Object to Set to</param>
        /// <param name="sPropertyName">The Property Name</param>
        /// <param name="aPropertyValue">The Property Value in Array List Type</param>
        public void SetProperty(DirectoryEntry oDE, string sPropertyName, ArrayList aPropertyValue)
        {
            //Check if the Value is Valid
            if (aPropertyValue.Count != 0)
            {
                foreach (string sPropertyValue in aPropertyValue)
                {
                    oDE.Properties[sPropertyName].Add(sPropertyValue);
                    oDE.CommitChanges();
                    oDE.Close();
                }
            }
        }

        /// <summary>
        /// This Method will Clear the Property Values
        /// </summary>
        /// <param name="oDE">The Directory Object to Set to</param>
        /// <param name="sPropertyName">The Property Name to be cleared</param>
        public void ClearProperty(DirectoryEntry oDE, string sPropertyName)
        {
            //Check if the Property Exists
            if (oDE.Properties.Contains(sPropertyName))
            {
                oDE.Properties[sPropertyName].Clear();
                oDE.CommitChanges();
                oDE.Close();
            }
        }

        /// <summary>
        /// This is an Internal Method for Retreiving a New Directory Entry Object
        /// </summary>
        /// <returns></returns>
        private DirectoryEntry GetDirectoryObject()
        {
            oDE = new DirectoryEntry(sADPath, sADUser, sADPassword, AuthenticationTypes.Secure);
            return oDE;
        }

        /// <summary>
        /// Override Function that that will Attempt a Logon based on the Username and Password
        /// </summary>
        /// <param name="sUserName"></param>
        /// <param name="sPassword"></param>
        /// <returns></returns>
        private DirectoryEntry GetDirectoryObject(string sUserName, string sPassword)
        {
            oDE = new DirectoryEntry(sADPath, sUserName, sPassword, AuthenticationTypes.Secure);
            return oDE;
        }

        /// <summary>
        /// This will Create the Directory Entry based on the Domain Reference
        /// </summary>
        /// <param name="sDomainReference"></param>
        /// <returns></returns>
        private DirectoryEntry GetDirectoryObject(string sDomainReference)
        {
            oDE = new DirectoryEntry(sADPath + sDomainReference, sADUser, sADPassword, AuthenticationTypes.Secure);
            return oDE;
        }

        /// <summary>
        ///This will Create the Directory Entry based on the LDAP Path
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
        public DirectoryEntry GetDirectoryObject_ByPath(string sPath)
        {
            oDE = new DirectoryEntry(sADPathPrefix + sPath, sADUser, sADPassword, AuthenticationTypes.Secure);
            return oDE;
        }

        /// <summary>
        /// Additional Override that will Allow oObject to be Created based on the Username and Password.
        /// </summary>
        /// <param name="DomainReference"></param>
        /// <param name="sUserName"></param>
        /// <param name="sPassword"></param>
        /// <returns></returns>
        private DirectoryEntry GetDirectoryObject(string sDomainReference, string sUserName, string sPassword)
        {
            oDE = new DirectoryEntry(sADPath + sDomainReference, sUserName, sPassword, AuthenticationTypes.Secure);
            return oDE;
        }

        /// <summary>
        /// This will retreive the Distinguished Name from the DirectoryEntry Object
        /// </summary>
        /// <param name="oDE">The Directory Entry Object to get the Distinguisehd Name From</param>
        /// <returns></returns>
        public string GetDistinguishedName(DirectoryEntry oDE)
        {
            if (oDE.Properties.Contains("distinguishedName"))
            {
                return oDE.Properties["distinguishedName"][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// This will retreive the Distinguished Name from the User Name
        /// </summary>
        /// <param name="oDE">The User Name to get the Distinguisehd Name From</param>
        /// <returns></returns>
        public string GetDistinguishedName(string sUserName)
        {
            oDE = GetUser(sUserName);

            if (oDE.Properties.Contains("distinguishedName"))
            {
                return oDE.Properties["distinguishedName"][0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// This Method will Get the Array List of the Directory Object Attribute
        /// </summary>
        /// <param name="sAttributeName"></param>
        /// <param name="sUserName"></param>
        /// <param name="oValuesCollection"></param>
        /// <returns></returns>
        public ArrayList AttributeValuesMultiString(string sAttributeName, string sUserName, ArrayList oValuesCollection)
        {
            oDE = GetUser(sUserName);

            PropertyValueCollection oValueCollection = oDE.Properties[sAttributeName];
            IEnumerator oIEn = oValueCollection.GetEnumerator();

            while (oIEn.MoveNext())
            {
                if (oIEn.Current != null)
                {
                    if (!oValuesCollection.Contains(oIEn.Current.ToString()))
                    {
                        oValuesCollection.Add(oIEn.Current.ToString());
                    }
                }
            }
            oDE.Close();
            oDE.Dispose();
            return oValuesCollection;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// This will read in the ADServer Value from the Web.config and will Return it
        /// as an LDAP Path
        /// eg. DC=testing, DC=co, DC=nz.
        /// This is required when Creating Directory Entry other than the Root.
        /// </summary>
        /// <returns></returns>
        private string GetLDAPDomain()
        {
            StringBuilder LDAPDomain = new StringBuilder();
            string[] LDAPDC = sADServer.Split('.');

            for (int i = 0; i < LDAPDC.GetUpperBound(0) + 1; i++)
            {
                LDAPDomain.Append("DC=" + LDAPDC[i]);
                if (i < LDAPDC.GetUpperBound(0))
                {
                    LDAPDomain.Append(",");
                }
            }
            return LDAPDomain.ToString();
        }

        /// <summary>
        /// This method will Create a Dataset Stucture Containing all Relevant Fields for a User Object
        /// </summary>
        /// <returns></returns>
        private DataSet CreateUserDataSet()
        {
            oDs = new DataSet();

            oTb = oDs.Tables.Add("User");

            //Create All the Columns
            oTb.Columns.Add("company");
            oTb.Columns.Add("department");
            oTb.Columns.Add("description");
            oTb.Columns.Add("displayName");
            oTb.Columns.Add("facsimileTelephoneNumber");
            oTb.Columns.Add("givenName");
            oTb.Columns.Add("homePhone");
            oTb.Columns.Add("employeeNumber");
            oTb.Columns.Add("initials");
            oTb.Columns.Add("ipPhone");
            oTb.Columns.Add("l");
            oTb.Columns.Add("mail");
            oTb.Columns.Add("manager");
            oTb.Columns.Add("mobile");
            oTb.Columns.Add("name");
            oTb.Columns.Add("pager");
            oTb.Columns.Add("physicalDeliveryOfficeName");
            oTb.Columns.Add("postalAddress");
            oTb.Columns.Add("postalCode");
            oTb.Columns.Add("postOfficeBox");
            oTb.Columns.Add("sAMAccountName");
            oTb.Columns.Add("sn");
            oTb.Columns.Add("st");
            oTb.Columns.Add("street");
            oTb.Columns.Add("streetAddress");
            oTb.Columns.Add("telephoneNumber");
            oTb.Columns.Add("title");
            oTb.Columns.Add("userPrincipalName");
            oTb.Columns.Add("wWWHomePage");
            oTb.Columns.Add("whenCreated");
            oTb.Columns.Add("whenChanged");
            oTb.Columns.Add("distinguishedName");
            oTb.Columns.Add("info");

            return oDs;
        }

        /// <summary>
        /// This method will Create a Dataset Stucture Containing all Relevant Fields for a Group Object
        /// </summary>
        /// <param name="sTableName"></param>
        /// <returns></returns>
        private DataSet CreateGroupDataSet(string sTableName)
        {

            oDs = new DataSet();

            oTb = oDs.Tables.Add(sTableName);

            //Create all the Columns
            oTb.Columns.Add("distinguishedName");
            oTb.Columns.Add("name");
            oTb.Columns.Add("friendlyname");
            oTb.Columns.Add("description");
            oTb.Columns.Add("domainType");
            oTb.Columns.Add("groupType");
            oTb.Columns.Add("groupTypeDesc");

            return oDs;
        }

        /// <summary>
        /// This Method will Return a DataRow Object which will be added to the User Dataset Object
        /// </summary>
        /// <param name="oUserSearchResult"></param>
        /// <param name="oUserTable"></param>
        /// <returns></returns>
        private DataRow PopulateUserDataSet(SearchResult oUserSearchResult, DataTable oUserTable)
        {
            //Sets a New Empty Row
            oRwUser = oUserTable.NewRow();

            oRwUser["company"] = GetProperty(oUserSearchResult, "company");
            oRwUser["department"] = GetProperty(oUserSearchResult, "department");
            oRwUser["description"] = GetProperty(oUserSearchResult, "description");
            oRwUser["displayName"] = GetProperty(oUserSearchResult, "displayName");
            oRwUser["facsimileTelephoneNumber"] = GetProperty(oUserSearchResult, "facsimileTelephoneNumber");
            oRwUser["givenName"] = GetProperty(oUserSearchResult, "givenName");
            oRwUser["homePhone"] = GetProperty(oUserSearchResult, "homePhone");
            oRwUser["employeeNumber"] = GetProperty(oUserSearchResult, "employeeNumber");
            oRwUser["initials"] = GetProperty(oUserSearchResult, "initials");
            oRwUser["ipPhone"] = GetProperty(oUserSearchResult, "ipPhone");
            oRwUser["l"] = GetProperty(oUserSearchResult, "l");
            oRwUser["mail"] = GetProperty(oUserSearchResult, "mail");
            oRwUser["manager"] = GetProperty(oUserSearchResult, "manager");
            oRwUser["mobile"] = GetProperty(oUserSearchResult, "mobile");
            oRwUser["name"] = GetProperty(oUserSearchResult, "name");
            oRwUser["pager"] = GetProperty(oUserSearchResult, "pager");
            oRwUser["physicalDeliveryOfficeName"] = GetProperty(oUserSearchResult, "physicalDeliveryOfficeName");
            oRwUser["postalAddress"] = GetProperty(oUserSearchResult, "postalAddress");
            oRwUser["postalCode"] = GetProperty(oUserSearchResult, "postalCode");
            oRwUser["postOfficeBox"] = GetProperty(oUserSearchResult, "postOfficeBox");
            oRwUser["sAMAccountName"] = GetProperty(oUserSearchResult, "sAMAccountName");
            oRwUser["sn"] = GetProperty(oUserSearchResult, "sn");
            oRwUser["st"] = GetProperty(oUserSearchResult, "st");
            oRwUser["street"] = GetProperty(oUserSearchResult, "street");
            oRwUser["streetAddress"] = GetProperty(oUserSearchResult, "streetAddress");
            oRwUser["telephoneNumber"] = GetProperty(oUserSearchResult, "telephoneNumber");
            oRwUser["title"] = GetProperty(oUserSearchResult, "title");
            oRwUser["userPrincipalName"] = GetProperty(oUserSearchResult, "userPrincipalName");
            oRwUser["wWWHomePage"] = GetProperty(oUserSearchResult, "wWWHomePage");
            oRwUser["whenCreated"] = GetProperty(oUserSearchResult, "whenCreated");
            oRwUser["whenChanged"] = GetProperty(oUserSearchResult, "whenChanged");
            oRwUser["distinguishedName"] = GetProperty(oUserSearchResult, "distinguishedName");
            oRwUser["info"] = GetProperty(oUserSearchResult, "info");

            return oRwUser;

        }

        /// <summary>
        /// This Method will Return a DataRow object which will be added to the Group Dataset Object
        /// </summary>
        /// <param name="oSearchResult"></param>
        /// <param name="oTable"></param>
        /// <returns></returns>
        private DataRow PopulateGroupDataSet(SearchResult oSearchResult, DataTable oTable)
        {
            //Sets a New Empty Row
            oRwResult = oTable.NewRow();

            string sFullOU = GetProperty(oSearchResult, "distinguishedName");
            string[] splita = sCharactersToTrim.ToString().Split(new Char[] { ';' });
            foreach (string sa in splita)
            {
                sFullOU = sFullOU.Replace(sa, "");
            }

            string sDisplayName = "";
            string sRawString = "";
            string[] split1 = sFullOU.Split(new Char[] { ',' });
            foreach (string s1 in split1)
            {
                sRawString = s1;
                sRawString = sRawString.Replace("OU=", "");
                sRawString = sRawString.Replace("DC=", "");
                sRawString = sRawString.Replace("CN=", "");
                sDisplayName = sRawString + "/" + sDisplayName;
            }

            oRwResult["distinguishedName"] = GetProperty(oSearchResult, "distinguishedName");
            oRwResult["name"] = GetProperty(oSearchResult, "name");
            oRwResult["friendlyname"] = sDisplayName.Substring(0, sDisplayName.Length - 1); ;
            oRwResult["description"] = GetProperty(oSearchResult, "description");
            oRwResult["domainType"] = sADServer;

            string sGroupType = GetProperty(oSearchResult, "groupType");
            oRwResult["groupType"] = sGroupType;

            switch (sGroupType)
            {
                case "2": oRwResult["groupTypeDesc"] = "Global, Distribution"; break;
                case "4": oRwResult["groupTypeDesc"] = "Domain, Distribution"; break;
                case "8": oRwResult["groupTypeDesc"] = "Universal, Distribution"; break;
                case "-2147483640": oRwResult["groupTypeDesc"] = "Universal, Security"; break;
                case "-2147483646": oRwResult["groupTypeDesc"] = "Global, Security"; break;
                case "-2147483644": oRwResult["groupTypeDesc"] = "Domain, Security"; break;
                default: oRwResult["groupTypeDesc"] = ""; break;
            }

            return oRwResult;

        }

        #endregion
        #endregion

    }
}