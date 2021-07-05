﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Back_End
{
    public class Message
    {

        public int errorCode { get; set; }

        public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();


        public string ReturnJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }


    public class LoginMessage:Message
    {
        public LoginMessage()
        {
            errorCode = 300;
            data.Add("loginState", false);
            data.Add("userName", null);
            data.Add("userAvatar", null);
        }
    }

    public class CheckPhoneMessage:Message
    {
        public CheckPhoneMessage()
        {
            errorCode = 300;
            data.Add("phoneunique", false);
        }
    }

    public class RegisterMessage:Message
    {
        public RegisterMessage()
        {
            errorCode = 300;
            data.Add("registerSate", false);
        }
    }

    public class CustomerDetailMessage:Message
    {
        public CustomerDetailMessage()
        {
            errorCode = 400;
            data.Add("userNickName", null);
            data.Add("userAvatar", null);
            data.Add("evalNum", null);
            data.Add("userGroupLevel", null);
            data.Add("emailTag", false);
            data.Add("userScore", null);
            data.Add("registerDate", null);
            data.Add("hostCommentList", null);
        }
    }

    public class VCcodeMessage:Message
    {
        public VCcodeMessage()
        {
            errorCode = 300;
            data.Add("sendstate", false);
        }
    }

    public class AdminMessage:Message
    {
        AdminMessage()
        {
            errorCode = 300;
            data.Add("avatar", null);
            data.Add("ID", null);
            data.Add("name",null);
        }
    }

}
