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
        [HttpPost]
        public ActionResult Start(string nameContainer)
        {
            Session["Name"] = nameContainer;
            string value = ExecuteCommandSync("docker run -it -d -v C:/Users:/data --name " + nameContainer + " ubuntu:18.04");
            if (value == "")
            {
                ExecuteCommandSync("docker stop " + nameContainer);
                ExecuteCommandSync("docker run -it -d -v C:/Users:/data --name CLOUDCompile-2 ubuntu:18.04");
            }
            return RedirectToAction("/Index");
        }
        public ActionResult Stop()
        {
            string nameContainer = (string)Session["Name"];
            ExecuteCommandSync("docker stop " + nameContainer);
            ExecuteCommandSync("docker rm /" + nameContainer);
            ExecuteCommandSync("docker stop CLOUDCompile-2");
            ExecuteCommandSync("docker rm /CLOUDCompile-2");
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

        public ActionResult SeeImage()
        {
            ViewBag.Image = ExecuteCommandSync("docker images --digests");
            ViewBag.Container = ExecuteCommandSync("docker container ls --all");
            return View();
        }
    }
}