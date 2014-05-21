using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;
using MvcAppMenu.Models;
using System.Text;
using System.IO;
using System.Web.UI;

namespace MvcAppMenu.Controllers
{

    public class HomeController : Controller
    {
        //Declaração Constantes que recebem classes do .css
        private const string urlJs = "javascript:;";
        private const string urlClass = "ajaxify";
        private const string urlClassStart = "ajaxify start";
        private const string liClass = "";
        private const string liClassStart = "start";
        private const string iconClass = "fa fa-home";
        private const string spanTitle = "title";
        private const string spanArrow = "arrow";
        private const string spanSelected = "selected";
        private const string submenuClass = "sub-menu";
        private const string linkClass = "ajaxify";

        //Lista que armazena o menu pesquisado no banco
        List<Menu> menuLista = null;

        [HttpGet]
        public ActionResult Index()
        {
            var meuMenu = string.Empty; //menu que será criado
            var renderizado = string.Empty;

            //Pesquisa os dados para montagem do menu
            menuLista = PesquisarDadosMenu();

            //Caso encontre informações...
            if (menuLista != null && menuLista.Count > 0)
            {
                foreach (var itemMenu in menuLista.Where(x => x.MenuPai == 0))
                {
                    meuMenu = CriarMenu(itemMenu);
                    renderizado += meuMenu;
                }
            }

            return View((object)renderizado);
        }

        //Início Região Menu Principal
        #region CriarMenu

        /// <summary>
        /// Inicia a criação do menu percorrendo os itens roots
        /// </summary>
        /// <returns>string com menu construído</returns>
        private string CriarMenu(Menu menu)
        {

            var stringWriter = new StringWriter();
            var htmlWriter = new HtmlTextWriter(stringWriter);

            //Inicia o menu principal
            if (menu.iOrdemMenu == 0)
            {
                //<li>
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, liClassStart);
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Li);
            }

            else
            {
                //<li>
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, liClass);
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Li);
            }

            if (PesquisarFilhos(menu))
            {
                //<a>
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, urlJs);
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);

            }

            else
            {
                if (menu.iOrdemMenu == 0)
                {
                    //<a>
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, urlClassStart);
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, menu.Contoller);
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                }

                else
                {
                    //<a>
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, urlClass);
                    htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, menu.Contoller);
                    htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);
                }

            }

            //<i>
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, iconClass);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.I);
            htmlWriter.RenderEndTag();
            //</i>

            //<span>
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, spanTitle);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
            htmlWriter.Write(menu.Nome);
            htmlWriter.RenderEndTag();
            //</span>

            //<span>
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, spanSelected);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
            htmlWriter.RenderEndTag();
            //</span>

            //<span>
            if (PesquisarFilhos(menu))
            {
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, spanArrow);
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
                htmlWriter.RenderEndTag();
            }
            //</span>

            htmlWriter.RenderEndTag();
            //</a>

            if (PesquisarFilhos(menu))
            {
                //<ul>
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, submenuClass);
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Ul);

                //Percorre  a lista e verifica se o [apenas o nível root, ou seja, o nível que não tem pai
                foreach (var menuItem in menuLista.Where(m => m.MenuPai != 0 && m.MenuPai == menu.Id))
                {
                    CriarSubMenu(htmlWriter, menuItem);
                }

                //Fim do submenu
                htmlWriter.RenderEndTag();
                //</ul>

                //Fim do menu principal
                htmlWriter.RenderEndTag();
                //</li>
            }
            return stringWriter.ToString();
        }

        #endregion //Fim Região Menu Principal

        //Inicio Região SubMenu
        #region CriarSubMenu

        /// <summary>
        /// Criar a estrutura do menu com seus respectivos itens e sub itens
        /// </summary>
        /// <param name="htmlWriter">Escritor das tags html</param>
        /// <param name="itemCorrente">Item corrente do menu</param>
        private void CriarSubMenu(HtmlTextWriter htmlWriter, Menu itemCorrente)
        {
            try
            {
                MontarItemMenu(htmlWriter, itemCorrente);

            }
            catch (Exception)
            {

            }

        }

        #endregion //Fim Região SubMenu


        //Ínicio Região de Montar itens do Submenu
        #region MontarItemMenu
        /// <summary>
        /// Cria formatações, link e demais detalhes dos itens do menu
        /// </summary>
        /// <param name="htmlWriter">Escritor das tags html</param>
        /// <param name="itemCorrente"></param>
        private void MontarItemMenu(HtmlTextWriter htmlWriter, Menu itemCorrente)
        {
            //bool temFilho = listaFilhos.Count() > 0;

            //Verifica se o submenu tem um submenu interno (submenu de submenu)
            if (PesquisarFilhos(itemCorrente))
            {
                CriarMultiLevelMenu(htmlWriter, itemCorrente);
            }
            else
            {
                //<li>
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.Li);

                //<a>
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, linkClass);
                htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, itemCorrente.Contoller);
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);

                //<i>
                htmlWriter.RenderBeginTag(HtmlTextWriterTag.I);
                htmlWriter.RenderEndTag();
                //</i>

                htmlWriter.Write(itemCorrente.Nome);

                htmlWriter.RenderEndTag();
                //</a>

                htmlWriter.RenderEndTag();
                //</li>
            }
        }

        private void CriarMultiLevelMenu(HtmlTextWriter htmlWriter, Menu itemCorrente)
        {
            //<li>
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, liClass);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Li);

            //<a>
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Href, urlJs);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.A);

            //<span>
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, spanTitle);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
            htmlWriter.Write(itemCorrente.Nome);
            htmlWriter.RenderEndTag();
            //</span>

            //<span>
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, spanSelected);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
            htmlWriter.RenderEndTag();
            //</span>

            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, spanArrow);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Span);
            htmlWriter.RenderEndTag();

            htmlWriter.RenderEndTag();
            //</a>

            //<ul>
            htmlWriter.AddAttribute(HtmlTextWriterAttribute.Class, submenuClass);
            htmlWriter.RenderBeginTag(HtmlTextWriterTag.Ul);


            //Percorre  a lista e verifica se o apenas o nível root, ou seja, o nível que não tem pai
            foreach (var menuItem in menuLista.Where(m => m.MenuPai != 0 && m.MenuPai == itemCorrente.Id))
            {
                CriarSubMenu(htmlWriter, menuItem);
            }

            //Finaliza o submenu
            htmlWriter.RenderEndTag();
            //</ul>

            //finaliza o <li>
            htmlWriter.RenderEndTag();
            //</li>

        }

        #endregion //Fim Região de Montar itens do Submenu

        //Início Região Para Encontrar Filhos no item do menu
        #region PesquisarFilhos

        private bool PesquisarFilhos(Menu itemCorrente)
        {
            //Pequisa os filhos do item do menu
            var listaFilhos = menuLista.Where(m => m.MenuPai == itemCorrente.Id);

            // verificação que indica se o item do menu tem filhos
            bool temFilho = listaFilhos.Count() > 0;
            return temFilho;
        }
        #endregion //Fim Região Para Encontrar Filhos no item do menu

        //Ínicio Área de Teste de Dados 
        #region PesquisarDadosMenu
        /// <summary>
        /// Simula uma pesquina do menu no bando de dados.
        /// </summary>
        /// <returns></returns>
        private List<Menu> PesquisarDadosMenu()
        {
            List<Menu> menuLista = new List<Menu>();

            menuLista.Add(new Menu(50, "Dashboard", "Dashboard", string.Empty, 0, 0));
            menuLista.Add(new Menu(1, "Empresa", string.Empty, string.Empty, 0, 1));
            menuLista.Add(new Menu(2, "Cadastro", "Empresa", "Cadastro", 1, 2));
            menuLista.Add(new Menu(3, "Consulta", "Empresa", "Consulta", 1, 3));
            menuLista.Add(new Menu(4, "Excluir", "Empresa", "Excluir", 1, 4));
            menuLista.Add(new Menu(5, "Editar", "Empresa", "Editar", 1, 5));
            menuLista.Add(new Menu(6, "Treinamento", string.Empty, string.Empty, 0, 7));
            menuLista.Add(new Menu(7, "Cadastro", "Treinamento", string.Empty, 6, 8));
            menuLista.Add(new Menu(8, "Consulta", "Treinamento", string.Empty, 6, 9));
            menuLista.Add(new Menu(9, "Excluir", "Treinamento", string.Empty, 6, 10));
            menuLista.Add(new Menu(10, "Editar", "Treinamento", string.Empty, 6, 11));
            menuLista.Add(new Menu(16, "Relatório", "Relatorio", string.Empty, 0, 17));
            menuLista.Add(new Menu(17, "Histórico", "Historico", string.Empty, 0, 18));
            menuLista.Add(new Menu(18, "Gerenciar", string.Empty, string.Empty, 0, 19));
            menuLista.Add(new Menu(19, "Usuário", string.Empty, string.Empty, 18, 20));
            menuLista.Add(new Menu(20, "Cadastrar", "Usuario", string.Empty, 19, 21));
            menuLista.Add(new Menu(21, "Consulta", "Usuario", string.Empty, 19, 22));
            menuLista.Add(new Menu(22, "Excluir", "Usuario", string.Empty, 19, 23));
            menuLista.Add(new Menu(23, "Administrador", string.Empty, string.Empty, 18, 24));
            menuLista.Add(new Menu(24, "Cadastrar", "Administrador", string.Empty, 23, 25));
            menuLista.Add(new Menu(25, "Consulta", "Administrador", string.Empty, 23, 26));
            menuLista.Add(new Menu(26, "Excluir", "Administrador", string.Empty, 23, 27));
            menuLista.Add(new Menu(27, "Teste", "Administrador", string.Empty, 26, 28));
            return menuLista;
        }

        #endregion //Fim Área de Teste de Dados
    }
}
