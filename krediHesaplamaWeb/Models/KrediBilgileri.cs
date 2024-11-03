namespace krediHesaplamaWeb.Models
{
    public class KrediBilgileri
    {
        public double krediTutari { get; set; }
        public int taksitSayisi { get; set; }
        public DateTime krediTarihi { get; set; }
        public double yillikFaizOrani { get; set; }
        public double aylikFaizOrani { get; set; }
        public double kkdfOrani { get; set; }
        public DateTime taksitTarihi { get; set; }
        public int gunAraligi { get; set; }
        public double taksitTutari { get; set; }
        public double anaparaTutari { get; set; }
        public double faizTutari { get; set; }
        public double kkdfTutari { get; set; }
        public double kalanAnapara { get; set; }
        public double guncelAnapara { get; set; }
        public double guncelKalanAnapara { get; set; }
    }
}
