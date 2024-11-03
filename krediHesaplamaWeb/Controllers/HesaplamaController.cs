using krediHesaplamaWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace krediHesaplamaWeb.Controllers
{
    public class HesaplamaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult hesaplama(KrediBilgileri krediBilgileri)
        {
            double KrediTutari = krediBilgileri.krediTutari;
            int TaksitSayisi = krediBilgileri.taksitSayisi;
            double YillikFaizOrani = krediBilgileri.yillikFaizOrani;
            double AylikFaiz = YillikFaizOrani / 12;
            double KKDF = krediBilgileri.kkdfOrani;
            //double AylikFaizOrani = (krediBilgileri.aylikFaizOrani * krediBilgileri.kkdfOrani) + krediBilgileri.aylikFaizOrani;
            double AylikFaizOrani = (AylikFaiz * krediBilgileri.kkdfOrani) + AylikFaiz;
            DateTime KrediTarihi = krediBilgileri.krediTarihi.Date;
            double TaksitTutari = KrediTutari * (AylikFaizOrani * Math.Pow(1 + AylikFaizOrani, TaksitSayisi)) / (Math.Pow(1 + AylikFaizOrani, TaksitSayisi) - 1);
            double KalanAnapara = KrediTutari;

            List<KrediBilgileri> hesaplanmisDegerler = new List<KrediBilgileri>();
            int taksitGunu = krediBilgileri.krediTarihi.Day; // kullanıcının girdiği gün
            DateTime ilkTaksitTarihi = krediBilgileri.krediTarihi.AddMonths(1);// kredi tarihinden 1 ay sonra
            DateTime sonTaksitTarihi = krediBilgileri.krediTarihi.AddMonths(TaksitSayisi);// kredi tarihinden taksit sayısı kadar git
            int toplamGunSayisi = (sonTaksitTarihi - ilkTaksitTarihi).Days + 30;// toplam gün farkını hesapla

            for (int i = 0; i < TaksitSayisi; i++)
            {
                DateTime sonrakiTaksitTarihi = new DateTime(KrediTarihi.Year, KrediTarihi.Month, taksitGunu).AddMonths(1);

                // cumartesiye gelirse cumaya, pazara gelirse pazartesiye yuvarlama yap.
                if (sonrakiTaksitTarihi.DayOfWeek == DayOfWeek.Saturday)
                {
                    sonrakiTaksitTarihi = sonrakiTaksitTarihi.AddDays(-1);
                }
                else if (sonrakiTaksitTarihi.DayOfWeek == DayOfWeek.Sunday)
                {
                    sonrakiTaksitTarihi = sonrakiTaksitTarihi.AddDays(1);
                }
                //vadeler arası gün farkını hesapla
                TimeSpan gunFarki = sonrakiTaksitTarihi - KrediTarihi;

                // ilgili hesaplamalar
                double FaizTutari = KalanAnapara * YillikFaizOrani * gunFarki.Days / 360;
                double KkdfTutari = FaizTutari * KKDF;
                double AnaparaTutari = TaksitTutari - (FaizTutari + KkdfTutari);

                KalanAnapara -= AnaparaTutari;

                // sonrakitaksittarihini kredi tarihine eşitliyoruz ki döngü başında tekrar sonraki ayı almak için
                KrediTarihi = sonrakiTaksitTarihi;

                hesaplanmisDegerler.Add(new KrediBilgileri
                {
                    taksitTarihi = sonrakiTaksitTarihi,
                    gunAraligi = gunFarki.Days,
                    taksitTutari = TaksitTutari,
                    anaparaTutari = AnaparaTutari,
                    faizTutari = FaizTutari,
                    kkdfTutari = KkdfTutari,
                    kalanAnapara = KalanAnapara,
                    guncelAnapara = 0,
                    guncelKalanAnapara = 0,
                });
            }
            double gunlukbazliEkTutar = KalanAnapara / toplamGunSayisi;
            double guncelKA = KrediTutari;

            foreach (var item in hesaplanmisDegerler)
            {
                // gün aralığına göre anaparaya eklenecek tutarı hesaplıyoruz
                double ekTutar = gunlukbazliEkTutar * item.gunAraligi;
                double guncelA = item.anaparaTutari + ekTutar; // burada ise güncel anaparaya ekliyoruz

                guncelKA -= guncelA;

                // burada listeye gönderiyoruz
                item.guncelAnapara = guncelA;
                item.guncelKalanAnapara = guncelKA;
            }
            return View(hesaplanmisDegerler);
        }
    }
}