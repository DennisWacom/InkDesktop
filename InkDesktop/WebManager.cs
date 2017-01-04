using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using InkPlatform.Hardware;
using InkPlatform.Hardware.Wacom;
using InkPlatform.Ink;
using InkPlatform.UserControls;
using InkPlatform.UserInterface;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace InkDesktop
{
    public class WebManager
    {
        public delegate void SendLog(string msg, int alertType);
        public SendLog LogFunction;

        protected InkHub _inkHub;

        public static string ACCESS_CONTROL_ALLOW_ORIGIN = "Access-Control-Allow-Origin";
        public static string ACCESS_CONTROL_ALLOW_CREDENTIALS = "Access-Control-Allow-Credentials";
        public static string ACCESS_CONTROL_EXPOSE_HEADERS = "Access-Control-Expose-Headers";

        public static string QUERY_NAME = "name";
        public static string QUERY_REASON = "reason";
        public static string QUERY_LAYOUT = "layout[]";
        public static string QUERY_CURRENT = "current";
        public static string QUERY_LAYOUT_JSON = "layoutJson[]";

        private const string CAPTURE_IMAGE = "captureimage";
        private const string CAPTURE_BASE64 = "capturebase64";
        private const string CAPTURE_JSON = "capturejson";
        private const string RUN_LAYOUT_FILES = "runlayoutfiles";
        private const string RUN_LAYOUT_JSONS = "runlayoutjsons";
        private const string SIGNATURE = "signature";
        private const string FILE = "file";

        protected string _rootDirectory;
        protected int _port;

        private Thread _serverThread;
        private HttpListener _listener;
        public HttpListenerContext currentContext;
        private bool _threadStarted = false;
        private int _webManagerTimeout = 5000;

        public bool Running
        {
            get { return _threadStarted; }
        }

        public WebManager(string rootDir, int port, InkHub inkHub)
        {
            _rootDirectory = rootDir;
            _port = port;
            _inkHub = inkHub;
        }

        public int Port
        {
            get { return _port; }
            set
            {
                _port = value;
            }
        }

        public string RootDirectory
        {
            get { return _rootDirectory; }
        }

        protected void Log(string msg)
        {
            Log(msg, 0);
        }

        protected void Log(string msg, int alertType)
        {
            if(LogFunction != null)
            {
                LogFunction(msg, alertType);
            }
        }

        public void Start()
        {
            Log("Start Webmanager");

            if(Properties.Settings.Default.PortRegistered == false)
            {
                Log("Register port " + Properties.Settings.Default.WebManagerPort.ToString());
                RegisterPort(Properties.Settings.Default.WebManagerPort);
                Properties.Settings.Default.PortRegistered = true;
                Properties.Settings.Default.Save();
            }

            try
            {
                _serverThread = new Thread(Listen);
                _serverThread.Start();
                _threadStarted = true;
                Log("WebManager started with root dir = " + _rootDirectory + " on Port " + _port.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        private void Listen()
        {
            //need to run "netsh http add urlacl url=http://*:8008/ user=Dennis listen=yes" with admin privilege
            //else calling Start() will prompt exception "Access Denied"
            Log("Listener initialised with prefix - http://*." + _port.ToString() + "/");
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");

            try
            {
                _listener.Start();
            }
            catch (Exception ex)
            {
                Log(ex.Message, 1);
                _threadStarted = false;
                MessageBox.Show(strings.WEB_MANAGER + ": " + strings.PORT_LISTEN_EXCEPTION + " : " + ex.Message);
            }
            
            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception)
                {

                }
            }
        }
        
        private void ProcessCaptureImageRequest(HttpListenerContext context, string name, string reason)
        {
            Log("Capture Image");
            ContextPenData contextPenData = null;
            try
            {
                Log("Call CaptureSignatureDelegate to InkHub");
                contextPenData = (ContextPenData)_inkHub.Invoke(_inkHub.CaptureSignatureDelegate, new object[] { name, reason });
                Bitmap bitmap = null;
                Log("ContextPenDate received");
                Log("Generate Image with contextPenData");
                InkProcessor.GenerateImageResult result = InkProcessor.GenerateImageFromContextPenData(out bitmap, contextPenData, Pens.Black, Color.White, true, true);
                if (bitmap != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] bmpBytes = ms.ToArray();
                        Log("Send png to client");
                        sendResponse(context, bmpBytes, "image/png", HttpStatusCode.OK);
                    }
                }
                else
                {
                    Log("Error generating image from context pen data", 1);
                    sendErrorResponse(context, strings.ERROR_GEN_IMAGE);
                }
            }
            catch (Exception)
            {
                Log("Error capturing image from inkhub", 1);
                sendErrorResponse(context, strings.ERROR_INK_HUB_SIGN);
            }
        }

        private void ProcessCaptureSignatureJsonRequest(HttpListenerContext context, string name, string reason)
        {
            Log("Capture Json");
            string json = "";
            try
            {
                Log("Call CaptureSignatureJsonDelegate to inkhub");
                json = (string)_inkHub.Invoke(_inkHub.CaptureSignatureJsonDelegate, new object[] { name, reason });
            }
            catch (Exception)
            {
                Log("Error capturing json from inkhub", 1);
            }

            if (json == null || json.Length == 0)
            {
                Log("Json received is empty");
                byte[] msgBytes = Encoding.UTF8.GetBytes("Error");
                sendResponse(context, msgBytes, "text/plain", HttpStatusCode.InternalServerError);
            }
            else
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                Log("Sending json to client");
                sendResponse(context, jsonBytes, "text/plain", HttpStatusCode.OK);
            }
        }

        private void ProcessCaptureSignatureBase64Request(HttpListenerContext context, string name, string reason)
        {
            Log("Capture Base64");
            string json = "";
            try
            {
                Log("Call CaptureSignatureJsonDelegate to inkhub");
                json = (string)_inkHub.Invoke(_inkHub.CaptureSignatureJsonDelegate, new object[] { name, reason });
            }
            catch (Exception)
            {
                Log("Error capturing json from inkhub", 1);
            }

            if (json == null || json.Length == 0)
            {
                Log("Json received is empty");
                byte[] msgBytes = Encoding.UTF8.GetBytes("Error");
                sendResponse(context, msgBytes, "text/plain", HttpStatusCode.InternalServerError);
            }
            else
            {
                Log("Json received");
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                Log("Convert json to base64 string");
                string base64 = Convert.ToBase64String(jsonBytes);
                byte[] base64Bytes = Encoding.UTF8.GetBytes(base64);
                Log("Sending base64 to client");
                sendResponse(context, base64Bytes, "text/plain", HttpStatusCode.OK);
            }
        }

        private void ProcessRunLayoutsRequest(HttpListenerContext context, string[] layoutFiles, Dictionary<string, string> variables)
        {
            Log("Run Layouts");
            ContextPenData contextPenData = null;
            try
            {
                Log("Call RunLayoutDelegate to Inkhub");
                contextPenData = (ContextPenData)_inkHub.Invoke(_inkHub.RunLayoutDelegate, new object[] { layoutFiles, variables });
                if (contextPenData != null)
                {
                    Log("ContextPenData received");
                    Bitmap bitmap = null;
                    Log("Generate image from contextPenData");
                    InkProcessor.GenerateImageResult result = InkProcessor.GenerateImageFromContextPenData(out bitmap, contextPenData, Pens.Black, Color.White, true, true);
                    if (bitmap != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Log("Convert image to png format");
                            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] pngBytes = ms.ToArray();
                            Log("Convert png to base 64");
                            string base64 = Convert.ToBase64String(pngBytes);
                            byte[] base64Bytes = Encoding.UTF8.GetBytes(base64);
                            Log("Sending png base64 to client");
                            sendResponse(context, base64Bytes, "image/png", HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        Log("Error generating image from context pen data", 1);
                        sendErrorResponse(context, strings.ERROR_GEN_IMAGE);
                    }
                }
                else
                {
                    Log("ContextPenData received is empty", 1);
                    sendErrorResponse(context, strings.ERROR_GEN_IMAGE);
                }
            }
            catch (Exception)
            {
                Log("Error generating image from context pen data", 1);
                sendErrorResponse(context, strings.ERROR_GEN_IMAGE);
            }
        }

        private void ProcessRunLayoutJsonsRequest(HttpListenerContext context, List<string> layoutJsons, Dictionary<string, string> variables)
        {
            Log("Run Layout Jsons");
            ContextPenData contextPenData = null;

            try
            {
                Log("Call RunLayoutDelegate to Inkhub");
                contextPenData = (ContextPenData)_inkHub.Invoke(_inkHub.RunLayoutJsonDelegate, new object[] { layoutJsons, variables });
                if (contextPenData != null)
                {
                    Log("ContextPenData received");
                    Bitmap bitmap = null;
                    Log("Generate image from contextPenData");
                    InkProcessor.GenerateImageResult result = InkProcessor.GenerateImageFromContextPenData(out bitmap, contextPenData, Pens.Black, Color.White, true, true);
                    if (bitmap != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            Log("Convert image to png format");
                            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            byte[] pngBytes = ms.ToArray();
                            Log("Convert png to base 64");
                            string base64 = Convert.ToBase64String(pngBytes);
                            byte[] base64Bytes = Encoding.UTF8.GetBytes(base64);
                            Log("Sending png base64 to client");
                            sendResponse(context, base64Bytes, "image/png", HttpStatusCode.OK);
                        }
                    }
                    else
                    {
                        Log("Error generating image from context pen data", 1);
                        sendErrorResponse(context, strings.ERROR_GEN_IMAGE);
                    }
                }
                else
                {
                    Log("ContextPenData received is empty", 1);
                    sendErrorResponse(context, strings.ERROR_GEN_IMAGE);
                }
            }
            catch (Exception)
            {
                Log("Error generating image from context pen data", 1);
                sendErrorResponse(context, strings.ERROR_GEN_IMAGE);
            }
        }

        private void Process(HttpListenerContext context)
        {
            Log("Context received, Start Processing");
            if (currentContext != null)
            {
                Thread.Sleep(_webManagerTimeout);
                currentContext = null;
            }

            currentContext = context;
            string filename = context.Request.Url.AbsolutePath;

            Log("Processing request for url - " + filename + " from " + context.Request.RemoteEndPoint.Address + " on Port :" + context.Request.RemoteEndPoint.Port);
            filename = filename.Substring(1);

            NameValueCollection query = context.Request.QueryString;
            string name = query[QUERY_NAME];
            string reason = query[QUERY_REASON];
            string layout = query[QUERY_LAYOUT];
            string current = query[QUERY_CURRENT];
            string ljson = query[QUERY_LAYOUT_JSON];

            Dictionary<string, string> Variables = new Dictionary<string, string>();
            string[] keys = query.AllKeys;
            foreach(string key in keys)
            {
                if(key != null && key != QUERY_NAME && key != QUERY_REASON && key != QUERY_LAYOUT && key != QUERY_CURRENT && key != QUERY_LAYOUT_JSON)
                {
                    Variables.Add(key, query[key]);
                }
            }
            
            string[] layoutFiles = new string[] { };
            if (layout != null)
            {
                layoutFiles = layout.Split(new char[] { ',' });
            }

            if (current != null)
            {
                for (int i = 0; i < layoutFiles.Length; i++)
                {
                    layoutFiles[i] = current + layoutFiles[i];
                }
            }

            List<string> jsons = new List<string>();
            if(ljson != null)
            {
                ljson = "[" + ljson + "]";
                jsons = JSONSerializer.SplitJsonArray(ljson);
            }

            if (filename.Equals(CAPTURE_IMAGE))
            {
                ProcessCaptureImageRequest(context, name, reason);
            }
            else if (filename.Equals(CAPTURE_JSON))
            {
                ProcessCaptureSignatureJsonRequest(context, name, reason);
            }
            else if (filename.Equals(CAPTURE_BASE64))
            {
                ProcessCaptureSignatureBase64Request(context, name, reason);
            }
            else if (filename.Equals(RUN_LAYOUT_FILES) && layoutFiles != null)
            {
                ProcessRunLayoutsRequest(context, layoutFiles, Variables);
            }
            else if(filename.Equals(RUN_LAYOUT_JSONS) && ljson != null)
            {
                ProcessRunLayoutJsonsRequest(context, jsons, Variables);
            }

            currentContext = null;
            
        }
        
        private void sendErrorResponse(HttpListenerContext context, string msg)
        {
            byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
            sendResponse(context, msgBytes, "text/plain", HttpStatusCode.InternalServerError);
        }

        private void sendResponseServerBusy(HttpListenerContext context)
        {
            byte[] msgBytes = Encoding.UTF8.GetBytes(strings.SERVER_BUSY_PREV_REQ);
            sendResponse(context, msgBytes, "text/plain", HttpStatusCode.InternalServerError);
        }

        private void sendResponse(HttpListenerContext context, byte[] data, string mimeType, HttpStatusCode statusCode)
        {
            context.Response.AddHeader(ACCESS_CONTROL_ALLOW_ORIGIN, "*");
            context.Response.AddHeader(ACCESS_CONTROL_ALLOW_CREDENTIALS, "true");

            context.Response.SendChunked = false;

            if (mimeType != null)
            {
                context.Response.ContentType = mimeType;
            }

            if (data != null && data.Length > 0)
            {
                context.Response.ContentLength64 = data.Length;

                Stream ms = new MemoryStream(data);
                byte[] buffer = new byte[1024 * 16];
                int nbytes;
                while ((nbytes = ms.Read(buffer, 0, buffer.Length)) > 0)
                    context.Response.OutputStream.Write(buffer, 0, nbytes);
                ms.Close();
            }

            context.Response.StatusCode = (int)statusCode;
            context.Response.OutputStream.Flush();

            context.Response.OutputStream.Close();
        }
        
        private void RegisterPort(int portNo)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.Verb = "runas";
            info.FileName = "cmd.exe";
            info.Arguments = "/C netsh http add urlacl url=http://*:" + portNo.ToString() + "/ user=everyone";
            info.UseShellExecute = true;
            Process process = new Process();
            process.StartInfo = info;
            process.Start();
        }

        public bool Stop()
        {
            Log("Stopping Webmanager");

            try
            {
                _serverThread.Abort();
                Log("Thread aborted");

                _listener.Stop();
                Log("Listener Stopped");
                
                _threadStarted = false;

                return true;
            }
            catch (Exception)
            {
                Log("Stop webmanager failed", 1);
                return false;
            }

            
        }
    }
    
}
