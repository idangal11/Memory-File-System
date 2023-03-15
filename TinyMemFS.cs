using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;


namespace TinyMemFSApp
{   
    class TinyMemFS
    {
        Dictionary<string, byte[]> content = new Dictionary<string, byte[]>();
        Dictionary<string, List<string>> headers = new Dictionary<string, List<string>>();       
        static byte[] IV = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 };

        public TinyMemFS(){}

        public bool add(String fileName, String fileToAdd)
        {
            // fileName - The name of the file to be added to the file system
            // fileToAdd - The file path on the computer that we add to the systemdd
            // return false if operation failed for any reason
            // Example:
            // add("name1.pdf", "C:\\Users\\user\Desktop\\report.pdf")
            // note that fileToAdd isn't the same as the fileName

            if(fileName == null || fileToAdd == null) { return false; }
                
            if (this.content.ContainsKey(fileName))
            {
                Console.WriteLine("File allready exist in the file system");
                return false;
            }
            if (!File.Exists(fileToAdd)) { Console.WriteLine("File doesnt exist"); return false; }          
           //save path
            List<string> path = new List<string>();
            path.Add(fileToAdd);
            //headers.Add(fileName, fileToAdd);           
            //save data
            FileInfo oFileInfo = new FileInfo(fileToAdd);
            string name = oFileInfo.Name;
            long length = new FileInfo(fileToAdd).Length;
            string creation = oFileInfo.CreationTime.ToString();
            string data = "{" + name + ", " + length.ToString() + ", " + creation + "}";    
            path.Add(data);
            headers.Add(fileName, path);
            //save content
            byte[] contentBytes = File.ReadAllBytes(fileToAdd);                    
            content.Add(fileName, contentBytes);

            return true;
        }
        public bool remove(String fileName)
        {
            // fileName - remove fileName from the system
            // this operation releases all allocated memory for this file
            // return false if operation failed for any reason
            // Example:
            // remove("name1.pdf")

            if (fileName == null) { return false; }  
            
            if (!content.ContainsKey(fileName))
            {
                Console.WriteLine("File not exist in the file system");
                return false;
            }
            content.Remove(fileName);//dictinary maintenance correctly after remove
            headers.Remove(fileName);
            return true;
        }
        public List<String> listFiles()
        {
            // The function returns a list of strings with the file information in the system
            // Each string holds details of one file as following: "fileName,size,creation time"
            // Example:{
            // "report.pdf,630KB,Friday, May 13, 2022, ‏12:16:32 PM",
            // "table1.csv,220KB,Monday, February 14, 2022, 8:38:24 PM" }
            // You can use any format for the creation time and date
         
            List<String> files = new List<string>();
           
            foreach (KeyValuePair<String, List<string>> pair in headers)
            {
                files.Add("file name " + pair.Key + " Exist, path and data: ");
                files.Add(pair.Value[0]);
                files.Add("\n");
                files.Add(pair.Value[1]);
                files.Add("\n");
                files.Add("\n");
            }
            return files;
                
        }
        

        public bool save(String fileName, String fileToAdd)
        {
            // this function saves file from the TinyMemFS file system into a file in the physical disk
            // fileName - file name from TinyMemFS to save in the computer
            // fileToAdd - The file path to be saved on the computer
            // return false if operation failed for any reason
            // Example:
            // save("name1.pdf", "C:\\tmp\\fileName.pdf")

            if (fileName == null || fileToAdd == null) { return false; }               

            if (!this.content.ContainsKey(fileName))
            {
                Console.WriteLine("File not exist in the system");
                return false;
            }   
            
             using (FileStream fs = File.Create(fileToAdd))
             {
                 // Add information to the file
                 fs.Write(content[fileName], 0, content[fileName].Length);
             }
            
            return true;
        }
        static private byte[] getPasswordFromContainer(string key)
        {
            // Create the CspParameters object and set the key container
            // name used to store the RSA key pair.
#pragma warning disable CA1416 // Validate platform compatibility
            var parameters = new CspParameters
            {
                KeyContainerName = key
            };
#pragma warning restore CA1416 // Validate platform compatibility
            // Create a new instance of RSACryptoServiceProvider that accesses
            // the key container MyKeyContainerName.
#pragma warning disable CA1416 // Validate platform compatibility
            var rsa = new RSACryptoServiceProvider(parameters);
#pragma warning restore CA1416 // Validate platform compatibility
                              // Display the key information to the console.
                              //Console.WriteLine($"Key added to container: \n  {rsa.ToXmlString(true)}");

            byte[] b = TinyMemFS.getProperKeyForAlgorithm(rsa.ToXmlString(true));
            return b;
        }
           
            static private byte[] getProperKeyForAlgorithm(string s)
        {             
            List<Byte> list = new List<Byte>();           
            string theKey = s;
            for (int t = 0; t < 16; t++)
            {              
                list.Add((Byte)theKey[t]);               
            }                    
            byte[] dataBytes = list.ToArray();                    
            return dataBytes;
        }


        public bool encrypt(string key)
        {
            // key - Encryption key to encrypt the contents of all files in the system 
            // You can use an encryption algorithm of your choice
            // return false if operation failed for any reason
            // Example:
            // encrypt("myFSpassword")
           
            AesManaged aes = new AesManaged();                                             
            aes.IV = IV;
                      
            aes.Key = getPasswordFromContainer(key);
            
            foreach (KeyValuePair<String, byte[]> pair in content)
            {
                MemoryStream memorystream = new MemoryStream();
                CryptoStream cryptostream = new CryptoStream(memorystream, aes.CreateEncryptor(), CryptoStreamMode.Write);

                byte[] bytes = content[pair.Key].ToArray();
                string file = Encoding.Default.GetString(bytes);
               
                byte[] InputBytes = bytes;
                cryptostream.Write(InputBytes, 0, InputBytes.Length);
               
                byte[] encreapted = memorystream.ToArray();
                

                //overide and write encrypted data               
                for(int i=0; i < encreapted.Length; i++)
                {
                    content[pair.Key][i] = encreapted[i];
                }
                            
            }
         
            return true;
        }

        public bool decrypt(String key)
        {
            // fileName - Decryption key to decrypt the contents of all files in the system 
            // return false if operation failed for any reason
            // Example:
            // decrypt("myFSpassword")
                    
            AesManaged aes = new AesManaged();      
            aes.Key = getPasswordFromContainer(key);
            aes.IV = IV;           

            foreach (KeyValuePair<String, byte[]> pair in content)
            {
                MemoryStream memorystream = new MemoryStream();
                CryptoStream cryptostream = new CryptoStream(memorystream, aes.CreateDecryptor(), CryptoStreamMode.Write);

                byte[] bytes = content[pair.Key].ToArray();
                string file = Encoding.Default.GetString(bytes);

                byte[] InputBytes = bytes;
                cryptostream.Write(InputBytes, 0, InputBytes.Length);
                
                byte[] decrypted = memorystream.ToArray();
                        

                for (int i = 0; i < decrypted.Length; i++)
                {
                    content[pair.Key][i] = decrypted[i];
                }

            }
          
            return true;
        }

    }

}



