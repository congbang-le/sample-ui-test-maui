using Android.Security.Keystore;
using Java.Security;
using Javax.Crypto;

namespace VisitTracker;

public partial class BiometricSnapshotService
{
    private const string AndroidKeyStoreName = "AndroidKeyStore";
    private KeyStore KeyStore = KeyStore.GetInstance(AndroidKeyStoreName);

    public partial bool SnapshotBiometricState()
    {
        try
        {
            KeyStore.Load(null);

            if (KeyStore.ContainsAlias(BiometricKeyName))
                KeyStore.DeleteEntry(BiometricKeyName);

            var keyGenerator = KeyGenerator.GetInstance(KeyProperties.KeyAlgorithmAes, AndroidKeyStoreName);
            var builder = new KeyGenParameterSpec.Builder(BiometricKeyName,
                    KeyStorePurpose.Encrypt | KeyStorePurpose.Decrypt)
                .SetBlockModes(KeyProperties.BlockModeCbc)
                .SetEncryptionPaddings(KeyProperties.EncryptionPaddingPkcs7)
                .SetUserAuthenticationRequired(true)
                .SetInvalidatedByBiometricEnrollment(true);

            keyGenerator.Init(builder.Build());
            var secretKey = keyGenerator.GenerateKey();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public partial void ClearBiometricState()
    {
        KeyStore.Load(null);
        KeyStore.DeleteEntry(BiometricKeyName);
    }

    public partial bool? HasBiometricStateChanged()
    {
        try
        {
            if (!AppServices.Current.AppPreference.HasBiometricReg)
                return null;

            KeyStore.Load(null);
            var secretKey = KeyStore.GetKey(BiometricKeyName, null);

            var cipher = Cipher.GetInstance($"{KeyProperties.KeyAlgorithmAes}/{KeyProperties.BlockModeCbc}/{KeyProperties.EncryptionPaddingPkcs7}");
            cipher.Init(CipherMode.EncryptMode, secretKey);

            return false;
        }
        catch (KeyPermanentlyInvalidatedException)
        {
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}