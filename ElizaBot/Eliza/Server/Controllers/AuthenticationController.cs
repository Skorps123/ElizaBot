﻿using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Discord;
using Eliza.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Eliza.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        [HttpGet("login")]
        [HttpPost("logout")]
        public IActionResult LogIn()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, DiscordAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("logout")]
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }

        [HttpGet("userinfo")]
        public ActionResult<UserInfoDTO> GetUserInfoAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return Ok(new UserInfoDTO());

            var userInfo = new UserInfoDTO()
            {
                IsAuthenticated = User.Identity.IsAuthenticated
            };

            foreach (var claim in User.Claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.NameIdentifier:
                        userInfo.UserId = ulong.Parse(claim.Value);
                        break;

                    case ClaimTypes.Name:
                        userInfo.Username = claim.Value;
                        break;

                    case DiscordAuthenticationConstants.Claims.AvatarHash:
                        userInfo.AvatarHash = claim.Value;
                        break;

                    case Constants.IsBotOwner:
                        userInfo.Claims.Add(claim.Type,claim.Value);
                        break;
                }
            }

            return Ok(userInfo);
        }
    }
}