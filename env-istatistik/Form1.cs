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

        string pwrf = Properties.Settings.Default.wrfDataPath, penv = Properties.Settings.Default.ecmDataPath, prad = Properties.Settings.Default.radDataPath, prs = Properties.Settings.Default.radSicaklikPath;

        private float ortalama(float[] veri)
        {
            float ort = 0;
            int say = veri.Length;
            int bolum = 0;
            for (int v = 0; v < say;v++)
            {
                if ((int)veri[v] != -99)
                {
                    ort += veri[v];
                    bolum++;
                }
            }
            if (bolum > 0)
                ort = ort / bolum;
            else
                ort = -99;
            return ort;
        }

        private float mb(float[] tahminVeri, float[] olcumVeri)
        {
            float tahminOrt = ortalama(tahminVeri);
            float olcumOrt = ortalama(olcumVeri);
            if ((int)tahminOrt != -99 && (int)olcumOrt != -99)
                return olcumOrt - tahminOrt;
            else
                return -99;
        }

        private float rmb(float[] tahminVeri, float[] olcumVeri)
        {
            float olcumOrt = ortalama(olcumVeri);
            float mb1 = mb(tahminVeri, olcumVeri);
            if ((int)olcumOrt != -99 && (int)mb1 != -99)
                return mb1 / olcumOrt;
            else
                return -99;
        }

        private float rms(float[] tahminVeri, float[] olcumVeri)
        {
            int bolum = 0;
            float farkToplam = 0;
            for (int v = 0; v < tahminVeri.Length; v++)
            {
                if ((int)tahminVeri[v] != -99 && (int)olcumVeri[v] != -99)
                {
                    farkToplam += (float)Math.Pow((tahminVeri[v] - olcumVeri[v]), 2);
                    bolum++;
                }
            }
            if (bolum != 0)
                return farkToplam / bolum;
            else
                return -99;
        }

        private float rrms(float[] tahminVeri, float[] olcumVeri)
        {
            float rms1 = rms(tahminVeri, olcumVeri);
            float olcumOrt = ortalama(olcumVeri);
            if ((int)olcumOrt != -99 && (int)rms1 != -99)
                return rms1 / olcumOrt;
            else
                return -99;
        }

        private float stdSapma(float[] tahminVeri, float[] olcumVeri)
        {
            float rms1 = rms(tahminVeri, olcumVeri);
            float mb1 = mb(tahminVeri, olcumVeri);
            if ((int)rms1 != -99 && (int)mb1 != -99)
            {
                rms1 = (float)Math.Pow(rms1, 2);
                mb1 = (float)Math.Pow(mb1, 2);
                return (float)Math.Sqrt(rms1 - mb1);
            }
            else
                return -99;
        }

        private float cor(float[] tahminVeri, float[] olcumVeri)
        {
            float tahminOrt = ortalama(tahminVeri), olcumOrt = ortalama(olcumVeri);
            float farkCarpim = 0, farkKareTahmin = 0, farkKareOlcum = 0, farkKareToplamTahmin = 0, farkKareToplamOlcum = 0, farkCarpimToplam = 0, payda = 0;
            if ((int)tahminOrt != -99 && (int)olcumOrt != -99)
            {
                for (int v = 0; v < tahminVeri.Length; v++)
                {
                    if ((int)tahminVeri[v] != -99 && (int)olcumVeri[v] != -99)
                    {
                        farkCarpim = (tahminVeri[v] - tahminOrt) * (olcumVeri[v] - olcumOrt);
                        farkKareTahmin = (float)Math.Pow(tahminVeri[v] - tahminOrt, 2);
                        farkKareOlcum = (float)Math.Pow(olcumVeri[v] - olcumOrt, 2);
                        farkKareToplamTahmin += farkKareTahmin;
                        farkKareToplamOlcum += farkKareOlcum;
                        farkCarpimToplam += farkCarpim;
                    }
                }
                payda = (float)Math.Sqrt(farkKareToplamTahmin * farkKareToplamOlcum);
            }
            if (payda != 0)
                return farkCarpimToplam / payda;
            else
                return -99;
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
                string tempYol = Properties.Settings.Default.tmpPath;
                File.WriteAllLines(tempYol + "d_17130.txt", yazList_17130.ToArray());
                File.WriteAllLines(tempYol + "d_17240.txt", yazList_17240.ToArray());
                File.WriteAllLines(tempYol + "d_17351.txt", yazList_17351.ToArray());
                File.WriteAllLines(tempYol + "d_17220.txt", yazList_17220.ToArray());
                File.WriteAllLines(tempYol + "d_17064.txt", yazList_17064.ToArray());
                File.WriteAllLines(tempYol + "d_17030.txt", yazList_17030.ToArray());
                File.WriteAllLines(tempYol + "d_17095.txt", yazList_17095.ToArray());
                File.WriteAllLines(tempYol + "d_17281.txt", yazList_17281.ToArray());
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
            int ilkSeviye = Int32.Parse(satirlar[0].Split(';')[5]);
            List<string> yazList = new List<string>();
            string tmpSatir = "";
            for (int i = 1; i < satirlar.Length; i++)
            {
                tmpSatir = satirlar[i];
                tekSatir = tmpSatir.Split(';');
                seviye = Int32.Parse(tekSatir[5]);
                if (seviye <= ilkSeviye + 1500)
                {
                    tmpSatir = tmpSatir.Replace('.', ',');
                    yazList.Add(tmpSatir);
                }
            }
            File.WriteAllLines(dosyaYolu, yazList.ToArray());
        }

        private void sicaklikFarkiHesapla(string dosyaYolu, string gTarih, string saat)
        {
            string yazDosya = prs + gTarih + "_rsf.txt";
            if(!(File.Exists(yazDosya)))
            {
                FileStream fs = File.Create(yazDosya);
                fs.Close();
            }
            string[] tumSatirlar = File.ReadAllLines(dosyaYolu);
            if (tumSatirlar.Length > 0)
            {
                string istNo = dosyaYolu.Substring(dosyaYolu.Length - 9, 5);
                float ilkS = 0;
                float n;
                if (float.TryParse(tumSatirlar[0].Split(';')[6], out n))
                    ilkS = float.Parse(tumSatirlar[0].Split(';')[6]);
                float tmpS = 0, sBas = 0, sBit = 0, sf = 0;
                bool envBulundu = false;
                for (int i = 1; i < tumSatirlar.Length; i++)
                {
                    if (float.TryParse(tumSatirlar[i].Split(';')[6], out n))
                        tmpS = float.Parse(tumSatirlar[i].Split(';')[6]);
                    else
                        continue;
                    if (envBulundu)
                    {
                        if (tmpS < ilkS)
                        {
                            sBit = ilkS;
                            break;
                        }
                    }
                    else
                    {
                        if (tmpS > ilkS)
                        {
                            envBulundu = true;
                            sBas = ilkS;
                        }
                    }
                    ilkS = tmpS;
                }
                sf = sBit - sBas;
                if (saat == "00")
                {
                    File.AppendAllText(yazDosya, istNo + ";00;" + sf.ToString("F1") + "\n");
                }
                else
                {
                    string[] satirTemp = File.ReadAllLines(yazDosya);
                    bool istVar = false;
                    for (int i = 0; i < satirTemp.Length; i++)
                    {
                        if (satirTemp[i].Contains(istNo))
                        {
                            satirTemp[i] += ";12;" + sf.ToString("F1");
                            istVar = true;
                            break;
                        }
                    }
                    if (istVar)
                    {
                        File.WriteAllLines(yazDosya, satirTemp);
                    }
                    else
                    {
                        File.AppendAllText(yazDosya, istNo + ";00;-99;12;" + sf.ToString("F1") + "\n");
                    }
                }
            }
        }

        private float[] veriAlRad(string path, int minDate, int maxDate, string ist, int sira)
        {
            List<float> alinan = new List<float>();
            string[] dosyalar = Directory.GetFiles(path);
            int dosyaTarih = 0;
            string strTarih = "";
            int n;
            string[] satirlar, tekSatir;
            char[] ayr = { ';'};
            for (int i = 0; i < dosyalar.Length; i++)
            {
                strTarih = dosyalar[i].Substring(dosyalar[i].Length - 16, 8);
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
                                break;
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
            string[] dosyalar = Directory.GetFiles(prad);
            string[] tmpDosyalar;
            string dosyaAd = "", tmpD;
            FileInfo fi, tmpFi;
            string dTarih = "";
            string ss = "00";
            for (int i = 0; i < dosyalar.Length; i++)
            {
                dosyaAd = dosyalar[i];
                fi = new FileInfo(dosyaAd);
                if (dosyaAd.Contains("radio_") && fi.Length > 0)
                {
                    dTarih = dosyaAd.Substring(dosyaAd.Length - 16, 8);
                    istasyonAyir(dosyaAd);
                    tmpDosyalar = Directory.GetFiles(Properties.Settings.Default.tmpPath);
                    for (int j = 0; j < tmpDosyalar.Length; j++)
                    {
                        tmpD = tmpDosyalar[j];
                        tmpFi = new FileInfo(tmpD);
                        if (tmpD.Contains("d_") && tmpFi.Length > 0)
                        {
                            if (dosyaAd.Contains("0549"))
                                ss = "00";
                            else
                                ss = "12";
                            seviyeLimit(tmpD);
                            sicaklikFarkiHesapla(tmpD, dTarih, ss);
                        }
                    }
                }
            }
            //int a = Int32.Parse(textBox1.Text);
            //string wrfPath = Properties.Settings.Default.wrfDataPath;
            //MessageBox.Show(a.ToString());
            //float[] wrfVeri = veriAlWrf(wrfPath, 20151011, 20160201, "Izmir", 4);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*string[] istasyonlar = { "17130", "17240", "17351", "17220", "17064", "17030", "17095", "17281" };
            string[] istasyonlar2 = { "Ankara", "Isparta", "Adana", "Izmir", "Istanbul", "Samsun", "Erzurum", "Diyarbakir" };
            int ilkTarih = 20150611, sonTarih = 20160331;
            for (int i = 0; i < istasyonlar.Length; i++)
            {
                float[] veriWrf = veriAlWrf(pwrf, ilkTarih, sonTarih, istasyonlar[i], 4);
                float[] veriRad = veriAlRad(prs, ilkTarih, sonTarih, istasyonlar2[i], 2);
            }*/
            //rTamamla(prs);
            ristTam(prs);
            MessageBox.Show("123");
        }

        private void rTamamla(string klasor)
        {
            string[] dosyalar = Directory.GetFiles(klasor);
            string[] satirlar, sutunlar;
            for (int i = 0; i < dosyalar.Length; i++)
            {
                satirlar = File.ReadAllLines(dosyalar[i]);
                for (int j = 0; j < satirlar.Length; j++)
                {
                    sutunlar = satirlar[j].Split(';');
                    while (sutunlar.Length < 5)
                    {
                        satirlar[j] += ";-99";
                        sutunlar = satirlar[j].Split(';');
                    }
                }
                File.WriteAllLines(dosyalar[i], satirlar);
            }
        }

        private void ristTam(string klasor)
        {
            //dosyalarda eksik istasyon varsa tamamlar
            string[] istasyonlar = { "17130", "17240", "17351", "17220", "17064", "17030", "17095", "17281" };
            string[] dosyalar = Directory.GetFiles(klasor);
            string[] satirlar;
            bool var;
            for (int i = 0; i < dosyalar.Length; i++)
            {
                satirlar = File.ReadAllLines(dosyalar[i]);
                for (int j = 0; j < istasyonlar.Length; j++)
                {
                    var = false;
                    for (int k = 0; k < satirlar.Length; k++)
                    {
                        if (satirlar[k].Contains(istasyonlar[j]))
                        {
                            var = true;
                        }
                    }
                    if (!var)
                        File.AppendAllText(dosyalar[i], istasyonlar[j] + ";0;-99;12;-99\n");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] istasyonlar = { "17130", "17240", "17351", "17220", "17064", "17030", "17095", "17281" };
            float[] envSicakliklar, radSicakliklar;
            List<string> yazSatirlar = new List<string>();
            float fltTemp = 0;
            string strTemp = "";
            yazSatirlar.Add("Istasyon;Yil;ay;MB;RMB;RMS;RRMS;STD;COR");
            for (int i = 0; i < istasyonlar.Length; i++)
            {
                for (int yil = 2015; yil <= 2016; yil++)
                {
                    for (int ay = 1; ay <= 12; ay++)
                    {
                        envSicakliklar = envSOku(istasyonlar[i], yil, ay, 0);
                        radSicakliklar = radSOku(istasyonlar[i], yil, ay, 0);
                        fltTemp = mb(envSicakliklar, radSicakliklar);
                        strTemp = istasyonlar[i] + ";" + yil.ToString() + ";" + ay.ToString() + ";" + formatla(fltTemp);
                        fltTemp = rmb(envSicakliklar, radSicakliklar);
                        strTemp += ";" + formatla(fltTemp);
                        fltTemp = rms(envSicakliklar, radSicakliklar);
                        strTemp += ";" + formatla(fltTemp);
                        fltTemp = rrms(envSicakliklar, radSicakliklar);
                        strTemp += ";" + formatla(fltTemp);
                        fltTemp = stdSapma(envSicakliklar, radSicakliklar);
                        strTemp += ";" + formatla(fltTemp);
                        fltTemp = cor(envSicakliklar, radSicakliklar);
                        strTemp += ";" + formatla(fltTemp);
                        yazSatirlar.Add(strTemp);
                    }
                }
            }
            File.WriteAllLines("ecmrad.txt", yazSatirlar.ToArray());
        }

        private string formatla(float sayi)
        {
            string strDelim = "F3";
            if ((int)sayi == -99)
                return "-99";
            else
                return sayi.ToString(strDelim);
        }

        private string sifirEkle(int gSayi)
        {
            string sonuc = gSayi.ToString();
            if (gSayi < 10)
                sonuc = "0" + gSayi;
            return sonuc;
            
        }

        private int aySonGun(int yil, int ay)
        {
            int sonGun = 31;
            if (ay == 2)
                if (yil % 4 == 0)
                    sonGun = 29;
                else
                    sonGun = 28;
            else if (ay == 4 || ay == 6 || ay == 9 || ay == 11)
                sonGun = 30;
            return sonGun;
        }

        private float[] radSOku(string istasyon, int yil, int ay, int olcum)
        {
            List<float> radSicakliklar = new List<float>();
            int sonGun = aySonGun(yil, ay);
            string[] satirlar, sutunlar;
            string gun = "01", teksatir = "", okunacakDosya = "";
            bool istVar = false;
            float n;
            int sutunSira = 2, uzunluk = 3;
            if (olcum == 12)
            {
                sutunSira = 4;
                uzunluk = 5;
            }
            for (int i = 1; i <= sonGun; i++)
            {
                gun = sifirEkle(i);
                okunacakDosya = Properties.Settings.Default.radSicaklikPath + yil + sifirEkle(ay) + gun + "_rsf.txt";
                if (File.Exists(okunacakDosya))
                {
                    satirlar = File.ReadAllLines(okunacakDosya);
                    for (int k = 0; k < satirlar.Length; k++)
                    {
                        teksatir = satirlar[k];
                        if (teksatir.Contains(istasyon))
                        {
                            istVar = true;
                            sutunlar = teksatir.Split(';');
                            if (sutunlar.Length >= uzunluk)
                            {
                                if (float.TryParse(sutunlar[sutunSira], out n))
                                    radSicakliklar.Add(float.Parse(sutunlar[sutunSira]));
                                else
                                    radSicakliklar.Add(-99);
                            }
                            else
                            {
                                radSicakliklar.Add(-99);
                            }
                        }
                    }
                    if (!istVar) radSicakliklar.Add(-99);
                    istVar = false;
                }
                else
                {
                    radSicakliklar.Add(-99);
                }
            }
            return radSicakliklar.ToArray();
        }

        private string istAdBul(string istNo)
        {
            string istAd = "";
            switch (istNo)
            {
                case "17030":
                    istAd = "SAMSUN";
                    break;
                case "17064":
                    istAd = "ISTANBUL";
                    break;
                case "17095":
                    istAd = "ERZURUM";
                    break;
                case "17130":
                    istAd = "ANKARA";
                    break;
                case "17220":
                    istAd = "IZMIR";
                    break;
                case "17240":
                    istAd = "ISPARTA";
                    break;
                case "17281":
                    istAd = "DIYARBAKIR";
                    break;
                case "17351":
                    istAd = "ADANA";
                    break;
            }
            return istAd;
        }

        private float[] envSOku(string istasyon, int yil, int ay, int olcum)
        {
            List<float> envSicakliklar = new List<float>();
            int sonGun = aySonGun(yil, ay);
            string[] satirlar, sutunlar;
            string gun = "01", teksatir = "", okunacakDosya = "";
            bool istVar = false;
            float n;
            string istAd = istAdBul(istasyon);
            string olcumSaati = "12";
            if (olcum == 12) olcumSaati = "24";
            for (int i = 1; i <= sonGun; i++)
            {
                gun = sifirEkle(i);
                okunacakDosya = Properties.Settings.Default.ecmDataPath + yil + sifirEkle(ay) + gun + "_fark_env_out.txt";
                if (File.Exists(okunacakDosya))
                {
                    satirlar = File.ReadAllLines(okunacakDosya);
                    for (int k = 0; k < satirlar.Length; k++)
                    {
                        teksatir = satirlar[k];
                        if (teksatir.Contains(istAd + " " + olcumSaati))
                        {
                            istVar = true;
                            sutunlar = teksatir.Split(new string[] { }, StringSplitOptions.RemoveEmptyEntries);
                            if (sutunlar.Length >= 3)
                            {
                                if (float.TryParse(sutunlar[2], out n))
                                    envSicakliklar.Add(float.Parse(sutunlar[2]));
                                else
                                    envSicakliklar.Add(-99);
                            }
                            else
                            {
                                envSicakliklar.Add(-99);
                            }
                            break;
                        }
                    }
                    if (!istVar) envSicakliklar.Add(-99);
                    istVar = false;
                }
                else
                {
                    envSicakliklar.Add(-99);
                }
            }
            return envSicakliklar.ToArray();
        }

        private float[] wrfSOku(string istasyon, int yil, int ay, int olcum)
        {
            List<float> wrfSicakliklar = new List<float>();
            int sonGun = aySonGun(yil, ay);
            string[] satirlar, sutunlar;
            string gun = "01", teksatir = "", okunacakDosya = "";
            bool istVar = false;
            float n;
            int sutunSira = 11, uzunluk = 12;
            if (olcum == 12)
            {
                sutunSira = 4;
                uzunluk = 5;
            }
            for (int i = 1; i <= sonGun; i++)
            {
                gun = sifirEkle(i);
                okunacakDosya = Properties.Settings.Default.wrfDataPath + yil + sifirEkle(ay) + gun + "fark.log";
                if (File.Exists(okunacakDosya))
                {
                    satirlar = File.ReadAllLines(okunacakDosya);
                    for (int k = 0; k < satirlar.Length; k++)
                    {
                        teksatir = satirlar[k];
                        if (teksatir.Contains(istasyon))
                        {
                            istVar = true;
                            sutunlar = teksatir.Split(new string[] { }, StringSplitOptions.RemoveEmptyEntries);
                            if (sutunlar.Length >= uzunluk)
                            {
                                if (float.TryParse(sutunlar[sutunSira], out n))
                                    wrfSicakliklar.Add(float.Parse(sutunlar[sutunSira]));
                                else
                                    wrfSicakliklar.Add(-99);
                            }
                            else
                            {
                                wrfSicakliklar.Add(-99);
                            }
                        }
                    }
                    if (!istVar) wrfSicakliklar.Add(-99);
                    istVar = false;
                }
                else
                {
                    wrfSicakliklar.Add(-99);
                }
            }
            return wrfSicakliklar.ToArray();
        }
    }
}
