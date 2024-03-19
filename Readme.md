# Extension Manager

## Creating a signing certificate
Please run the following code inside PowerShell to create a self-signed certificate for signing your licenses.

> **_Note:_** Please replace the `$MyOrg` and `$MyCountry` variables with your own values. The snippet will create a certificate with the following settings:
> - **Key Usage**: Digital Signature
> - **Enhanced Key Usage**: Client Authentication, Code Signing
> - **Key Length**: 4096
> - **Key Algorithm**: RSA
> - **Hash Algorithm**: SHA256
> - **Friendly Name**: CA License Signing
> - **Not Before**: 2024-01-01
> - **Not After**: 2100-01-01
> - **Export Policy**: Non-Exportable

```powershell
$MyOrg = "Matrix42 Community Extensions"
$MyCountry = "DE"
New-SelfSignedCertificate -CertStoreLocation Cert:\CurrentUser\My\ -Subject "CN=Central Administration License Signing,O=$MyOrg,C=$MyCountry" -KeyExportPolicy NonExportable -KeyLength 2048 -KeyAlgorithm RSA -HashAlgorithm SHA256 -FriendlyName "CA License Signing" -KeyUsage DigitalSignature -HardwareKeyUsage None -Type Custom  -NotBefore "2024-01-01T00:00:00Z" -NotAfter "2100-01-01T00:00:00Z" -KeySpec Signature -Provider "Microsoft Enhanced RSA and AES Cryptographic Provider" -TextExtension "2.5.29.37={text}1.3.6.1.5.5.7.3.2,1.3.6.1.5.5.7.3.3"
```

Please note the Thumbprint of the certificate and enter it in the Extension Manager settings.