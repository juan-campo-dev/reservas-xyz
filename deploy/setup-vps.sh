#!/bin/bash
# ══════════════════════════════════════════════════════════
# SCRIPT DE DESPLIEGUE — ReservasXYZ
# Ejecutar en el VPS Ubuntu después de clonar el repo
# ══════════════════════════════════════════════════════════
set -euo pipefail

echo "═══════════════════════════════════════════"
echo "  ReservasXYZ — Setup VPS Ubuntu"
echo "═══════════════════════════════════════════"

# ── 1. Actualizar sistema ──
echo "[1/5] Actualizando sistema..."
sudo apt-get update && sudo apt-get upgrade -y

# ── 2. Instalar Docker ──
echo "[2/5] Instalando Docker..."
if ! command -v docker &> /dev/null; then
    curl -fsSL https://get.docker.com | sudo sh
    sudo usermod -aG docker $USER
    echo "⚠️  Cierra sesión y vuelve a entrar para que el grupo docker aplique."
fi

# ── 3. Instalar Docker Compose plugin ──
echo "[3/5] Verificando Docker Compose..."
if ! docker compose version &> /dev/null; then
    sudo apt-get install -y docker-compose-plugin
fi

# ── 4. Crear archivo .env ──
echo "[4/5] Configurando variables de entorno..."
if [ ! -f .env ]; then
    cp .env.example .env
    echo "⚠️  EDITA el archivo .env con tus valores reales:"
    echo "    nano .env"
    echo ""
    echo "  Campos obligatorios:"
    echo "    - SA_PASSWORD (contraseña SQL Server)"
    echo "    - DOMAIN (tu subdominio)"
    echo "    - SMTP_EMAIL y SMTP_PASSWORD (si quieres correos)"
    exit 0
fi

# ── 5. Reemplazar dominio en nginx.conf ──
echo "[5/5] Configurando nginx..."
DOMAIN=$(grep '^DOMAIN=' .env | cut -d '=' -f2)
if [ -n "$DOMAIN" ]; then
    sed -i "s/DOMAIN_PLACEHOLDER/$DOMAIN/g" deploy/nginx.conf
    echo "✅ Nginx configurado para: $DOMAIN"
fi

echo ""
echo "═══════════════════════════════════════════"
echo "  ¡Listo! Ejecuta:"
echo ""
echo "  docker compose up -d --build"
echo ""
echo "  Después de verificar que funciona en HTTP,"
echo "  ejecuta para SSL:"
echo ""
echo "  docker compose run --rm certbot certonly \\"
echo "    --webroot -w /var/lib/letsencrypt \\"
echo "    -d $DOMAIN --email tu@email.com --agree-tos"
echo ""
echo "═══════════════════════════════════════════"
