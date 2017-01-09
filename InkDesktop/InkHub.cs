﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Management;
using InkPlatform.Hardware;
using InkPlatform.Hardware.Wacom;
using InkPlatform.Ink;
using InkPlatform.UserControls;
using InkPlatform.UserInterface;
using System.Globalization;

namespace InkDesktop
{
    public partial class InkHub : Form
    {
        public delegate string CaptureSignatureJsonFn(string who, string why);
        public CaptureSignatureJsonFn CaptureSignatureJsonDelegate;

        public delegate ContextPenData CaptureSignatureFn(string who, string why);
        public CaptureSignatureFn CaptureSignatureDelegate;

        public delegate ContextPenData RunLayoutFn(string[] layoutFiles, Dictionary<string, string> variables);
        public RunLayoutFn RunLayoutDelegate;

        public delegate ContextPenData RunLayoutJsonFn(List<string> jsons, Dictionary<string, string> variables);
        public RunLayoutJsonFn RunLayoutJsonDelegate;

        public delegate ContextPenData CloseDefaultSignpadWindowFn();
        public CloseDefaultSignpadWindowFn CloseDefaultSignpadWindowDelegate;

        NotifyIcon _notifyIcon;
        ContextMenuStrip _contextMenu;
        WebManager _webManager;
        DeviceScanner _deviceScanner;
        InkHubLogger _logger;
        SignatureCapture _signCapt;

        public const int WM_DEVICECHANGE = 0x0219;
        public const int DBT_DEVICEARRIVAL = 0x8000;
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;

        private List<PenDevice> _penDeviceList = new List<PenDevice>();
        private PenDevice _currentPenDevice = null;
        private SignpadWindow _defaultSignpadWindow;

        public InkHub()
        {
            InitializeComponent();

            if(Properties.Settings.Default.Logging == true)
            {
                try
                {
                    _logger = new InkHubLogger();
                }
                catch (Exception)
                {
                    MessageBox.Show(strings.INIT_LOG_FAIL);
                }
            }
            
            Log("InkHub Initialised (Version " + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")");
            
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = Properties.Resources.bluepen;
            _notifyIcon.Text = strings.INK_DESKTOP;
            _notifyIcon.MouseClick += _notifyIcon_MouseClick;
            _notifyIcon.Visible = true;

            _contextMenu = new ContextMenuStrip();
            _contextMenu.Items.Add("Exit", null, Exit);
            _notifyIcon.ContextMenuStrip = _contextMenu;

            this.CaptureSignatureJsonDelegate = CaptureSignatureJson;
            this.CaptureSignatureDelegate = CaptureSignature;
            this.RunLayoutDelegate = RunLayouts;
            this.RunLayoutJsonDelegate = RunLayoutJsons;
            this.CloseDefaultSignpadWindowDelegate = CloseDefaultSignpadWindow;
            
            _webManager = new WebManager(".", Properties.Settings.Default.WebManagerPort, this);
            _webManager.LogFunction = WebManagerLog;

            this.Hide();
        }
        
        protected void Exit(object sender, EventArgs e)
        {
            Log("Testing webmanager availability");
            if(_webManager != null && _webManager.Running)
            {
                Log("Stopping webmanager");
                _webManager.Stop();
                _webManager = null;
            }
            
            Log("Application Exit");
            Log("----------------------------------------------------------------------");

            _logger.Dispose();

            Application.Exit();
        }

        protected void Log(string msg)
        {
            Log(msg, 0);
        }

        protected void Log(string msg, int alertType)
        {
            if(_logger != null)
            {
                _logger.Log(msg, alertType);
            }
            
        }

        protected void WebManagerLog(string msg, int alertType)
        {
            Log("[WebManager] " + msg, alertType);
        }

        protected void SignpadWindowLog(string msg, int alertType)
        {
            Log("[SignpadWindow] " + msg, alertType);
        }

        private void OpenAndFocus()
        {
            if (Application.OpenForms[this.Name] == null)
            {
                this.Show();
            }
            else
            {
                Application.OpenForms[this.Name].Focus();
            }
        }
        
        private void UpdateLayoutElementsWithVariableValues(List<Layout> layoutList, Dictionary<string, string> variables)
        {
            string[] keys = variables.Keys.ToArray();
            foreach (Layout l in layoutList)
            {
                if (l.ElementList == null) continue;
                foreach (Element e in l.ElementList)
                {
                    foreach (string k in keys)
                    {
                        if (e.Name == k)
                        {
                            if (e.ElementType == Element.ELEMENT_TYPE.BUTTON)
                            {
                                ElementButton btn = (ElementButton)e;
                                btn.Text = variables[k];
                            }
                            else if (e.ElementType == Element.ELEMENT_TYPE.TEXT)
                            {
                                ElementText txt = (ElementText)e;
                                txt.Text = variables[k];
                            }
                        }
                    }
                }
            }
        }

        public ContextPenData CloseDefaultSignpadWindow()
        {
            Log("CloseDefaultSignpadWindow");
            if(_defaultSignpadWindow != null && _defaultSignpadWindow.Visible == true)
            {
                Log("Save pen data and close window");
                ContextPenData contextPenData = _defaultSignpadWindow.ContextPenData;
                _defaultSignpadWindow.Close();
                return contextPenData;
            }
            else
            {
                Log("But no signpad window opened");
                return null;
            }
        }

        public ContextPenData RunLayouts(string[] layout, Dictionary<string, string> variables)
        {
            OpenAndFocus();
            Log("Run layout files");
            if (_currentPenDevice == null)
            {
                Log("No pen devices connected");
                MessageBox.Show(strings.NOT_CONNECTED);
                return new ContextPenData((int)PEN_DEVICE_ERROR.NOT_CONNECTED, SerializablePenDevice.ErrorMessage(PEN_DEVICE_ERROR.NOT_CONNECTED));
            }

            List<Layout> layoutList = new List<Layout>();
            try
            {
                layoutList = LayoutManager.ReadLayoutFiles(layout);
            }
            catch (Exception)
            {
                Log("Read layout files failed");
                MessageBox.Show(SerializablePenDevice.ErrorMessage(PEN_DEVICE_ERROR.LAYOUT_FAIL));
                return new ContextPenData((int)PEN_DEVICE_ERROR.LAYOUT_FAIL, SerializablePenDevice.ErrorMessage(PEN_DEVICE_ERROR.LAYOUT_FAIL));
            }

            UpdateLayoutElementsWithVariableValues(layoutList, variables);

            _defaultSignpadWindow = new SignpadWindow();
            _defaultSignpadWindow.LogFunction = SignpadWindowLog;

            int result = _defaultSignpadWindow.DisplayLayoutsDialog(layoutList, _currentPenDevice, 0, this);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                ContextPenData contextPenData = _defaultSignpadWindow.ContextPenData;
                return contextPenData;
            }
            else
            {
                Log(_currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result));
                //MessageBox.Show(_currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result));
                return new ContextPenData(result, SerializablePenDevice.ErrorMessage((PEN_DEVICE_ERROR)result));
            }
        }

        public ContextPenData RunLayoutJsons(List<string> jsons, Dictionary<string, string> variables)
        {
            OpenAndFocus();
            Log("Run layout json");
            if (_currentPenDevice == null)
            {
                Log("No pen devices connected");
                MessageBox.Show(strings.NOT_CONNECTED);
                return new ContextPenData((int)PEN_DEVICE_ERROR.NOT_CONNECTED, SerializablePenDevice.ErrorMessage(PEN_DEVICE_ERROR.NOT_CONNECTED));
            }

            List<Layout> layoutList = new List<Layout>();
            try
            {
                for(int i=0; i<jsons.Count; i++)
                {
                    Layout layout = (Layout)JSONSerializer.DeserializeLayout(jsons[i]);
                    layoutList.Add(layout);
                }
            }
            catch (Exception)
            {
                Log("Read layout files failed");
                MessageBox.Show(SerializablePenDevice.ErrorMessage(PEN_DEVICE_ERROR.LAYOUT_FAIL));
                return new ContextPenData((int)PEN_DEVICE_ERROR.LAYOUT_FAIL, SerializablePenDevice.ErrorMessage(PEN_DEVICE_ERROR.LAYOUT_FAIL));
            }

            UpdateLayoutElementsWithVariableValues(layoutList, variables);

            _defaultSignpadWindow = new SignpadWindow();
            _defaultSignpadWindow.LogFunction = SignpadWindowLog;

            int result = _defaultSignpadWindow.DisplayLayoutsDialog(layoutList, _currentPenDevice, 0, this);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                ContextPenData contextPenData = _defaultSignpadWindow.ContextPenData;
                return contextPenData;
            }
            else 
            {
                Log(_currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result));
                //MessageBox.Show(_currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result));
                return new ContextPenData(result, SerializablePenDevice.ErrorMessage((PEN_DEVICE_ERROR)result));
            }
        }

        public ContextPenData CaptureSignature(string who, string why)
        {
            OpenAndFocus();

            Log("Signature Capture for " + who + " for reason: " + why);
            if (_currentPenDevice == null)
            {
                Log("No pen devices connected");
                MessageBox.Show(strings.NOT_CONNECTED);
                return new ContextPenData((int)PEN_DEVICE_ERROR.NOT_CONNECTED, SerializablePenDevice.ErrorMessage(PEN_DEVICE_ERROR.NOT_CONNECTED));
            }

            _defaultSignpadWindow = new SignpadWindow();
            _defaultSignpadWindow.LogFunction = SignpadWindowLog;
            int result = _defaultSignpadWindow.CaptureSignatureDialog(who, why, _currentPenDevice, this);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                ContextPenData contextPenData = _defaultSignpadWindow.ContextPenData;
                contextPenData.AddData("name", who);
                contextPenData.AddData("reason", why);
                return contextPenData;
            }
            else
            {
                Log(_currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result));
                MessageBox.Show(_currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result));
                return new ContextPenData(result, SerializablePenDevice.ErrorMessage((PEN_DEVICE_ERROR)result));
            }
        }

        public string CaptureSignatureJson(string who, string why)
        {
            OpenAndFocus();

            Log("Signature Capture (Json) for " + who + " for reason: " + why);
            if (_currentPenDevice == null)
            {
                Log("No pen devices connected");
                MessageBox.Show(strings.NOT_CONNECTED);
                return PEN_DEVICE_ERROR.NOT_CONNECTED.ToString();
            }

            _defaultSignpadWindow = new SignpadWindow();
            _defaultSignpadWindow.LogFunction = SignpadWindowLog;
            int result = _defaultSignpadWindow.CaptureSignatureDialog(who, why, _currentPenDevice, this);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                ContextPenData contextPenData = _defaultSignpadWindow.ContextPenData;
                contextPenData.AddData("name", who);
                contextPenData.AddData("reason", why);
                return contextPenData.ToString();
            }
            else
            {
                Log(_currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result));
                MessageBox.Show(_currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result));
                return _currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result);
            }
        }
        
        protected void ShowPenDevice(List<PenDevice> penDeviceList)
        {
            Log("Show pen device");
            if (penDeviceList == null || penDeviceList.Count == 0)
            {
                Log("No pen device detected");
                cboHardware.Items.Clear();
                picHardware.Image = Properties.Resources.settings;
            }
            else
            {
                _penDeviceList = penDeviceList;
                _currentPenDevice = _penDeviceList[0];
                Log("Set current pen device to be " + _penDeviceList[0].ProductModel);

                Log("Populate hardware dropdown");
                cboHardware.Items.Clear();
                for(int i=0; i<_penDeviceList.Count; i++)
                {
                    cboHardware.Items.Add(_penDeviceList[i].ProductModel);
                }
                cboHardware.SelectedIndex = 0;
                if(_penDeviceList[0].GetType().IsSubclassOf(typeof(WacomSignpad)))
                {
                    picHardware.Image = Properties.Resources.signpad;
                }
                else
                {
                    picHardware.Image = Properties.Resources.pendisplay;
                }
            }
            
        }

        protected List<PenDevice> DetectPenDevice()
        {
            Log("Detect pen device");
            ManagementClass USBClass;
            ManagementObjectCollection USBCollection;
            Log("Get list of supported vid");
            DeviceScanner deviceScanner = new DeviceScanner();
            List<ushort> supportedVidList = deviceScanner.SupportedVidList();

            try
            {
                USBClass = new ManagementClass("Win32_USBControllerDevice");
                USBCollection = USBClass.GetInstances();

                foreach (System.Management.ManagementObject usb in USBCollection)
                {
                    string dependent = usb["dependent"].ToString();
                    string deviceId = dependent.Split('=')[1];
                    deviceId = deviceId.Replace("\"", "");

                    string[] parts = deviceId.Split(new[] { "\\\\" }, StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        if (parts.Length > 0)
                        {
                            if (parts[0] == "USB")
                            {
                                if (parts.Length == 3)
                                {
                                    string hwClass = parts[0];
                                    string hwId = parts[1];
                                    string serialNo = parts[2];
                                    string[] hwIdParts = hwId.Split('&');
                                    if (hwIdParts.Length == 2)
                                    {
                                        string[] vidStr = hwIdParts[0].Split('_');
                                        string vid = vidStr[1];
                                        string[] pidStr = hwIdParts[1].Split('_');
                                        string pid = pidStr[1];

                                        ushort vidShort = 0;
                                        ushort.TryParse(vid, NumberStyles.HexNumber, null, out vidShort);
                                        if(vidShort != 0)
                                        {
                                            if (supportedVidList.Contains(vidShort))
                                            {
                                                Log("Detected supported vid " + vid);
                                                Log("Run device scanner");
                                                return deviceScanner.Scan();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }

                USBCollection.Dispose();
                USBClass.Dispose();

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_DEVICECHANGE)
            {
                Log("Device changed detected");

                //reserved for later usage
                if((int)m.WParam == DBT_DEVICEARRIVAL)
                {

                }else if((int)m.WParam == DBT_DEVICEREMOVECOMPLETE)
                {

                }

                RefreshPenDevice();
                //DetectPenDevice();
            }
        }

        private void _notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void InkHub_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                _notifyIcon.ShowBalloonTip(2000, strings.INK_DESKTOP, strings.TOOL_TIP_MSG_NOT_CLOSED, ToolTipIcon.Info);
            }
        }

        private void InkHub_Activated(object sender, EventArgs e)
        {
            this.Left = Screen.PrimaryScreen.WorkingArea.Left + Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Top + Screen.PrimaryScreen.WorkingArea.Height - this.Height;
        }

        protected void RefreshPenDevice()
        {
            Log("Refresh pen device");
            if(_deviceScanner == null)
            {
                _deviceScanner = new DeviceScanner();
            }

            List<PenDevice> penDeviceList = _deviceScanner.Scan();
            if (penDeviceList != null)
            {
                ShowPenDevice(penDeviceList);
            }
        }

        private void picRefresh_Click(object sender, EventArgs e)
        {
            RefreshPenDevice();
            if(_currentPenDevice != null)
            {
                _currentPenDevice.Reset();
            }
        }

        private void picSettings_Click(object sender, EventArgs e)
        {
            Log("Settings clicked");
            if(_currentPenDevice != null)
            {
                Log("Showing settings for " + _currentPenDevice.ProductModel);
                PenDeviceInfo info = new PenDeviceInfo();
                info.ShowPenDeviceInfo(_currentPenDevice);
            }
            else
            {
                Log("No pen device connected, abort showing settings");
            }
        }

        private void InkHub_Load(object sender, EventArgs e)
        {
            RefreshPenDevice();

            startWebManager();
        }

        private void startWebManager()
        {
            Log("Starting Webmanager");

            if (_webManager.Running)
            {
                return;
            }

            try
            {
                _webManager.Start();
                Thread.Sleep(1000);
                bool result = _webManager.Running;
                if (result)
                {
                    btnWebServer.Image = Properties.Resources.webserver_running;
                    tsmiToggle.Text = strings.STOP;
                    tsmiPort.Text = _webManager.Port.ToString();
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private bool stopWebManager()
        {
            Log("Stopping WebManager");
            if (!_webManager.Running) return true;

            if(MessageBox.Show(strings.CFM_STOP,strings.WEB_MANAGER, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                bool result = _webManager.Stop();
                if (result)
                {
                    btnWebServer.Image = Properties.Resources.webserver_stop;
                    tsmiToggle.Text = strings.START;
                    return true;
                }
            }

            return false;
            
        }

        private void tsmiToggle_Click(object sender, EventArgs e)
        {
            if (_webManager.Running)
            {
                stopWebManager();
            }
            else
            {
                startWebManager();
            }
        }

        private void btnWebServer_Click(object sender, EventArgs e)
        {

        }

        private void ChangePort(int newPort)
        {
            if (_webManager.Running)
            {
                if (stopWebManager() == false)
                {
                    return;
                }
            }
            _webManager.Port = newPort;
            Properties.Settings.Default.WebManagerPort = newPort;
            Properties.Settings.Default.PortRegistered = false;
            tsmiPort.Text = newPort.ToString();
        }

        private void tsmiPort_Click(object sender, EventArgs e)
        {
            string newPort = SettingChangeDialog.ShowDialog(this, "Port", Properties.Settings.Default.WebManagerPort);
            if (newPort != null && newPort != "")
            {
                int newPortInt = 0;
                bool result = int.TryParse(newPort, out newPortInt);
                if (result)
                {
                    ChangePort(newPortInt);
                    return;
                }
            }

            MessageBox.Show(strings.INVALID_PORT);
           
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            if(_signCapt == null || _signCapt.Disposing || _signCapt.IsDisposed)
            {
                _signCapt = new SignatureCapture();
            }
            try
            {
                _signCapt.Show();
            }
            catch (Exception)
            {
                Log("Cannot show Signature Capture");
                MessageBox.Show(strings.SIGN_CAPT_UNABLE_SHOW);
            }
            
        }
    }
}
