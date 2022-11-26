using InventoryClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace InventoryClientApp.Controllers
{
    public class ProductsController : Controller
    {
        public static string baseUrl = "https://localhost:7166/api/products/";
        public async Task<IActionResult> Index()
        {
            var products = await GetProducts();
            return View(products);
        }



        [HttpGet]
        public async Task<List<Products>> GetProducts()
        {
            //Use the access token to call a protected web API
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);

            var res = JsonConvert.DeserializeObject<List<Products>>(jsonStr).ToList();

            return res;
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Products products)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var stringCotent = new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json");
            await client.PostAsync(url, stringCotent);

            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Use the access token to call a protected web API
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<Products>(jsonStr);

            if (res == null)
            {
                return NotFound();
            }

            return View(res);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Products products)
        {
            if (id != products.ProdId)
            {
                return NotFound();
            }

            //Use the access token to call a protected web API
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var stringCotent = new StringContent(JsonConvert.SerializeObject(products), Encoding.UTF8, "application/json");
            await client.PutAsync(url, stringCotent);


            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Use the access token to call a protected web API
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<Products>(jsonStr);

            if (res == null)
            {
                return NotFound();
            }

            return View(res);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            await client.DeleteAsync(url);

            return RedirectToAction($"{nameof(Index)}");
        }





        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Use the access token to call a protected web API
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = baseUrl + id;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            string jsonStr = await client.GetStringAsync(url);
            var product = JsonConvert.DeserializeObject<Products>(jsonStr);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
