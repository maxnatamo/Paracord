# Creating self-signed SSL certificates

When trying to use a certificate made with `makecert` or otherwise, it might throw an error, like this:
```
Unhandled exception. System.Security.Authentication.AuthenticationException: Authentication failed, see inner exception.
 ---> Interop+OpenSsl+SslException: SSL Handshake failed with OpenSSL error - SSL_ERROR_SSL.
 ---> Interop+Crypto+OpenSslCryptographicException: error:0A000418:SSL routines::tlsv1 alert unknown ca
 ...
```

This is because the certificate is seen as self-signed, as it has no issuer and/or Certificate Authority (CA).
To remedy this, you have to create your own local Certificate Authority, trust it and create a self-signed certificate from that.

On Linux, you can use `openssl`, which most likely comes pre-installed. On Windows, you might need to download it separately or use the
one included with Git (`C:\Program Files\Git\usr\bin\openssl.exe`).

## Creating a Certificate Authority (CA)

First, create the private key for the CA (`DevelopmentCA.key`).

```sh
openssl genrsa -out DevelopmentCA.key 4096
```

> [!TIP]
> If you want the private key to be password protected, add the `-aes256`-flag after `genrsa`.

Then, create the CA certificate (`DevelopmentCA.crt`)

```sh
openssl req -x509 -new -nodes -sha256 -days 3650 \
    -key DevelopmentCA.key \
    -out DevelopmentCA.crt \
    -subj '/CN=Development CA/C=DK/ST=Region Hovedstaden/L=Copenhagen/O=MyOrg'
```

> [!CAUTION]
> The subject of the CA certificate can be changed at will, but the Common Name (CN) shall not be the domain name.

## Create certificate for the domain

Again, first thing is to create a private key for the certificate:
```sh
openssl genrsa -out localhost.key 2048
```

> [!TIP]
> If you want the private key to be password protected, add the `-des3`-flag after `genrsa`.

Next, we need to create a *Certificate Signing Request* (CSR), which the CA can sign:
```sh
openssl req -new \
    -key localhost.key \
    -out localhost.csr \
    -subj '/CN=localhost/C=DK/ST=Region Hovedstaden/L=Copenhagen/O=MyOrg'
```

## Sign the domain certificate

The two certificates (the Root CA (`DevelopmentCA`) and the domain certificate (`localhost`)) are still completely separate.
So, to sign the domain certificate, we need to sign the domain's CSR with the Root CA:

```sh
cat > localhost.v3.ext << EOF
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
subjectAltName = @alt_names
[alt_names]
DNS.1 = localhost
EOF
```

> [!CAUTION]
> The `DNS.1` field should be the domain, that you wish to use the certificate for, i.e. `localhost`.

```sh
openssl x509 -req -days 3650 -CAcreateserial \
    -CA DevelopmentCA.crt \
    -CAkey DevelopmentCA.key \
    -in localhost.csr \
    -out localhost.crt \
    -extfile localhost.v3.ext
```

## Convert the PEM certificate to PKCS12 (`.pfx`)

For Paracord to read the certificate properly, we need to convert the certificate to the PKCS12-format:
```sh
openssl pkcs12 -export \
    -inkey localhost.key \
    -in localhost.crt \
    -out localhost.pfx
```

This will ask for a password, but you don't need to set one. To avoid it, just press `Enter` for both prompts.

## Install Certificate Authority

> [!IMPORTANT]
> If you don't install the certificate authority certificate, Paracord still won't work.

# [Ubuntu-based](#tab/ubuntu)

First, make sure `ca-certificates` is installed. While this should be the case already, installation won't work without it:
```sh
sudo apt install -y ca-certificates
```

Copy the certificate to the local trust and update the store:
```sh
sudo cp DevelopmentCA.crt /usr/local/share/ca-certificates
sudo update-ca-certificates
```

# [Fedora-based](#tab/fedora)

Copy the certificate to the local trust and update the store:
```sh
sudo cp DevelopmentCA.crt /etc/pki/ca-trust/source/anchors/
sudo update-ca-trust
```

# [Windows](#tab/windows)

TODO