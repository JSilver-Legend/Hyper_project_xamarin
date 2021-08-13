using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Security.Cryptography;




namespace Hyper_project
{

   
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            GetList();


        }


        public async void GetList()
        {

            var httpClient = new HttpClient();
            var userList = new List<string>();
            var response = await httpClient.GetAsync("http://infrasistemas.sytes.net:2030/api/user/listar_usuarios");
            var jsonString = await response.Content.ReadAsStringAsync();
            dynamic stuff = JsonConvert.DeserializeObject(jsonString);
            foreach (var item in stuff.Data)
            {
               userList.Add(item.Nombre.ToString());
            }
            user.ItemsSource = userList;
        }
        

        public async void GetNext()
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(pass.Text.ToString()));
            var client = new HttpClient();
            byte[] md5result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < md5result.Length; i++)
            {
                strBuilder.Append(md5result[i].ToString("x2"));
            }

            string jsonData = "{'Nombre' : '" + user.SelectedItem.ToString() + "', 'Contraseña' : '" + strBuilder.ToString() + "'}";
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("http://infrasistemas.sytes.net:2030/api/user/Chequear_Pass", content);
            var result = await response.Content.ReadAsStringAsync();
            if (result.Contains("true"))
            {
                await Navigation.PushAsync(new MenuPage());
            }
            else
            {
                await DisplayAlert("aviso", "La contraseña es incorrecta.", "Confirmar");
            }

        }
       



        private void NavigateButton_OnClicked(object sender, EventArgs e)
        {
            if(user.SelectedItem != null)
            {
                if (pass.Text != null)
                {
                    GetNext(); 
                }
                else
                {
                    DisplayAlert("aviso", "Por favor, introduzca su contraseña.", "Confirmar");
                }
            }
            else
            {
                DisplayAlert("aviso", "Elige tu nombre de usuario.", "Confirmar");
            }
            

        }



        private async void ExitButton_OnClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("aviso", "Seguro que quieres terminarlo?", "sí", "No");
            if(answer == true)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }
        


    }
}
