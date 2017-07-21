using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiarioBordo.Domain
{
    /// <summary>
    /// Registro do diário de bordo.
    /// </summary>
    public class Registro
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Título é obrigatório.")]
        [StringLength(100, ErrorMessage = "Título não deve exceder 100 caracteres.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatório.")]
        [StringLength(1000, ErrorMessage = "Descrição não deve exceder 1000 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Criticidade é obrigatório")]
        public Criticidade Criticidade { get; set; }

        public DateTime CriadoEm { get; set; }

        public long TagId { get; set; }

        public Tag Tag { get; set; }

        public Registro()
        { }

        public Registro(string titulo, string descricao, int criticidade, long tagId)
        {
            var dic = new Dictionary<int, Criticidade>() { { 1, Criticidade.Baixa }, { 2, Criticidade.Media }, { 3, Criticidade.Alta } };
            Titulo = titulo;
            Descricao = descricao;
            Criticidade = dic[criticidade];
            TagId = tagId;
            CriadoEm = DateTime.Now;
        }
    }
}
