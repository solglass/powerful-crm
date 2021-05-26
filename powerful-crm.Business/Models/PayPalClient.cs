using PayPalCheckoutSdk.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace powerful_crm.Business.Models
{
    public class PayPalClient
    {
        public static PayPalEnvironment environment()
        {
            return new SandboxEnvironment("AReuOvuqjOOKB6FDjOl3s8Ny-TcYB3pGxs-vOzxbcX0lYaY00xmFFM34XD9_i--zdMIBsqK1YCANFUBw", "EDdl4IbbO08n8rBkMDBk2rFYBH71oIU29PqaP6yHlZvu0byd_rrq74mVnbKQocVfBpzHnpF0kYW-Uljx");
        }

        /**
            Returns PayPalHttpClient instance to invoke PayPal APIs.
         */
        public static PayPalHttpClient client()
        {
            return new PayPalHttpClient(environment());
        }

        public static PayPalHttpClient client(string refreshToken)
        {
            return new PayPalHttpClient(environment(), refreshToken);
        }

        /**
            Use this method to serialize Object to a JSON string.
        */
        public static String ObjectToJSONString(Object serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream();
            var writer = JsonReaderWriterFactory.CreateJsonWriter(
                        memoryStream, Encoding.UTF8, true, true, "  ");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(serializableObject.GetType(), new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });
            ser.WriteObject(writer, serializableObject);
            memoryStream.Position = 0;
            StreamReader sr = new StreamReader(memoryStream);
            return sr.ReadToEnd();
        }
    }
}
