using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;

namespace Aplikacja3
{
    class Tapeta
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        public void Polaczenie(ref string odp, byte i)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create("http://www.bing.com/hpimagearchive.aspx?format=xml&idx=" + i + "&n=1&mbl=1&mkt=en-US");
            webrequest.KeepAlive = false;
            webrequest.Method = "GET";
            using (HttpWebResponse odpstrony = (HttpWebResponse)webrequest.GetResponse())
            using (StreamReader zapstrony = new StreamReader(odpstrony.GetResponseStream()))
                odp = zapstrony.ReadToEnd();
        }

        public void ProcesXml(ref string xmlString)
        {
            using (System.Xml.XmlReader czyt = System.Xml.XmlReader.Create(new StringReader(xmlString)))
            {
                czyt.ReadToFollowing("urlBase");
                xmlString = czyt.ReadElementContentAsString();
            }
        }

        public void UstTapeta(string plik)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER,
            0,
            plik,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
    class Program
    {
        private static String imgDir = @"F:\Wallpaper\";
        
        static void Main(string[] args)
        {
            String imageFileName;
            if (!Directory.Exists(imgDir))
                Directory.CreateDirectory(imgDir);
            byte i = 0;
            imageFileName = imgDir + "Zdj" + DateTime.Today.ToString("yyyy-mm-dd") + ".jpg";
            Tapeta tap = new Tapeta();
            string odpowiedz = null;
            tap.Polaczenie(ref odpowiedz, i);
            tap.ProcesXml(ref odpowiedz);
            using (WebClient client = new WebClient())
                client.DownloadFile("http://www.bing.com" + odpowiedz + "_1366x768.jpg", imageFileName);
            tap.UstTapeta(imageFileName);
            File.Delete(imageFileName);
        }
    }
}
