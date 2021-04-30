using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace TalkaTIPClientV2
{
    class Security
    {
        // Initializes a new instance of the ECDiffieHellmanCng class with a random key pair
        ECDiffieHellmanCng owner = new ECDiffieHellmanCng();
        byte[] iv = { 126, 122, 93, 86, 153, 51, 216, 230, 93, 82, 240, 192, 201, 239, 119, 120 };

        public ECDiffieHellmanPublicKey GetOwnerPublicKey()
        {
            return owner.PublicKey;
        }

        public void SetDerivationFunction()
        {
            // Sets the key derivation function for the ECDiffieHellmanCng class
            owner.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            owner.HashAlgorithm = CngAlgorithm.Sha256;
        }

        // Set session key
        public byte[] SetSessionKey(byte[] clientPublicKey) 
        {
            // The first argument is public key from the client
            byte[] sessionKey = owner.DeriveKeyMaterial(ECDiffieHellmanCngPublicKey.FromByteArray(clientPublicKey, CngKeyBlobFormat.EccPublicBlob));           

            return sessionKey;
        }

        /// <summary>
        /// Returns a ciphered text in Base64 format
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secretMessage"></param>
        /// <returns></returns>
        public string EncryptMessage(byte[] key, string secretMessage)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = iv;

                // Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage);
                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Close();
                    byte[] encryptedMessage = ciphertext.ToArray();
                    return Convert.ToBase64String(encryptedMessage);
                }
            }
        }

        public string DecryptMessage(string encryptedMessage, byte[] sessionKey)
        {
            byte[] convertedMessage = Convert.FromBase64String(encryptedMessage);
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = sessionKey;
                aes.IV = iv;
                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(convertedMessage, 0, convertedMessage.Length);
                        cs.Close();
                        string message = Encoding.UTF8.GetString(plaintext.ToArray());
                        return message;
                    }
                }
            }
        }

        /// <summary>
        /// The argument is password + login. Returns 256-bit hexadecimal hash.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string hashPassword(string password)
        {
            byte[] bytePasswd = Encoding.Default.GetBytes(password);
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytePasswd = sha256.ComputeHash(bytePasswd);
                string hashedPassword = BitConverter.ToString(hashBytePasswd).Replace("-", string.Empty);
                return hashedPassword;
            }
        }
    }
}
