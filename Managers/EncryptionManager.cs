using System;
using System.IO;
using System.Security.Cryptography;

namespace Discord_UWP.Managers
{
    class EncryptionManager
    {

        //ASK FOR ENCRYPTION
        private static DiffieHellman.DiffieHellman me;
        public static byte[] key;
        public static string GetHandshakeRequest()
        {
            me = new DiffieHellman.DiffieHellman(256).GenerateRequest();
            return "[[[DUWP_E2E]-HandshakeRequest]{" + me.ToString() + "}]";
        }

        //ACCEPT IT AND SET THE KEY
        public static string GetHandshakeResponse(string request)
        {
            me = new DiffieHellman.DiffieHellman(256).GenerateResponse(request);
            SetKey();
            return "[[[DUWP_E2E]-HandshakeResponse]{" + me.ToString() + "}]";
            //Set Alice's key
        }

        //HANDLE THE ACCEPTANCE BY SETTING THE KEY
        public static string HandleHandshakeResponse(string response)
        {
            if (me != null)
            {
                me.HandleResponse(response);
                SetKey();
                return me.Key.ToString();
            }
            else
                return "Uncrackable!";
        }

        public static void SetKey()
        {
            if (Storage.EncryptionKeys.ContainsKey(App.CurrentChannelId))
                Storage.EncryptionKeys[App.CurrentChannelId] = me.Key;
            else
                Storage.EncryptionKeys.Add(App.CurrentChannelId, me.Key);
            Storage.SaveEncryptionKeys();
            key = me.Key;
        }
        public static void UpdateKey(string channelId)
        {
            if (Storage.EncryptionKeys.ContainsKey(channelId))
                key = Storage.EncryptionKeys[channelId];
            else
                key = null;
        }
        public static string EncryptMessage(string content)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = me.Key;
                aesAlg.IV = new byte[] { 0x91, 0xf9, 0xec, 0x64, 0x3e, 0xb9, 0x22, 0x90, 0xe7, 0x56, 0xbd, 0x77, 0x78, 0xec, 0x49, 0xe4 };
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(content);
                        }
                    }
                    return "[[[DUWP_E2E]-Encrypted]{" + Convert.ToBase64String(msEncrypt.ToArray()) + "}]";
                }
            }
        }
        public static string DecryptMessage(string content)
        {
            try
            {
                content = content.Remove(0, 24);
                content = content.Remove(content.Length - 2);
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = key;
                    aesAlg.IV = new byte[] { 0x91, 0xf9, 0xec, 0x64, 0x3e, 0xb9, 0x22, 0x90, 0xe7, 0x56, 0xbd, 0x77, 0x78, 0xec, 0x49, 0xe4 };
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(content)))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                return "**`Failed to decrypt message`**";
            }
        }
    }
}
