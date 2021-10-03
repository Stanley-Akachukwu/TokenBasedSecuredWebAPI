using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace UCare.Models.AppSettingsModel
{
    public class Policies
    {
        //policy names

        public const string IsAdmin = "IsAdmin";
        public const string IsUser = "IsUser";
        public const string IsSuperAdmin = "IsSuperAdmin";
        public const string IsAdminView = "IsAdminView";
        public const string IsAdminAction = "IsAdminAction";
        // policy claims
        public const string Admin = "Admin";
        public const string User = "User";
        public const string SuperAdmin = "SuperAdmin";
        public const string ViewUserList = "View users";
        public const string CreateRoleAction = "Create role";
        public const string DeleteRoleAction = "Delete role";
        public const string EditRoleAction = "Edit role";
        //public static AuthorizationPolicy IsAdminPolicy()
        //{
        //    return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
        //                                          .RequireClaim(Admin, "true")
        //                                           .Build();
        //}

        //public static AuthorizationPolicy IsUserPolicy()
        //{
        //    return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
        //                                           .RequireClaim(User, "true")
        //                                           .Build();
        //}
        //public static AuthorizationPolicy IsSuperAdminPolicy()
        //{
        //    return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
        //                                          .RequireRole("SuperAdmin")
        //                                          .RequireRole("Admin")
        //                                           .RequireRole("User")
        //                                           .Build();
        //}
        //public static AuthorizationPolicy IsAdminRolePolicy()
        //{
        //    return new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
        //                                          .RequireClaim(ClaimTypes.Role, "Admin")
        //                                          .RequireClaim("View users")
        //                                           .RequireClaim("Create role")
        //                                           .RequireClaim("Delete role")
        //                                           .RequireClaim("Edit role")
        //                                           .Build();
        //}
    }
}
