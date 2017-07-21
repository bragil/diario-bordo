using System;
using System.Collections.Generic;
using System.Text;

namespace DiarioBordo.Domain
{
    public class Page<T>
    {
        /// <summary>
        /// Quantidade total de registros.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Número da página atual
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Quantidade de itens por página
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Quantidade de páginas
        /// </summary>
        public int PageCount
        {
            get
            {
                var c = Math.Ceiling(Convert.ToDecimal(Convert.ToDecimal(Count) / Convert.ToDecimal(PageSize)));
                return (int) c;
            }
        }

        /// <summary>
        /// Lista de itens da página
        /// </summary>
        public List<T> Items { get; set; }


        public Page(int count, int pageNumber, int pageSize, List<T> itens)
        {
            Count = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            Items = itens;
        }
    }
}
