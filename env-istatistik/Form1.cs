using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace env_istatistik
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private float ortalama(float[] veri)
        {
            float ort = 0;
            int say = veri.Length;
            for (int v = 0; v < say;v++)
            {
                ort += veri[v];
            }
            if (say > 0)
                ort = ort / say;
            return ort;
        }

        private float mb(float[] gVeri, float[] mVeri)
        {
            float gOrt = ortalama(gVeri);
            float mOrt = ortalama(mVeri);
            return mOrt - gOrt;
        }

        private float[] veriAlWrf(string path, int minDate, int maxDate, string ist, int sira)
        {
            List<float> alinan = new List<float>();
            string[] dosyalar = Directory.GetFiles(path);
            int dosyaTarih = 0;
            string strTarih = "";
            int n;
            string[] satirlar, tekSatir;
            char[] ayr = { '\t', ' ' };
            for (int i = 0; i < dosyalar.Length; i++)
            {
                strTarih = dosyalar[i].Substring(dosyalar[i].Length - 17, 8);
                if (int.TryParse(strTarih, out n))
                {
                    dosyaTarih = Int32.Parse(strTarih);
                    if (dosyaTarih >= minDate && dosyaTarih <= maxDate)
                    {
                        satirlar = File.ReadAllLines(dosyalar[i]);
                        for (int j = 0; j < satirlar.Length; j++)
                        {
                            if (satirlar[j].Contains(ist))
                            {
                                tekSatir = satirlar[j].Split(ayr);
                                alinan.Add(float.Parse(tekSatir[sira]));
                            }
                        }
                    }
                }
            }
            return alinan.ToArray();
        }

        private float[] veriAlEnv(string path, int minDate, int maxDate, string ist, int sira)
        {
            List<float> alinan = new List<float>();
            string[] dosyalar = Directory.GetFiles(path);
            int dosyaTarih = 0;
            string strTarih = "";
            int n;
            string[] satirlar, tekSatir;
            char[] ayr = { '\t', ' ' };
            for (int i = 0; i < dosyalar.Length; i++)
            {
                strTarih = dosyalar[i].Substring(dosyalar[i].Length - 25, 8);
                if (int.TryParse(strTarih, out n))
                {
                    dosyaTarih = Int32.Parse(strTarih);
                    if (dosyaTarih >= minDate && dosyaTarih <= maxDate)
                    {
                        satirlar = File.ReadAllLines(dosyalar[i]);
                        for (int j = 0; j < satirlar.Length; j++)
                        {
                            if (satirlar[j].Contains(ist))
                            {
                                tekSatir = satirlar[j].Split(ayr);
                                alinan.Add(float.Parse(tekSatir[sira]));
                            }
                        }
                    }
                }
            }
            return alinan.ToArray();
        }

        private void istasyonAyir(string dosyaYolu)
        {
            string[] istasyonlar = { "17130", "17240", "17351", "17220", "17064", "17030", "17095", "17281" };
            try
            {
                string satir = "";
                int istasyon = 0;
                string[] degerler;
                StreamReader rdrDosya = new StreamReader(dosyaYolu);
                List<string> yazList_17130 = new List<string>();
                List<string> yazList_17240 = new List<string>();
                List<string> yazList_17351 = new List<string>();
                List<string> yazList_17220 = new List<string>();
                List<string> yazList_17064 = new List<string>();
                List<string> yazList_17030 = new List<string>();
                List<string> yazList_17095 = new List<string>();
                List<string> yazList_17281 = new List<string>();
                while ((satir = rdrDosya.ReadLine()) != null)
                {
                    degerler = satir.Split(';');
                    istasyon = Int32.Parse(degerler[0]);
                    switch (istasyon)
                    {
                        case 17130:
                            yazList_17130.Add(satir);
                            break;
                        case 17240:
                            yazList_17240.Add(satir);
                            break;
                        case 17351:
                            yazList_17351.Add(satir);
                            break;
                        case 17220:
                            yazList_17220.Add(satir);
                            break;
                        case 17064:
                            yazList_17064.Add(satir);
                            break;
                        case 17030:
                            yazList_17030.Add(satir);
                            break;
                        case 17095:
                            yazList_17095.Add(satir);
                            break;
                        case 17281:
                            yazList_17281.Add(satir);
                            break;
                        default: break;
                    }
                }
                rdrDosya.Close();
                File.WriteAllLines("d_17130.txt", yazList_17130.ToArray());
                File.WriteAllLines("d_17240.txt", yazList_17240.ToArray());
                File.WriteAllLines("d_17351.txt", yazList_17351.ToArray());
                File.WriteAllLines("d_17220.txt", yazList_17220.ToArray());
                File.WriteAllLines("d_17064.txt", yazList_17064.ToArray());
                File.WriteAllLines("d_17030.txt", yazList_17030.ToArray());
                File.WriteAllLines("d_17095.txt", yazList_17095.ToArray());
                File.WriteAllLines("d_17281.txt", yazList_17281.ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void seviyeLimit(string dosyaYolu)
        {
            string[] satirlar = File.ReadAllLines(dosyaYolu);
            string[] tekSatir;
            int seviye = 0;
            int ilkSeviye = Int32.Parse(satirlar[0].Split(';')[0]);
            List<string> yazList = new List<string>();
            string tmpSatir = "";
            for (int i = 1; i < satirlar.Length; i++)
            {
                tmpSatir = satirlar[i];
                tekSatir = tmpSatir.Split(';');
                seviye = Int32.Parse(tekSatir[0]);
                if (seviye <= ilkSeviye + 1500)
                {
                    tmpSatir = tmpSatir.Replace('.', ',');
                    yazList.Add(tmpSatir);
                }
            }
            File.WriteAllLines(dosyaYolu, yazList.ToArray());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int a = Int32.Parse(textBox1.Text);
            string wrfPath = Properties.Settings.Default.wrfDataPath;
            //MessageBox.Show(a.ToString());
            float[] wrfVeri = veriAlWrf(wrfPath, 20151011, 20160201, "Izmir", 4);
        }
    }
}
