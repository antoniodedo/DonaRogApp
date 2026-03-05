#!/bin/sh
# Genera dynamic-env.json a partire dalle variabili d'ambiente del container,
# poi avvia nginx. In questo modo gli URL dell'API sono configurabili
# senza dover ricostruire l'immagine Docker.

cat > /usr/share/nginx/html/dynamic-env.json << EOF
{
  "oAuthConfig": {
    "issuer": "${API_URL}/",
    "requireHttps": false
  },
  "apis": {
    "default": {
      "url": "${API_URL}"
    },
    "AbpAccountPublic": {
      "url": "${API_URL}/"
    }
  }
}
EOF

echo "dynamic-env.json generato con API_URL=${API_URL}"
exec nginx -g 'daemon off;'
