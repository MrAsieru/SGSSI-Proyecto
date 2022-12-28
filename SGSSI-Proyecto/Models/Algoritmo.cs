using System.Security.Cryptography;

namespace SGSSI_Proyecto.Models
{
    internal class Algoritmo
    {
        public string Nombre { get; set; }

        public HashAlgorithm Implementacion { get; set; }

        public Algoritmo(string nombre, HashAlgorithm implementacion)
        {
            Nombre = nombre;
            Implementacion = implementacion;
        }
    }
}
