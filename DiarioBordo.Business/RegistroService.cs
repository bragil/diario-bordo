using DiarioBordo.Data;
using DiarioBordo.Domain;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiarioBordo.Business
{
    public class RegistroService
    {
        private TagRepository tagRepository;
        private RegistroRepository repository;

        public RegistroService(RegistroRepository repository, TagRepository tagRepository)
        {
            this.repository = repository;
            this.tagRepository = tagRepository;
        }

        public async Task Adicionar(Registro registro)
        {
            await repository.Inserir(registro);
        }

        public async Task Atualizar(Registro registro)
        {
            await repository.Atualizar(registro);
        }

        public async Task Excluir(long id)
        {
            await repository.Excluir(id);
        }

        public async Task<Registro> Obter(long id)
        {
            var registro = await repository.Obter(id);
            registro.Tag = await tagRepository.Obter(registro.TagId);
            return registro;
        }

        /// <summary>
        /// Lista todos os registros, com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página</param>
        /// <param name="pageSize">Quantidade de registros por página</param>
        /// <returns>Página de registros</returns>
        public async Task<Page<Registro>> ObterTodos(int pageNumber, int pageSize)
        {
            var page = await repository.ObterTodos(pageNumber, pageSize);
            foreach (Registro r in page.Items)
            {
                r.Tag = await tagRepository.Obter(r.TagId);
            }
            return page;
        }

        /// <summary>
        /// Lista os registros de um determinado dia.
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Lista de registros</returns>
        public async Task<Page<Registro>> ObterRegistrosDoDia(DateTime data, int pageNumber, int pageSize)
        {
            var page = await repository.ObterRegistrosDoDia(data, pageNumber, pageSize);
            foreach(Registro r in page.Items)
            {
                r.Tag = await tagRepository.Obter(r.TagId);
            }
            return page;
        }

        /// <summary>
        /// Lista todos os registros de uma determinada tag.
        /// </summary>
        /// <param name="tagId">ID da tag</param>
        /// <returns>Lista de registros</returns>
        public async Task<Page<Registro>> ObterRegistrosPorTag(long tagId, int pageNumber, int pageSize)
        {
            var page = await repository.ObterRegistrosPorTag(tagId, pageNumber, pageSize);
            foreach (Registro r in page.Items)
            {
                r.Tag = await tagRepository.Obter(r.TagId);
            }
            return page;
        }
    }
}
