using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InkPlatform.Hardware;
using InkPlatform.Hardware.Wacom;
using InkPlatform.Ink;
using InkPlatform.UserControls;
using InkPlatform.UserInterface;

namespace InkDesktop
{
    public partial class SignatureCapture : Form
    {
        DeviceScanner _deviceScanner;
        List<PenDevice> _penDeviceList = new List<PenDevice>();
        PenDevice _currentPenDevice = null;

        public delegate void SendLog(string msg, int alertType);
        public SendLog LogFunction;
        
        public SignatureCapture()
        {
            InitializeComponent();
            _deviceScanner = new DeviceScanner();
        }
        
        private void loadPenDevices()
        {
            Log("LoadPenDevices");
            cboPenDevice.Items.Clear();
            _penDeviceList = _deviceScanner.Scan();
            foreach(PenDevice penDevice in _penDeviceList)
            {
                cboPenDevice.Items.Add(penDevice.ProductModel);
            }

            if(_penDeviceList.Count > 0)
            {
                cboPenDevice.SelectedIndex = 0;
                _currentPenDevice = _penDeviceList[0];
                resizeForm(_currentPenDevice);
            }
        }

        private void Log(string msg)
        {
            Log(msg, 0);
        }

        private void Log(string msg, int alertType)
        {
            if(LogFunction != null)
            {
                LogFunction("[SignatureCapture] " + msg, alertType);
            }
        }

        private void resizeForm(PenDevice signpad)
        {
            Log("Resize Form");
            signpadControl.Width = signpad.ScreenDimension.Width;
            signpadControl.Height = signpad.ScreenDimension.Height;
            ClientSize = new Size(signpadControl.Width, signpadControl.Location.Y + signpadControl.Size.Height);
            //MessageBox.Show(ClientSize.Width.ToString() + " - " + signpadControl1.Width.ToString());
        }

        private void SignatureCapture_Load(object sender, EventArgs e)
        {
            Log("Form loaded");
            loadPenDevices();
        }

        private void cboPenDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            Log("Pen device changed");

            if(cboPenDevice.Items.Count == 0)
            {
                _currentPenDevice = null;
            }
            else
            {
                if(_currentPenDevice != null)
                {
                    Log("Disconnect " + _currentPenDevice.ProductModel);
                    _currentPenDevice.Disconnect();
                }
                _currentPenDevice = _penDeviceList[cboPenDevice.SelectedIndex];
                Log("Set current pen device = " + _currentPenDevice.ProductModel);
                resizeForm(_currentPenDevice);
            }

            
        }

        private void btnSign_Click(object sender, EventArgs e)
        {
            Log("Signature requested");
            if(_currentPenDevice != null)
            {
                signpadControl.DonePressed = SignatureDone;
                Log("Set inking = true");
                signpadControl.SetInking(true);
                Log("Capture signature for " + txtName.Text + " with reason = " + txtReason.Text);
                int result = signpadControl.CaptureSignature(txtName.Text, txtReason.Text, _currentPenDevice);
                if(result != (int)PEN_DEVICE_ERROR.NONE)
                {
                    MessageBox.Show(SerializablePenDevice.ErrorMessage((PEN_DEVICE_ERROR)result));
                }
            }
            else
            {
                Log("No pen devices attached");
            }
            
        }

        private void SaveAs()
        {
            Log("Save As");
            if(signpadControl.CurrentBitmap == null)
            {
                MessageBox.Show(strings.NO_SIGNATURE_PRESENT);
                return;
            }

            saveFileDialog.Filter = "Portable Network Graphics | *.png";
            DialogResult res = saveFileDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                signpadControl.CurrentBitmap.Save(saveFileDialog.FileName);
            }
        }

        private bool SignatureDone(object sender, EventArgs e)
        {
            Log("Signature done");
            if (sender.GetType() == typeof(ElementButton))
            {
                ElementButton btn = (ElementButton)sender;
                if (btn.NextScreenName != null && btn.NextScreenName != "")
                {
                    return true;
                }
            }

            if (sender.GetType() == typeof(ElementImage))
            {
                ElementImage btn = (ElementImage)sender;
                if (btn.NextScreenName != null && btn.NextScreenName != "")
                {
                    return true;
                }
            }

            Log("Set inking = false");
            signpadControl.SetInking(false);

            Bitmap bitmap = new Bitmap(_currentPenDevice.ScreenDimension.Width, _currentPenDevice.ScreenDimension.Height);
            if (InkProcessor.GenerateImageFromInkData(
                out bitmap,
                signpadControl.PenData,
                _currentPenDevice.TabletDimension,
                _currentPenDevice.ScreenDimension,
                signpadControl.GetDefaultPen(),
                Color.White,
                true,
                true) == InkProcessor.GenerateImageResult.Successful
            )
            {
                Log("Show signature image");
                signpadControl.DisplayBitmap(bitmap, _currentPenDevice);
                
                return false;
            }
            else
            {
                return true;
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Log("Cancel clicked");
            Log("Clear Screen");
            signpadControl.ClearScreen();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("Copy");
            if (signpadControl.CurrentBitmap == null)
            {
                Log("No signature to copy");
                MessageBox.Show(strings.NO_SIGNATURE_PRESENT);
                return;
            }
            else
            {
                Clipboard.SetImage(signpadControl.CurrentBitmap);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAs();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            loadPenDevices();
        }

        private void SignatureCapture_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log("Form closed");
            if(_currentPenDevice != null)
            {
                _currentPenDevice.Disconnect();
            }
        }
    }
}
