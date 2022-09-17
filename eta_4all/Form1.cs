using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eta_4all
{
    public partial class Form1 : Form
    {

        string client_id;string client_secret;
        string folder_path;
        string tok;
        public Form1()
        {
            InitializeComponent();
        }
      

        void connect()
        {

            Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
            using (IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, config_class.DllLibPath, AppType.MultiThreaded))
            {
                ISlot slot = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent).FirstOrDefault();

                if (slot is null)
                {
                    MessageBox.Show("No slots found");
                    return;
                }

                ITokenInfo tokenInfo = slot.GetTokenInfo();

                ISlotInfo slotInfo = slot.GetSlotInfo();


                using (var session = slot.OpenSession(SessionType.ReadWrite))
                {
                    try
                    {
                        session.Login(CKU.CKU_USER, Encoding.UTF8.GetBytes(config_class.token_pin));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("pin is not correct");
                        return;
                    }

                    var certificateSearchAttributes = new List<IObjectAttribute>()
                    {
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
                    };

                    IObjectHandle certificate = session.FindAllObjects(certificateSearchAttributes).FirstOrDefault();

                    if (certificate is null)
                    {
                        MessageBox.Show("Certificate not found");
                        return;

                    }

                    X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.MaxAllowed);

                    // find cert by thumbprint
                    var foundCerts = store.Certificates.Find(X509FindType.FindByIssuerName, config_class.token_type, false);

                    //var foundCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, "2b1cdda84ace68813284519b5fb540c2", true);



                    if (foundCerts.Count == 0)
                    {
                        MessageBox.Show("no device detected");
                        return;
                    }


                    var certForSigning = foundCerts[0];
                    store.Close();
                   /* config_class.token_pin = txt_pin.Text;
                    config_class.token_type = cmb_token.EditValue.ToString();*/

                }
            }

        }

        private string GetBearerToken()
        {
            try
            {
                var client = new RestClient(config_class.tokenurl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&client_id=" +
                  client_id + "&client_secret=" + client_secret, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);



                dynamic resp = JObject.Parse(response.Content);
                String token = resp.access_token;

               
                // MessageBox.Show(token);
                return token;
            }
            catch (Exception ex)
            {

            }

            return "";
        }
        private async Task sendinvoice(string invoice_sn)
        {
            /* asunc task for sending invoice  */

            
                tok = GetBearerToken();
            


            var client = new RestClient(config_class.BaseUrl + "/api/v1.0/documentsubmissions");

            var request = new RestRequest(Method.POST);
            request.AddHeader("authorization", @"Bearer " + tok);
            request.AddHeader("cache-control", "no-cache");

            //  request.AddHeader("Cookie", "75fd0698a2e84d6b8a3cb94ae54530f3=054e7d0fb7353830e763d83ee8bd30d6");
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("User-Agent", "swap-erp");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Accept-Encoding", "gzip, deflate, br");
            request.AddHeader("Connection", "keep-alive");
            string data = get_signed_invoice(invoice_sn);

            /* if(data.Contains("No slots found"))
             {
                 MessageBox.Show("token not found");
                 return;
             }*/
            string d = "";
           
            IRestResponse result = client.Execute(request.AddJsonBody(data));
            if (result.Content != "")
            {
                JObject o = JObject.Parse(result.Content);


                



                  //




            }
        }
        private string get_signed_invoice(string invoice)
        {

            return Signer.add_signatuer(invoice, config_class.token_pin);

        }
        private async void send()

        {
            connect();
            string[] fileEntries = Directory.GetFiles(folder_path);
            foreach (string fileName in fileEntries)
            {
                 string readText = File.ReadAllText(fileName);
                await sendinvoice(readText);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            config_class.token_type= System.Configuration.ConfigurationManager.AppSettings.Get("token_type");
            config_class.token_pin= System.Configuration.ConfigurationManager.AppSettings.Get("token_pin");

            client_id= System.Configuration.ConfigurationManager.AppSettings.Get("client_id");
            client_secret= System.Configuration.ConfigurationManager.AppSettings.Get("client_secrit");
            folder_path = System.Configuration.ConfigurationManager.AppSettings.Get("folder_path");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            config_form fr = new config_form();
            fr.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            send();
            MessageBox.Show("done");
        }

        private void ch_prepod_CheckedChanged(object sender, EventArgs e)
        {
            if(ch_prepod.Checked)
            {
                config_class.BaseUrl = "https://api.preprod.invoicing.eta.gov.eg";
                config_class.tokenurl = "https://id.preprod.eta.gov.eg/connect/token";

                
               
                
            }
            else
            {
                config_class.BaseUrl = "https://api.invoicing.eta.gov.eg";
                config_class.tokenurl = "https://id.eta.gov.eg/connect/token";
            }
        }
    }
}
