#!/bin/bash

# Couleurs
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${BLUE}?? D�marrage du d�ploiement de la PWA...${NC}\n"

# Se d�placer dans le r�pertoire du projet
cd /home/admin_maef/Projects/PWA/pwa_core/src/PWA.Auth

# Build de la PWA Blazor WebAssembly
echo -e "${BLUE}?? Build de la PWA...${NC}"
dotnet publish -c Release -o out

# Verifier si le build a r�ussi
if [ $? -ne 0 ]; then
    echo -e "${RED}? Erreur lors du build${NC}"
    exit 1
fi

# Copie des fichiers vers le site
echo -e "${BLUE}?? Copie des fichiers sauf env et appsettings...${NC}"
sudo rsync -av --delete --exclude='appsettings.json' \ --exclude='.env'  /home/admin_maef/Projects/PWA/pwa_core/src/PWA.Auth/out/wwwroot/ /var/www/PWA_Auth/


echo -e "\n${GREEN}? D�ploiement r�ussi ! La PWA est disponible.${NC}"
