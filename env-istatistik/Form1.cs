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
