using System.Collections.Generic;
using System.Security.Cryptography;

namespace SGSSI_Proyecto.WinUI3.Models
{
    internal class ListaAlgoritmos
    {
        public List<Algoritmo> Lista { get; }

        public ListaAlgoritmos()
        {
            Lista = new List<Algoritmo>
            {
                new Algoritmo("MD5", MD5.Create()),
                new Algoritmo("SHA1", SHA1.Create()),
                new Algoritmo("SHA256", SHA256.Create()),
                new Algoritmo("SHA384", SHA384.Create()),
                new Algoritmo("SHA512", SHA512.Create())
            };

        }

        public Algoritmo GetAlgoritmo(string Nombre)
        {
            return Lista.Find(a => a.Nombre == Nombre);
        }
    }
}
