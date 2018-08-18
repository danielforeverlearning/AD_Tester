using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//need to go to References and add System.DirectoryServices
using System.DirectoryServices;

namespace AD_Tester
{
    class Program
    {
        // Return an arraylist of properties to load for a user.
        public static ArrayList UserProperties()
        {
            ArrayList _de_properties = new ArrayList();
            _de_properties.Add("sn");
            _de_properties.Add("givenname");
            _de_properties.Add("employeeID");
            _de_properties.Add("displayName");
            _de_properties.Add("department");
            _de_properties.Add("mail");
            _de_properties.Add("memberOf");
            _de_properties.Add("company");
            _de_properties.Add("accountexpires");
            _de_properties.Add("LockOutTime");


            return _de_properties;
        }

        public static DirectoryEntry SelectUserWhereUsername(String sUsername)
        {
            // Test to make sure an email has been passed.
            // Silent fail is ok here.
            if (sUsername.Length < 2)
                return null;

            // Clean username.
            sUsername = AD_Tester.Security.CleanInput(sUsername, "_-", 30);

            // Begin searching
            //DirectoryEntry de = new DirectoryEntry("LDAP://DC=admin,DC=com");
            DirectorySearcher search = new DirectorySearcher();
            search.Filter = "(&(samAccountName=" + sUsername + ")(objectClass=user))";

            foreach (string s in UserProperties())
                search.PropertiesToLoad.Add(s);

            SearchResultCollection results = search.FindAll();

            if (1 == results.Count)
            {
                foreach (SearchResult ir in results)
                    return ir.GetDirectoryEntry();
            }

            return null;
        }

        public static bool IsUserMemberOfFaculty(DirectoryEntry User)
        {
            // Format of group names from AD is
            // Groups are nested, so if they have any group starting with directory_faculty, 
            //      then they're ok.
            // CN=Groupname,Hostgroup,...
            try
            {
                string Groupname = "";
                foreach (string s in (Array)User.Properties["memberOf"].Value)
                {
                    Groupname = s.Substring(3, s.IndexOf(",") - 3);
                    if (Groupname.Contains("Directory_Faculty"))
                        return true;
                }
            }
            catch (Exception e)
            {
                // We can get an error when the user isn't a member of any groups.
                // Return false in that case.
                e.ToString();
            }
            return false;
        }

        public static bool IsUserMemberOfGroup(DirectoryEntry User, string GroupName)
        {
            // Format of group names from AD is
            // CN=Groupname,Hostgroup,...
            try
            {

                foreach (string s in (Array)User.Properties["memberOf"].Value)
                {
                    if (s.Contains(GroupName))
                        return true;
                }
            }
            catch (Exception e)
            {
                // We can get an error when the user isn't a member of any groups.
                e.ToString();
            }
            return false;
        }

        public static Boolean HasUploadRights(string sUserName)
        {
            System.DirectoryServices.DirectoryEntry de = SelectUserWhereUsername(sUserName);

            if (null == de)
                return false;

            if (IsUserMemberOfFaculty(de)) //Directory_Faculty
                return true;
            if (IsUserMemberOfGroup(de, "Directory_Staff_AdministrativeAssistants"))
                return true;
            if (IsUserMemberOfGroup(de, "Directory_Cabinet"))
                return true;
            if (IsUserMemberOfGroup(de, "Directory_Deans"))
                return true;
            if (IsUserMemberOfGroup(de, "Directory_Chairs"))
                return true;
            if (sUserName == "lauh")
                return true;
            if (sUserName == "stanleym")
                return true;

            return false;
        }

        static void Main(string[] args)
        {
            //Boolean user_rights = HasUploadRights("schweitzerd");

            Boolean user_rights = HasUploadRights("dewaarda");

        }
    }
}
