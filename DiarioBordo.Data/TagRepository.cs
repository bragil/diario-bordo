using Dapper;
using DiarioBordo.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiarioBordo.Data
{
    public class TagRepository
    {
        private IDbConnection connection;

        private const string table = "Tags";
        private const string fields = "Id, Nome";
        private string insert = $"INSERT INTO {table}(Nome) VALUES (@Nome)";
        private string update = $"UPDATE {table} SET Nome = @Nome WHERE Id = @Id";
        private string delete = $"DELETE FROM {table} WHERE Id = @Id";
        private string selectAll = $"SELECT {fields} FROM {table}";
        private string selectById = $"SELECT {fields} FROM {table} WHERE Id = @Id";
        private string selectByNome = $"SELECT {fields} FROM {table} WHERE Nome = @Nome";
        private string selectByTermo = $"SELECT {fields} FROM {table} WHERE Nome LIKE @Termo";

        public TagRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Insere uma nova tag.
        /// </summary>
        /// <param name="tag">Tag</param>
        public async Task<Tag> Inserir(Tag tag)
        {
            await connection.ExecuteAsync(insert, tag);
            return await ObterPorNome(tag.Nome);
        }

        /// <summary>
        /// Exclui uma tag.
        /// </summary>
        /// <param name="id">ID da tag</param>
        public async Task Excluir(long id)
        {
            await connection.ExecuteAsync(delete, new { Id = id });
        }

        /// <summary>
        /// Dado o ID, retorna uma tag.
        /// </summary>
        /// <param name="id">ID da tag</param>
        /// <returns>Tag</returns>
        public async Task<Tag> Obter(long id)
        {
            return await connection.QueryFirstAsync<Tag>(selectById, new { Id = id });
        }

        /// <summary>
        /// Busca tags pelo nome.
        /// </summary>
        /// <param name="termo">Termo informado</param>
        /// <returns>Lista de tags</returns>
        public async Task<List<Tag>> Buscar(string termo)
        {
            var lista = await connection.QueryAsync<Tag>(selectByTermo, new { Termo = "%" + termo + "%" });
            return lista.OrderBy(t => t.Nome).ToList();
        }

        /// <summary>
        /// Dado o nome, retorna uma tag.
        /// </summary>
        /// <param name="nome">Nome da tag</param>
        /// <returns>Tag</returns>
        public async Task<Tag> ObterPorNome(string nome)
        {
            return await connection.QueryFirstOrDefaultAsync<Tag>(selectByNome, new { Nome = nome });
        }

        /// <summary>
        /// Retorna todas as tags
        /// </summary>
        /// <returns>Lista de tags</returns>
        public async Task<List<Tag>> ObterTodos()
        {
            var lista = await connection.QueryAsync<Tag>(selectAll);
            return lista.OrderBy(t => t.Nome).ToList();
        }
    }
}
