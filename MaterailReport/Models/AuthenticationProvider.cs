using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.IO;

namespace MaterailReport.Models
{
    public class LoginModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class UserInfo
    {
        public string Login { get; set; }
        public string Full_Name { get; set; }
        public int Access_Level { get; set; }
        public byte[] Photo { get; set; }

        public string GetPhotoForPage()
        {
            string base64String = Convert.ToBase64String(Photo, 0, Photo.Length);
            return "data:image/jpg;base64," + base64String;
        }
    }


    public class AuthenticationProvider : IDisposable
    {
        SqlConnection _connection;

        public AuthenticationProvider(SqlConnection connection)
        {
            _connection = connection;
        }

        public AuthenticationProvider()
        {
            _connection = MRConnection.GetConnection();
        }

        public UserInfo Authenticate(string login, string password)
        {
            return _connection.QueryFirstOrDefault<UserInfo>($@"execute Authenticate N'{login}', N'{password}'");
        }

        public UserInfo GetUserInfo(string login)
        {
            return _connection.QueryFirstOrDefault<UserInfo>($@"execute GetUserInfoByLogin N'{login}'");
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}