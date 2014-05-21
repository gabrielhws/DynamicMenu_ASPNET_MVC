using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcAppMenu.Models
{
    //Criar Tabela no DB tbMenu com o Entity Framework
    [Table("Menu")]
    public class Menu
    {
        private int menuPai = 0;

        public int MenuPai
        {
            get { return menuPai; }
            set { menuPai = value; }
        }

        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Contoller { get; set; }

        public string Action { get; set; }

        public int? iOrdemMenu { get; set; }
        

        public Menu(int _id, string _itemNome, string _itemController, string _itemAction, int _menuPai, int _ordem)
        {
            Id = _id;
            Nome = _itemNome;
            Contoller = _itemController;
            Action = _itemAction;
            MenuPai = _menuPai;
            iOrdemMenu = _ordem;
        }

        public Menu()
        {
            
        }
    }
}