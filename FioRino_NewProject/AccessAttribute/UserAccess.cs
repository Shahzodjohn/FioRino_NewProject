using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.AccessAttribute
{
    public class UserAccess : IAuthorizationRequirement
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool? Hurt { get; set; }
        public bool? Magazyn { get; set; }
        public bool? Archive { get; set; }

        public virtual DmUser User { get; set; }
    }
    public class AccessHandler : AuthorizationHandler<UserAccess>
    {
        private readonly FioRinoBaseContext _context;

        public AccessHandler(FioRinoBaseContext context)
        {
            _context = context;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAccess requirement)
        {
            var User = _context.DmUsers.FirstOrDefault(x => x.Email == context.User.Identity.Name);
            var findUser = _context.DmUsersAccesses.FirstOrDefault(x => x.UserId == User.Id);
            if (findUser.Hurt == true && requirement.Hurt == true || findUser.Magazyn == true && requirement.Magazyn == true || findUser.Archive == true && requirement.Archive == true)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}