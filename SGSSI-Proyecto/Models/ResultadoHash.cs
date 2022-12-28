namespace SGSSI_Proyecto.Models
{
    internal class ResultadoHash
    {
        public string Algoritmo { get; }

        public string Hash { get; }

        public ResultadoHash(string algoritmo, string hash)
        {
            this.Algoritmo = algoritmo;
            this.Hash = hash;
        }
    }
}
