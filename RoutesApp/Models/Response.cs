﻿using Newtonsoft.Json;
using RoutesApp.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RoutesApp.Models
{
    public class Response
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int statusCode { get; set; }
    }


    public class SignInResponse : Response
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string token { get; set; }
    }

    public class QRUsersResponse : Response
    {
        public int userId { get; set; }
        public string email { get; set; }
    }

    public class SignInResponsez : Response
    {
        public LoginUser user { get; set; }
        public string Token { get; set; }
    }



    public class Pagination
    {
        public int offset { get; set; }
        public int limit { get; set; }
        public int total { get; set; }
    }

    public class PromotionsResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<Promotion> data { get; set; }
    }
    public class CouponGetResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<Coupon> data { get; set; }
        public CouponIncluded included { get; set; }
    }

    public class CouponResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<Coupon> data { get; set; }
        public Included included { get; set; }
    }
    public class CouponIncluded
    {
        public List<Promotion> promotions { get; set; }
    }
    public class Included
    {
        public List<Promotion> promotions { get; set; }
        public List<User> users { get; set; }
    }

    public class CouponListData
    {
        public string Id { get; set; }
        public Promotion Promotion { get; set; }
        public string QrCodeImage { get; set; }
        public int Count { get; set; }
    }

    public class CouponDetailData
    {
        public string Id { get; set; }
        public Promotion Promotion { get; set; }
        public User User { get; set; }
        public string QrCodeImage { get; set; }
    }

    public class RedemptionResponse : Response
    {
        public string RedemptionId { get; set; }
    }

    public class QRCodeAll
    {
        public string Id { get; set; }
        public Promotion Promotion { get; set; }
    }

    public class RedemptionGetResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<RedemptionGetModel> data { get; set; }
        public RedeemIncluded included { get; set; }
    }

    public class RedeemIncluded
    {
        public List<Officer> officers { get; set; }
        public List<CouponModel> coupons { get; set; }
        public List<User> users { get; set; }
    }

    public class RedeemModel
    {
        public string RedemptionId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CouponId { get; set; }
        public string OfficerId { get; set; }
        public CouponModel coupons { get; set; }
    }

    public class RedeemHistory
    {
        public string RedemptionId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string CouponId { get; set; }
        public string OfficerId { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string LogoUrl { get; set; }
    }

    public class RedeemDetailModel : RedeemModel
    {
        public User Users { get; set; }
    }

    public class RolesModel
    {
        public string ApplicationId { get; set; }
        public string PrivilegeId { get; set; }
    }

    public class OfficersResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<OfficersModel> data { get; set; }
        public OfficersIncluded included { get; set; }
    }

    public class OfficersModel
    {
        public string OfficerId { get; set; }
        public string UserId { get; set; }
        public string InstitutionId { get; set; }
    }

    public class OfficersIncluded
    {
        public List<User> Users { get; set; }
        public List<Institutions> users { get; set; }
    }

    public class Institutions
    {
        public string InstitutionId { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryIso { get; set; }
    }

    public class ErrorResponse
    {
        public List<ErrorDetails> errors { get; set; }
    }

    public class ErrorDetails
    {
        public int statusCode { get; set; }
        public string detail { get; set; }
        public int code { get; set; }
    }

    public class PromotionsGetResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<Promotion> data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JObject included { get; set; }
    }


    public class PromotionDetailsData
    {
        public Promotion promotion { get; set; }
        public string QrCodeImage { get; set; }
    }

    public class LinkResponse : Response
    {
        public Pagination pagination { get; set; }
        public List<LinksModel> data { get; set; }
    }

    public class LinksModel
    {
        public string LinkId { get; set; }
        public string PromotionId { get; set; }
        public string Web { get; set; }
        public string Ios { get; set; }
        public string Android { get; set; }
    }

}
