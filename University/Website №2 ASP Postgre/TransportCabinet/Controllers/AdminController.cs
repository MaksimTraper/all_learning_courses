using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using TransportCabinet.Models;
using TransportCabinet.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Route = TransportCabinet.Models.Route;
using TransportCabinet.Data;
using System.Data.Common;

namespace TransportCabinet.Controllers
{
    public class AdminController : Controller
    {
        TransportCabinetContext db;

        public AdminController(TransportCabinetContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult AdminPanel()
        {
            AllAboutUser user = GetAboutUser();
            return View(user);
        }


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


        [HttpGet]
        public IActionResult SQLQuery()
        {
            SqlQueryModel sqlQueryM = new SqlQueryModel(null, GetAboutUser(), null, 0);
            return View(sqlQueryM);
        }


        [HttpPost]
        public IActionResult SQLQuery(SqlQueryModel model)
        {
            int countColumns = 0;
            List<string> results = this.RawSqlQuery(model.query, ref countColumns);
            SqlQueryModel sqlQueryM = new SqlQueryModel(null, GetAboutUser(), results, countColumns);
            return View(sqlQueryM);
        }

        private List<string> RawSqlQuery(string query, ref int countColumns)
        {
            string firstW = null;
            List<string> results = new List<string>();
            try
            {
                firstW = query.Split(" ")[0];
            }
            catch
            {
                results.Add("Пустой запрос");
                return results;
            }


            try
            {
                if (firstW.Contains("Select", StringComparison.OrdinalIgnoreCase))
                {
                    using (var command = db.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = query;
                        command.CommandType = System.Data.CommandType.Text;

                        db.Database.OpenConnection();

                        using (var result = command.ExecuteReader())
                        {
                            int y = 0;

                            while (result.Read())
                            {
                                if (y == 0)
                                {
                                    for (int i = 0; i < result.FieldCount; i++)
                                    {
                                        results.Add(Convert.ToString(result.GetName(i)));
                                    }
                                    y = 1;
                                }
                                for (int i = 0; i < result.FieldCount; i++)
                                {
                                    results.Add(Convert.ToString(result.GetValue(i)));
                                }

                            }
                            countColumns = result.FieldCount;
                        }
                    }
                }
                else
                    results.Add("Доступен только запрос с SELECT");
            }
            catch (DbException ex)
            {
                results.Add("exc " + ex.Message.ToString());
            }
            return results;
        }

        /// <summary>
        /// Первоначальный вывод страницы смены данные
        /// </summary>
        /// <param name="except"></param>
        /// <param name="select"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditPage(string except = null, string select = null)
        {
            EditModel previousModel = (EditModel)cache.Get("EditModel");
            if (previousModel != null)
            {
                previousModel.users = db.UserAccounts.ToList();
                previousModel.drivers = db.Drivers.ToList();
                previousModel.routes = db.Routes.ToList();
                previousModel.transports = db.Transports.ToList();
                previousModel.transportCards = db.TransportCards.ToList();
                previousModel.transportVehicles = db.TransportVehicles.ToList();
                previousModel.trips = db.Trips.ToList();
                previousModel.purchases = db.Purchases.ToList();
                return View(previousModel);
            }
            return View(new EditModel { user = GetAboutUser(), exception = except, selected = select });
        }

        public IMemoryCache cache;

        /// <summary>
        /// Вывод таблицы данных во вкладке смены данных
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditPage(EditModel model)
        {
            model.users = db.UserAccounts.ToList();
            model.drivers = db.Drivers.ToList();
            model.routes = db.Routes.ToList();
            model.transports = db.Transports.ToList();
            model.transportCards = db.TransportCards.ToList();
            model.transportVehicles = db.TransportVehicles.ToList();
            model.trips = db.Trips.ToList();
            model.purchases = db.Purchases.ToList();
            model.user = GetAboutUser();

            if (model.submitButton != null)
            {
                string firstWord = null;
                int id = 0;

                try
                {
                    firstWord = model.submitButton.Split(" ")[0];
                    id = Convert.ToInt32(model.submitButton.Split(' ')[1].Substring(1));
                }
                catch { }

                switch (firstWord)
                {
                    case "Изменить": return RedirectToAction("EditPage2", "Admin", new { user = GetAboutUser(), selected = cache.Get(1), num = id });
                    case "Удалить": return RedirectToAction("Delete", "Admin", new { selected = cache.Get(1), num = id });
                    case "Добавить": return RedirectToAction("EditPageCreate", "Admin", new { selected = cache.Get(1) });
                }
            }
            cache.Set(1, model.selected, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            //List<UserAccount> users = db.UserAccounts.ToList(); для пополнений
            model.selected = Convert.ToString(cache.Get(1));
            cache.Set("EditModel", model, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            return View("EditPage", model);
        }

        /// <summary>
        /// Для смены данных
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangeDBInfo(IFormCollection form)
        {
            try
            {
                EditPage2Model oldModel = (EditPage2Model)cache.Get("EditModel2");

                if (Convert.ToString(cache.Get(1)) == "Водители")
                {
                    oldModel.driver.name = Convert.ToString(form["name"]);
                    oldModel.driver.surname = Convert.ToString(form["surname"]);
                    oldModel.driver.patronymic = Convert.ToString(form["patronymic"]);

                    db.Drivers.Update(oldModel.driver);
                }
                else if (Convert.ToString(cache.Get(1)) == "Маршруты")
                {
                    oldModel.route.start_point = Convert.ToString(form["start_point"]);
                    oldModel.route.end_point = Convert.ToString(form["end_point"]);

                    db.Routes.Update(oldModel.route);
                }
                else if (Convert.ToString(cache.Get(1)) == "Транспорт")
                {
                    oldModel.transport.fk_id_vehicle = Convert.ToInt32(form["fk_id_vehicle"]);

                    db.Transports.Update(oldModel.transport);
                }
                else if (Convert.ToString(cache.Get(1)) == "Транспортные карты")
                {
                    oldModel.card.fk_id_owner = Convert.ToString(form["fk_id_owner"]);
                    if (Convert.ToInt32(form["balance"]) >= 0)
                    {
                        oldModel.card.balance = Convert.ToInt32(form["balance"]);
                    }
                    if (Convert.ToInt32(form["num_days"]) >= 0)
                    {
                        oldModel.card.num_days = Convert.ToInt32(form["num_days"]);
                    }
                    oldModel.card.data_issue = DateOnly.FromDateTime((Convert.ToDateTime(form["data_issue"].ToString())));


                    db.TransportCards.Update(oldModel.card);
                }
                else if (Convert.ToString(cache.Get(1)) == "Поездки")
                {
                    oldModel.trip.fk_id_tr = Convert.ToString(form["fk_id_tr"]);
                    oldModel.trip.num_route = Convert.ToInt16(form["fk_id_card"]);
                    oldModel.trip.num_route = Convert.ToInt16(form["fk_id_driver"]);
                    oldModel.trip.time_pay = Convert.ToDateTime(form["time_pay"]);
                    oldModel.trip.num_route = Convert.ToInt16(form["num_route"]);

                    db.Trips.Update(oldModel.trip);
                }
                else if (Convert.ToString(cache.Get(1)) == "Пользователи")
                {
                    oldModel.userAc.passport_num = Convert.ToString(form["passport_num"]);
                    oldModel.userAc.name = Convert.ToString(form["name"]);
                    oldModel.userAc.surname = Convert.ToString(form["surname"]);
                    oldModel.userAc.patronymic = Convert.ToString(form["patronymic"]);
                    oldModel.userAc.password = Password.Hash(Convert.ToString(form["password"]));
                    oldModel.userAc.email = Convert.ToString(form["email"]);
                    if (Convert.ToString(form["role"]) == "admin" || Convert.ToString(form["role"]) == "user")
                    {
                        oldModel.userAc.role = Convert.ToString(form["role"]);
                    }
                    oldModel.userAc.birthday = DateOnly.FromDateTime((Convert.ToDateTime(form["birthday"])));

                    db.UserAccounts.Update(oldModel.userAc);
                }
                else if (Convert.ToString(cache.Get(1)) == "Модели транспорта")
                {
                    oldModel.vehicle.brand = Convert.ToString(form["brand"]);
                    oldModel.vehicle.model = Convert.ToString(form["model"]);

                    db.TransportVehicles.Update(oldModel.vehicle);
                }
                else if (Convert.ToString(cache.Get(1)) == "Случаи пополнения")
                {
                    oldModel.purchase.pk_num_purchase = Convert.ToInt16(form["pk_num_purchase"]);
                    oldModel.purchase.fk_id_card = Convert.ToInt16(form["fk_id_card"]);
                    oldModel.purchase.name_purchase = Convert.ToString(form["name_purchase"]);
                    oldModel.purchase.price = Convert.ToDecimal(form["price"]);
                    oldModel.purchase.amount = Convert.ToInt16(form["amount"]);

                    db.Purchases.Update(oldModel.purchase);
                }

                db.SaveChanges();
            }
            catch { }
            return RedirectToAction("EditPage", "Admin");
        }

        [HttpPost]
        public IActionResult CreateDBInfo(IFormCollection form)
        {
            try
            {
                if (Convert.ToString(cache.Get(1)) == "Водители")
                {
                    Driver driver = new Driver();

                    driver.name = Convert.ToString(form["name"]);
                    driver.surname = Convert.ToString(form["surname"]);
                    driver.patronymic = Convert.ToString(form["patronymic"]);

                    db.Drivers.Add(driver);
                }
                else if (Convert.ToString(cache.Get(1)) == "Маршруты")
                {
                    Route route = new Route();

                    route.pk_id_route = Convert.ToInt16(form["pk_id_route"]);
                    route.start_point = Convert.ToString(form["start_point"]);
                    route.end_point = Convert.ToString(form["end_point"]);

                    db.Routes.Add(route);
                }
                else if (Convert.ToString(cache.Get(1)) == "Транспорт")
                {
                    Transport transport = new Transport();

                    transport.pk_car_num = Convert.ToString(form["pk_car_num"]);
                    transport.fk_id_vehicle = Convert.ToInt32(form["fk_id_vehicle"]);

                    db.Transports.Add(transport);
                }
                else if (Convert.ToString(cache.Get(1)) == "Транспортные карты")
                {
                    TransportCard card = new TransportCard();

                    card.fk_id_owner = Convert.ToString(form["fk_id_owner"]);
                    card.balance = Convert.ToInt32(form["balance"]);
                    card.data_issue = DateOnly.FromDateTime((Convert.ToDateTime(form["data_issue"].ToString())));

                    db.TransportCards.Add(card);
                }
                else if (Convert.ToString(cache.Get(1)) == "Поездки")
                {
                    Trip trip = new Trip();

                    trip.fk_id_tr = Convert.ToString(form["fk_id_tr"]);
                    trip.num_route = Convert.ToInt16(form["fk_id_card"]);
                    trip.num_route = Convert.ToInt16(form["fk_id_driver"]);
                    trip.time_pay = Convert.ToDateTime(form["time_pay"]);
                    trip.num_route = Convert.ToInt16(form["num_route"]);

                    db.Trips.Add(trip);
                }
                else if (Convert.ToString(cache.Get(1)) == "Пользователи")
                {
                    UserAccount userAc = new UserAccount();

                    userAc.name = Convert.ToString(form["name"]);
                    userAc.surname = Convert.ToString(form["surname"]);
                    userAc.patronymic = Convert.ToString(form["patronymic"]);
                    userAc.pk_login = Convert.ToString(form["pk_login"]);
                    userAc.passport_num = Convert.ToString(form["passport_num"]);
                    userAc.password = Password.Hash(Convert.ToString(form["password"]));
                    userAc.email = Convert.ToString(form["email"]);
                    userAc.role = Convert.ToString(form["role"]);
                    userAc.birthday = DateOnly.FromDateTime((Convert.ToDateTime(form["birthday"])));

                    db.UserAccounts.Add(userAc);
                }
                else if (Convert.ToString(cache.Get(1)) == "Модели транспорта")
                {
                    TransportVehicle vehicle = new TransportVehicle();

                    vehicle.brand = Convert.ToString(form["brand"]);
                    vehicle.model = Convert.ToString(form["model"]);

                    db.TransportVehicles.Add(vehicle);
                }
                else if (Convert.ToString(cache.Get(1)) == "Случаи пополнения")
                {
                    Purchase purchase = new Purchase();

                    purchase.pk_num_purchase = Convert.ToInt16(form["pk_num_purchase"]);
                    purchase.fk_id_card = Convert.ToInt16(form["fk_id_card"]);
                    purchase.name_purchase = Convert.ToString(form["name_purchase"]);
                    purchase.price = Convert.ToInt16(form["price"]);
                    purchase.amount = Convert.ToInt16(form["amount"]);

                    db.Purchases.Add(purchase);
                }

                db.SaveChanges();
            }
            catch { }
            return RedirectToAction("EditPage", "Admin");
        }

        [HttpGet]
        public IActionResult EditPage2(string selected, int num)
        {
            Driver driver = null; Route route = null; Transport transport = null; TransportCard card = null;
            Trip trip = null; UserAccount user = null; TransportVehicle vehicle = null; Purchase purchase = null;

            if (selected == "Водители")
            {
                driver = db.Drivers.ToList()[num - 1];
            }
            else if (selected == "Маршруты")
            {
                route = db.Routes.ToList()[num - 1];
            }
            else if (selected == "Транспорт")
            {
                transport = db.Transports.ToList()[num - 1];
            }
            else if (selected == "Транспортные карты")
            {
                card = db.TransportCards.ToList()[num - 1];
            }
            else if (selected == "Поездки")
            {
                trip = db.Trips.ToList()[num - 1];
            }
            else if (selected == "Пользователи")
            {
                user = db.UserAccounts.ToList()[num - 1];
            }
            else if (selected == "Модели транспорта")
            {
                vehicle = db.TransportVehicles.ToList()[num - 1];
            }
            else if (selected == "Случаи пополнения")
            {
                purchase = db.Purchases.ToList()[num - 1];
            }
            EditPage2Model model = new EditPage2Model(GetAboutUser(), selected, num, user, driver, route, transport, card, vehicle, trip, purchase);
            cache.Set("EditModel2", model, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            return View(model);
        }

        public IActionResult Delete(string selected, int num)
        {
            Driver driver = null; Route route = null; Transport transport = null; TransportCard card = null;
            Trip trip = null; UserAccount user = null; TransportVehicle vehicle = null; Purchase purchase = null;
            string mes = null;
            try
            {
                if (selected == "Водители")
                {
                    db.Drivers.Remove(driver = (db.Drivers.ToList()[num - 1]));
                }
                else if (selected == "Маршруты")
                {
                    db.Routes.Remove(route = db.Routes.ToList()[num - 1]);
                }
                else if (selected == "Транспорт")
                {
                    db.Transports.Remove(transport = db.Transports.ToList()[num - 1]);
                }
                else if (selected == "Транспортные карты")
                {
                    db.TransportCards.Remove(card = db.TransportCards.ToList()[num - 1]);
                }
                else if (selected == "Поездки")
                {
                    db.Trips.Remove(trip = db.Trips.ToList()[num - 1]);
                }
                else if (selected == "Пользователи")
                {
                    db.UserAccounts.Remove(user = db.UserAccounts.ToList()[num - 1]);
                }
                else if (selected == "Модели транспорта")
                {
                    db.TransportVehicles.Remove(vehicle = db.TransportVehicles.ToList()[num - 1]);
                }
                else if (selected == "Случаи пополнения")
                {
                    db.Purchases.Remove(purchase = db.Purchases.ToList()[num - 1]);
                }
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                mes = ex.InnerException.Message;
            }

            if (mes == null)
            {
                return RedirectToAction("EditPage", "Admin");
            }
            return RedirectToAction("EditPage", "Admin", new { except = mes });
        }

        public IActionResult EditPageCreate()
        {
            return View(new EditPage2Model { user = GetAboutUser(), selected = Convert.ToString(cache.Get(1)) });
        }
    }
}
