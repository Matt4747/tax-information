using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.IO;
using System.Reflection;

namespace USTaxInformation
{

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        IDictionary<string, List<string>> lookupDict = new Dictionary<string, List<string>>();
        IDictionary<string, List<int>> financeDict = new Dictionary<string, List<int>>();
        List<int> states;
        List<string> names;
        String[] toks;

        public MainPage()
        {
            InitializeComponent();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "USTaxInformation.zipcodes.tsv.txt";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader input = new StreamReader(stream))
            {
                    while (!input.EndOfStream)
                    {
                        names = new List<string>();
                        states = new List<int>();
                        string line = input.ReadLine();
                        toks = line.Split('\t');
                        string zip = toks[1];
                        string cityName = toks[3];
                        string stateName = toks[4];
                        Int32.TryParse(toks[16], out int returns);
                        Int32.TryParse(toks[18], out int wages);
                        String averageString;
                        if (returns != 0 && wages != 0)
                        {
                            int average = wages / returns;
                            averageString = average.ToString();
                        }
                        else
                        {
                        averageString = "N/A";
                        }
                        names = new List<string>();
                        if (!lookupDict.ContainsKey(zip))
                        {
                            names.Add(cityName);
                            names.Add(stateName);
                            names.Add(zip);
                            names.Add(averageString);
                            lookupDict.Add(zip, names);
                        }
                    }
            }
        }
        void OnButtonClicked(object sender, EventArgs e)
        {
            // CityState
            selectedZip.Text = "";
            selectedCity.Text = "";
            selectedState.Text = "";
            selectedReturn.Text = "";
            if (!(type.IsToggled)){
                if (city.Text == null || state.Text == null || city.Text == "" || state.Text == "")
                {
                    alert.Text = "Enter a valid City and State";
                    alert.TextColor = Color.Red;
                }
                else {
                    alert.Text = "tap entry to expand";
                    alert.TextColor = Color.Black;
                    string cityQuery = city.Text.ToUpper();
                    string stateQuery = state.Text.ToUpper();
                    List<string> l = new List<string>();
                    string header = "Zipcode" + "     " + "City" + "     " + "State" + "     " + "Return";
                    l.Add(header);
                    foreach (KeyValuePair<string, List<string>> entry in lookupDict)
                    {
                        if (entry.Value[0] == cityQuery && entry.Value[1] == stateQuery)
                        {
                            string res = entry.Value[2] + " " + entry.Value[0] + " " + entry.Value[1] + " " + entry.Value[3];
                            l.Add(res);
                        }
                    }
                    listView.ItemsSource = l;
                }
            }
            // Amount
            else
            {
                string amountQuery = amount.Text;
                Int32.TryParse(amountQuery, out int intAmount);
                if (intAmount == 0)
                {
                    alert.Text = "Enter a valid amount";
                }
                else
                {
                    alert.Text = "tap entry to expand";
                    alert.TextColor = Color.Black;
                    int lowerBound = intAmount - 100;
                    int upperBound = intAmount + 100;
                    List<string> l = new List<string>();
                    string header = "Zipcode" + "     " + "City" + "     " + "State" + "     " + "Return";
                    l.Add(header);
                    foreach (KeyValuePair<string, List<string>> entry in lookupDict)
                    {
                        Int32.TryParse(entry.Value[3], out int intAvg);
                        if (intAvg > lowerBound && intAvg < upperBound)
                        {
                            string res = entry.Value[2] + " " + entry.Value[0] + " " + entry.Value[1] + " " + entry.Value[3];
                            l.Add(res);
                        }
                    }
                    listView.ItemsSource = l;
                }
            }
        }
        void OnItemSelected(object sender, EventArgs e)
        {
            alert.Text = "";
            string[] selectedArr;
            selectedArr = listView.SelectedItem.ToString().Split(' ');
            selectedZip.Text = "Zipcode: " + selectedArr[0];
            //Handle cities with 2 word names
            if (selectedArr.Length == 5)
            {
                selectedCity.Text = "City: " + selectedArr[1] + " " + selectedArr[2];
                selectedState.Text = "State: " + selectedArr[3];
                selectedReturn.Text = "Average Tax Return: " + selectedArr[4];
            }
            //Handle cities with 3 word names
            else if (selectedArr.Length == 6)
            {
                selectedCity.Text = "City: " + selectedArr[1] + " " + selectedArr[2] + " " + selectedArr[3];
                selectedState.Text = "State: " + selectedArr[4];
                selectedReturn.Text = "Average Tax Return: " + selectedArr[5];
            }
            //Handle cities with 4 word names (in case there are any)
            else if (selectedArr.Length == 7)
            {
                selectedCity.Text = "City: " + selectedArr[1] + " " + selectedArr[2] + " " + selectedArr[3] + " " + selectedArr[4];
                selectedState.Text = "State: " + selectedArr[5];
                selectedReturn.Text = "Average Tax Return: " + selectedArr[6];
            }
            else
            {
                selectedCity.Text = "City: " + selectedArr[1];
                selectedState.Text = "State: " + selectedArr[2];
                selectedReturn.Text = "Average Tax Return: " + selectedArr[3];
            }
        }
    }
}
