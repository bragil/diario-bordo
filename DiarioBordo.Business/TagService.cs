using DiarioBordo.Data;
using DiarioBordo.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiarioBordo.Business
{
    public class TagService
    {
        private TagRepository repository;

        public TagService(TagRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Verifica se a tag existe, se não existir, insere. 
        /// Retorna o objeto Tag.
        /// </summary>
        /// <param name="tagNome">Nome da tag</param>
        /// <returns>Tag</returns>
        public async Task<Tag> Adicionar(string tagNome)
        {
            var tag = await repository.ObterPorNome(tagNome);

            if (tag == null)
                tag = await repository.Inserir(new Tag(tagNome));
            
            return tag;
        }

        /// <summary>
        /// Busca tags pelo nome.
        /// </summary>
        /// <param name="termo">Termo informado</param>
        /// <returns>Lista de tags</returns>
        public async Task<List<Tag>> Buscar(string termo)
        {
            return await repository.Buscar(termo);
        }

        /// <summary>
        /// Dado o ID, retorna a tag.
        /// </summary>
        /// <param name="id">ID da tag</param>
        /// <returns>Tag</returns>
        public async Task<Tag> ObterPorId(long id)
        {
            return await repository.Obter(id);
        }


        /// <summary>
        /// Dado o nome, retorna a tag.
        /// </summary>
        /// <param name="nome">Nome da tag</param>
        /// <returns>Tag</returns>
        public async Task<Tag> ObterPorNome(string nome)
        {
            return await repository.ObterPorNome(nome);
        }

        /// <summary>
        /// Retorna todas as tags.
        /// </summary>
        /// <returns>Lista de tags</returns>
        public async Task<List<Tag>> ObterTodos()
        {
            return await repository.ObterTodos();
        }
    }
}
