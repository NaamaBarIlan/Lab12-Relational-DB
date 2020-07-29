﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Lab12_Relational_DB.Model;
using Lab12_Relational_DB.Model.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Lab12_Relational_DB.Controllers
{
    /// <summary>
    /// This API controller handles User Identity, 
    /// including user registration and user login
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private IConfiguration _config;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;

        }

        /// <summary>
        /// This custom API route handles user registration to the application
        /// while implementing a RegisterDTO
        /// </summary>
        /// <param name="register">A unique RegisterDTO object</param>
        /// <returns>Task result: either an Ok or a BadRequest message</returns>
        // api/account/register
        [HttpPost, Route("register")]
        public async Task<IActionResult> Register(RegisterDTO register)
        {   
            ApplicationUser user = new ApplicationUser()
            {
                Email = register.Email,
                UserName = register.Email,
                FirstName = register.FirstName,
                LastName = register.LastName
            };

            // Create the user
            var result = await _userManager.CreateAsync(user, register.Password);

            if(result.Succeeded)
            {
                if(user.Email == _config["DistrictManagerSeed"])
                {
                    await _userManager.AddToRoleAsync(user, ApplicationRoles.DistrictManager);
                }

                //sign the user in if it was successful.
                await _signInManager.SignInAsync(user, false);

                return Ok();
            }

            return BadRequest("Invalid Registration");

        }

        /// <summary>
        /// This custom API route handles user login to the application
        /// while implementing a LoginDTO
        /// </summary>
        /// <param name="login">A unique LoginDTO object</param>
        /// <returns>Task result: either an Ok or a BadRequest message</returns>
        [HttpPost, Route("Login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false);
             if(result.Succeeded)
            {
                // look the user up
                var user = await _userManager.FindByEmailAsync(login.Email);

                var token = CreateToken(user);

                // make them a token based on their account

                // send that JWT token back to the user

                // log the user in
                return Ok(new
                {
                    jwt = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });

            }

            return BadRequest("Invalid attempt");

        }

        [HttpPost, Route("assign/role")]
        [Authorize(Roles = ApplicationRoles.DistrictManager)]
        public async Task AssignRoleToUser(AssignRoleDTO assignment)
        {
            var user = await _userManager.FindByEmailAsync(assignment.Email);


            //validation here to confirm the role is valid
            //string role = "";
            //if(assignment.Role.ToUpper == "Advisor")
            //{
            //    role = ApplicationRoles.Advisor;
            //}

            await _userManager.AddToRoleAsync(user, assignment.Role);
        }

        private JwtSecurityToken CreateToken(ApplicationUser user)
        {
            // Token requires pieces of information called "claims"
            // Person/User is the principle
            // A principle can have many forms of identity
            // An identity contains many claims
            // a claim is a single statement about the user

            var authClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("UserId", user.Id)
            };
            var token = AuthenticateToken(authClaims);

            return token;
        }

        private JwtSecurityToken AuthenticateToken(Claim[] claims)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTKey"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWTIssuer"],
                audience: _config["JWTIssuer"],
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
    }
}
