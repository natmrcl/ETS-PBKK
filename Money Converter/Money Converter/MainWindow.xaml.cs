using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Money_Converter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ClearControls();
            BindCurrency();
        }

        #region Bind Currency From and To Combobox
        private void BindCurrency()

        {
            
            string appId = "dfeae187d90d4c5a97a5f9644f614c26"; // ganti dengan App ID Anda
            string url = string.Format("https://openexchangerates.org/api/latest.json?app_id={0}", appId);

            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(url);
                StreamReader reader = new StreamReader(stream);
                string jsonData = reader.ReadToEnd();
                JObject json = JObject.Parse(jsonData);

                // ambil nilai tukar dari JSON dan masukkan ke dalam DataTable
                DataTable dtCurrency = new DataTable();
                dtCurrency.Columns.Add("CurrencyCode", typeof(string));
                dtCurrency.Columns.Add("ExchangeRate", typeof(decimal));
                dtCurrency.Rows.Add("SELECT ONE", 0);
                foreach (JProperty property in json["rates"])
                {
                    string currencyCode = property.Name;
                    decimal exchangeRate = (decimal)property.Value;
                    dtCurrency.Rows.Add(currencyCode, exchangeRate);
                }

                cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
                cmbFromCurrency.DisplayMemberPath = "CurrencyCode";
                cmbFromCurrency.SelectedValuePath = "ExchangeRate";
                cmbFromCurrency.SelectedIndex = 0;

                cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
                cmbToCurrency.DisplayMemberPath = "CurrencyCode";
                cmbToCurrency.SelectedValuePath = "ExchangeRate";
                cmbToCurrency.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }


        }
        #endregion

        #region Button Click Event

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue;

            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus();
                return;
            }
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency From", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus();
                return;
            }
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select Currency To", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus();
                return;
            }

            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                ConvertedValue = double.Parse(txtCurrency.Text);
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString()) * (double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString()));
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N6");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }
        #endregion

        #region Extra Events

        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }
        #endregion
    }
}
