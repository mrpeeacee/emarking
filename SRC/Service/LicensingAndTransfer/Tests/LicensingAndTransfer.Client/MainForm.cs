using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LicensingAndTransfer.Client
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            //TODO: Call proxy method
            CreateFTPFolder();
        }

        public void CreateFTPFolder()
        {
            //localhost.LicensingAndTransferClient objClient = new LicensingAndTransfer.Client.localhost.LicensingAndTransferClient();
            //BindingList<localhost.PackageStatistics> objList = null;
            ////objClient.CreateFTPSession("", true, "", LicensingAndTransfer.Client.localhost.ServerTypes.ControllerOfExamination, LicensingAndTransfer.Client.localhost.Operations.QPackFetch, ref objList,localhost.TransferMedium.SharedPath);
        }
    }
}