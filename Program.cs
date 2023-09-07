using System;
using System.Collections.Generic;
using System.Text;
using Baileysoft.Services.Ldap;
using Novell.Directory.Ldap;

namespace LdapSearchDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Set globals 
            Settings.Server = "192.168.2.1";
            Settings.ServiceAccountDn = "CN=Neal T. Bailey,OU=USERS,OU=HOME,DC=baileysoft,DC=com";
            Settings.ServiceAccountPassword = "Cr@zyP@$$";
            Settings.SearchBase = "dc=baileysoft,dc=com";

            runDemo();

            Console.WriteLine("");
            Console.WriteLine("Press Any Key to Exit");
            Console.ReadLine();
        }

        private static void runDemo()
        {
            //Check if object exists
            Console.WriteLine(Search.Exists("sAMAccountName", "neal.bailey"));

            //Find Users
            foreach (LdapEntry user in Search.ForUsers())
                Console.WriteLine(user.DN);

            //Find User Based On Property Value
            if (Search.Exists("sAMAccountName", "neal.bailey"))
            {
                Console.WriteLine(Search.ForUser("sAMAccountName", "neal.bailey").DN);
            }

            //Find Group
            if (Search.Exists("cn", "FTP_USERS"))
            {
                Console.WriteLine(Search.ForGroup("cn", "FTP_USERS").DN);
            }

            //Find Groups
            foreach (LdapEntry group in Search.ForGroups())
                Console.WriteLine(group.DN);

            //Find objects based on ADO query 

            //Locate all FTP groups (change this to a valid patter for your directory)
            foreach (LdapEntry ftpGroup in Search.ForLdapEntries(null, null, "(&(objectClass=Group)(cn=*FTP*))"))
                Console.WriteLine(ftpGroup.DN);

            //Get Single String Attribute Value for User
            if (Search.Exists("sAMAccountName", "neal.bailey"))
            {
                LdapEntry user = Search.ForUser("sAMAccountName", "neal.bailey");
                LdapAttribute cn = user.getAttribute("cn");
                Console.WriteLine(cn.Name + ": " + cn.StringValue);
            }

            //Get Multi-string Attribute Values 
            if (Search.Exists("sAMAccountName", "neal.bailey"))
            {
                LdapEntry user = Search.ForUser("sAMAccountName", "neal.bailey");
                LdapAttribute members = user.getAttribute("memberOf");
                if (members != null)
                {
                    System.Collections.IEnumerator parser = members.StringValues;
                    while (parser.MoveNext())
                    {
                        Console.WriteLine(members.Name + ": " + parser.Current);
                    }
                }
            }

            //Specify the properties to load on search
            if (Search.Exists("sAMAccountName", "neal.bailey"))
            {
                string[] attribs = { "name", "userPrincipalName", "createTimeStamp" };
                LdapEntry user = Search.ForUser("sAMAccountName", "neal.bailey", attribs);

                LdapAttributeSet foundAttribs = user.getAttributeSet();
                System.Collections.IEnumerator ienum = foundAttribs.GetEnumerator();
                while (ienum.MoveNext())
                {
                    LdapAttribute attribute = (LdapAttribute)ienum.Current;
                    string attributeName = attribute.Name;
                    string attributeVal = attribute.StringValue;
                    Console.WriteLine(attributeName + ": " + attributeVal);
                }
            }


            Console.WriteLine();

        }
    }
}

