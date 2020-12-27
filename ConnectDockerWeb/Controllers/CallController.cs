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

        public ActionResult CommitDocker(string name, string imageName)
        {
            try
            {
                string command = "docker commit " + name + " " + imageName;
                string test = ExecuteCommandSync(command);
                return View("Index");
            }
            catch
            {

                return View("Index");
            }
            //string test = ExecuteCommandSync("ipconfig");
            //ViewBag.ip = test;
        }

        public ActionResult DeployPortainer(string localIP)
        {
            //- Deploy Portainer 
            string command1 = ExecuteCommandSync("docker volume create portainer_data");
            string command2 = ExecuteCommandSync("docker run -d -p 8000:8000 -p 9000:9000 --name=portainer --restart=always -v /var/run/docker.sock:/var/run/docker.sock -v portainer_data:/data portainer/portainer");
            //- Deploy docker stack
            string command3 = ExecuteCommandSync("curl -L https://downloads.portainer.io/portainer-agent-stack.yml -o portainer-agent-stack.yml");
            string command4 = ExecuteCommandSync("docker stack deploy --compose-file=portainer-agent-stack.yml portainer");
            //get IP to open web UI Portainer
            ViewBag.Link = localIP+":9000";
            return View("Index");
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