#region Usings

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace AcePokerSolutions.BusinessHelper
{
    public class EncryptionHelper
    {
        /// <summary>
        /// Encrypts the specified text.
        /// </summary>
        /// <param name="value">The text to be encrypted.</param>
        /// <returns></returns>
        public static string Encrypt(string value)
        {
            try
            {
                const string password = "Ac3P0kerS0lutions";

                using (RijndaelManaged rijndaelCipher = new RijndaelManaged())
                {
                    byte[] plainText = Encoding.Unicode.GetBytes(value);
                    byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
                    PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
                    //Creates a symmetric encryptor object. 
                    ICryptoTransform encryptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32),
                                                                                secretKey.GetBytes(16));
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        //Defines a stream that links data streams to cryptographic transformations
                        using (
                            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)
                            )
                        {
                            cryptoStream.Write(plainText, 0, plainText.Length);
                            //Writes the final state and clears the buffer
                            cryptoStream.FlushFinalBlock();
                            byte[] cipherBytes = memoryStream.ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                            string encryptedData = Convert.ToBase64String(cipherBytes);
                            return encryptedData;
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Decrypts the specified text.
        /// </summary>
        /// <param name="value">The text to be decrypted.</param>
        /// <returns></returns>
        public static string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            try
            {
                const string password = "Ac3P0kerS0lutions";

                using (RijndaelManaged rijndaelCipher = new RijndaelManaged())
                {
                    byte[] encryptedData = Convert.FromBase64String(value);
                    byte[] salt = Encoding.ASCII.GetBytes(password.Length.ToString());
                    //Making of the key for decryption
                    PasswordDeriveBytes secretKey = new PasswordDeriveBytes(password, salt);
                    //Creates a symmetric Rijndael decryptor object.
                    ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32),
                                                                                secretKey.GetBytes(16));
                    using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                    {
                        //Defines the cryptographics stream for decryption.THe stream contains decrpted data
                        using (
                            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read)
                            )
                        {
                            byte[] plainText = new byte[encryptedData.Length];
                            int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            //Converting to string
                            string decryptedData = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
                            return decryptedData;
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}