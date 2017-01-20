using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using InkPlatform.Hardware;
using InkPlatform.Hardware.Wacom;
using InkPlatform.Ink;
using InkPlatform.UserControls;
using InkPlatform.UserInterface;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace InkDesktop
{
    public partial class SlideshowManager : Form
    {
        List<PenDevice> _penDeviceList = new List<PenDevice>();
        DeviceScanner _deviceScanner = new DeviceScanner();
        PenDevice _currentPenDevice;
        SlideShowSettings _currentSlideShowSettings;
        List<string> _currentImageFiles = new List<string>();
        List<Thread> _slideShowThreads = new List<Thread>();
        Dictionary<PenDevice, int> _pauseDeviceDict = new Dictionary<PenDevice, int>();
        List<PenDevice> _penDevicesWithSlideShowRunning = new List<PenDevice>();

        string _slideShowSettingsPath = Path.Combine(Application.StartupPath, Properties.Settings.Default.SlideShowSettings);
        string _slideShowFolder = Path.Combine(Application.StartupPath, Properties.Settings.Default.SlideShowFolder);
        int _intervalInSeconds = 4;

        public delegate void SendLog(string msg, int alertType);
        public SendLog LogFunction;

        public SlideshowManager()
        {
            InitializeComponent();
            InitSlideShowSettings();
        }

        public void InitSlideShowSettings()
        {
            bool createFile = false;
            if (!File.Exists(_slideShowSettingsPath))
            {
                Log("SlideShowSettings file (" + _slideShowSettingsPath + ") does not exist, create file");
                createFile = true;
            }
            else
            {
                FileInfo fi = new FileInfo(_slideShowSettingsPath);
                if (fi.Length == 0)
                {
                    createFile = true;
                }
                else
                {
                    _currentSlideShowSettings = ReadSlideShowSettingsFile(_slideShowSettingsPath);
                    if(_currentSlideShowSettings == null)
                    {
                        createFile = true;
                    }
                }
            }
            
            if (createFile)
            {
                _currentSlideShowSettings = GenerateDefaultSlideShowSettings();
                WriteSlideShowSettingsFile(_currentSlideShowSettings, _slideShowSettingsPath);
            }
            
        }
        
        public bool PausePenDevice(PenDevice pauseDevice, int time)
        {
            if (pauseDevice == null) return false;

            Log("Calling for " + pauseDevice.ProductModel + " to be paused");

            foreach(PenDevice device in _pauseDeviceDict.Keys)
            {
                if (device.IsSameDevice(pauseDevice))
                {
                    //Already called to pause
                    return false;
                }
            }

            foreach(PenDevice pd in _penDevicesWithSlideShowRunning)
            {
                if (pd.IsSameDevice(pauseDevice))
                {
                    pd.Disconnect();
                    Log(pauseDevice.ProductModel + " disconnected");
                }
            }
            
            _pauseDeviceDict.Add(pauseDevice, time);
            

            return true;
        }

        private SlideShowSettings ReadSlideShowSettingsFile(string path)
        {
            Log("Slide show settings path (" + _slideShowSettingsPath + ") exists, attempting deserialization");
            
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    SlideShowSettings settings = JsonConvert.DeserializeObject<SlideShowSettings>(json);
                    return settings;
                }
            }
            catch (Exception ex)
            {
                Log("Error opening and deserializing slide show settings - " + path);
                Log(ex.Message);
            }

            return null;
        }

        private void WriteSlideShowSettingsFile(SlideShowSettings settings, string path)
        {
            try
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                StreamWriter sw = new StreamWriter(path, false);
                sw.Write(json);
                sw.Flush();
            }
            catch (Exception)
            {
                Log("Error serializing default slide show settings to path - " + path);
                return;
            }
            Log("Default slide show settings written to path (" + path + ") successfully");
        }

        public SlideShowSettings GenerateDefaultSlideShowSettings()
        {
            Log("Generate Default SlideShow Settings");
            SlideShowSettings settings = new SlideShowSettings();

            string SlideShowFolder = Path.Combine(Application.StartupPath, Properties.Settings.Default.SlideShowFolder);
            if (SlideShowFolder.Trim() == "")
            {
                Log("SlideShowFolder settings missing", 1);
                return null;
            }
            if (Directory.Exists(SlideShowFolder) == false)
            {
                Log("SlideShowFolder - " + SlideShowFolder + " cannot be found");
                return null;
            }

            List<ProductSlideShowSettings> settingsList = new List<ProductSlideShowSettings>();

            ProductSlideShowSettings stu300 = new ProductSlideShowSettings();
            stu300.Vendor = STU300.VENDOR_NAME;
            stu300.ProductModel = STU300.PRODUCT_MODEL;
            stu300.Vid = STU300.VID;
            stu300.Pid = STU300.PID;
            stu300.Interval = _intervalInSeconds;
            string folder = Path.Combine(SlideShowFolder, stu300.ProductModel);
            stu300.ImageList = getImagesInFolder(folder);
            settingsList.Add(stu300);

            ProductSlideShowSettings stu430 = new ProductSlideShowSettings();
            stu430.Vendor = STU430.VENDOR_NAME;
            stu430.ProductModel = STU430.PRODUCT_MODEL;
            stu430.Vid = STU430.VID;
            stu430.Pid = STU430.PID;
            stu430.Interval = _intervalInSeconds;
            folder = Path.Combine(SlideShowFolder, stu430.ProductModel);
            stu430.ImageList = getImagesInFolder(folder);
            settingsList.Add(stu430);

            ProductSlideShowSettings stu430v = new ProductSlideShowSettings();
            stu430v.Vendor = STU430V.VENDOR_NAME;
            stu430v.ProductModel = STU430V.PRODUCT_MODEL;
            stu430v.Vid = STU430V.VID;
            stu430v.Pid = STU430V.PID;
            stu430v.Interval = _intervalInSeconds;
            folder = Path.Combine(SlideShowFolder, stu430v.ProductModel);
            stu430v.ImageList = getImagesInFolder(folder);
            settingsList.Add(stu430v);

            ProductSlideShowSettings stu500 = new ProductSlideShowSettings();
            stu500.Vendor = STU500.VENDOR_NAME;
            stu500.ProductModel = STU500.PRODUCT_MODEL;
            stu500.Vid = STU500.VID;
            stu500.Pid = STU500.PID;
            stu500.Interval = _intervalInSeconds;
            folder = Path.Combine(SlideShowFolder, stu500.ProductModel);
            stu500.ImageList = getImagesInFolder(folder);
            settingsList.Add(stu500);

            ProductSlideShowSettings stu520 = new ProductSlideShowSettings();
            stu520.Vendor = STU520.VENDOR_NAME;
            stu520.ProductModel = STU520.PRODUCT_MODEL;
            stu520.Vid = STU520.VID;
            stu520.Pid = STU520.PID;
            stu520.Interval = _intervalInSeconds;
            folder = Path.Combine(SlideShowFolder, stu520.ProductModel);
            stu520.ImageList = getImagesInFolder(folder);
            settingsList.Add(stu520);

            ProductSlideShowSettings stu530 = new ProductSlideShowSettings();
            stu530.Vendor = STU530.VENDOR_NAME;
            stu530.ProductModel = STU530.PRODUCT_MODEL;
            stu530.Vid = STU530.VID;
            stu530.Pid = STU530.PID;
            stu530.Interval = _intervalInSeconds;
            folder = Path.Combine(SlideShowFolder, stu530.ProductModel);
            stu530.ImageList = getImagesInFolder(folder);
            settingsList.Add(stu530);

            settings.Items = settingsList.ToArray();

            return settings;
        }
        
        private string[] getImagesInFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                string[] filenames = Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".bmp") || s.EndsWith(".jpg") || s.EndsWith(".gif") || s.EndsWith(".png"))
                    .ToArray();

                return filenames;
            }
            return null;
        }
        
        private void loadSlideShowSettings(SlideShowSettings settings)
        {
            Log("Load slide show settings");
            cboPenDevice.Items.Clear();

            if(settings.Items != null)
            {
                foreach (ProductSlideShowSettings ps in settings.Items)
                {
                    PenDevice pd = _deviceScanner.IdentifyPenDevice(ps.Vid, ps.Pid);
                    if (pd != null)
                    {
                        _penDeviceList.Add(pd);
                        cboPenDevice.Items.Add(pd.ProductModel);
                    }
                }
            }
            
            if (_penDeviceList.Count > 0)
            {
                Log("Setting " + _penDeviceList[0].ProductModel + " as selected");
                cboPenDevice.SelectedIndex = 0;
                _currentPenDevice = _penDeviceList[0];
                showSlideShowImagesForDevice(_currentPenDevice);

            }
        }
        
        private void Log(string msg)
        {
            Log(msg, 0);
        }

        private void Log(string msg, int alertType)
        {
            if (LogFunction != null)
            {
                LogFunction(msg, alertType);
            }
        }
        
        private void PenDeviceLog(string msg, int alertType)
        {
            Log("[PenDevice] " + msg, 12);
        }

        private void SlideshowManager_Load(object sender, EventArgs e)
        {
            loadSlideShowSettings(_currentSlideShowSettings);
        }

        private void SlideshowManager_Resize(object sender, EventArgs e)
        {
            
        }

        private void ChangeAspectRatioOfImageList(PenDevice signpad)
        {
            float aspectRatio = (float)signpad.ScreenDimension.Width / (float)signpad.ScreenDimension.Height;
            int width = (int)(imageList.ImageSize.Height * aspectRatio);
            int height = imageList.ImageSize.Height;
            imageList.ImageSize = new Size(width, height);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            loadSlideShowSettings(_currentSlideShowSettings);
        }

        private void removeItemFromListView(ListViewItem lvi)
        {
            Log("Removing list view item : " + lvi.ImageIndex);
            SuspendLayout();
            int removeIndex = lvi.ImageIndex;
            int i = removeIndex + 1;
            for(;i<listView.Items.Count; i++)
            {
                ListViewItem item = listView.Items[i];
                item.ImageIndex--;
            }
            
            imageList.Images.RemoveAt(removeIndex);
            listView.Items.RemoveAt(removeIndex);
            _currentImageFiles.RemoveAt(removeIndex);

            ResumeLayout();
            listView.Refresh();
        }

        private void addImageToListView(string filename)
        {
            Log("Adding " + filename + " to image list");
            Image img = Image.FromFile(filename, true);
            imageList.Images.Add(img);

            int nextIndex = listView.Items.Count;

            ListViewItem lvi = new ListViewItem();
            lvi.Text = Path.GetFileName(filename);
            lvi.ImageIndex = nextIndex;
            listView.Items.Add(lvi);

            _currentImageFiles.Add(filename);
        }

        private void addImageToListView(string[] filenames)
        {
            foreach (string filename in filenames)
            {
                try
                {
                    addImageToListView(filename);
                }
                catch (Exception)
                {
                    Log("Failed adding " + filename);
                }
            }
            listView.LargeImageList = imageList;
            listView.Refresh();
        }

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            Log("Add image");
            openFileDialog.Filter = "Image files (*.jpg, *.gif, *.bmp, *.png) | *.jpg; *.gif; *.bmp; *.png";
            openFileDialog.Multiselect = true;

            if (Directory.Exists(_slideShowFolder))
            {
                Log("Set initial directory to " + _slideShowFolder);
                openFileDialog.InitialDirectory = _slideShowFolder;
            }

            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Log("Images selected");
                addImageToListView(openFileDialog.FileNames);
            }
            else
            {
                Log("Add image cancelled");
            }
        }

        private void saveSlideShowSettings(PenDevice penDevice)
        {
            Log("Save slide show settings");
            
            if (_currentSlideShowSettings == null)
            {
                Log("Current slide show settinsg is null, create");
                _currentSlideShowSettings = new SlideShowSettings();
            }

            ProductSlideShowSettings prodSettings = null;
            Log("Retrieving slide show settings for pendevice - " + penDevice.ProductModel);
            if(_currentSlideShowSettings.Items != null)
            {
                foreach (ProductSlideShowSettings ps in _currentSlideShowSettings.Items)
                {
                    if (ps.Vid == penDevice.Vid && ps.Pid == penDevice.Pid)
                    {
                        Log("Slideshow settings found");
                        prodSettings = ps;
                        break;
                    }
                }
            }
            
            if(prodSettings == null)
            {
                Log("Slideshow settings not found, attempting to create");
                prodSettings = new ProductSlideShowSettings();
                prodSettings.Vid = penDevice.Vid;
                prodSettings.Vendor = penDevice.VendorName;
                prodSettings.Pid = penDevice.Pid;
                prodSettings.ProductModel = penDevice.ProductModel;
                prodSettings.Interval = int.Parse(txtInterval.Text);

                List<ProductSlideShowSettings> psList = _currentSlideShowSettings.Items.ToList();
                psList.Add(prodSettings);
                _currentSlideShowSettings.Items = psList.ToArray();
            }

            prodSettings.ImageList = _currentImageFiles.ToArray();

            foreach(ProductSlideShowSettings prod in _currentSlideShowSettings.Items)
            {
                prod.Interval = int.Parse(txtInterval.Text);
            }

            WriteSlideShowSettingsFile(_currentSlideShowSettings, _slideShowSettingsPath);
        }

        private void showSlideShowImagesForDevice(PenDevice penDevice)
        {
            Log("Show slide show images for device " + penDevice.ProductModel);
            if (_currentSlideShowSettings == null)
            {
                Log("Current slide show settings is null");
                return;
            }

            Log("Clearing image list and list view");
            imageList.Images.Clear();
            listView.Items.Clear();
            _currentImageFiles.Clear();
            Log("Searching slideshow settings for current pen device");
            if(_currentSlideShowSettings.Items != null)
            {
                foreach (ProductSlideShowSettings settings in _currentSlideShowSettings.Items)
                {
                    if (settings.Vid == penDevice.Vid && settings.Pid == penDevice.Pid)
                    {
                        Log("Slideshow settings found for Vid: " + penDevice.Vid.ToString() + " Pid: " + penDevice.Pid.ToString());
                        ChangeAspectRatioOfImageList(penDevice);
                        addImageToListView(settings.ImageList);
                        txtInterval.Text = settings.Interval.ToString();
                        break;
                    }
                }
            }
            
        }

        private void cboPenDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            Log("Pen device changed");

            if (cboPenDevice.Items.Count == 0)
            {
                _currentPenDevice = null;
            }
            else
            {
                _currentPenDevice = _penDeviceList[cboPenDevice.SelectedIndex];
                Log("Set current pen device = " + _currentPenDevice.ProductModel);
                showSlideShowImagesForDevice(_currentPenDevice);
            }
        }
        
        public void AbortAllSlideShowThreads()
        {
            Log("Abort current slideshow threads : " + _slideShowThreads.Count.ToString());
            int count = 0;

            Log("Disconnect sign pads");
            foreach (PenDevice pd in _penDevicesWithSlideShowRunning)
            {
                pd.Disconnect();
                Log(pd.ProductModel + " disconnected");
            }

            foreach (Thread t in _slideShowThreads)
            {
                try
                {
                    t.Abort();
                    count++;
                }
                catch (Exception)
                {
                    Log("Error aborting", 1);
                }
            }
            Log(count.ToString() + " threads aborted");
            lblStatus.Text = strings.STOPPED;
            btnStartStop.Text = strings.START;
            _slideShowThreads.Clear();
            _penDevicesWithSlideShowRunning.Clear();
        }

        public void DisplaySlideShowForConnectedDevices()
        {
            Log("Display slide show for connected devices");

            AbortAllSlideShowThreads();

            List<PenDevice> connectedDevices = _deviceScanner.Scan();
            foreach(PenDevice connectedDevice in connectedDevices)
            {
                try
                {
                    Thread t = new Thread(new ParameterizedThreadStart(DisplaySlideShow));
                    _slideShowThreads.Add(t);
                    t.Start(connectedDevice);
                }
                catch (Exception)
                {
                    Log("Error starting slideshow thread for " + connectedDevice.ProductModel);
                }
            }

            lblStatus.Text = strings.RUNNING;
            btnStartStop.Text = strings.STOP;
        }

        private ProductSlideShowSettings GetProductSlideShowSettingsForPenDevice(SlideShowSettings settings, PenDevice penDevice)
        {
            if (settings.Items == null) return null;

            foreach (ProductSlideShowSettings ps in settings.Items)
            {
                if (ps.Vid == penDevice.Vid && ps.Pid == penDevice.Pid)
                {
                    return ps;
                }
            }

            return null;
        }

        private List<Bitmap> getSlideShowImages(ProductSlideShowSettings settings)
        {
            List<Bitmap> bmpList = new List<Bitmap>();

            Log("Get image files from current slide show settings");
            
            List<string> files = settings.ImageList.ToList();
            
            if (files != null && files.Count > 0)
            {
                Log(files.Count.ToString() + " files listed");
                foreach (string file in files)
                {
                    try
                    {
                        Bitmap bmp = new Bitmap(file);
                        bmpList.Add(bmp);
                    }
                    catch (Exception)
                    {
                        Log("File (" + file + ") is not a valid image");
                    }
                }
            }
            else
            {
                Log("No files listed for slideshow");
            }

            return bmpList;
        }
        
        private int getPenDevicePausedTiming(PenDevice penDevice)
        {
            if(_pauseDeviceDict != null && _pauseDeviceDict.Keys.Count > 0)
            {
                foreach(PenDevice key in _pauseDeviceDict.Keys)
                {
                    if (key.IsSameDevice(penDevice))
                    {
                        return _pauseDeviceDict[key];
                    }
                }
            }

            return 0;
        }

        private void completePauseTime(PenDevice penDevice)
        {
            if (_pauseDeviceDict != null && _pauseDeviceDict.Keys.Count > 0)
            {
                foreach (PenDevice key in _pauseDeviceDict.Keys)
                {
                    if (key.IsSameDevice(penDevice))
                    {
                        _pauseDeviceDict.Remove(key);
                        return; 
                    }
                }
            }
            
        }
        
        public void DisplaySlideShow(object penDevice)
        {
            if (penDevice.GetType().IsSubclassOf(typeof(PenDevice)) || penDevice.GetType() == typeof(PenDevice))
            {
                PenDevice subject = (PenDevice)penDevice;
                Log("Initiating slideshow for " + subject.ProductModel);
                _penDevicesWithSlideShowRunning.Add(subject);

                subject.LogFunction = PenDeviceLog;

                ProductSlideShowSettings prodSettings = GetProductSlideShowSettingsForPenDevice(_currentSlideShowSettings, subject);
                List<Bitmap> bmpList = getSlideShowImages(prodSettings);

                int i = 0;
                while (true)
                {
                    int pauseTime = getPenDevicePausedTiming(subject);
                    if (pauseTime > 0)
                    {
                        Log("Pause slideshow for " + pauseTime.ToString() + " milliseconds on " + subject.ProductModel);
                        Log("Slideshow for " + subject.ProductModel + " sleeping " + pauseTime.ToString() + "ms");
                        Thread.Sleep(pauseTime);
                        completePauseTime(subject);
                        Log("Pause slideshow finised");
                    }
                    
                    int connectError = subject.IsConnected() ? 0 : -1;
                    if (!subject.IsConnected())
                    {
                        Log(subject.ProductModel + " not connected, attempting connection");
                        connectError = subject.Connect();
                        Log(subject.PenDeviceErrorMessage((PEN_DEVICE_ERROR)connectError));
                    }
                    if(connectError == 0)
                    {
                        Log("Slideshow display for " + subject.ProductModel, 2);
                        subject.DisplayBitmap(bmpList[i]);
                    }
                    Thread.Sleep(prodSettings.Interval * 1000);
                    i++;
                    if(i >= bmpList.Count)
                    {
                        i = 0;
                    }
                }
            }
            else
            {
                Log("Display slide show cannot start because pen device is invalid");
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            saveSlideShowSettings(_currentPenDevice);
        }

        private void SlideshowManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }

        private void txtInterval_TextChanged(object sender, EventArgs e)
        {
            int result = -1;
            if(int.TryParse(txtInterval.Text, out result) == false)
            {
                txtInterval.Text = "";
            }
        }

        private void listView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                foreach(ListViewItem lvi in listView.Items)
                {
                    if (lvi.Selected)
                    {
                        removeItemFromListView(lvi);
                    }
                }
            }
        }

        public void RefreshSlideShow()
        {
            bool run = Running();
            AbortAllSlideShowThreads();
            if (run) DisplaySlideShowForConnectedDevices();
        }

        public bool Running()
        {
            if (_slideShowThreads == null || _slideShowThreads.Count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            if(!Running())
            {
                DisplaySlideShowForConnectedDevices();
            }
            else
            {
                AbortAllSlideShowThreads();
            }
        }
    }
}
