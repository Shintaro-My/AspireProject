using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Models
{
    public class UserModel
    {
        [Key]
        public Guid UserId { get; set; } = Guid.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRoles Role { get; set; } = UserRoles.Guest;

        static public string getHashFromPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i].ToString("x2"));
                }
                string hash = stringBuilder.ToString();
                return hash;
            }
        }
        public UserModel Merge(Guid userId, UserRequestModel req, bool canRoleChange = false)
        {
            if (userId == Guid.Empty && UserId == Guid.Empty) // 新規作成
            {
                UserId = Guid.NewGuid();
                Role = UserRoles.Guest;
            }
            else if (userId == UserId) // 編集
            {
                if (canRoleChange && req.Role != null)
                {
                    Role = req.Role.Value;
                }
            }
            else
            {
                throw new InvalidOperationException("異なるIDのユーザーはマージできません");
            }
            UserName = req.UserName;
            PasswordHash = getHashFromPassword(req.Password);
            return this;
        }
    }

    [Flags]
    public enum UserRoles
    {
        [Description("Guest")]
        Guest = 0,
        [Description("User")]
        User = 1,
        [Description("Moderator")]
        Moderator = 2,
        [Description("Administrator")]
        Admin = 3
    }
    public class UserRolesUtil
    {
        static public string GetDescription(UserRoles uRole)
        {
            var fi = uRole.GetType().GetField(uRole.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            var desciptionString = attributes.Select(n => n.Description).FirstOrDefault();
            if (desciptionString != null)
            {
                return desciptionString;
            }
            return uRole.ToString();
        }
        static public UserRoles GetEnum(string description)
        {
            var fields = typeof(UserRoles).GetFields();
            var field = fields
                .SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Att = a })
                .Where(a => ((DescriptionAttribute)a.Att).Description == description)
                .SingleOrDefault();
            if (field == null)
            {
                throw new Exception($"存在しないUserRoleです: {description}");
            }
            var value = (UserRoles)field.Field.GetRawConstantValue();
            return value;
        }
    }

    public class UserModelDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public UserRoles Role { get; set; }

        public UserModelDto() { }
        public UserModelDto(UserModel user)
        {
            UserId = user.UserId;
            UserName = user.UserName;
            Role = user.Role;
        }
    }

    public class UserRequestModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRoles? Role { get; set; }
    }

    public class UserLoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
