#!/bin/bash
# ============================================================
# DonaRog App — Script di deploy per Linux
# ============================================================
# Uso:
#   chmod +x deploy.sh
#   ./deploy.sh           # DB locale (postgres in Docker)
#   ./deploy.sh --ext-db  # DB esterno (configura DB_HOST in .env)

set -e

COMPOSE_FILES="-f docker-compose.yml"
USE_LOCAL_DB=true

# Parsing argomenti
for arg in "$@"; do
  case $arg in
    --ext-db)
      USE_LOCAL_DB=false
      echo "Modalità: DB esterno (DB_HOST=$(grep DB_HOST .env | cut -d= -f2))"
      ;;
    --help|-h)
      echo "Uso: ./deploy.sh [opzioni]"
      echo ""
      echo "Opzioni:"
      echo "  --ext-db    Usa un DB PostgreSQL esterno (configura DB_HOST in .env)"
      echo "  --help      Mostra questo messaggio"
      exit 0
      ;;
  esac
done

# Verifica che .env esista
if [ ! -f ".env" ]; then
  echo "ERRORE: file .env non trovato."
  echo "Copia .env.example in .env e configura le variabili."
  echo "  cp .env.example .env"
  exit 1
fi

# Aggiungi compose file per DB locale se necessario
if [ "$USE_LOCAL_DB" = true ]; then
  COMPOSE_FILES="$COMPOSE_FILES -f docker-compose.local-db.yml"
  echo "Modalità: DB locale (PostgreSQL in Docker)"
fi

echo ""
echo "=== Avvio DonaRog App ==="
echo "  SERVER_HOST : $(grep SERVER_HOST .env | cut -d= -f2)"
echo "  API_PORT    : $(grep API_PORT .env | cut -d= -f2)"
echo "  FRONTEND    : $(grep FRONTEND_PORT .env | cut -d= -f2)"
echo "  DB_HOST     : $(grep DB_HOST .env | cut -d= -f2)"
echo ""

# Pull immagini base, build e avvio
docker compose $COMPOSE_FILES pull --ignore-buildable 2>/dev/null || true
docker compose $COMPOSE_FILES build
docker compose $COMPOSE_FILES up -d

echo ""
echo "=== Avvio completato ==="
echo "  Frontend : http://$(grep SERVER_HOST .env | cut -d= -f2):$(grep FRONTEND_PORT .env | cut -d= -f2)"
echo "  API      : http://$(grep SERVER_HOST .env | cut -d= -f2):$(grep API_PORT .env | cut -d= -f2)"
echo "  Swagger  : http://$(grep SERVER_HOST .env | cut -d= -f2):$(grep API_PORT .env | cut -d= -f2)/swagger"
echo ""
echo "Per i log: docker compose $COMPOSE_FILES logs -f"
