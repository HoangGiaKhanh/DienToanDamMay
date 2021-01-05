using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace ConnectDockerWeb.Controllers
{
    public class CallController : Controller
    {
        #region trang chính
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string code)
        {
            string test = ExecuteCommandSync(code);
            ViewBag.ip = test;
            return View();
        }
        #endregion

        #region Start - Stop
        //public ActionResult Start()
        //{
        //    //không dùng nữa
        //    ModelState.AddModelError("", "Kết Nối Thành Công");
        //    ExecuteCommandSync("docker run -it -d -v C:/Users:/data --name javacompile ubuntu:18.04");
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        public ActionResult Start(string nameContainer, string numberCPUs, string numberRAMs)
        {
            Session["Name"] = nameContainer;
            var test = "docker run -it --name " + nameContainer + " --memory " + numberRAMs + " --cpus " + numberCPUs + " ubuntu:18.04";
            //string value = ExecuteCommandSync(test);
            //if (value == "")
            //{
            //    ViewBag.ip = "Không thành công, đã xảy ra lỗi!";
            //}
            ExecuteCommandLineWithoutReturn(test);
            return View("Index");
        }
        public ActionResult Stop()
        {
            string container = Session["Name"].ToString();
            ExecuteCommandSync("docker stop " + container);
            ExecuteCommandSync("docker rm /" + container);
            Session["Name"] = null;
            return RedirectToAction("Index");
        }
        #endregion

        #region Test code
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
        #endregion

        #region docker actions

        public ActionResult SeeStats()
        {
            ExecuteCommandLineWithoutReturn("docker stats");
            return View("Index");
        }
        public ActionResult SeeImage()
        {
            ViewBag.Image = ExecuteCommandSync("docker images --digests");
            ViewBag.Container = ExecuteCommandSync("docker container ls --all");
            return View("Index");
        }

        public ActionResult CommitDocker(string name, string imageName)
        {
            //chức năng lưu cấu hình container theo tên
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
        }
        #endregion

        #region Portainer
        public ActionResult DeployPortainer()
        {
            //remove container nếu đã tồn tại
            string commandrm1 = ExecuteCommandSync("docker container stop portainer");
            string commandrm2 = ExecuteCommandSync("docker container rm portainer");
            //- Deploy Portainer 
            string command1 = ExecuteCommandSync("docker volume create portainer_data");
            string command2 = ExecuteCommandSync("docker run -d -p 8000:8000 -p 9000:9000 --name=portainer --restart=always -v /var/run/docker.sock:/var/run/docker.sock -v portainer_data:/data portainer/portainer");
            //- Deploy docker stack
            string command3 = ExecuteCommandSync("curl -L https://downloads.portainer.io/portainer-agent-stack.yml -o portainer-agent-stack.yml");
            string command4 = ExecuteCommandSync("docker stack deploy --compose-file=portainer-agent-stack.yml portainer");
            //tạo đường dẫn đến giao diện web portainer
            ViewBag.Link = "localhost:9000";
            return View("Index");
        }

        public ActionResult ClosePortainer()
        {
            //remove container
            string commandrm1 = ExecuteCommandSync("docker container stop portainer");
            string commandrm2 = ExecuteCommandSync("docker container rm portainer");
            return View("Index");
        }
        #endregion

        public string ExecuteCommandSync(object command)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c " + command);
                processStartInfo.WindowStyle = ProcessWindowStyle.Normal;

                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;
                Process process = new Process();
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

        public ActionResult ExecuteCommandLineWithoutReturn(object command)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c " + command);
                processStartInfo.WindowStyle = ProcessWindowStyle.Normal;

                processStartInfo.UseShellExecute = true;
                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();
                return View("Index");
            }
            catch (Exception)
            {
                ViewBag.ip = "Không thành công, đã xảy ra lỗi!";
                return View("Index");
            }
        }
    }
}