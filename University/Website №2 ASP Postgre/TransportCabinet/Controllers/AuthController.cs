using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using TransportCabinet.Models;
using Microsoft.AspNetCore.Authentication;
using TransportCabinet.ViewModels;
using TransportCabinet.Data;

namespace TransportCabinet.Controllers
{
    public class AuthController : Controller
	{
		TransportCabinetContext db;
		public AuthController(TransportCabinetContext context)
		{
			db = context;
		}

		[HttpGet]
		public ActionResult SignupForm() => View();


		[HttpGet]
		public ActionResult LoginForm() => View();


		/// <summary>
		/// Метод регистрации
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> SignupForm(SignupModel model)
		{
			var context = HttpContext;
			if (ModelState.IsValid)
			{
				UserAccount user = null;
				// Три проверки на существование логина и E-mail среди зарегистрированных пользователей
				user = db.UserAccounts.FirstOrDefault(u => u.email == model.Email && u.pk_login == model.Login);
				if (user != null)
				{
					ModelState.AddModelError(string.Empty, "Пользователь с таким логином и E-mail уже существует");
					return View(model);
				}
				user = db.UserAccounts.FirstOrDefault(u => u.pk_login == model.Login);
				if (user != null)
				{
					ModelState.AddModelError(string.Empty, "Пользователь с таким логином уже существует");
					return View(model);
				}
				user = db.UserAccounts.FirstOrDefault(u => u.email == model.Email);
				if (user != null) 
				{
					ModelState.AddModelError(string.Empty, "Пользователь с таким E-mail уже существует");
					return View(model);
				}
                user = db.UserAccounts.FirstOrDefault(u => u.passport_num == model.passport_num);
                if (user != null)
                {
                    ModelState.AddModelError(string.Empty, "Пользователь с таким паспортом уже существует");
                    return View(model);
                }

                // Проверки пройдены, регистрация нового пользователя возможна
                if (user == null)
				{
					try
					{
						Convert.ToInt64(model.passport_num);
					}
					catch
					{
                        ModelState.AddModelError(string.Empty, "ID паспорта должно состоять только из цифр");
                        return View(model);
                    }
					// создаем нового пользователя
					int maxId = db.TransportCards.OrderBy(u => u.pk_id_card).LastOrDefault().pk_id_card;
					db.UserAccounts.Add(new UserAccount { email = model.Email, password = Password.Hash(model.Password), pk_login = model.Login, passport_num = model.passport_num, role = "user"});
					db.SaveChanges();
					db.TransportCards.Add(new TransportCard(model.Login, maxId + 1));
					db.SaveChanges();
					user = db.UserAccounts.Where(u => u.email == model.Email && u.password == Password.Hash(model.Password)).FirstOrDefault();
					// если пользователь удачно добавлен в бд
					if (user != null)
					{
						var claims = new List<Claim>
						{
							new Claim(ClaimsIdentity.DefaultNameClaimType, user.pk_login),
							new Claim(ClaimsIdentity.DefaultRoleClaimType, user.role)
						};
						// создаем объект ClaimsIdentity
						//ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType
						ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
						// установка аутентификационных куки
						await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
						return RedirectToAction("PrivateCab", "Home");
					}
				}
			}
			return View(model);
		}

		/// <summary>
		/// Метод входа в аккаунт
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> LoginForm(LoginModel model)
		{
			var context = HttpContext;
			// Если ошибок в введённых данных нет
			if (ModelState.IsValid)
			{
				// поиск пользователя в бд
				UserAccount user = null;
				user = db.UserAccounts.FirstOrDefault(u => u.pk_login == model.login && u.password == Password.Hash(model.password));
				if (user != null)
				{
					var claims = new List<Claim>
						{
							new Claim(ClaimsIdentity.DefaultNameClaimType, user.pk_login),
							new Claim(ClaimsIdentity.DefaultRoleClaimType, user.role)
						};
					// создаем объект ClaimsIdentity
					ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
					// установка аутентификационных куки
					await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
					return RedirectToAction("PrivateCab", "Home");
				}
				else
				{
					ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
				}
			}
			return View(model);
		}

		/// <summary>
		/// Выход из аккаунта
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult> Logout()
		{
			var context = HttpContext;
			await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("StartPage", "Home");
		}
	}
}
