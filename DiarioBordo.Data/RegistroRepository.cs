using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using DiarioBordo.Domain;
using System.Threading.Tasks;

namespace DiarioBordo.Data
{
    public class RegistroRepository
    {
        private IDbConnection connection;

        private const string table = "Registros";
        private const string count = "COUNT(*)";
        private const string fields = "Id, Titulo, Descricao, Criticidade, CriadoEm, TagId";
        private const string pagination = "OFFSET @PageSize * (@PageNumber - 1) ROWS FETCH NEXT @PageSize ROWS ONLY";
        private string insert = $"INSERT INTO {table}(Titulo, Descricao, Criticidade, CriadoEm, TagId) VALUES (@Titulo, @Descricao, @Criticidade, @CriadoEm, @TagId)";
        private string update = $"UPDATE {table} SET Titulo = @Titulo, Descricao = @Descricao, Criticidade = @Criticidade WHERE Id = @Id";
        private string delete = $"DELETE FROM {table} WHERE Id = @Id";

        private string selectAllCount = $"SELECT {count} FROM {table}";
        private string selectAll = $"SELECT {fields} FROM {table} ORDER BY CriadoEm DESC {pagination}";

        private string selectById = $"SELECT {fields} FROM {table} WHERE Id = @Id";

        private string selectByDateCount = $"SELECT {count} FROM {table} WHERE CONVERT(VARCHAR(10), CriadoEm, 120) = @Data";
        private string selectByDate = $"SELECT {fields} FROM {table} WHERE CONVERT(VARCHAR(10), CriadoEm, 120) = @Data ORDER BY CriadoEm DESC {pagination}";

        private string selectByTagCount = $"SELECT {count} FROM {table} WHERE TagId = @TagId";
        private string selectByTag = $"SELECT {fields} FROM {table} WHERE TagId = @TagId ORDER BY CriadoEm DESC {pagination}";

        public RegistroRepository(IDbConnection connection)
        {
            this.connection = connection;
        }

        /// <summary>
        /// Insere um novo registro.
        /// </summary>
        /// <param name="registro">Registro</param>
        public async Task Inserir(Registro registro)
        {
            await connection.ExecuteAsync(insert, registro);
        }

        /// <summary>
        /// Atualiza um registro existente.
        /// </summary>
        /// <param name="registro">Registro</param>
        public async Task Atualizar(Registro registro)
        {
            await connection.ExecuteAsync(update, registro);
        }

        /// <summary>
        /// Exclui um registro.
        /// </summary>
        /// <param name="id">ID do registro</param>
        public async Task Excluir(long id)
        {
            await connection.ExecuteAsync(delete, new { Id = id });
        }

        /// <summary>
        /// Dado o ID, retorna um registro.
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <returns>Registro</returns>
        public async Task<Registro> Obter(long id)
        {
            return await connection.QueryFirstAsync<Registro>(selectById, new { Id = id });
        }

        /// <summary>
        /// Lista todos os registro, com paginação.
        /// </summary>
        /// <param name="pageNumber">Número da página</param>
        /// <param name="pageSize">Quantidade de registros por página</param>
        /// <returns>Lista de registros</returns>
        public async Task<Page<Registro>> ObterTodos(int pageNumber, int pageSize)
        {
            int count = await connection.ExecuteScalarAsync<int>(selectAllCount);
            var lista = await connection.QueryAsync<Registro>(selectAll, new { PageNumber = pageNumber, PageSize = pageSize });
            var page = new Page<Registro>(count, pageNumber, pageSize, lista.ToList());
            return page;
        }

        /// <summary>
        /// Lista todos os registros de um determinado dia.
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Lista de registros</returns>
        public async Task<Page<Registro>> ObterRegistrosDoDia(DateTime data, int pageNumber, int pageSize)
        {
            int count = await connection.ExecuteScalarAsync<int>(selectByDateCount, new { Data = data.ToString("yyyy-MM-dd") });
            var lista = await connection.QueryAsync<Registro>(selectByDate, new { Data = data.ToString("yyyy-MM-dd"), PageNumber = pageNumber, PageSize = pageSize });
            var page = new Page<Registro>(count, pageNumber, pageSize, lista.ToList());
            return page;
        }

        /// <summary>
        /// Lista todos os registros de uma determinada tag.
        /// </summary>
        /// <param name="tagId">ID da tag</param>
        /// <returns>Lista de registros</returns>
        public async Task<Page<Registro>> ObterRegistrosPorTag(long tagId, int pageNumber, int pageSize)
        {
            int count = await connection.ExecuteScalarAsync<int>(selectByTagCount, new { TagId = tagId });
            var lista = await connection.QueryAsync<Registro>(selectByTag, new { TagId = tagId, PageNumber = pageNumber, PageSize = pageSize });
            var page = new Page<Registro>(count, pageNumber, pageSize, lista.ToList());
            return page;
        }
    }
}
