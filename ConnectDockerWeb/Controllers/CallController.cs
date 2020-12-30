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
        [HttpGet]
        public ActionResult Index(string code)
        {
            ViewBag.Image = ExecuteCommandSync("docker images --digests");
            ViewBag.Container = ExecuteCommandSync("docker container ls --all");
            string test = ExecuteCommandSync(code);
            ViewBag.ip = test;
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
            //string test = ExecuteCommandSync("ipconfig");
            //ViewBag.ip = test;
        }

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