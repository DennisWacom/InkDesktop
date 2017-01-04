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
using System.Reflection;

namespace InkDesktop
{
    public partial class PenDeviceInfo : Form
    {
        public PenDeviceInfo()
        {
            InitializeComponent();
        }

        public void ShowPenDeviceInfo(PenDevice penDevice)
        {
            if (penDevice == null) return;

            bool RmbToDisconnect = false;
            if (penDevice.IsConnected() == false)
            {
                int status = penDevice.Connect();
                RmbToDisconnect = true;
            }
            
            SortedDictionary<string, string> dict = new SortedDictionary<string, string>();
            PropertyInfo[] props = penDevice.GetType().GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                try
                {
                    PropertyInfo prop = props[i];
                    dict.Add(prop.Name, prop.GetValue(penDevice).ToString());
                }
                catch (Exception) { }
            }
            
            treeView.BeginUpdate();
            treeView.Nodes.Add(penDevice.ProductModel);

            int count = 0;
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                treeView.Nodes[0].Nodes.Add(kvp.Key);
                treeView.Nodes[0].Nodes[count].Nodes.Add(kvp.Value);
                count++;
            }

            treeView.EndUpdate();

            treeView.ExpandAll();

            if (RmbToDisconnect)
            {
                penDevice.Disconnect();
            }

            this.ShowDialog();
        }
    }
}
