using System;
using System.ComponentModel.DataAnnotations;

namespace SMarket.Core.Model
{
    public class Itens
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal PrecoCusto { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]
        public decimal PrecoVenda { get; set; }
        public string Ncm { get; set; }
        public string Referencia { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataCadastro { get; set; }
    }
}
