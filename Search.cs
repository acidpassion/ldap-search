using System;
using System.Collections.Generic;
using System.Text;
using Novell.Directory.Ldap;
using Baileysoft.Services.Ldap;

namespace Baileysoft.Services.Ldap
{
    public class Search
    {
        protected Search() { }

        /// <summary>
        /// Determines the existence of an object
        /// </summary>
        /// <param name="key">The property</param>
        /// <param name="value">The property value</param>
        /// <returns></returns>
        public static bool Exists(string key, string value)
        {
            LdapEntry obj = Search.ForLdapEntry(key, value);
            return obj != null;
        }

        /// <summary>
        /// Finds a User Object
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LdapEntry ForUser(string key, string value)
        {
            return ForLdapEntry(null, null, String.Format("(&(&(objectClass=user)(!(objectClass=computer)))({0}={1}))", key, value)); 
        }

        public static LdapEntry ForUser(string key, string value, string[] PropertiesToLoad)
        {
            return ForLdapEntry(null, null, String.Format("(&(&(objectClass=user)(!(objectClass=computer)))({0}={1}))", key, value), PropertiesToLoad);
        }

        /// <summary>
        /// Finds Users
        /// </summary>
        /// <returns>List</returns>
        public static List<LdapEntry> ForUsers()
        {
            return ForLdapEntries(null, null, "(&(objectClass=user)(!(objectClass=computer)))"); 
        }

        public static List<LdapEntry> ForUsers(string[] PropertiesToLoad)
        {
            return ForLdapEntries(null, null, "(&(objectClass=user)(!(objectClass=computer)))", PropertiesToLoad); 
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LdapEntry ForGroup(string key, string value)
        {
            return ForLdapEntry(null, null, String.Format("(&(objectClass=group)({0}={1}))", key, value)); 
        }

        public static List<LdapEntry> ForGroups()
        {
            return ForLdapEntries(null, null, "objectClass=user"); 
        }

        /// <summary>
        /// Finds Users
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<LdapEntry> ForUsers(string key, string value)
        {
            return ForLdapEntries(null, null, "(&(&(objectClass=user)(!(objectClass=computer)))(" + key + "=" + value + "))"); 
        }

        /// <summary>
        /// Finds ldap entry
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static LdapEntry ForLdapEntry(string key, string value)
        {
            return ForLdapEntry(key, value, string.Format("{0}={1}", key, value));
        }

        /// <summary>
        /// Finds ldap entry
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static LdapEntry ForLdapEntry(string key, string value, string filter)
        {
            return ForLdapEntry(key, value, filter, null);
        }

        /// <summary>
        /// Finds ldap entry
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filter">The ADO query filter</param>
        /// <param name="attributes">Properties to load</param>
        /// <returns></returns>
        public static LdapEntry ForLdapEntry(string key, string value, string filter, string[] attributes)
        {
            ValidateSetup();
            LdapEntry searchResult = null;
            LdapConnection conn = new LdapConnection();
            conn.Connect(Settings.Server, Settings.Port);

            conn.Bind(Settings.ServiceAccountDn, Settings.ServiceAccountPassword);

            //Search
            LdapSearchResults results = conn.Search(Settings.SearchBase, //search base
                                                    LdapConnection.SCOPE_SUB, //scope 
                                                    filter, //filter
                                                    attributes, //attributes 
                                                    false); //types only 

            while (results.hasMore())
            {
                try
                {
                    searchResult = results.next();
                    break;
                }
                catch (LdapException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            conn.Disconnect();
            return searchResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<LdapEntry> ForLdapEntries(string key, string value)
        {
            return ForLdapEntries(key, value, String.Format("{0}={1}"), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static List<LdapEntry> ForLdapEntries(string key, string value, string filter)
        {
            return ForLdapEntries(key, value, filter, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="filter"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static List<LdapEntry> ForLdapEntries(string key, string value, string filter, string[] attributes)
        {
            ValidateSetup();
            List<LdapEntry> searchResults = new List<LdapEntry>();
            LdapConnection conn = new LdapConnection();
            conn.Connect(Settings.Server, Settings.Port);
            conn.Bind(Settings.ServiceAccountDn, Settings.ServiceAccountPassword);

            LdapSearchResults results = conn.Search(Settings.SearchBase, //search base
                                                    LdapConnection.SCOPE_SUB, //scope 
                                                    filter, //filter
                                                    attributes, //attributes 
                                                    false); //types only 

            while (results.hasMore())
            {
                LdapEntry nextEntry = null;
                try
                {
                    nextEntry = results.next();
                    if (nextEntry != null)
                        searchResults.Add(nextEntry);
                }
                catch (LdapException)
                {
                    //Console.WriteLine(e.Message);
                    continue;
                }
            }
            conn.Disconnect();
            return searchResults;
        }

        private static void ValidateSetup()
        {
            if (Settings.Server == null || Settings.SearchBase == null)
                throw new Exception("Must specify required parameters in the Settings.cs class");
        }
    }
}
