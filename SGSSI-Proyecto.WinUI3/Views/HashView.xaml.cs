using CommunityToolkit.WinUI.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SGSSI_Proyecto.WinUI3.Models;

namespace SGSSI_Proyecto.WinUI3.Views
{
    public sealed partial class HashView : Page
    {
        internal static Algoritmo AlgoritmoSeleccionado { get; set; }

        internal ObservableCollection<string> nombresAlgoritmos = new ObservableCollection<string>();
        internal ListaAlgoritmos algoritmos = new ListaAlgoritmos();
        private readonly TrulyObservableCollection<Elemento> _listaElementos = new TrulyObservableCollection<Elemento>();
        internal TrulyObservableCollection<Elemento> ListaElementos
        {
            get { return _listaElementos; }
        }

        public HashView()
        {
            this.InitializeComponent();

            foreach (Algoritmo alg in algoritmos.Lista)
            {
                nombresAlgoritmos.Add(alg.Nombre);
            }

            ComboBoxAlgoritmo.SelectedIndex = algoritmos.Lista.IndexOf(algoritmos.GetAlgoritmo("SHA256"));
        }

        private async void ElegirArchivos(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add("*");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            var files = await picker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                RecorrerArchivos(files);
            }
            else
            {
                Console.WriteLine("Error");
            }
        }

        private async void ElegirCarpeta(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop
            };
            folderPicker.FileTypeFilter.Add("*");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.m_window);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                IReadOnlyList<StorageFile> archivos = await folder.GetFilesAsync();
                RecorrerArchivos(archivos);
            }
            else
            {
                Console.WriteLine("Error");
            }
        }
        private void RecorrerArchivos()
        {
            RecorrerArchivos(ListaElementos.Select(e => e.Archivo).ToList());
        }

        private void RecorrerArchivos(IReadOnlyList<StorageFile> archivos)
        {
            foreach (StorageFile a in archivos)
            {
                CalcularHashAsync(a);
            }
        }

        private async void CalcularHashAsync(StorageFile sf)
        {
            Task<Stream> streamTask = sf.OpenStreamForReadAsync();

            streamTask.Wait();

            Stream stream = streamTask.Result;
            try
            {
                string nombre = sf.Name;
                string direccion = sf.Path;
                DateTimeOffset modificacion = (await sf.GetBasicPropertiesAsync()).DateModified;
                Elemento tmp_elem = _listaElementos.FirstOrDefault(e => e.Direccion == direccion && e.UltimaModificacion == modificacion);
                Elemento elem;

                Algoritmo alg = AlgoritmoSeleccionado;

                if (tmp_elem != null)
                {
                    elem = tmp_elem;
                } else
                {
                    elem = new Elemento(nombre, direccion, sf, modificacion);
                    _listaElementos.Add(elem);
                }

                if (!elem.ExisteAlgoritmo(AlgoritmoSeleccionado.Nombre))
                {
                    stream.Position = 0;
                    byte[] hash = alg.Implementacion.ComputeHash(stream);
                    elem.AñadirResultado(new ResultadoHash(alg.Nombre, BitConverter.ToString(hash).Replace("-", "").ToLower()));
                } else
                {
                    elem.ForzarActualizacion();
                }

                FiltrarBusqueda();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error"+e);
            }
        }

        private void ComboBoxAlgoritmo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AlgoritmoSeleccionado = algoritmos.GetAlgoritmo(ComboBoxAlgoritmo.SelectedItem.ToString());

            RecorrerArchivos();
        }

        private void ButtonBorrar_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Elemento> elementosSeleccionados = _listaElementos.Where(elem => elem.Seleccionado).ToList();
            foreach (Elemento elem in elementosSeleccionados)
            {
                if (elem.Seleccionado)
                {
                    _listaElementos.Remove(elem);
                }
            }

            CheckBoxElementos.IsChecked = false;

            FiltrarBusqueda();
        }

        private void CheckBoxElementos_Checked(object sender, RoutedEventArgs e)
        {
            foreach (Elemento elem in _listaElementos)
            {
                elem.Seleccionado = true;
            }
        }

        private void CheckBoxElementos_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (Elemento elem in _listaElementos)
            {
                elem.Seleccionado = false;
            }
        }

        private void CheckBoxElemento_Click(object sender, RoutedEventArgs e)
        {
            bool and = true;
            bool or = false;
            int contador = 0;
            foreach (Elemento elem in _listaElementos)
            {
                and = and && elem.Seleccionado;
                or = or || elem.Seleccionado;
                if (elem.Seleccionado) contador++;
            }

            if (!and && or)
            {
                CheckBoxElementos.IsChecked = null;
            } else if (and)
            {
                CheckBoxElementos.IsChecked = true;
            } else
            {
                CheckBoxElementos.IsChecked = false;
            }

            if (contador > 1)
            {
                IEnumerable<string> resumenes = _listaElementos.Where(elem => elem.Seleccionado).Select(elem => elem.ConseguirHash(AlgoritmoSeleccionado.Nombre));
                
                if (resumenes.All(r => r.Equals(resumenes.First())))
                {
                    TextBlockIguales.Visibility = Visibility.Visible;
                    TextBlockDiferentes.Visibility = Visibility.Collapsed;
                } else
                {
                    TextBlockIguales.Visibility = Visibility.Collapsed;
                    TextBlockDiferentes.Visibility = Visibility.Visible;
                }
                GridTextoComparacion.Visibility = Visibility.Visible;

            } else
            {
                TextBlockIguales.Visibility = Visibility.Collapsed;
                TextBlockDiferentes.Visibility = Visibility.Collapsed;
                GridTextoComparacion.Visibility = Visibility.Collapsed;
            }
        }

        private void DataGridHash_Sorting(object sender, DataGridColumnEventArgs e)
        {
            if (e.Column.Tag.ToString() == "Archivo")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    DataGridHash.ItemsSource = new ObservableCollection<Elemento>(from item in _listaElementos
                                                                                  orderby item.Nombre ascending
                                                                                  select item);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                } else
                {
                    DataGridHash.ItemsSource = new ObservableCollection<Elemento>(from item in _listaElementos
                                                                                  orderby item.Nombre descending
                                                                                  select item);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }                
            } else if (e.Column.Tag.ToString() == "Ult.modificacion")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    DataGridHash.ItemsSource = new TrulyObservableCollection<Elemento>(from item in _listaElementos
                                                                                  orderby item.UltimaModificacion ascending
                                                                                  select item);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    DataGridHash.ItemsSource = new TrulyObservableCollection<Elemento>(from item in _listaElementos
                                                                                  orderby item.UltimaModificacion descending
                                                                                  select item);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            } else if (e.Column.Tag.ToString() == "Hash")
            {
                if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                {
                    DataGridHash.ItemsSource = new TrulyObservableCollection<Elemento>(from item in _listaElementos
                                                                                  orderby item.ConseguirHash(AlgoritmoSeleccionado.Nombre) ascending
                                                                                  select item);
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
                else
                {
                    DataGridHash.ItemsSource = new TrulyObservableCollection<Elemento>(from item in _listaElementos
                                                                                  orderby item.ConseguirHash(AlgoritmoSeleccionado.Nombre) descending
                                                                                  select item);
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
            }
        }

        internal void AutoSuggestBoxBusqueda_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            FiltrarBusqueda();
        }

        private void FiltrarBusqueda()
        {
            if (!AutoSuggestBoxBusqueda.Text.Equals(""))
            {
                DataGridHash.ItemsSource = new TrulyObservableCollection<Elemento>(from item in _listaElementos
                                                                                   where item.ConseguirHash(AlgoritmoSeleccionado.Nombre) != null && item.ConseguirHash(AlgoritmoSeleccionado.Nombre).Contains(AutoSuggestBoxBusqueda.Text)
                                                                                   select item);
            }
            else
            {
                DataGridHash.ItemsSource = _listaElementos;
            }
        }
    }
}
