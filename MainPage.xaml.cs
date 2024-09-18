using System.Text.Json;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        private List<Character> allCharacters = new List<Character>();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCargarPersonajesClicked(object sender, EventArgs e)
        {
            string apiUrl = "https://rickandmortyapi.com/api/character";

            HttpClient client = new HttpClient();

            try
            {
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    var data = JsonSerializer.Deserialize<RickAndMortyResponse>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // llevar la data al listado con todos los personajes inicialmente
                    allCharacters = data.Results;
                    PersonajesCollectionView.ItemsSource = allCharacters;

                    // Extraer las especies y agregarlas al Picker
                    var especies = allCharacters.Select(c => c.Species).Distinct().ToList();
                    SpeciePicker.ItemsSource = especies;
                    
                    // Extraer los estados y agregarlos al Picker
                    var estados = allCharacters.Select(c => c.Status).Distinct().ToList();
                    StatusPicker.ItemsSource = estados;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
            }
        }

        public void ApplyCombinedFilter()
        {
            string selectedSpecie = SpeciePicker.SelectedItem as string;
            string selectedStatus = StatusPicker.SelectedItem as string;

            var filteredCharacters = allCharacters.AsQueryable();

            if (!string.IsNullOrEmpty(selectedSpecie))
            {
                filteredCharacters = filteredCharacters.Where(c => c.Species == selectedSpecie);
            }

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                filteredCharacters = filteredCharacters.Where(c => c.Status == selectedStatus);
            }

            PersonajesCollectionView.ItemsSource = filteredCharacters.ToList();
        }

        private void OnSpecieSelected(object sender, EventArgs e)
        {
            ApplyCombinedFilter();
        }

        private void OnStatusSelected(object sender, EventArgs e)
        {
            ApplyCombinedFilter();
        }
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue;

            if (!string.IsNullOrEmpty(searchText))
            {
                var filteredCharacters = allCharacters.Where(c => c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                PersonajesCollectionView.ItemsSource = filteredCharacters;
            }
            else
            {
                PersonajesCollectionView.ItemsSource = allCharacters;
            }
        }
    }

    public class RickAndMortyResponse
    {
        public List<Character> Results { get; set; }
    }

    public class Character
    {
        public string Name { get; set; }
        public string Species { get; set; }
        public string Image { get; set; } 
        public string Status { get; set; }
    }
}
