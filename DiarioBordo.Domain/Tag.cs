using System;
using System.Collections.Generic;
using System.Text;

namespace DiarioBordo.Domain
{

    public class Tag
    {
        public long Id { get; set; }
        public string Nome { get; set; }

        public Tag()
        { }

        public Tag(string nome)
        {
            Nome = nome;
        }

        public Tag(long id, string nome)
        {
            Id = id;
            Nome = nome;
        }
    }
}
