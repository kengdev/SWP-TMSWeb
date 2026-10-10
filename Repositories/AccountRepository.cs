using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TMSWeb.Models;
using TMSWeb.Models.Entities.HR;
using TMSWeb.Models.Entities.LOG;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace TMSWeb.Repositories;
public class AccountRepository
{
    private DbavalogContext db;

    public AccountRepository(DbavalogContext context)
    {
        db = context;
    }

    public bool SignIn(string username, string password)
    {
        try
        {
            password = this.SaltHash(password);
            var query = db.TblAccounts.Where(w => w.AccUsername.ToLower() == username.ToLower() &&
                            w.AccPassword == password && w.IsActived == true).FirstOrDefault();
            bool result = (query != null ? true : false);
            return result;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public bool CustomerLogin(string username, string password)
    {
        try
        {
            password = this.SaltHash(password);
            var query = dbLog.SeCustomerAccounts.Where(w => w.CusUsername == username &&
                            w.CusPassword == password).FirstOrDefault();
            bool result = (query != null ? true : false);
            return result;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private string MD5Hash(string input)
    {
        StringBuilder hash = new StringBuilder();
        MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
        byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

        for (int i = 0; i < bytes.Length; i++)
        {
            hash.Append(bytes[i].ToString("x2"));
        }
        return hash.ToString();
    }

    private string SaltHash(string input)
    {
        Rfc2898DeriveBytes PBKDF2_hash = new Rfc2898DeriveBytes(input, Encoding.ASCII.GetBytes("rm4fSDh0sofK"), 20000);
        HMACSHA256 HMACSHA256_hash = new HMACSHA256();
        //System.Text.Encoding.UTF8.GetString(bytes);
        string hPassword = Convert.ToBase64String(PBKDF2_hash.GetBytes(32));

        return hPassword;
    }
}