#!/bin/bash

# Couleurs
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${BLUE}?? Dï¿½marrage du dï¿½ploiement de la PWA...${NC}\n"

# Se dï¿½placer dans le rï¿½pertoire du projet
cd /home/admin_maef/Projects/PWA/pwa_core/src/PWA.Auth

# Build de la PWA Blazor WebAssembly
echo -e "${BLUE}?? Build de la PWA...${NC}"
dotnet publish -c Release -o out

# Verifier si le build a rï¿½ussi
if [ $? -ne 0 ]; then
    echo -e "${RED}? Erreur lors du build${NC}"
    exit 1
fi

# Vérification des fichiers générés
echo -e "${YELLOW}?? Vérification des fichiers générés...${NC}"
FILE_COUNT=$(find out/wwwroot/_framework -type f | wc -l)
echo -e "   Fichiers dans _framework: ${FILE_COUNT}"

if [ $FILE_COUNT -lt 10 ]; then
    echo -e "${RED}? Trop peu de fichiers générés, build potentiellement incomplet${NC}"
    exit 1
fi


# Copie des fichiers vers le site
echo -e "${BLUE}?? Copie des fichiers sauf env et appsettings...${NC}"
sudo rsync -av --delete \
    --exclude='appsettings.Development.json' \
    --exclude='.env' \
    /home/admin_maef/Projects/PWA/pwa_core/src/PWA.Auth/out/wwwroot/ \
    /var/www/PWA_Auth/

# Vérification du déploiement
echo -e "${YELLOW}?? Vérification du déploiement...${NC}"
DEPLOYED_COUNT=$(sudo find /var/www/PWA_Auth/_framework -type f 2>/dev/null | wc -l)
echo -e "   Fichiers déployés dans _framework: ${DEPLOYED_COUNT}"

# Nettoyage du dossier out
echo -e "${YELLOW}?? Nettoyage du dossier out/...${NC}"
rm -rf out/


echo -e "\n${GREEN}? Dï¿½ploiement rï¿½ussi ! La PWA est disponible.${NC}"
