using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DiarioBordo.Domain;
using DiarioBordo.Business;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace DiarioBordo.Web.Controllers
{
    public class HomeController : Controller
    {
        private TagService tagService;
        private RegistroService registroService;

        private int pageSize = 10;

        public HomeController(RegistroService registroService, TagService tagService)
        {
            this.tagService = tagService;
            this.registroService = registroService;
        }

        /// <summary>
        /// Tratamento de erros.
        /// </summary>
        [Route("/error")]
        public ActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            return View("Error", error);
        }

        /// <summary>
        /// Busca as tags cujo nome coincidam com o termo digitado.
        /// </summary>
        /// <param name="term">Termo digitado pelo usuário</param>
        /// <returns>Lista de tags</returns>
        [Route("/tags"), HttpPost]
        public async Task<ActionResult> ObterTags(string term)
        {
            try
            {
                var lista = await tagService.Buscar(term);
                return Json(lista);
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message, Error = true });
            }
        }

        /// <summary>
        /// Lista os registros da data atual.
        /// </summary>
        /// <param name="pageNumber">Número da página (default: 1)</param>
        /// <returns>ViewResult</returns>
        public async Task<ActionResult> Index(int pageNumber = 1)
        {
            try
            {
                var page = await registroService.ObterTodos(pageNumber, pageSize);

                if (page != null)
                    ViewBag.Page = page;

                ViewBag.Titulo = "Registros";
                ViewBag.UrlBase = "/";
                ViewBag.PageNumber = pageNumber;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao obter registros: " + ex.Message;
                return View();
            }
        }

        /// <summary>
        /// Lista os registros de uma determinada data.
        /// </summary>
        /// <param name="data">Data no formato AAAAMMDD</param>
        /// <returns>ViewResult</returns>
        [Route("/registros/data/{data}")]
        public async Task<ActionResult> PorData(string data, int pageNumber = 1)
        {
            try
            {
                var date = DateTime.ParseExact(data, "yyyyMMdd", null);
                var page = await registroService.ObterRegistrosDoDia(date, pageNumber, pageSize);
                if (page != null)
                    ViewBag.Page = page;
                ViewBag.Titulo = "Registros - " + date.ToString("dd/MM/yyyy");
                ViewBag.UrlBase = "/data/" + data + "/";
                ViewBag.PageNumber = pageNumber;
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao obter registros do dia: " + ex.Message;
                return View("Index");
            }
        }

        /// <summary>
        /// Lista os registros de uma determinada tag.
        /// </summary>
        /// <param name="tagId">ID da teg</param>
        /// <returns>ViewResult</returns>
        [Route("/registros/tag/{tagId}")]
        public async Task<ActionResult> PorTag(long tagId, int pageNumber = 1)
        {
            try
            {
                Tag tag = await tagService.ObterPorId(tagId);
                if (tag != null)
                {
                    ViewBag.Tag = tag.Nome;
                    var page = await registroService.ObterRegistrosPorTag(tagId, pageNumber, pageSize);
                    if (page != null)
                        ViewBag.Page = page;
                    ViewBag.Titulo = "Registros - " + tag.Nome;
                    ViewBag.UrlBase = "/tag/" + tagId + "/";
                    ViewBag.PageNumber = pageNumber;
                    return View("Index");
                }
                TempData["Erro"] = "Tag não encontrada!";
                return View("Index");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao obter registros do dia: " + ex.Message;
                return View("Index");
            }
        }

        /// <summary>
        /// Exclui um registro.
        /// </summary>
        /// <param name="id">ID do registro</param>
        /// <returns></returns>
        [Route("/registros/excluir/{id}")]
        public async Task<ActionResult> Excluir(long id)
        {
            try
            {
                await registroService.Excluir(id);
                TempData["Sucesso"] = "Registro excluído com sucesso!";
                return Redirect("/");
            }
            catch (Exception)
            {
                TempData["Erro"] = "Ocorreu um erro ao excluir o registro.";
                return Redirect("/");
            }
        }

        [Route("/registros/editar/{id}")]
        public async Task<ActionResult> Editar(long id)
        {
            try
            {
                var registro = await registroService.Obter(id);
                ViewBag.Novo = false;
                return View("Form", registro);
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Ocorreu um erro ao obter os dados do registro: " + ex.Message;
                return Redirect("/");
            }
        }

        /// <summary>
        /// Exibe o formulário.
        /// </summary>
        /// <returns>ViewResult</returns>
        [Route("/registros/novo")]
        public ActionResult Form()
        {
            ViewBag.Novo = true;
            return View(new Registro());
        }

        /// <summary>
        /// Adiciona um novo registro no banco de dados.
        /// </summary>
        /// <param name="tagNome">Nome da tag</param>
        /// <param name="idTag">ID da tag</param>
        /// <param name="titulo">Título</param>
        /// <param name="descricao">Descrição</param>
        /// <param name="criticidade">Nível de criticidade</param>
        /// <param name="idRegistro">ID do registro, se já existir</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Salvar(string tagNome, string idTag, string titulo, string descricao, int criticidade, string idRegistro = "")
        {
            try
            {
                ValidarRegistro(titulo, descricao, criticidade);
                var registro = await PreparaObjeto(tagNome, idTag, titulo, descricao, criticidade);

                if (!ModelState.IsValid)
                    return View("Index", registro);

                if (!string.IsNullOrWhiteSpace(idRegistro))
                {
                    registro.Id = Convert.ToInt64(idRegistro);
                    await registroService.Atualizar(registro);
                    TempData["Sucesso"] = "Registro atualizado com sucesso!";
                }
                else
                {
                    await registroService.Adicionar(registro);
                    TempData["Sucesso"] = "Registro adicionado com sucesso!";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao salvar registro: " + ex.Message;
                return View("Index", new Registro());
            }
        }

        /// <summary>
        /// Prepara os objetos para serem gravados.
        /// </summary>
        /// <param name="tagNome">Nome da tag</param>
        /// <param name="idTag">ID da tag</param>
        /// <param name="titulo">Título</param>
        /// <param name="descricao">Descrição</param>
        /// <param name="criticidade">Criticidade</param>
        /// <returns>Registro</returns>
        private async Task<Registro> PreparaObjeto(string tagNome, string idTag, string titulo, string descricao, int criticidade)
        {
            Tag tag = null;
            if (string.IsNullOrWhiteSpace(idTag))
                tag = await tagService.Adicionar(tagNome.Trim());
            else
                tag = new Tag(long.Parse(idTag), tagNome);

            var registro = new Registro(titulo, descricao, criticidade, tag.Id);
            registro.Tag = tag;

            return registro;
        }

        /// <summary>
        /// Realiza a validação dos valores informados.
        /// </summary>
        /// <param name="titulo">Título</param>
        /// <param name="descricao">Descrição</param>
        /// <param name="criticidade">Criticidade</param>
        private void ValidarRegistro(string titulo, string descricao, int criticidade)
        {
            if (titulo == null)
                ModelState.AddModelError("Titulo", "Título é obrigatório.");

            if (titulo != null && titulo.Length > 100)
                ModelState.AddModelError("Titulo", "Título não deve exceder 100 caracteres.");

            if (descricao == null)
                ModelState.AddModelError("Descricao", "Descrição é obrigatório.");

            if (descricao != null && descricao.Length > 1000)
                ModelState.AddModelError("Descricao", "Descrição não deve exceder 1000 caracteres.");

            if (criticidade < 1 || criticidade > 3)
                ModelState.AddModelError("Criticidade", "Criticidade é obrigatório.");
        }

    }
}
