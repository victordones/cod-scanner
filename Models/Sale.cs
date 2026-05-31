using System;

namespace cod_scanner.Models
{
    public class Sale
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ProductName { get; set; } = string.Empty;
        // Peso em Kg
        public decimal WeightKg { get; set; }
        // Valores em centavos
        public long TotalCents { get; set; }
        public long PricePerKgCents { get; set; }
        // Horário em UTC em que o código foi lido
        public DateTime ScannedAt { get; set; }
        // Código interno do produto
        public string ProductCode { get; set; } = string.Empty;
        // Barcode salvo exatamente como lido
        public string Barcode { get; set; } = string.Empty;
    }
}