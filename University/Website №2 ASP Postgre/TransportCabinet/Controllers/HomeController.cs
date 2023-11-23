using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using TransportCabinet.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography.X509Certificates;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Http.HttpResults;
using Route = TransportCabinet.Models.Route;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TransportCabinet.Models;
using TransportCabinet.Data;

namespace TransportCabinet.Controllers
{
    public class HomeController : Controller
    {
        TransportCabinetContext db;


        public HomeController(TransportCabinetContext context)
        {
            db = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(Index);
        }

        [HttpGet]
        public IActionResult StartPage()
        {
            if (User.Identity.IsAuthenticated)
            {
                AllAboutUser userTransportCard = GetAboutUser();
                return View(userTransportCard);
            }
            return View();
        }


        [HttpGet]
        public IActionResult TransportVehicles()
        {
            List<AllAboutCar> cars = new List<AllAboutCar>();
            List<Transport> allCars = db.Transports.ToList();
            for (int i = 0; allCars.Count > i; i++)
            {
                cars.Add(new AllAboutCar { transportVehicle = null, car = allCars[i] });
                cars[i].SetVehicle(db.TransportVehicles.Where(u => u.pk_id_vehicle == cars[i].car.fk_id_vehicle).FirstOrDefault());
            }
            AllAboutUser userTransportCard = GetAboutUser();
            VehiclesView view = new VehiclesView(cars, userTransportCard);
            return View(view);
        }


        [HttpGet]
        public IActionResult Unavailable() => View();


        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public IActionResult History()
        {
            AllAboutUser userTransportCard = GetAboutUser();
            List<Trip> trips = db.Trips.Where(u => u.fk_id_card == userTransportCard.GetCard().pk_id_card).ToList();
            List<Driver> drivers = db.Drivers.ToList();
            HistoryView history = new HistoryView(trips, userTransportCard, drivers);
            return View(history);
        }


        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public IActionResult PurchasesHistory()
        {
            AllAboutUser user = GetAboutUser();
            List<Purchase> purchases = db.Purchases.Where(u => u.fk_id_card == GetAboutUser().GetCard().pk_id_card).ToList();
            PurchasesView history = new PurchasesView(purchases, user);
            return View(history);
        }


        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public IActionResult BuyCard(BuyCardModel model)
        {
            model.user = GetAboutUser();
            return View(model);
        }


        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 0)]
        [Authorize(Roles = "user, admin")]
        [HttpGet]
        public IActionResult PrivateCab(string message)
        {
            AllAboutUser userTransportCard = GetAboutUser();
            if (message != null)
            { return View(new PrivateCabModel(userTransportCard, message)); }
            return View(new PrivateCabModel(userTransportCard, null));
        }


        /// <summary>
        /// Загрузка аватара
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult LoadPhoto()
        {
            try
            {
                var context = HttpContext;

                var response = context.Response;
                var request = context.Request;

                IFormFileCollection files = request.Form.Files;

                var exePath = AppDomain.CurrentDomain.BaseDirectory;
                exePath = exePath.Replace("\\bin\\Debug\\net7.0", "");
                string filePath = Path.Combine(exePath, "wwwroot\\Content\\img\\");
                filePath = filePath.Replace('\\', '/');
                string userLogin = User.Identity.Name + "avatar.png";
                filePath = string.Concat(filePath, userLogin);

                foreach (var file in files)
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
            }
            catch { }
            return RedirectToAction("PrivateCab", "Home");
        }


        /// <summary>
        /// Смена информации о себе (актуальном пользователе)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangeInfo(ChangeInfoModel model)
        {
            try
            {
                string outMessage = null;
                AllAboutUser user = GetAboutUser();
                UserAccount userAcc = user.GetUser();

                UserAccount userDb = db.UserAccounts.FirstOrDefault(u => u.passport_num == userAcc.passport_num);

                if (model.name != null)
                {
                    userDb.name = model.name;
                }
                if (model.surname != null)
                {
                    userDb.surname = model.surname;
                }
                if (model.patronymic != null)
                {
                    userDb.patronymic = model.patronymic;
                }
                if (model.email != null)
                {
                    userDb.email = model.email;
                }
                if (model.birthday != null)
                {
                    userDb.birthday = model.birthday;
                }

                if (userDb != null)
                {
                    db.UserAccounts.Update(userDb);
                    db.SaveChanges();
                }
            }
            catch { }
            return RedirectToAction("PrivateCab", "Home", new { message = "Данные успешно изменены" });
        }



        /// <summary>
        /// Поменять свой пароль
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            string outMessage = null;
            // Если ошибок в введённых данных нет
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                UserAccount user = GetAboutUser().GetUser();
                if (user != null)
                {
                    UserAccount userDb = db.UserAccounts.FirstOrDefault(u => u.passport_num == user.passport_num);
                    if (Password.Hash(model.password) != user.password)
                    {
                        return RedirectToAction("PrivateCab", "Home", new { message = "Актуальный пароль введён неверно!" });
                    }
                    userDb.password = Password.Hash(model.newPassword);
                    db.UserAccounts.Update(userDb);
                    db.SaveChanges();
                    return RedirectToAction("PrivateCab", "Home", new { message = "Пароль успешно изменён" });
                }
            }
            else if (model.password == null || model.confirmPassword == null || model.newPassword == null)
            {
                outMessage = "Не все поля заполнены";
            }
            else if (model.password != model.confirmPassword)
            {
                outMessage = "Пароли не совпадают";
            }
            return RedirectToAction("PrivateCab", "Home", new { message = outMessage });
        }


        /// <summary>
        /// Получить информацию об актульном юзере (о своём аккаунте)
        /// </summary>
        /// <returns></returns>
        public AllAboutUser GetAboutUser()
        {
            AllAboutUser userTransportCard = null;
            UserAccount user = null;
            user = db.UserAccounts.FirstOrDefault(u => u.pk_login == User.Identity.Name);
            if (user == null)
            {
                AuthController f = new AuthController(db);
                return null;
            }

            TransportCard card = user.GetTransportCard(db);

            var exePath = AppDomain.CurrentDomain.BaseDirectory;
            exePath = exePath.Replace("\\bin\\Debug\\net7.0", "");
            string filePath = Path.Combine(exePath, "wwwroot\\Content\\img");
            string[] files = Directory.GetFiles(filePath, Convert.ToString(user.pk_login) + "avatar.png");
            if (files.Length != 0)
            {
                string photo = files[0];
                photo = photo.Replace(exePath + "wwwroot\\", "");
                photo = photo.Replace('\\', '/');
                photo = photo.Insert(0, "../../");
                userTransportCard = new AllAboutUser(user, card, photo);
            }
            else
            {
                userTransportCard = new AllAboutUser(user, card);
            }
            return userTransportCard;
        }


        /// <summary>
        /// Пополнение проездного
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddBalance(BuyCardModel model)
        {
            AllAboutUser user = GetAboutUser();
            UserAccount userAcc = user.GetUser();
            TransportCard curUs = db.TransportCards.FirstOrDefault(u => u.fk_id_owner == userAcc.pk_login);

            if (Convert.ToDouble(model.balance) > 0 & Convert.ToDouble(model.balance) <= 100000)
            {
                curUs.balance += Convert.ToDouble(model.balance);
                db.TransportCards.Update(curUs);
                int pk_num_purchase = db.Purchases.OrderBy(u => u.pk_num_purchase).LastOrDefault().pk_num_purchase;
                db.Purchases.Add(new Purchase(pk_num_purchase + 1, curUs.pk_id_card, "add balance", Convert.ToDecimal(model.balance), 0));
                db.SaveChanges();
            }
            return RedirectToAction("BuyCard", "Home");
        }

        public IActionResult BuyTripCard(BuyCardModel model)
        {
            TransportCard curUs = db.TransportCards.Where(u => u.fk_id_owner == GetAboutUser().GetCard().fk_id_owner).FirstOrDefault();
            double balance = curUs.balance;
            if (balance >= model.price & model.price > 0)
            {
                TransportCard card = GetAboutUser().GetCard();
                card.balance -= model.price;
                card.num_days += model.days;
                //card.days += model.days;
                db.Update(card);
                int pk_num_purchase = db.Purchases.OrderBy(u => u.pk_num_purchase).LastOrDefault().pk_num_purchase;
                db.Purchases.Add(new Purchase(pk_num_purchase + 1, curUs.pk_id_card, "buy days", Convert.ToDecimal(model.price), model.days));
                db.SaveChanges();
            }
            else
            {
                return RedirectToAction("BuyCard", "Home", new BuyCardModel { message = "Недостаточно денег на балансе!" });
            }
            return RedirectToAction("BuyCard", "Home", new BuyCardModel { message = "Покупка прошла успешно!" });
        }
    }
}
