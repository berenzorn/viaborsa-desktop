using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace viaborsa {
    public partial class Form1 : Form {
        private static readonly String[] dns_names = new string[] { "<proxy 1>", "<proxy 2>" };
        private static readonly int[] ports = new int[] { <port 1>, <port 2>, <port 3>, <port 4> };
        private static readonly byte[] packetData = new byte[] { 0x00, 0x23, 0xf8, 0x4e, 0x45,
            0x7e, 0xf4, 0x6d, 0x04, 0x39, 0xd2, 0xeb, 0x08, 0x00, 0x45, 0x00, 0x00, 0x34, 0x13,
            0xea, 0x40, 0x00, 0x80, 0x06, 0x61, 0x19, 0xc0, 0xa8, 0x01, 0x22, 0x5d, 0x59, 0x66,
            0x9d, 0x65, 0x48, 0x00, 0x50, 0x05, 0xe6, 0x36, 0xbb, 0x00, 0x00, 0x00, 0x00, 0x80,
            0x02, 0x20, 0x00, 0x27, 0xde, 0x00, 0x00, 0x02, 0x04, 0x04, 0xec, 0x01, 0x03, 0x03,
            0x08, 0x01, 0x01, 0x04, 0x02 };

        public Form1() {
            InitializeComponent();
        }

        private String rdp_form(String addr) {
            String[] rdp_array = new string[]
            {
                "authentication level:i:0", "negotiate security layer:i:1", "screen mode id:i:2",
                "use multimon:i:0", "session bpp:i:16", "compression:i:1", "keyboardhook:i:2",
                "audiocapturemode:i:0", "videoplaybackmode:i:1", "connection type:i:6",
                "networkautodetect:i:0", "bandwidthautodetect:i:1", "displayconnectionbar:i:1",
                "enableworkspacereconnect:i:0", "disable wallpaper:i:0", "allow font smoothing:i:1",
                "allow desktop composition:i:1", "disable full window drag:i:0", "disable menu anims:i:0",
                "disable themes:i:0", "disable cursor setting:i:0", "bitmapcachepersistenable:i:1",
                "audiomode:i:2", "redirectprinters:i:1", "redirectcomports:i:0", "redirectsmartcards:i:0",
                "redirectclipboard:i:1", "redirectposdevices:i:0", "autoreconnection enabled:i:1",
                "prompt for credentials:i:0", "remoteapplicationmode:i:0", "gatewayusagemethod:i:4",
                "gatewaycredentialssource:i:4", "gatewayprofileusagemethod:i:1", "promptcredentialonce:i:0",
                "gatewaybrokeringtype:i:0", "use redirection server name:i:0", "rdgiskdcproxy:i:0"
            };
            String rdp_addr = "full address:s:" + addr;
            int rnd = new Random().Next(135791113, 246810121);
            String name = rnd.ToString() + ".rdp";
            File.WriteAllLines(name, rdp_array);
            using (StreamWriter file = new StreamWriter(name, true))
                file.WriteLine(rdp_addr);
            return name;
        }

        private void button1_Click(object sender, EventArgs e) {
            IPAddress addr = null;
            String proxy_name = "";
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            if (radioButton1.Checked) {
                addr = Dns.GetHostAddresses(dns_names[0])[0];
                proxy_name = dns_names[0];
            }
            else if (radioButton2.Checked) {
                addr = Dns.GetHostAddresses(dns_names[1])[0];
                proxy_name = dns_names[1];
            }
            progressBar1.Step = 5;
            foreach (var port in ports) {
                socket.SendTo(packetData, new IPEndPoint(addr, port));
                for (int i = 0; i < 50; i++) {
                    progressBar1.PerformStep();
                    Application.DoEvents();
                    Thread.Sleep(10);
                }
            }
            Thread.Sleep(500);
            Application.DoEvents();
            String filename = rdp_form(proxy_name);
            ProcessStartInfo rdp = new ProcessStartInfo();
            rdp.FileName = "mstsc.exe";
            rdp.Arguments = filename + " /f";
            Process.Start(rdp);
            progressBar2.Step = -5;
            for (int i = 0; i < 2000; i++) {
                Thread.Sleep(10);
                progressBar2.PerformStep();
                Application.DoEvents();
            }
            File.Delete(filename);
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e) {

        }
    }
}
