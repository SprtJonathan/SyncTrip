# SyncTrip

> Application mobile de suivi de convoi en temps réel pour voyages multi-véhicules

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/apps/maui)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Android%20%7C%20iOS-lightgrey)](https://github.com/synctrip/synctrip)

## Vue d'ensemble

**SyncTrip** permet à des groupes de voyageurs circulant dans différents véhicules de :

- Créer des groupes persistants réutilisables avec code permanent
- Gérer plusieurs voyages (trips) dans un même groupe
- Se suivre en temps réel sur une carte interactive
- Définir destinations et points d'arrêt à la volée
- Communiquer via chat de groupe et notifications
- Conserver un historique détaillé avec statistiques

### Cas d'usage typiques

**Voyage simple** : Famille partant en Pologne dans deux voitures
- Crée un convoy "Famille Dupont" avec destination Varsovie
- Chacun navigue à son rythme en se voyant sur la carte
- Ajout de points d'arrêt spontanés pour se retrouver

**Voyage avec escale** : Trajet trop long, pause hôtel
- Trip 1 : Paris → Hôtel (500km, terminé le soir)
- Trip 2 : Hôtel → Varsovie (500km, démarré le lendemain)
- Même groupe, même code, deux voyages distincts

**Groupe réutilisable** : Amis motards
- Convoy "Potes Moto" créé une fois
- Trip 1 : Alpes (Été 2025)
- Trip 2 : Provence (Automne 2025)
- Trip 3 : Italie (Été 2026)
- Même code permanent, participants persistent

## Fonctionnalités principales

### Authentification sans mot de passe
- **Magic Link** : Connexion par email unique
- **2FA optionnelle** : Vérification téléphone par SMS
- Sécurisé, simple, moderne

### Gestion des groupes (Convoys)
- Création avec code alphanumérique permanent (6 caractères : `aB3xK9`)
- Invitation par lien ou code partageable
- Gestion des permissions par participant
- Rôles : Leader (admin) et Members
- Groupes réutilisables pour plusieurs voyages

### Gestion des voyages (Trips)
- Destination obligatoire pour chaque trip
- Préférence de route (Rapide, Sympa, Économique, Courte)
- États : Planifié, Actif, En pause, Terminé, Annulé
- Un seul trip actif par convoy à la fois
- Création de nouveaux trips dans un convoy existant

### Suivi GPS temps réel
- Mise à jour toutes les 5 secondes
- Visualisation de tous les véhicules
- Vitesse, direction, distance entre véhicules
- ETA (heure d'arrivée estimée)
- Mode offline avec synchronisation auto

### Carte interactive
- OpenStreetMap (gratuit, sans limites)
- Marqueurs personnalisables par véhicule
- Trajet effectué visible
- Ajout de points d'arrêt par appui long
- Cache offline des tuiles

### Communication
- Chat de groupe par convoy
- Messages rapides prédéfinis
- Notifications push (nouveau waypoint, participant arrivé, etc.)
- Statuts véhicule (En route, Pause, Problème, Arrivé)

### Historique et statistiques
- Tous les trips d'un convoy consultables
- Distance parcourue, vitesse moyenne, temps de pause
- Visualisation du trajet sur carte
- Export GPX/KML pour analyse
- Conformité RGPD avec export et suppression

## Architecture

### Concepts clés

**Convoy** = Groupe persistant de personnes
- Code permanent alphanumérique
- Participants persistent entre les voyages
- Peut contenir plusieurs trips

**Trip** = Instance de voyage
- Destination obligatoire
- Lié à un convoy
- Waypoints et historique GPS

### Stack technique

```
┌─────────────────────────────┐
│   .NET 10 MAUI App          │
│   (Android / iOS)           │
└──────────┬──────────────────┘
           │
           │ HTTPS + WebSocket (SignalR)
           │
┌──────────▼──────────────────┐
│   ASP.NET Core 10 API       │
│   + SignalR Hub             │
└──────────┬──────────────────┘
           │
    ┌──────┴───────┐
    │              │
┌───▼────┐    ┌───▼────┐
│PostgreSQL    │ Redis  │
│  (Data)│    │(Cache) │
└────────┘    └────────┘
```

**Frontend** : .NET MAUI 10, MVVM, Mapsui (OpenStreetMap), SignalR Client, SQLite local

**Backend** : ASP.NET Core 10, PostgreSQL 16, Redis 7, SignalR, JWT passwordless

**Services** : Magic link email, SMS OTP, Geocoding (Nominatim)

Voir [DOCUMENTATION.md](DOCUMENTATION.md) pour l'architecture complète.

## Structure du projet

```
SyncTrip/
├── SyncTrip/                  # Application MAUI
│   ├── Core/                  # Entities, Enums, Interfaces
│   ├── Services/              # API, SignalR, GPS, DB, Auth
│   ├── ViewModels/            # ViewModels MVVM
│   ├── Views/                 # Pages XAML
│   ├── Models/                # DTOs
│   └── Resources/             # Styles, Images
│
├── SyncTrip.Api/              # Backend ASP.NET Core (à créer)
│   ├── Controllers/           # API REST
│   ├── Hubs/                  # SignalR Hub
│   ├── Core/                  # Entities, Enums
│   ├── Application/           # Services métier
│   └── Infrastructure/        # Data, Cache, External
│
├── DOCUMENTATION.md           # Doc technique complète
├── IMPLEMENTATION_GUIDE.md    # Guide d'implémentation
└── README.md                  # Ce fichier
```

## Démarrage rapide

### Prérequis

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022 17.12+](https://visualstudio.microsoft.com/) ou [Rider 2024.3+](https://www.jetbrains.com/rider/)
- Workload MAUI : `dotnet workload install maui`
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (PostgreSQL et Redis)
- Émulateur Android 14+ ou appareil physique
- Xcode (pour iOS, Mac uniquement)

### Installation

1. **Cloner le repository**
   ```bash
   git clone https://github.com/votre-username/SyncTrip.git
   cd SyncTrip
   ```

2. **Lancer les services backend (Docker)**
   ```bash
   docker-compose up -d
   ```

   Ou manuellement :
   ```bash
   # PostgreSQL
   docker run --name synctrip-postgres \
     -e POSTGRES_DB=synctrip \
     -e POSTGRES_USER=synctrip_user \
     -e POSTGRES_PASSWORD=dev_password_123 \
     -p 5432:5432 -d postgres:16

   # Redis
   docker run --name synctrip-redis \
     -p 6379:6379 -d redis:7-alpine
   ```

3. **Créer et configurer le projet API Backend**
   ```bash
   cd SyncTrip.Api  # (à créer si n'existe pas)
   dotnet restore
   dotnet ef database update
   dotnet run
   ```

4. **Lancer l'application MAUI**
   ```bash
   cd SyncTrip
   dotnet restore

   # Android
   dotnet build -t:Run -f net10.0-android

   # iOS (Mac uniquement)
   dotnet build -t:Run -f net10.0-ios
   ```

### Configuration

Créer `appsettings.Development.json` dans le projet MAUI :

```json
{
  "ApiBaseUrl": "https://localhost:7001",
  "SignalRHubUrl": "https://localhost:7001/hubs/convoy",
  "LocationUpdateIntervalSeconds": 5,
  "MinDistanceMetersForUpdate": 10
}
```

## Fonctionnalités MVP

Phase 1 (3-4 mois) :

- ✅ Authentification magic link + 2FA téléphone
- ✅ Création convoy avec premier trip automatique
- ✅ Rejoindre convoy (code alphanumérique ou lien)
- ✅ Suivi GPS temps réel (5s)
- ✅ Carte OpenStreetMap interactive
- ✅ Destination obligatoire par trip
- ✅ Points d'arrêt à la volée
- ✅ Chat de groupe basique
- ✅ Notifications push
- ✅ Mode offline avec cache SQLite
- ✅ Multi-trips (escales, voyages successifs)

Voir [DOCUMENTATION.md#roadmap](DOCUMENTATION.md#roadmap) pour la roadmap complète.

## Sécurité et confidentialité

SyncTrip prend la sécurité au sérieux :

### Authentification passwordless
- Magic link par email (token unique, 15 min)
- 2FA optionnelle par téléphone (SMS OTP)
- Pas de mot de passe à stocker
- JWT avec refresh token (30 jours)

### Protection des données
- Chiffrement HTTPS/TLS obligatoire
- Données GPS temporaires (supprimées après trip)
- Conformité RGPD : export et suppression complète
- Stockage sécurisé (iOS Keychain, Android Keystore)
- Rate limiting anti-abus

### Code alphanumérique
- 6 caractères : A-Z, a-z, 0-9
- 62^6 = 56,8 milliards de combinaisons possibles
- Nettoyage auto des convoys inactifs (libère les codes)

Voir [DOCUMENTATION.md#sécurité-et-rgpd](DOCUMENTATION.md#sécurité-et-rgpd)

## Documentation

- **[DOCUMENTATION.md](DOCUMENTATION.md)** - Documentation technique et fonctionnelle complète
  - Architecture système et N-tiers
  - Modèle de données (ERD)
  - Diagrammes de séquence (6 cas d'usage)
  - Sécurité, RGPD, bonnes pratiques
  - Roadmap détaillée

- **[IMPLEMENTATION_GUIDE.md](IMPLEMENTATION_GUIDE.md)** - Guide d'implémentation pratique
  - Configuration environnement
  - Création projet backend
  - Code des entités et services
  - Exemples concrets

## Technologies

| Catégorie | Technologie |
|-----------|-------------|
| **Frontend** | .NET MAUI 10, MVVM Toolkit, Mapsui |
| **Backend** | ASP.NET Core 10, SignalR, EF Core 10 |
| **Databases** | PostgreSQL 16, Redis 7, SQLite (local) |
| **Maps** | OpenStreetMap, Nominatim (geocoding) |
| **Auth** | Magic Link (email), OTP (SMS), JWT |
| **Notifications** | Firebase Cloud Messaging (FCM) |
| **Tests** | xUnit, Moq, FluentAssertions |

## Contribuer

Les contributions sont les bienvenues ! Consultez [CONTRIBUTING.md](CONTRIBUTING.md) pour :
- Guidelines de code
- Process de pull request
- Standards de tests
- Code of conduct

## Roadmap

- **Phase 1 - MVP** (En cours) : Fonctionnalités de base, multi-trips
- **Phase 2 - Amélioration** : Permissions avancées, UX, scalabilité 10+
- **Phase 3 - Avancé** : Photos, météo, IA, replay trajets
- **Phase 4 - Enterprise** : Gestion flottes, 50+ véhicules, API publique

Détails : [DOCUMENTATION.md#roadmap](DOCUMENTATION.md#roadmap)

## Licence

Ce projet est sous licence MIT. Voir [LICENSE](LICENSE) pour plus de détails.

## Support

- **Documentation** : [DOCUMENTATION.md](DOCUMENTATION.md)
- **Issues** : [GitHub Issues](https://github.com/synctrip/synctrip/issues)
- **Email** : dev@synctrip.app

## Remerciements

- [OpenStreetMap](https://www.openstreetmap.org/) - Données cartographiques libres
- [Mapsui](https://mapsui.com/) - Contrôle de carte .NET
- Communauté .NET MAUI

---

**Développé avec .NET 10 MAUI** 🚀
