using mothercare.Data;
using mothercare.Helper;
using mothercare.Models.Home;
using mothercare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace mothercare.Controllers
{
    public class HomeController : Controller
    {
        dbmothercareEntities db = new dbmothercareEntities();
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        public ActionResult Index(string search, int? page)
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(search, 16, page));
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        public ActionResult Login()
        {
            if(Session["Username"]==null)
            {
                return View();
            }
            else
            {
                return Redirect("Index");
            }
        }
        public ActionResult UserCreate()
        {
            if (Session["Username"] == null)
            {
                return View();
            }
            else
            {
                return Redirect("Index");
            }
        }

        public JsonResult CheckLogin(string username, string password)
        {
            string md5StringPassword = AppHelper.GetMd5Hash(password);
            var dataItem = db.Tbl_Members.Where(x => x.EmailId == username && x.Password == md5StringPassword).SingleOrDefault();
           
            string isLogged = "";
            if (dataItem != null)
            { 
                Session["Username"] = dataItem.EmailId;
                var roleitem = db.Tbl_MemberRole.Where(y => y.memberId == dataItem.MemberId).SingleOrDefault();
                if (roleitem.RoleId==1)
                {
                    Session["Role"] = "Admin";
                }
                else
                {
                    Session["Role"] = "User";
                }
                if(dataItem.IsActive==false)
                {
                    Session["Username"] = null;
                    Session["Role"] = null;
                    isLogged = "email";
                }
                else
                {
                    isLogged = "all";
                }
            }
            else
            {
                isLogged = "none";
            }
            return Json(isLogged, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveUser(Tbl_Members user)
        {
            bool isSuccess = true;
            try
            {
                var random = new Random();
                int randomnumber = random.Next();
                user.EmailHash= AppHelper.GetMd5Hash(randomnumber.ToString());
                user.IsActive = false;
                user.Password = AppHelper.GetMd5Hash(user.Password);
                db.Tbl_Members.Add(user);
                db.SaveChanges();
                Tbl_MemberRole role = new Tbl_MemberRole();
                role.memberId = user.MemberId;
                role.RoleId = 2;
                db.Tbl_MemberRole.Add(role);
                db.SaveChanges();
                MailMessage msg = new MailMessage();
                System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
                string link = "http://mothercare.somee.com/Home/Verify?email=" + user.EmailId+"&hash=" + user.EmailHash;
                try
                {
                    msg.Subject = "Regarding Registration";
                    msg.Body = "<h1 style=" + "\"background-image: linear-gradient(to top, rgba(255,192,203,0.5), rgba(255,192,203,1));text-align: center;font - family: 'Montserrat'; font - size: 22px;\"> Mother Care </h1>"+
                                "<p> Greetings! </p>"+
                                "<p> Please Confirm Your Account! </p>"+
                                   "<a href=\"" + link+ "\">Click Here!</a>";
                    msg.From = new MailAddress("mother.care.aust@gmail.com", "Mother Care");
                    msg.To.Add(user.EmailId);
                    msg.IsBodyHtml = true;
                    client.Host = "smtp.gmail.com";
                    System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("mother.care.aust@gmail.com", "wluewaekokkgffui");
                    client.Port = int.Parse("587");
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = basicauthenticationinfo;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                isSuccess = false;
            }

            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Verify(string url)
        {
            string email = Request.QueryString["email"];
            string hash= Request.QueryString["hash"];
            var dataItem = db.Tbl_Members.Where(x => x.EmailId == email && x.EmailHash == hash).SingleOrDefault();
            if(dataItem!=null)
            {
                dataItem.IsActive = true;
                db.SaveChanges();
                ViewBag.Message = "Your account has been confirmed! Please Login.";
                ViewBag.Status = 1;
            }
            else
            {
                ViewBag.Message = "Anything wrong?";
                ViewBag.Status = 0;
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Location()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["Username"] = null;
            Session["Role"] = null;
            return RedirectToAction("Login");
        }
        public ActionResult AddToCart(int productId, string url)
        {
            if(Session["cart"]==null)
            {
                List<Item> cart = new List<Item>();
                var product = db.Tbl_Product.Find(productId);
                cart.Add(new Item()
                {
                    Product = product,
                    Quantity = 1
                });
                Session["cart"] = cart;

            }
            else
            {
                List<Item> cart =(List<Item>)Session["cart"];
                var product = db.Tbl_Product.Find(productId);
                int count = cart.Count();
                int flag = 0;
                foreach (var item in cart.ToList())
                {
                    if(item.Product.ProductId==productId)
                    {
                        int prevQty = item.Quantity;
                        cart.Remove(item);
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = prevQty+1
                        });
                        flag = 1;
                        break;
                    }
                    else
                    {
                        
                    }
                }
                if(flag==0)
                {
                    cart.Add(new Item()
                    {
                        Product = product,
                        Quantity = 1
                    });
                }
                Session["cart"] = cart;
                
            }
            return Redirect(url);

        }
        public ActionResult RemoveFromCart(int productId, string url)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            foreach(var item in cart)
            {
                if(item.Product.ProductId==productId)
                {
                    cart.Remove(item);
                    break;
                }
            }
            Session["cart"] = cart;
            return Redirect(url);

        }
        public ActionResult ViewProduct(int productId)
        {
            //var dataItem = db.Tbl_Product.Where(x => x.ProductId==productId).SingleOrDefault();
            productViewClass pro = new productViewClass();
            if(Session["Username"]!=null)
            {
                return View(pro.CreateModel(productId, Session["Username"].ToString()));
            }
            else
            {
                return View(pro.CreateModel(productId, "null"));
            }
        }
        public ActionResult Filter(string name, int category)
        {
            FilterClass filter = new FilterClass();
            return View(filter.CreateModel(name, category));
        }
        [AuthorizationFilter]
        public ActionResult Checkout()
        {
            return View();
        }
        [AuthorizationFilter]
        public ActionResult CheckoutDetails(string guid)
        {
            int Total2 = 0;
            Tbl_Cart cart = new Tbl_Cart();
            foreach (Item item in (List<Item>)Session["cart"])
            {
                int lineTotal = Convert.ToInt32(item.Quantity * item.Product.Price);
                Total2 = Convert.ToInt32(@Total2 + lineTotal);
            }
            cart.Total = Total2;
            string name = Session["Username"].ToString();
            var dataItem = db.Tbl_Members.Where(x => x.EmailId == name).SingleOrDefault();
            cart.MemberId = dataItem.MemberId;
            cart.Date = DateTime.Now;
            cart.CartStatusId = 1;
            cart.OrderId = guid;
            db.Tbl_Cart.Add(cart);
            db.SaveChanges();
            int Id = Convert.ToInt32(cart.OrderId);
            foreach (Item item in (List<Item>)Session["cart"])
            {
                Tbl_CartItems cartItems = new Tbl_CartItems();
                cartItems.CartId = cart.CartId;
                cartItems.ProductId = item.Product.ProductId;
                cartItems.Quantity = item.Quantity;
                db.Tbl_CartItems.Add(cartItems);
                db.SaveChanges();
            }
            ViewBag.Message = "Your OrderId is -> "+Id;
            ViewBag.Status = 1;
            return RedirectToAction("OrderHistory");
        }

        public ActionResult DecreaseQty(int productId)
        {
            if (Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = db.Tbl_Product.Find(productId);
                foreach (var item in cart)
                {
                    if (item.Product.ProductId == productId)
                    {
                        int prevQty = item.Quantity;
                        if (prevQty > 0)
                        {
                            cart.Remove(item);
                            cart.Add(new Item()
                            {
                                Product = product,
                                Quantity = prevQty - 1
                            });
                        }
                        break;
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Checkout");
        }

        public ActionResult IncreaseQty(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            var product = db.Tbl_Product.Find(productId);
            int flag = 0;
            if (Session["cart"] != null)
            {
                foreach (var item in cart)
                {
                    if (item.Product.ProductId == productId)
                    {
                        int prevQty = item.Quantity;
                        if (prevQty >= 0)
                        {
                            cart.Remove(item);
                            cart.Add(new Item()
                            {
                                Product = product,
                                Quantity = prevQty + 1
                            });
                        }
                        flag = 1;
                        break;
                    }
                }
                if(flag==0)
                {
                    cart.Add(new Item()
                    {
                        Product = product,
                        Quantity = 1
                    });
                }
                Session["cart"] = cart;
            }
            else
            {
                cart.Add(new Item()
                {
                    Product = product,
                    Quantity = 1
                });
                Session["cart"] = cart;
            }
            return Redirect("Checkout");
        }
        
        [AuthorizationFilter]
        public ActionResult OrderHistory()
        {
            OrderHistoryClass history = new OrderHistoryClass();
            string name = Session["Username"].ToString();
            return View(history.CreateModel(name));
        }

        public JsonResult RateComment(Tbl_comment rateComment)
        {
            bool isSuccess = true;
            try
            {
                string name = Session["Username"].ToString();
                var dataItem = db.Tbl_Members.Where(x => x.EmailId == name).SingleOrDefault();
                rateComment.MemberId = dataItem.MemberId;
                rateComment.Date=DateTime.Now;
                db.Tbl_comment.Add(rateComment);
                db.SaveChanges();
                double rating = 0, counter = 0;
                List<Tbl_comment> data5 = _unitOfWork.GetRepositoryInstance<Tbl_comment>().GetAllRecords().Where(x => x.Rating != null && x.ProductId == rateComment.ProductId).ToList();
                if (data5 != null)
                {
                    foreach (var rate in data5)
                    {
                        counter++;
                        rating += Convert.ToDouble(rate.Rating);
                    }
                    if (rating != 0 && counter != 0)
                    {
                        rating = rating / counter;
                    }
                    else
                    {
                        rating = 0;
                    }
                    rating = Math.Round(rating, 2);
                }
                var data =_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstorDefault(Convert.ToInt32(rateComment.ProductId));
                data.rating = Convert.ToDecimal(rating);
                _unitOfWork.GetRepositoryInstance<Tbl_Product>().Update(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                isSuccess = false;
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckEmailAvailable(string userdata)
        {
            System.Threading.Thread.Sleep(200);
            var searchEmail = db.Tbl_Members.Where(x => x.EmailId == userdata).SingleOrDefault();
            if(searchEmail!=null)
            {
                return Json(1);
            }
            else
            {
                return Json(0);
            }
        }
    }
}