namespace SGSSI_Proyecto.WinUI3.Models
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
