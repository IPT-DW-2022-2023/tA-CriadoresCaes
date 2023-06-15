// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

using CriadorCaes.Data;
using CriadorCaes.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CriadorCaes.Areas.Identity.Pages.Account {
   public class RegisterModel : PageModel {
      private readonly SignInManager<IdentityUser> _signInManager;
      private readonly UserManager<IdentityUser> _userManager;
      private readonly IUserStore<IdentityUser> _userStore;
      private readonly IUserEmailStore<IdentityUser> _emailStore;
      private readonly ILogger<RegisterModel> _logger;
      private readonly IEmailSender _emailSender;
      /// <summary>
      /// referência à BD do projeto
      /// </summary>
      private readonly ApplicationDbContext _context;

      public RegisterModel(
          UserManager<IdentityUser> userManager,
          IUserStore<IdentityUser> userStore,
          SignInManager<IdentityUser> signInManager,
          ILogger<RegisterModel> logger,
          IEmailSender emailSender,
          ApplicationDbContext context) {
         _userManager = userManager;
         _userStore = userStore;
         _emailStore = GetEmailStore();
         _signInManager = signInManager;
         _logger = logger;
         _emailSender = emailSender;
         _context = context;
      }

      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
      [BindProperty]
      public InputModel Input { get; set; }

      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
      public string ReturnUrl { get; set; }

      /// <summary>
      ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
      ///     directly from your code. This API may change or be removed in future releases.
      /// </summary>
      public IList<AuthenticationScheme> ExternalLogins { get; set; }

      /// <summary>
      /// esta classe define o tipo de dados a recolher no Registo  
      /// </summary>
      public class InputModel {
         /// <summary>
         ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
         [Required]
         [EmailAddress]
         [Display(Name = "Email")]
         public string Email { get; set; }

         /// <summary>
         ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
         [Required]
         [StringLength(100, ErrorMessage = "Tem de escrever, na {0}, entre {2} e {1} caracteres.", MinimumLength = 6)]
         [DataType(DataType.Password)]
         [Display(Name = "Password")]
         public string Password { get; set; }

         /// <summary>
         ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
         ///     directly from your code. This API may change or be removed in future releases.
         /// </summary>
         [DataType(DataType.Password)]
         [Display(Name = "Confirm password")]
         [Compare("Password", ErrorMessage = "A password e a sua confirmação não coincidem.")]
         public string ConfirmPassword { get; set; }

         /// <summary>
         /// dados do criador a serem recolhidos no Registo
         /// </summary>
         public Criadores Criador { get; set; }

      } // fim da 'inner' classe InputModel



      /// <summary>
      /// método que reage a uma invocação em GET do HTTP
      /// </summary>
      /// <param name="returnUrl"></param>
      /// <returns></returns>
      public async Task OnGetAsync(string returnUrl = null) {
         ReturnUrl = returnUrl;
         ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
      }


      /// <summary>
      /// método que reage ao verbo POST do HTTP
      /// Irá inserir os dados de um novo utilizaador/criador
      /// </summary>
      /// <param name="returnUrl"></param>
      /// <returns></returns>
      public async Task<IActionResult> OnPostAsync(string returnUrl = null) {
         returnUrl ??= Url.Content("~/");
      //   ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
       
         
         // avalia se os dados do InputModel, que vêm da view, estão válidos
         if (ModelState.IsValid) {

            var user = CreateUser();

            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
         
            // criação do USER na bd
            var result = await _userManager.CreateAsync(user, Input.Password);


            // se houve sucesso na criação do USER
            if (result.Succeeded) {

               _logger.LogInformation("User created a new account with password.");

               // *************************************
               // adicionar os dados do CRIADOR
               // *************************************
               // corrigir os dados não inseridos automaticamente
               Input.Criador.Email = Input.Email;
               Input.Criador.UserId=user.Id;
               try {
                  _context.Add(Input.Criador);
                  await _context.SaveChangesAsync();
               }
               catch (Exception) {
                  // falta fazer o tratamento da exceção
                  // se houver erro, devemos eliminar o novo USER, por exemplo
                  throw;
               }

               // *************************************

               var userId = await _userManager.GetUserIdAsync(user);
               var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
               code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
               var callbackUrl = Url.Page(
                   "/Account/ConfirmEmail",
                   pageHandler: null,
                   values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                   protocol: Request.Scheme);

               await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                   $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

               if (_userManager.Options.SignIn.RequireConfirmedAccount) {
                  return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
               }
               else {
                  await _signInManager.SignInAsync(user, isPersistent: false);
                  return LocalRedirect(returnUrl);
               }
            }
            foreach (var error in result.Errors) {
               ModelState.AddModelError(string.Empty, error.Description);
            }
         }

         // If we got this far, something failed, redisplay form
         return Page();
      }

      private IdentityUser CreateUser() {
         try {
            return Activator.CreateInstance<IdentityUser>();
         }
         catch {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
         }
      }

      private IUserEmailStore<IdentityUser> GetEmailStore() {
         if (!_userManager.SupportsUserEmail) {
            throw new NotSupportedException("The default UI requires a user store with email support.");
         }
         return (IUserEmailStore<IdentityUser>)_userStore;
      }
   }
}
