using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;


namespace eta_4all
{
    public class Signer
    {

        private readonly string DllLibPath = "eps2003csp11.dll";
        private string TokenPin = "";


        public static string add_signatuer(string JsonString, string Token_pin)
        {

            Signer tokenSigner = new Signer();

            // string folder_path = @"D:\ets";

            tokenSigner.ListCertificates();




            tokenSigner.TokenPin = Token_pin;



            String cades = "";
            String SourceDocumentJson = JsonString;


            JObject request = JsonConvert.DeserializeObject<JObject>(SourceDocumentJson, new JsonSerializerSettings()
            {
                FloatFormatHandling = FloatFormatHandling.String,
                FloatParseHandling = FloatParseHandling.Decimal,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.None
            });

            //Start serialize
            String canonicalString = tokenSigner.Serialize(request);

            // retrieve cades
            if (request["documentTypeVersion"].Value<string>() == "0.9")
            {
                cades = "";
            }
            else
            {
                try
                {
                    cades = tokenSigner.SignWithCMS(canonicalString);
                }
                catch (Exception ex)
                {
                    cades = "";
                }

            }
            //File.WriteAllBytes(folder_path + @"\Cades.txt", System.Text.Encoding.UTF8.GetBytes(cades));
            JObject signaturesObject = new JObject(
                                   new JProperty("signatureType", "I"),
                                   new JProperty("value", cades));

            JArray signaturesArray = new JArray();
            if (cades != "")
            {
                signaturesArray.Add(signaturesObject);

            }





            request.Add("signatures", signaturesArray);
            String fullSignedDocument = "{\"documents\":[" + request.ToString() + "]}";
            // File.WriteAllBytes(folder_path + @"\FullSignedDocument.json", System.Text.Encoding.UTF8.GetBytes(fullSignedDocument));

            return fullSignedDocument;

        }
    

        public static byte[] Hash_static(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var output = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return output;
            }
        }
        public byte[] Hash(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var output = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                return output;
            }
        }

        private byte[] HashBytes(byte[] input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var output = sha.ComputeHash(input);
                return output;
            }
        }
        public string SignWithCMS(String serializedJson)
        {
            byte[] data = Encoding.UTF8.GetBytes(serializedJson);
            Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
            using (IPkcs11Library pkcs11Library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, config_class.DllLibPath, AppType.MultiThreaded))
            {
                ISlot slot = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent).FirstOrDefault();

                if (slot is null)
                {
                    return "No slots found";
                }

                ITokenInfo tokenInfo = slot.GetTokenInfo();

                ISlotInfo slotInfo = slot.GetSlotInfo();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var session = slot.OpenSession(SessionType.ReadWrite))
                {

                    session.Login(CKU.CKU_USER, Encoding.UTF8.GetBytes(config_class.token_pin));

                    var certificateSearchAttributes = new List<IObjectAttribute>()
                    {
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_CERTIFICATE),
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_TOKEN, true),
                        session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CERTIFICATE_TYPE, CKC.CKC_X_509)
                    };

                    IObjectHandle certificate = session.FindAllObjects(certificateSearchAttributes).FirstOrDefault();

                    if (certificate is null)
                    {
                        return "Certificate not found";
                    }

                    X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    store.Open(OpenFlags.MaxAllowed);

                    // find cert by thumbprint
                    var foundCerts = store.Certificates.Find(X509FindType.FindByIssuerName, config_class.token_type, false);

                    //var foundCerts = store.Certificates.Find(X509FindType.FindBySerialNumber, "2b1cdda84ace68813284519b5fb540c2", true);



                    if (foundCerts.Count == 0)
                        return "no device detected";

                    var certForSigning = foundCerts[0];
                    store.Close();


                    ContentInfo content = new ContentInfo(new Oid("1.2.840.113549.1.7.5"), data);


                    SignedCms cms = new SignedCms(content, true);

                    EssCertIDv2 bouncyCertificate = new EssCertIDv2(new Org.BouncyCastle.Asn1.X509.AlgorithmIdentifier(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47")), this.HashBytes(certForSigning.RawData));

                    SigningCertificateV2 signerCertificateV2 = new SigningCertificateV2(new EssCertIDv2[] { bouncyCertificate });


                    CmsSigner signer = new CmsSigner(certForSigning);

                    signer.DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1");



                    signer.SignedAttributes.Add(new Pkcs9SigningTime(DateTime.UtcNow));
                    signer.SignedAttributes.Add(new AsnEncodedData(new Oid("1.2.840.113549.1.9.16.2.47"), signerCertificateV2.GetEncoded()));
                    signer.IncludeOption = X509IncludeOption.EndCertOnly;

                    cms.ComputeSignature(signer);

                    var output = cms.Encode();

                    return Convert.ToBase64String(output);
                }
            }
        }

        public string Serialize(JObject request)
        {
            return SerializeToken(request);
        }


        private string SerializeToken(JToken request)
        {
            string serialized = "";
            if (request.Parent is null)
            {
                SerializeToken(request.First);
            }
            else
            {

                
                
                if (request.Type == JTokenType.Property)
                {
                    string name = ((JProperty)request).Name.ToUpper();
                    serialized += "\"" + name + "\"";
                    foreach (var property in request)
                    {
                        //if (property.ToString().ToLower().Contains( "COMMERCIALDISCOUNT".ToLower())) { System.Diagnostics.Debugger.Break(); };
                        if (property.Type == JTokenType.Object)
                        {
                            serialized += SerializeToken(property);
                        }
                        if (property.Type == JTokenType.Boolean || property.Type == JTokenType.Integer || property.Type == JTokenType.Float || property.Type == JTokenType.Date)
                        {
                            serialized += "\"" + property.Value<string>() + "\"";
                        }
                        if (property.Type == JTokenType.String)
                        {
                            serialized += JsonConvert.ToString(property.Value<string>());
                        }

                        if (property.Type == JTokenType.Array)
                        {
                            // if (((JProperty)request).Name.ToUpper().Contains("ITEMDISCOUNT")) { System.Diagnostics.Debugger.Break(); };
                            foreach (var item in property.Children())
                            {
                                if (item.Type == JTokenType.Integer)
                                {
                                    serialized += "\"" + ((JProperty)request).Name.ToUpper() + "\"";
                                    serialized += JsonConvert.ToString(item.Value<string>());
                                }
                                else if (item.Type == JTokenType.Float)
                                {
                                    serialized += "\"" + ((JProperty)request).Name.ToUpper() + "\"";
                                    serialized += JsonConvert.ToString(item.Value<string>());
                                }
                                else
                                {
                                    serialized += "\"" + ((JProperty)request).Name.ToUpper() + "\"";
                                    serialized += SerializeToken(item);
                                }
                            }
                        }
                    }
                }
            }
            if (request.Type == JTokenType.Object)
            {
                foreach (var property in request.Children())
                {

                    if (property.Type == JTokenType.Object || property.Type == JTokenType.Property)
                    {
                        serialized += SerializeToken(property);
                    }
                }
            }

            return serialized;
        }



        public void ListCertificates()
        {

            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.MaxAllowed);
            X509Certificate2Collection collection = (X509Certificate2Collection)store.Certificates;
            X509Certificate2Collection fcollection = (X509Certificate2Collection)collection.Find(X509FindType.FindBySerialNumber, "2b1cdda84ace68813284519b5fb540c2", true);
            foreach (X509Certificate2 x509 in fcollection)
            {
                try
                {
                    byte[] rawdata = x509.RawData;
                    Console.WriteLine("Content Type: {0}{1}", X509Certificate2.GetCertContentType(rawdata), Environment.NewLine);
                    Console.WriteLine("Friendly Name: {0}{1}", x509.FriendlyName, Environment.NewLine);
                    Console.WriteLine("Certificate Verified?: {0}{1}", x509.Verify(), Environment.NewLine);
                    Console.WriteLine("Simple Name: {0}{1}", x509.GetNameInfo(X509NameType.SimpleName, true), Environment.NewLine);
                    Console.WriteLine("Signature Algorithm: {0}{1}", x509.SignatureAlgorithm.FriendlyName, Environment.NewLine);
                    Console.WriteLine("Public Key: {0}{1}", x509.PublicKey.Key.ToXmlString(false), Environment.NewLine);
                    Console.WriteLine("Certificate Archived?: {0}{1}", x509.Archived, Environment.NewLine);
                    Console.WriteLine("Length of Raw Data: {0}{1}", x509.RawData.Length, Environment.NewLine);
                    x509.Reset();
                }
                catch (CryptographicException ex)
                {
                    Console.WriteLine("Information could not be written out for this certificate.");
                    throw ex;
                }
            }
            store.Close();
        }

        public string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("X"));
                }
                return builder.ToString();
            }
        }


        static string ComputeSha256Hash_static(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();

                //return Convert.ToBase64String(bytes);
            }
        }
    }
}
