﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Threading;
using Aliyun.OSS.Common;
using System.Text;
using Aliyun.OSS.Util;
using System.Security.Cryptography;
using Aliyun.OSS;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;



//简单测试
namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ModelContext myContext;
        public CustomerController(ModelContext modelContext)
        {
            myContext = modelContext;
        }


        public static bool CustomerLogin(Customer customer, string password)
        {
            try
            {
                if (customer == null)
                {
                    return false;
                }
                return customer.CustomerPassword == password;
            }
            catch
            {
                return false;
            }
        }


        [HttpPost("phone")]
        public string CheckCustomerPhoneRegisitered()
        {
            CheckPhoneMessage checkPhoneMessage = new CheckPhoneMessage();
            string phone = Request.Form["phonenumber"];
            string prePhone = Request.Form["prenumber"];
            if (phone != null && prePhone != null)
            {
                checkPhoneMessage.errorCode = 200;
                if (SearchByPhone(phone, prePhone) == null)
                {
                    checkPhoneMessage.data["phoneunique"] = true;
                }
            }
            return checkPhoneMessage.ReturnJson();
        }



        public static Customer SearchByPhone(string phone, string prePhone)
        {
            try
            {
                ModelContext context = new ModelContext();
                var customer = context.Customers
                    .Single(b => b.CustomerPhone == phone && b.CustomerPrephone == prePhone);
                return customer;
            }
            catch
            {
                return null;
            }

        }
        public static Customer SearchById(int id)
        {
            try
            {
                ModelContext context = new ModelContext();
                var customer = context.Customers
                    .Single(b => b.CustomerId == id);
                return customer;
            }
            catch
            {
                return null;
            }

        }

        class CommentInfo
        {
            public string comment { get; set; }
            public decimal customerStars { get; set; }
            public DateTime commentTime { get; set; }
            public string hostNickName { get; set; }
            public string hostAvatar { get; set; }
            public DateTime hostRegisterDate { get; set; }
            public int hostId { get; set; }
        }

        [HttpGet("details")]
        public string GetCustomerDetails()
        {
            CustomerDetailMessage customerDetailMessage = new CustomerDetailMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                customerDetailMessage.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var customer = SearchById(id);
                    ICollection<Order> orders = customer.Orders;
                    List<CommentInfo> comments = new List<CommentInfo>();
                    foreach (var order in orders)
                    {
                        if(order.HostComment!=null)
                        {
                            var host = order.Generates.First().Room.Stay.Host;
                            comments.Add(new CommentInfo
                            {
                                comment = order.HostComment.HostComment1,
                                customerStars = order.HostComment.CustomerStars,
                                commentTime = order.HostComment.CommentTime,
                                hostId = (int)host.HostId,
                                hostAvatar = host.HostAvatar,
                                hostNickName = host.HostUsername,
                                hostRegisterDate = (DateTime)host.HostCreateTime
                            });
                        }
                    }
                    if (customer != null)
                    {
                        customerDetailMessage.errorCode = 200;
                        customerDetailMessage.data["userNickName"] = customer.CustomerName;
                        customerDetailMessage.data["userAvatar"] = customer.CustomerPhoto;
                        customerDetailMessage.data["evalNum"] = comments.Count;
                        customerDetailMessage.data["userGroupLevel"] = customer.CustomerLevel;
                        customerDetailMessage.data["emailTag"] = customer.CustomerEmail != null;
                        customerDetailMessage.data["userScore"] = customer.CustomerDegree;
                        customerDetailMessage.data["registerDate"] = customer.CustomerCreatetime;
                        customerDetailMessage.data["hostCommentList"] = comments;
                        customerDetailMessage.data["mood"] = customer.CustomerMood;
                        customerDetailMessage.data["userBirthDate"] = customer.CustomerBirthday;
                        customerDetailMessage.data["userSex"] = customer.CustomerGender;
                    }

                }
            }
            return customerDetailMessage.ReturnJson();
        }


        [HttpPut("avatar")]
        public string ChangeCustomerPhoto()
        {
            Message message = new Message();
            message.errorCode = 400;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    myContext.DetachAll();
                    int id = int.Parse(data["id"]);
                    var customer = SearchById(id);
                    myContext.Entry(customer).State = EntityState.Unchanged;
                    string photo = Request.Form["avatarCode"];
                    //Console.WriteLine(photo + "200");
                    if (photo != null)
                    {
                        try
                        {
                            string newPhoto = PhotoUpload.UploadPhoto(photo, "customerPhoto/" + id.ToString());
                            if (newPhoto != null)
                            {
                                customer.CustomerPhoto = newPhoto;
                                myContext.SaveChanges();
                                message.errorCode = 200;
                            }
                        }
                        catch
                        {

                        }
                    }

                }
            }
            return message.ReturnJson();
        }

        [HttpPut("basicinfo")]
        public string ChangeCustomerInfo()
        {
            Message message = new Message();
            message.errorCode = 400;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    myContext.DetachAll();
                    int id = int.Parse(data["id"]);
                    var customer = SearchById(id);
                    myContext.Entry(customer).State = EntityState.Unchanged;
                    string sex = Request.Form["userSex"];
                    DateTime birthday;
                    customer.CustomerName = Request.Form["userNickName"];
                    if (sex != null)
                    {
                        customer.CustomerGender = sex;
                    }
                    if (DateTime.TryParse(Request.Form["userBirthDate"], out birthday))
                    {
                        customer.CustomerBirthday = birthday;
                    }
                    string test = Request.Form["mood"].ToString();
                    if (Request.Form["mood"].ToString() != "")
                    {
                        customer.CustomerMood = decimal.Parse(Request.Form["mood"]);
                    }
                    try
                    {
                        message.errorCode = 200;
                        myContext.SaveChanges();
                    }
                    catch
                    {

                    }

                }
            }
            return message.ReturnJson();
        }

        [HttpPost("changepassword")]
        public string ChangeCustomerPassword()
        {
            ChangePasswordMessage message = new ChangePasswordMessage();
            string phone = Request.Form["phone"];
            string preNumber = Request.Form["prenumber"];
            string password = Request.Form["password"];
            if (phone!=null&&preNumber!=null)
            {
                myContext.DetachAll();
                var customer = SearchByPhone(phone, preNumber);
                myContext.Entry(customer).State = EntityState.Unchanged;
                if (password != null)
                {
                    message.errorCode = 200;
                    customer.CustomerPassword = password;
                    try
                    {
                        myContext.SaveChanges();
                        message.data["changestate"] = true;
                    }
                    catch
                    {

                    }

                }
            }
            else
            {
                StringValues token = default(StringValues);
                if (Request.Headers.TryGetValue("token", out token))
                {
                    message.errorCode = 300;
                    var data = Token.VerifyToken(token);
                    if (data != null)
                    {
                        myContext.DetachAll();
                        int id = int.Parse(data["id"]);
                        var customer = SearchById(id);
                        if (password != null)
                        {
                            message.errorCode = 200;
                            customer.CustomerPassword = password;
                            try
                            {
                                myContext.SaveChanges();
                                message.data["changestate"] = true;
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            return message.ReturnJson();
        }
    }

}
