using System;
using System.Collections.Generic;

namespace SGSSI_Proyecto.Models
{
    internal class ListaElementos
    {
        public List<Elemento> Lista { get; }

        public ListaElementos() 
        {
            Lista = new List<Elemento>();
        }

        public Elemento AñadirElemento(Elemento elemento)
        {
            Lista.Add(elemento);
            return elemento;
        }

        public bool ExisteNombreElemento(String nombre)
        {
            return Lista.Exists(e => e.Nombre == nombre);
        }

        public Elemento ConseguirElemento(String nombre)
        {
            return Lista.Find(e => e.Nombre == nombre);
        }
    }
}
