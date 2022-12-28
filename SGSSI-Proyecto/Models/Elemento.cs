using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Storage;

namespace SGSSI_Proyecto.Models
{
    internal class Elemento : INotifyPropertyChanged
    {
        internal string Nombre { get; }
        internal string Direccion { get; }
        internal DateTimeOffset UltimaModificacion { get; }
        private List<ResultadoHash> ListaResultados { get; }
        internal StorageFile Archivo { get; }
        private bool _seleccionado = false;
        public bool Seleccionado
        {
            get
            {
                return _seleccionado;
            }
            
            set
            {
                _seleccionado = value;
                OnPropertyChanged("Seleccionado");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public Elemento(string nombre, string direccion, StorageFile archivo, DateTimeOffset ultimaModificacion)
        {
            Nombre = nombre;
            Direccion = direccion;
            ListaResultados = new List<ResultadoHash>();
            Archivo = archivo;
            UltimaModificacion = ultimaModificacion;
        }

        public void AñadirResultado(ResultadoHash resultado)
        {
            ListaResultados.Add(resultado);
            OnPropertyChanged("ListaResultados");
        }

        public string ConseguirHash(string algoritmo)
        {
            return ListaResultados.Find(r => r.Algoritmo == algoritmo)?.Hash;
        }

        public List<string> ListaAlgoritmos()
        {
            return ListaResultados.Select(r => r.Algoritmo).ToList();
        }

        public bool ExisteAlgoritmo(string algoritmo)
        {
            return ListaAlgoritmos().Exists(a => a == algoritmo);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        internal void ForzarActualizacion()
        {
            OnPropertyChanged("Forzado");
        }
    }
}
