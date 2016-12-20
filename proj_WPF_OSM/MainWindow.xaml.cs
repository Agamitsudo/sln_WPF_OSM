using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;

namespace WpfApplication3
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            GMapControl gMapControl1 = new GMapControl();
            Carte.MapProvider = OpenStreetMapProvider.Instance;
            Carte.SetPositionByKeywords("Lyon, France");
            Carte.ShowCenter = false;
            Carte.MinZoom = 5;
            Carte.MaxZoom = 25;
            Carte.Zoom = 15;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string request(string value)
        {
            WebRequest wrGETURL;
            string url = "http://nominatim.openstreetmap.org/search?q=" + value + "&format=xml";
            wrGETURL = WebRequest.Create(url);
            string result = "";
            try
            {
                using (HttpWebResponse response = ((HttpWebResponse)wrGETURL.GetResponse()))
                {
                    StreamReader objReader = new StreamReader(response.GetResponseStream());
                    result = objReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search(object sender, RoutedEventArgs e)
        {
            try
            {
                DiplayName.Text = "";
                Carte.SetPositionByKeywords("Lyon, France");

                string nominatim = Nominatim.Text;
                string xmlString = request(nominatim);
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(xmlString);
                XmlNodeList nodes = xml.SelectNodes("//place");

                if (nodes.Count == 0)
                {
                    Carte.SetPositionByKeywords("Lyon, France");
                    DiplayName.Text = "Pas de résultat";
                    return;
                }

                XmlNode node = nodes[0];

                XmlDocument xmlNode = new XmlDocument();

                string lat = node.Attributes["lat"].Value.ToString();
                string lon = node.Attributes["lon"].Value.ToString();
                string display_name = node.Attributes["display_name"].Value.ToString();

                lat = lat.Replace(".", ",");
                lon = lon.Replace(".", ",");
                
                double dlat = Convert.ToDouble(lat);
                double dlon = Convert.ToDouble(lon);
                DiplayName.Text = display_name;

                //Center map on a point
                Carte.Position = new PointLatLng(dlat, dlon);

                GMapMarker marker = new GMapMarker(new PointLatLng(dlat, dlon));

                marker.Shape = new Ellipse
                {
                    Width = 12,
                    Height = 12,
                    Stroke = Brushes.Red,
                    StrokeThickness = 3
                };

                Carte.Markers.Clear();
                Carte.Markers.Add(marker);
                
                Carte.Zoom = 15;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
