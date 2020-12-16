using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConnectDockerWeb.Controllers
{
    public class CallController : Controller
    {
        // GET: Call
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Start()
        {
            ModelState.AddModelError("", "Kết Nối Thành Công");
            ExecuteCommandSync("docker run -it -d -v C:/Users:/data --name javacompile ubuntu:18.04");
            return RedirectToAction("/Index");
        }
        public ActionResult Stop()
        {
            ModelState.AddModelError("", "Dừng Docker Thành Công");
            ExecuteCommandSync("docker stop javacompile");
            return RedirectToAction("/Index");
        }

        public ActionResult Test()
        {
            //string test = ExecuteCommandSync("ipconfig");
            //ViewBag.ip = test;
            return View();
        }
        [HttpPost]
        public ActionResult Test(FormCollection fc)
        {
            string command = fc["code"];
            string test = ExecuteCommandSync(command);
            ViewBag.ip = test;
            return View();
        }

        public string ExecuteCommandSync(object command)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = processStartInfo;
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                return result;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}