# Thanks to https://github.com/DaniJG/ASPCoreGRPCSample/blob/master/Products/scripts/generate_certificates.sh !

echo "Creating certs folder ..."
mkdir certs
cd certs

echo "Generating certificates ..."

openssl genrsa -passout pass:1234 -des3 -out ca.key 4096

openssl req -passin pass:1234 -new -x509 -days 365 -key ca.key -out ca.crt -subj  "/C=US/ST=WV/L=Martinsburg/O=MahdLabs/OU=Main/CN=ca"

openssl genrsa -passout pass:1234 -des3 -out server.key 4096

openssl req -passin pass:1234 -new -key server.key -out server.csr -subj  "/C=US/ST=WV/L=Martinsburg/O=MahdLabs/OU=Server/CN=localhost"

openssl x509 -req -passin pass:1234 -days 365 -in server.csr -CA ca.crt -CAkey ca.key -set_serial 01 -out server.crt

openssl rsa -passin pass:1234 -in server.key -out server.key

openssl genrsa -passout pass:1234 -des3 -out client.key 4096

openssl req -passin pass:1234 -new -key client.key -out client.csr -subj  "/C=US/ST=WV/L=Martinsburg/O=MahdLabs/OU=Client/CN=localhost"

openssl x509 -passin pass:1234 -req -days 365 -in client.csr -CA ca.crt -CAkey ca.key -set_serial 01 -out client.crt

openssl rsa -passin pass:1234 -in client.key -out client.key

openssl pkcs12 -export -password pass:1234 -out client.pfx -inkey client.key -in client.crt

openssl pkcs12 -export -password pass:1234 -out server.pfx -inkey server.key -in client.crt