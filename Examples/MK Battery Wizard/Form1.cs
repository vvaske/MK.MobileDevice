using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using MK.MobileDevice;

namespace MK_Battery_Wizard
{
    public partial class Form1 : MetroForm
    {
        iPhone iph;
        Splash sp;
        Timer t;
        public Form1()
        {
            Form.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            sp = new Splash();
            sp.Show();
            t = new Timer();
            t.Interval = 3000;
            t.Tick += (s, e) => initPhone();
            t.Start();
        }

        private void initPhone()
        {
            t.Stop();
            try
            {
                iph = new iPhone();
                iph.Connect += Iph_Connect;
                iph.Disconnect += Iph_Disconnect;
                label4.Text = "Waiting for devices...";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            sp.Hide();
            this.Show();
        }

        private void Iph_Disconnect(object sender, ConnectEventArgs args)
        {
            ResetData();
        }

        private void ResetData()
        {
            this.label4.Text = "No device detected.";
            this.label5.Text = "Status: No active connection.";
            this.label6.Text = "Battery Cycle Count: ";
            this.label7.Text = "Battery Design Maximum Capacity: ";
            this.label8.Text = "Battery Current Maximum Capacity: ";
            this.label9.Text = "Battery Monitor Capability: ";
            this.label10.Text = "Fully Charged: ";
            this.label11.Text = "Battery Percentage: ";
            this.label12.Text = "IsCharging: ";
        }

        void setStatus(string status)
        {
            label5.Text = "Status: "+status;
        }

        private void Iph_Connect(object sender, ConnectEventArgs args)
        {
            if (iph.IsConnected)
            {
                label4.Text = "Connected to [" + iph.DeviceProductType + "] " + iph.DeviceName + " iOS " + iph.ProductVersion;
                setStatus("Fetching Device Information...");
                Dictionary<string, dynamic> battInfo = iph.RequestBatteryInfo();
                setStatus("Processing Information...");
                /*
                foreach (KeyValuePair<string, dynamic> kvp in battInfo)
                {
                    MessageBox.Show(kvp.Key+" : "+kvp.Value.ToString());
                }
                */
                int cyclecount = (int)battInfo["CycleCount"];
                int designcapacity = (int)battInfo["DesignCapacity"];
                int currentcapacity = (int)battInfo["FullChargeCapacity"];
                bool gasgauge = battInfo["GasGaugeCapability"] == true;
                bool fullycharged = battInfo["FullyCharged"] == true;
                bool charging = battInfo["BatteryIsCharging"] == true;
                int currentCharge = (int)battInfo["BatteryCurrentCapacity"];
                label6.Text += cyclecount.ToString();
                label7.Text += designcapacity.ToString();
                label8.Text += currentcapacity.ToString();
                label9.Text += gasgauge.ToString();
                label10.Text += fullycharged.ToString();
                label11.Text += currentCharge.ToString();
                label12.Text += charging.ToString();
                setStatus("Idle.");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Check that the cable is firmly plugged in.\nIf the device is still not detected, enter the passcode and unlock the device.");
        }
    }
}
