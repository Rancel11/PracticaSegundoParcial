using PracticaSegundoParcial.Data;
using PracticaSegundoParcial.Models;

namespace PracticaSegundoParcial;

public partial class MainPage : ContentPage
{
    private readonly ClienteDatabase _clienteDatabase;

    public MainPage(ClienteDatabase clienteDatabase)
    {
        InitializeComponent();
        _clienteDatabase = clienteDatabase;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        clientesList.ItemsSource = await _clienteDatabase.GetClientesAsync();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        var cliente = new Cliente
        {
            Cedula = cedulaEntry.Text,
            Nombre = nombreEntry.Text,
            Direccion = direccionEntry.Text,
            Telefono = telefonoEntry.Text
        };

        await _clienteDatabase.SaveClienteAsync(cliente);

        cedulaEntry.Text = nombreEntry.Text = direccionEntry.Text = telefonoEntry.Text = string.Empty;

        clientesList.ItemsSource = await _clienteDatabase.GetClientesAsync();
    }
}
