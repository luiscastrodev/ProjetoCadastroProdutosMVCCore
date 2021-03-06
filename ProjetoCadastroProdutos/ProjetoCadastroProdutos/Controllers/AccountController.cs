﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjetoCadastroFuncionarios.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoCadastroFuncionarios.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Copia os dados do RegisterViewModel para o IdentityUser
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Cidade = model.Cidade

                };
                // Armazena os dados do usuário na tabela AspNetUsers
                var result = await userManager.CreateAsync(user, model.Password);
                // Se o usuário foi criado com sucesso, faz o login do usuário
                // usando o serviço SignInManager e redireciona para o Método Action Index
                if (result.Succeeded)
                {
                    // Se o usuário estiver logado e for do perfil Admin
                    // então o usuário é o usuário Admin que esta criando
                    // um novo usuário, assim vamos direcioná-lo para o
                    // usuário Admin para a Action ListRoles
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Adminstration");
                    }
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }
                // Se houver erros então inclui no ModelState
                // que será exibido pela tag helper summary na validação
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");
                }
                ModelState.AddModelError(string.Empty, "Login Inválido");
            }
            return View(model);
        }

        //Prevenindo os ataques de redirecionamento abertos
        public IActionResult Login(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("index", "home");
            }
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"O Email {email} já está sendo usado.");
            }
        }
    }
}