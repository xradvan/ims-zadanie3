using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace zadanie3
{
    public partial class MainPage : ContentPage
    {
        // Vysledok Testu - ziskane z Azure web service
        public static string ExamResult { get; set; }
        // Nastavenia pre Azure web service
        const string apiKey = "Wx9Vm/6th8dEt2q4Jy606kRTHW+u5ANryJPkctFojwWcw0gpRkyk70nOIlMEot5OX38FMYpxBujWP5jYyhWAJg==";
        const string baseUri = "https://ussouthcentral.services.azureml.net/workspaces/b5e84eb6ad36427ba48ac79e73432169/services/79103c08007f42fdaea4d386633dace6/execute?api-version=2.0";

        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handler pre send button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Clicked(object sender, EventArgs e)
        {
            // Validacia vstupov
            if (string.IsNullOrEmpty(nameEntry.Text))
            {
                DisplayAlert("Chýbajúce údaje", "Zadajte prosím Vaše meno", "OK");
                return;
            }

            if (picker.SelectedIndex < 0)
            {
                DisplayAlert("Chýbajúce údaje", "Zadajte prosím náročnosť predmetu", "OK");
                return;
            }

            if (string.IsNullOrEmpty(prepareEntry.Text))
            {
                DisplayAlert("Chýbajúce údaje", "Zadajte prosím dĺžku prípravy", "OK");
                return;
            }

            // Nacitanie vstupov od pouzivatela
            var name = nameEntry.Text;
            var diff = picker.SelectedItem.ToString();
            var prep = prepareEntry.Text;

            // Webova poziadavka
            try
            {
                InvokeRequestResponseService(name, diff, prep).Wait();
            }
            catch (Exception)
            {
                DisplayAlert("Chyba", "Nie ste pripojený na internet.", "OK");
                return;
            }

            // Zobrazenie vysledku
            DisplayAlert("Milý " + name, "Očakávaná známka je " + ExamResult, "Ok");
        }

        /// <summary>
        /// Funkcia vykona web request na Azure cloud
        /// </summary>
        /// <param name="name">Meno pouzivatela</param>
        /// <param name="diff">Narocnost predmetu</param>
        /// <param name="prep">Pocet hodin pripravy</param>
        /// <returns></returns>
        static async Task InvokeRequestResponseService(string name, string diff, string prep)
        {
            // Vytvorenie poziadavky
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"Obtiaznost predmetu", "Pocet hodin ucenia"},
                                Values = new string[,] {  { diff, prep }  }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>() {}
                };
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri(baseUri);

                // Poslanie poziadavky
                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JToken token = JObject.Parse(result);
                    
                    // Zapis vysledku
                    ExamResult = token["Results"]["output1"]["value"]["Values"][0][0].ToString();
                }
            }
        }
    }
}
