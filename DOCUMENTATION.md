# SyncTrip - Documentation Technique et Fonctionnelle

## Table des matières

1. [Vue d'ensemble](#vue-densemble)
2. [Architecture Globale](#architecture-globale)
3. [Architecture Technique](#architecture-technique)
4. [Architecture Fonctionnelle](#architecture-fonctionnelle)
5. [Modèle de Données](#modèle-de-données)
6. [Flux et Cas d'Usage](#flux-et-cas-dusage)
7. [Sécurité et RGPD](#sécurité-et-rgpd)
8. [Technologies et Stack](#technologies-et-stack)
9. [Bonnes Pratiques](#bonnes-pratiques)
10. [Roadmap](#roadmap)

---

## Vue d'ensemble

### Concept

**SyncTrip** est une application mobile multiplateforme (Android/iOS) permettant à des groupes de voyageurs circulant dans différents véhicules de :
- Créer des groupes persistants (Convoys) avec des voyages multiples
- Se suivre en temps réel sur une carte
- Définir et partager des destinations et points d'arrêt
- Communiquer via chat de groupe
- Gérer des voyages multi-étapes (escales, voyages successifs)
- Conserver un historique détaillé des trajets

### Cas d'usage principaux

**Scénario 1 : Voyage familial simple**
- Famille se rendant en Pologne dans deux véhicules
- Crée un convoi "Famille Dupont - Pologne 2025"
- Un seul voyage : Paris → Varsovie
- Chacun navigue à son rythme en voyant les autres
- Ajout de points d'arrêt à la volée pour se retrouver

**Scénario 2 : Voyage avec escale**
- Même famille, trajet de 10h trop long
- Voyage 1 : Paris → Hôtel à mi-chemin (terminé le soir)
- Voyage 2 : Hôtel → Varsovie (démarré le lendemain)
- Même groupe, mêmes participants, deux voyages distincts

**Scénario 3 : Groupe réutilisable**
- Groupe d'amis motards "Potes Moto"
- Voyage 1 : Alpes (Été 2025) - Terminé
- Voyage 2 : Provence (Automne 2025) - Terminé
- Voyage 3 : Italie (Été 2026) - Planifié
- Même code de groupe, participants persistent

### Contraintes et Objectifs

- **Scalabilité** : De 2-3 véhicules (usage familial) à 10+ véhicules (groupes moto)
- **Flexibilité** : Groupes réutilisables, voyages multi-étapes
- **Connectivité** : Nécessite Internet mais avec cache offline robuste
- **Conformité** : Respect du RGPD avec données supprimables
- **Sécurité** : Authentification passwordless, données chiffrées
- **Code quality** : Architecture modulaire, maintenable, testable

---

## Architecture Globale

### Diagramme d'Architecture Système

```mermaid
graph TB
    subgraph "Frontend - MAUI Apps"
        A1[Android App<br/>.NET 10 MAUI]
        A2[iOS App<br/>.NET 10 MAUI]
    end

    subgraph "Backend Services"
        B1[API REST<br/>ASP.NET Core 10]
        B2[SignalR Hub<br/>Temps Réel]
        B3[Service Email<br/>Magic Links]
        B4[Service SMS<br/>OTP Téléphone]
    end

    subgraph "Données"
        C1[(PostgreSQL 16<br/>Base principale)]
        C2[(Redis 7<br/>Cache temps réel)]
        C3[(SQLite Local<br/>Cache offline)]
    end

    subgraph "Services Externes"
        D1[OpenStreetMap<br/>Cartes]
        D2[Nominatim<br/>Geocoding]
        D3[SendGrid/SES<br/>Emails]
        D4[Twilio<br/>SMS]
    end

    A1 --> B1
    A1 --> B2
    A2 --> B1
    A2 --> B2

    B1 --> C1
    B1 --> C2
    B2 --> C2
    B3 --> D3
    B4 --> D4

    A1 -.Cache local.-> C3
    A2 -.Cache local.-> C3

    A1 -.Tiles.-> D1
    A2 -.Tiles.-> D1
    B1 --> D2

    style A1 fill:#4CAF50
    style A2 fill:#4CAF50
    style B1 fill:#2196F3
    style B2 fill:#2196F3
    style C1 fill:#FF9800
    style C2 fill:#FF9800
```

### Architecture N-Tiers

```mermaid
graph LR
    subgraph "Présentation"
        P1[MAUI Views<br/>XAML]
        P2[Pages/Controls]
    end

    subgraph "Application"
        AP1[ViewModels<br/>MVVM]
        AP2[Services]
        AP3[State Management]
    end

    subgraph "Domaine"
        D1[Entities]
        D2[Business Logic]
        D3[Interfaces]
    end

    subgraph "Infrastructure"
        I1[API Client]
        I2[SignalR Client]
        I3[Local DB SQLite]
        I4[GPS Service]
    end

    P1 --> AP1
    P2 --> AP1
    AP1 --> AP2
    AP2 --> D2
    AP2 --> I1
    AP2 --> I2
    AP3 --> I3
    D2 --> D1
    I1 --> D3
    I4 --> AP2

    style P1 fill:#E8F5E9
    style AP1 fill:#E3F2FD
    style D1 fill:#FFF3E0
    style I1 fill:#FCE4EC
```

### Architecture Client-Serveur (pas de P2P)

**Choix architectural** : Serveur central plutôt que peer-to-peer

**Raisons** :
- Fiabilité : Synchronisation garantie même avec connexions intermittentes
- Simplicité : Pas de gestion de mesh network complexe
- Scalabilité : Supporte 10+ véhicules facilement
- Historique : Stockage centralisé des trajets
- Sécurité : Contrôle d'accès centralisé

---

## Architecture Technique

### Stack Technique Frontend (.NET MAUI)

#### Frameworks et Librairies

| Composant | Technologie | Version | Usage |
|-----------|-------------|---------|-------|
| **Framework** | .NET MAUI | 10.0 | Application multiplateforme |
| **Pattern** | MVVM | - | Séparation UI/Logique |
| **MVVM Toolkit** | CommunityToolkit.Mvvm | 8.x | Simplification MVVM |
| **Navigation** | Shell Navigation | Built-in | Navigation déclarative |
| **Cartes** | Mapsui | 5.x | Affichage cartes OpenStreetMap |
| **GPS** | Microsoft.Maui.Devices.Sensors | Built-in | Géolocalisation |
| **HTTP Client** | System.Net.Http | Built-in | API REST |
| **SignalR Client** | Microsoft.AspNetCore.SignalR.Client | 10.x | WebSocket temps réel |
| **Base locale** | SQLite-net-pcl | 1.9.x | Cache et offline |
| **Sérialisation** | System.Text.Json | Built-in | JSON |
| **DI Container** | Microsoft.Extensions.DependencyInjection | Built-in | Injection de dépendances |
| **Logging** | Microsoft.Extensions.Logging | Built-in | Logs structurés |

#### Structure du Projet Frontend

```
SyncTrip/
├── Platforms/              # Code spécifique par plateforme
│   ├── Android/
│   ├── iOS/
│   ├── Windows/
│   └── MacCatalyst/
├── Core/                   # Couche domaine
│   ├── Entities/          # Entités métier
│   │   ├── Convoy.cs
│   │   ├── Trip.cs
│   │   ├── ConvoyParticipant.cs
│   │   ├── Waypoint.cs
│   │   └── User.cs
│   ├── Enums/             # Énumérations
│   │   ├── ConvoyStatus.cs
│   │   ├── TripStatus.cs
│   │   ├── ParticipantRole.cs
│   │   ├── ParticipantPermissions.cs
│   │   └── WaypointType.cs
│   ├── Interfaces/        # Contrats de services
│   └── Constants/         # Constantes
├── Services/              # Couche infrastructure
│   ├── Api/               # Client API REST
│   ├── SignalR/           # Client SignalR
│   ├── Location/          # Service GPS
│   ├── Database/          # SQLite local
│   ├── Authentication/    # Gestion auth JWT
│   ├── Cache/             # Gestion cache
│   └── CodeGenerator/     # Génération codes convoi
├── ViewModels/            # ViewModels MVVM
│   ├── Base/              # BaseViewModel
│   ├── Auth/              # MagicLink, Verification
│   ├── Convoy/            # Gestion convois et trips
│   ├── Map/               # Carte et tracking
│   └── History/           # Historique
├── Views/                 # Pages XAML
│   ├── Auth/
│   ├── Convoy/
│   ├── Trip/
│   ├── Map/
│   └── History/
├── Controls/              # Composants réutilisables
├── Converters/            # Value Converters XAML
├── Behaviors/             # Behaviors XAML
├── Models/                # DTOs et ViewModels
│   ├── DTOs/              # Data Transfer Objects
│   ├── Requests/          # Requêtes API
│   └── Responses/         # Réponses API
├── Resources/             # Ressources
│   ├── Styles/            # Styles XAML
│   ├── Images/            # Images
│   └── Fonts/             # Polices
├── Helpers/               # Utilitaires
└── App.xaml               # Point d'entrée
```

### Stack Technique Backend (ASP.NET Core)

#### Frameworks et Librairies

| Composant | Technologie | Version | Usage |
|-----------|-------------|---------|-------|
| **Framework** | ASP.NET Core | 10.0 | API Web |
| **ORM** | Entity Framework Core | 10.x | Accès données |
| **Base de données** | PostgreSQL | 16.x | Stockage principal |
| **Cache** | Redis | 7.x | Cache distribué, positions temps réel |
| **Temps réel** | SignalR | 10.x | Communication bidirectionnelle |
| **Authentication** | JWT Bearer | 10.x | Authentification stateless |
| **Validation** | FluentValidation | 11.x | Validation des requêtes |
| **Mapping** | AutoMapper | 13.x | Mapping DTOs/Entities |
| **API Docs** | Swagger/OpenAPI | 6.x | Documentation API |
| **Logging** | Serilog | 4.x | Logs structurés |
| **Tests** | xUnit + Moq | - | Tests unitaires |
| **Email** | SendGrid / AWS SES | - | Magic links |
| **SMS** | Twilio | - | OTP téléphone |

#### Structure du Projet Backend

```
SyncTrip.Api/
├── Controllers/           # Contrôleurs API REST
│   ├── AuthController.cs
│   ├── ConvoysController.cs
│   ├── TripsController.cs
│   ├── WaypointsController.cs
│   ├── UsersController.cs
│   └── HistoryController.cs
├── Hubs/                  # SignalR Hubs
│   └── ConvoyHub.cs       # Hub temps réel
├── Core/                  # Couche domaine
│   ├── Entities/          # Entités EF Core
│   │   ├── Convoy.cs
│   │   ├── Trip.cs
│   │   ├── ConvoyParticipant.cs
│   │   ├── Waypoint.cs
│   │   ├── LocationHistory.cs
│   │   ├── User.cs
│   │   └── MagicLinkToken.cs
│   ├── Enums/
│   ├── Interfaces/        # Repository pattern
│   └── Specifications/    # Spec pattern
├── Application/           # Couche application
│   ├── Services/          # Services métier
│   │   ├── ConvoyService.cs
│   │   ├── TripService.cs
│   │   ├── AuthService.cs
│   │   ├── EmailService.cs
│   │   └── SmsService.cs
│   ├── DTOs/              # Data Transfer Objects
│   ├── Validators/        # FluentValidation
│   └── Mappings/          # AutoMapper profiles
├── Infrastructure/        # Couche infrastructure
│   ├── Data/              # DbContext, Repositories
│   ├── Cache/             # Redis service
│   ├── External/          # Services externes (Geocoding)
│   └── CodeGenerator/     # Génération codes convoi
├── Middleware/            # Middlewares custom
│   ├── ErrorHandlingMiddleware.cs
│   └── RequestLoggingMiddleware.cs
├── Configuration/         # Extensions et config
└── Program.cs             # Point d'entrée
```

---

## Architecture Fonctionnelle

### Concepts Clés

**Convoy (Groupe persistant)**
- Groupe de personnes qui voyagent ensemble
- Code permanent alphanumérique (6 caractères)
- Peut contenir plusieurs voyages (trips)
- Les participants persistent entre les voyages

**Trip (Instance de voyage)**
- Un trajet spécifique avec une destination obligatoire
- Lié à un convoy
- Un seul trip actif par convoy à la fois
- Contient les waypoints et l'historique de positions

**Waypoint (Point d'intérêt)**
- Destination (obligatoire pour chaque trip)
- Points d'arrêt planifiés ou à la volée
- Alertes routières

### Modules Fonctionnels

```mermaid
graph TB
    subgraph "Module Authentification"
        F1[Magic Link Email]
        F2[Vérification Téléphone OTP]
        F3[Gestion Profil]
    end

    subgraph "Module Convoy"
        F4[Créer Convoy + Premier Trip]
        F5[Rejoindre Convoy]
        F6[Gérer Participants]
        F7[Gérer Permissions]
    end

    subgraph "Module Trip"
        F8[Créer Nouveau Trip]
        F9[Démarrer Trip]
        F10[Terminer Trip]
        F11[Annuler Trip]
    end

    subgraph "Module Tracking"
        F12[Partage Position GPS]
        F13[Affichage Carte]
        F14[Visualisation Véhicules]
    end

    subgraph "Module Points d'Intérêt"
        F15[Définir Destination]
        F16[Ajouter Point d'Arrêt]
        F17[Modifier/Supprimer POI]
    end

    subgraph "Module Communication"
        F18[Chat de Groupe]
        F19[Messages Rapides]
        F20[Notifications Push]
    end

    subgraph "Module Historique"
        F21[Consulter Trips Passés]
        F22[Statistiques]
        F23[Export Données]
        F24[Suppression RGPD]
    end

    F1 --> F4
    F2 --> F4
    F4 --> F8
    F8 --> F12
    F12 --> F13
    F13 --> F14
    F15 --> F13
    F16 --> F13
    F10 --> F21

    style F1 fill:#BBDEFB
    style F4 fill:#C8E6C9
    style F8 fill:#FFE082
    style F12 fill:#FFCCBC
    style F18 fill:#F8BBD0
    style F21 fill:#E1BEE7
```

### Fonctionnalités Détaillées

#### 1. Authentification Passwordless

**Magic Link Email**
- Utilisateur entre son email
- Backend envoie un lien unique avec token
- Utilisateur clique sur le lien (ouvre l'app)
- Token validé → JWT access token généré
- Expiration : 15 minutes

**Vérification Téléphone (2FA au premier login)**
- Après magic link validé
- Utilisateur entre son numéro de téléphone
- Code OTP envoyé par SMS
- Validation du code → profil activé

**Profil**
- Nom, prénom, photo
- Email (vérifié)
- Téléphone (optionnel mais recommandé)
- Suppression de compte

#### 2. Gestion des Convoys

**Création de Convoy**
- Nom du convoy (ex: "Famille Dupont")
- Génération automatique d'un code alphanumérique (6 caractères : A-Z, a-z, 0-9)
  - 62^6 = 56,8 milliards de possibilités
- Génération d'un lien d'invitation
- **Création automatique du premier trip** avec destination obligatoire
- Le créateur devient "Leader" du convoy

**Rejoindre un Convoy**
- Via code PIN alphanumérique (ex: "aB3xK9")
- Via lien d'invitation
- Rejoint automatiquement le trip actif

**Participants**
- Liste des participants avec statuts
- Rôles : Leader, Member
- Permissions configurables par participant
- Exclusion (kick) ou ban d'un participant (leader uniquement)
- Promotion/rétrogradation de rôle

**États du Convoy**
- Active : convoy actif, peut créer des trips
- Archived : convoy archivé (read-only)
- Auto-archivage si aucun participant pendant 30 jours

**Nettoyage automatique**
- Convoys sans participants depuis 30 jours : archivés
- Convoys archivés depuis 1 an : supprimés (libère les codes)

#### 3. Gestion des Trips (Voyages)

**Création de Trip**
- **Premier trip** : créé automatiquement lors de la création du convoy
- **Trips suivants** : créés manuellement par le leader dans un convoy existant
- Nom du trip (ex: "Pologne Jour 2")
- **Destination obligatoire** (recherche d'adresse ou sélection sur carte)
- Préférence de route :
  - Rapide (autoroutes)
  - Sympa (routes pittoresques)
  - Économique (éviter péages)
  - Courte (distance minimale)
- Date de départ prévue (optionnel)

**Contrainte importante** : Un seul trip actif par convoy à la fois

**États du Trip**
- Planned : pas encore démarré
- Active : en cours (tracking GPS actif)
- Paused : en pause (escale hôtel par exemple)
- Finished : terminé avec succès
- Cancelled : annulé

**Opérations**
- Démarrer un trip : passage de Planned → Active
- Mettre en pause : Active → Paused (tracking continue mais marqué en pause)
- Reprendre : Paused → Active
- Terminer : Active → Finished (calcul statistiques automatique)
- Annuler : Any → Cancelled

#### 4. Tracking GPS et Carte

**Suivi en Temps Réel**
- Position GPS mise à jour toutes les 5-10 secondes (configurable)
- Optimisation : envoi uniquement si déplacement > 10 mètres
- Affichage de tous les véhicules du trip actif
- Affichage de la direction et vitesse
- Calcul automatique de la distance entre véhicules
- ETA (Estimated Time of Arrival) pour chaque véhicule

**Carte Interactive**
- Basée sur OpenStreetMap (Mapsui)
- Centrage automatique sur le groupe
- Zoom et déplacement manuel
- Marqueurs personnalisables par véhicule (couleur, icône)
- Trajet effectué (polyligne)
- Distance restante jusqu'à destination

**Mode Offline**
- Cache des tuiles de carte
- Dernières positions connues affichées
- Positions enregistrées localement (SQLite)
- Synchronisation automatique au retour de la connexion

#### 5. Points d'Intérêt (POI)

**Types de Waypoints**
- **Destination** : 1 obligatoire par trip (ne peut pas être supprimée)
- **PlannedStop** : arrêts planifiés à l'avance
- **QuickStop** : arrêts ajoutés à la volée pendant le trajet
- **Alert** : alertes (danger, travaux, etc.)

**Création de Waypoint**
- Recherche par adresse (geocoding Nominatim)
- Sélection sur la carte (appui long)
- Position actuelle du véhicule
- Catégories : Essence, Restauration, Repos, Autre, Danger

**Gestion Waypoints**
- Modification (créateur ou leader, selon permissions)
- Suppression (sauf destination)
- Marquer comme "atteint"
- Notification push à tous les participants
- Ordre des waypoints

**Permissions**
- Par défaut : tous les membres peuvent ajouter des waypoints
- Configurable par le leader (peut restreindre)

#### 6. Communication

**Chat de Groupe**
- Une seule discussion par convoy (visible dans tous les trips)
- Messages texte simples
- Historique limité (derniers 100 messages ou 7 jours)
- Pas de messages privés (utiliser WhatsApp/SMS pour ça)

**Messages Rapides Prédéfinis**
- "Je fais une pause"
- "J'ai un problème technique"
- "Je dois faire le plein"
- "Je suis arrivé"
- "Ralentissez, je suis loin"
- Boutons rapides dans l'interface

**Notifications Push**
- Nouveau participant dans le convoy
- Nouveau trip créé
- Nouveau waypoint ajouté
- Modification de destination (leader uniquement)
- Message du leader (marqué spécial)
- Participant arrivé à destination
- SOS / Problème technique

**Statuts Véhicule**
- En route (vert)
- En pause (orange)
- Problème technique (rouge)
- Arrivé (bleu)
- Hors ligne (gris)

#### 7. Permissions et Rôles

**Rôles**
- **Leader** : tous les droits sur le convoy et les trips
- **Member** : permissions configurables

**Permissions (flags combinables)**
- `CanAddWaypoints` : peut ajouter des points d'arrêt
- `CanSendMessages` : peut envoyer des messages au chat
- `CanModifyRoute` : peut suggérer modifications de route
- `CanSeeAllPositions` : peut voir positions de tous (sinon juste les leaders)

**Permissions par défaut** pour un nouveau membre :
- ✅ Ajouter waypoints
- ✅ Envoyer messages
- ✅ Voir toutes les positions
- ❌ Modifier route

**Actions du Leader**
- Promouvoir un membre en Leader (co-leader)
- Rétrograder un leader en membre
- Modifier permissions individuelles
- Kick (retirer du convoy)
- Ban (bloquer définitivement)
- Modifier destination du trip actif
- Créer/terminer des trips

#### 8. Historique et Statistiques

**Trips Passés**
- Liste de tous les trips d'un convoy
- Filtres : Terminés, Annulés, Tous
- Détails : date, participants, trajet, durée
- Visualisation du trajet sur carte (GeoJSON)
- Points d'arrêt effectués

**Statistiques par Trip**
- Distance totale parcourue (km)
- Temps de trajet (HH:MM)
- Vitesse moyenne (km/h)
- Nombre d'arrêts
- Temps de pause total
- Durée effective de conduite

**Statistiques Agrégées (Convoy)**
- Nombre total de trips
- Distance totale tous trips confondus
- Temps total de voyage
- Nombre de participants différents
- Carte de chaleur des zones visitées

**Export et RGPD**
- Export complet des données personnelles (JSON)
- Export d'un trip spécifique (GPX, KML, JSON)
- Suppression de trips spécifiques
- Suppression complète du compte
- Conservation configurable (30, 90, 365 jours, illimité)

---

## Modèle de Données

### Diagramme Entité-Relation

```mermaid
erDiagram
    User ||--o{ ConvoyParticipant : "participe"
    User ||--o{ Convoy : "crée"
    User ||--o{ Waypoint : "crée"
    User ||--o{ LocationHistory : "enregistre"
    User ||--o{ Message : "envoie"
    User ||--o{ MagicLinkToken : "possède"

    Convoy ||--o{ ConvoyParticipant : "contient"
    Convoy ||--o{ Trip : "possède"
    Convoy ||--o{ Message : "contient"

    Trip ||--o{ Waypoint : "possède"
    Trip ||--o{ LocationHistory : "trace"
    Trip ||--|| Waypoint : "a destination"

    User {
        uuid Id PK
        string Email UK
        string FirstName
        string LastName
        string PhoneNumber
        bool PhoneVerified
        string AvatarUrl
        datetime CreatedAt
        datetime LastLoginAt
        bool IsActive
    }

    Convoy {
        uuid Id PK
        string Name
        string Code UK "6 chars alphanum"
        string InviteLink
        uuid CreatorId FK
        string Status "Active|Archived"
        datetime CreatedAt
        datetime ArchivedAt
        int MaxParticipants
        int DefaultMemberPermissions
    }

    ConvoyParticipant {
        uuid Id PK
        uuid ConvoyId FK
        uuid UserId FK
        string Role "Leader|Member"
        int Permissions "Flags"
        string VehicleType "Car|Motorcycle|Truck|Van"
        string VehicleName
        string Color
        string Status "Active|Paused|Problem|Arrived|Offline"
        datetime JoinedAt
        datetime LeftAt
        bool IsBanned
    }

    Trip {
        uuid Id PK
        uuid ConvoyId FK
        string Name
        uuid DestinationWaypointId FK
        string RoutePreference "Fastest|Scenic|Economical|Shortest"
        string Status "Planned|Active|Paused|Finished|Cancelled"
        datetime CreatedAt
        datetime PlannedDepartureDate
        datetime StartedAt
        datetime FinishedAt
        double TotalDistanceKm
        double AverageSpeedKmh
        int StopCount
        int TotalPauseMinutes
        jsonb RouteGeoJson
    }

    Waypoint {
        uuid Id PK
        uuid TripId FK
        uuid CreatedById FK
        string Type "Destination|PlannedStop|QuickStop|Alert"
        string Category "Fuel|Restaurant|Rest|Other|Danger"
        string Name
        string Description
        double Latitude
        double Longitude
        string Address
        datetime CreatedAt
        datetime ScheduledAt
        bool IsReached
        int Order "0 pour destination"
    }

    LocationHistory {
        uuid Id PK
        uuid TripId FK
        uuid UserId FK
        double Latitude
        double Longitude
        double Accuracy
        double Speed
        double Heading
        datetime Timestamp
        bool IsSentToServer
    }

    Message {
        uuid Id PK
        uuid ConvoyId FK
        uuid SenderId FK
        string Type "Text|QuickMessage|System"
        string Content
        jsonb Metadata
        datetime SentAt
    }

    MagicLinkToken {
        uuid Id PK
        uuid UserId FK
        string Token UK
        datetime ExpiresAt
        bool IsUsed
        datetime CreatedAt
    }
```

### Schémas JSON

#### DTO: CreateConvoyRequest

```json
{
  "convoyName": "Famille Dupont",
  "firstTripName": "Pologne 2025",
  "destinationName": "Varsovie, Pologne",
  "destinationLatitude": 52.2297,
  "destinationLongitude": 21.0122,
  "routePreference": "Fastest",
  "plannedDepartureDate": "2025-11-20T08:00:00Z",
  "maxParticipants": 10
}
```

#### DTO: ConvoyDetailResponse

```json
{
  "id": "uuid",
  "name": "Famille Dupont",
  "code": "aB3xK9",
  "inviteLink": "https://synctrip.app/join/xyz789",
  "status": "Active",
  "createdAt": "2025-01-15T08:00:00Z",
  "creator": {
    "id": "uuid",
    "firstName": "Jean",
    "lastName": "Dupont",
    "avatarUrl": "https://..."
  },
  "participants": [
    {
      "id": "uuid",
      "userId": "uuid",
      "firstName": "Jean",
      "lastName": "Dupont",
      "role": "Leader",
      "permissions": ["CanAddWaypoints", "CanSendMessages", "CanModifyRoute", "CanSeeAllPositions"],
      "vehicleType": "Car",
      "vehicleName": "Peugeot 3008",
      "color": "#FF5722",
      "status": "Active",
      "currentLocation": {
        "latitude": 48.8566,
        "longitude": 2.3522,
        "speed": 110.5,
        "heading": 45.2,
        "timestamp": "2025-01-15T10:30:00Z"
      }
    }
  ],
  "activeTrip": {
    "id": "uuid",
    "name": "Pologne 2025",
    "status": "Active",
    "routePreference": "Fastest",
    "destination": {
      "id": "uuid",
      "name": "Varsovie, Pologne",
      "latitude": 52.2297,
      "longitude": 21.0122,
      "address": "Warsaw, Poland"
    },
    "waypoints": [
      {
        "id": "uuid",
        "type": "QuickStop",
        "category": "Restaurant",
        "name": "Aire de Service ABC",
        "latitude": 49.1234,
        "longitude": 3.5678,
        "order": 1,
        "isReached": false
      }
    ],
    "statistics": {
      "totalDistance": 245.8,
      "averageSpeed": 105.3,
      "elapsedTime": 3600
    }
  },
  "pastTrips": [
    {
      "id": "uuid",
      "name": "Alpes 2024",
      "status": "Finished",
      "finishedAt": "2024-08-15T18:00:00Z"
    }
  ]
}
```

#### SignalR Message: LocationUpdate

```json
{
  "type": "LocationUpdate",
  "tripId": "uuid",
  "userId": "uuid",
  "location": {
    "latitude": 48.8566,
    "longitude": 2.3522,
    "accuracy": 10.5,
    "speed": 110.5,
    "heading": 45.2,
    "timestamp": "2025-01-15T10:30:00Z"
  }
}
```

#### SignalR Message: WaypointAdded

```json
{
  "type": "WaypointAdded",
  "tripId": "uuid",
  "waypoint": {
    "id": "uuid",
    "type": "QuickStop",
    "category": "Restaurant",
    "name": "Aire de Service ABC",
    "latitude": 49.1234,
    "longitude": 3.5678,
    "createdBy": {
      "id": "uuid",
      "firstName": "Jean"
    },
    "createdAt": "2025-01-15T10:25:00Z"
  }
}
```

---

## Flux et Cas d'Usage

### Cas d'Usage 1 : Authentification Passwordless

```mermaid
sequenceDiagram
    participant U as User
    participant A as App MAUI
    participant API as API Backend
    participant Email as Email Service
    participant SMS as SMS Service

    U->>A: Entre son email
    A->>API: POST /api/auth/request-magic-link
    API->>API: Générer token unique
    API->>Email: Envoyer magic link
    Email->>U: Email avec lien

    U->>U: Clique sur lien (ouvre app)
    A->>API: POST /api/auth/verify-magic-link
    API->>API: Valider token, expiration

    alt Premier login
        API-->>A: {needsProfile: true, tempToken}
        A->>U: Formulaire profil
        U->>A: Nom, Prénom, Téléphone
        A->>API: POST /api/auth/complete-profile
        API->>SMS: Envoyer OTP
        SMS->>U: SMS avec code
        U->>A: Entre code OTP
        A->>API: POST /api/auth/verify-otp
        API->>API: Activer compte
        API-->>A: {accessToken, refreshToken}
    else Déjà inscrit
        API-->>A: {accessToken, refreshToken}
    end

    A->>A: Stocker tokens (Secure Storage)
    A-->>U: Connecté, redirection accueil
```

### Cas d'Usage 2 : Créer Convoy et Premier Trip

```mermaid
sequenceDiagram
    participant U as User (Leader)
    participant A as App MAUI
    participant API as API Backend
    participant DB as Database

    U->>A: "Créer nouveau convoy"
    A->>U: Formulaire

    Note over U: Remplit formulaire
    U->>A: Nom convoy: "Famille Dupont"<br/>Nom trip: "Pologne 2025"<br/>Destination: "Varsovie"<br/>Route: Rapide

    A->>API: POST /api/convoys

    API->>DB: BEGIN TRANSACTION

    Note over API: Génération code
    API->>API: Générer code alphanum (6 chars)
    API->>DB: Vérifier unicité code

    Note over API: Créer entités
    API->>DB: INSERT Convoy (code="aB3xK9")
    API->>DB: INSERT Waypoint (type=Destination)
    API->>DB: INSERT Trip (destinationId=waypoint.id)
    API->>DB: INSERT ConvoyParticipant (role=Leader)

    API->>DB: COMMIT

    API-->>A: {convoy, trip, code, inviteLink}
    A-->>U: "Convoy créé! Code: aB3xK9"<br/>Afficher lien partage
```

### Cas d'Usage 3 : Voyage avec Escale (Multi-Trip)

```mermaid
sequenceDiagram
    participant U as Leader
    participant A as App
    participant API as API
    participant Hub as SignalR Hub

    Note over U,A: Soir, arrivé à l'hôtel (500km parcourus)

    U->>A: "Terminer ce voyage"
    A->>API: PUT /api/trips/{tripId}/finish
    API->>API: Set status = Finished
    API->>API: Calculer statistiques
    API->>API: Générer GeoJSON trajet
    API-->>A: Trip 1 terminé

    Hub-->>A: TripFinished notification
    A-->>U: "Voyage terminé! 500km en 6h"

    Note over U,A: Lendemain matin, petit-déjeuner

    U->>A: "Nouveau voyage"
    A->>U: Formulaire nouveau trip

    U->>A: Nom: "Pologne Jour 2"<br/>Destination: "Varsovie"<br/>Route: Sympa (routes pittoresques)

    A->>API: POST /api/convoys/{convoyId}/trips

    API->>API: Vérifier qu'aucun trip actif
    API->>API: CREATE Waypoint (destination)
    API->>API: CREATE Trip (status=Planned)
    API-->>A: {newTrip}

    Hub-->>A: NewTripCreated (tous participants)
    A-->>U: "Nouveau voyage créé!"

    U->>A: "Démarrer"
    A->>API: PUT /api/trips/{tripId}/start
    API->>API: Set status = Active
    API-->>A: Trip démarré

    Hub-->>A: TripStarted
    A->>A: Activer tracking GPS
    A-->>U: Carte avec nouveau trajet
```

### Cas d'Usage 4 : Suivi GPS en Temps Réel

```mermaid
sequenceDiagram
    participant GPS1 as GPS Service 1
    participant A1 as App MAUI 1
    participant Cache as SQLite Local
    participant Hub as SignalR Hub
    participant Redis as Redis Cache
    participant A2 as App MAUI 2
    participant Map2 as Carte 2

    loop Toutes les 5 secondes
        GPS1->>A1: Position update
        A1->>A1: Vérifier déplacement > 10m

        alt Déplacement significatif
            A1->>Cache: Save location

            alt Internet disponible
                A1->>Hub: SendLocationUpdate(tripId)
                Hub->>Redis: Store latest position (TTL 5min)
                Hub-->>A2: LocationUpdate event
                A2->>Map2: Mettre à jour marqueur
            else Pas d'Internet
                A1->>Cache: Mark as not sent
                Note over A1: Icône "Mode offline"
            end
        end
    end

    Note over A1,A2: Connexion restaurée
    A1->>Cache: Get unsent locations
    A1->>Hub: Batch send locations
    Hub->>Redis: Update positions
    Hub-->>A2: LocationUpdateBatch event
    A2->>Map2: Animer trajet
```

### Cas d'Usage 5 : Ajouter Point d'Arrêt à la Volée

```mermaid
sequenceDiagram
    participant U1 as User 1 (Leader)
    participant A1 as App MAUI 1
    participant Map as Carte
    participant API as API Backend
    participant DB as Database
    participant Hub as SignalR Hub
    participant A2 as App MAUI 2
    participant U2 as User 2

    U1->>Map: Appui long sur carte
    Map->>A1: Coordonnées sélectionnées
    A1->>U1: Menu contextuel<br/>"Ajouter point d'arrêt"

    U1->>A1: Sélectionne catégorie (Restaurant)
    U1->>A1: Entre nom "Aire ABC"<br/>Description "Café dispo"

    A1->>API: POST /api/trips/{tripId}/waypoints
    API->>DB: INSERT Waypoint (type=QuickStop)
    API-->>A1: Waypoint créé

    API->>Hub: BroadcastWaypointAdded(tripId)
    Hub-->>A1: WaypointAdded event
    Hub-->>A2: WaypointAdded event

    A1->>Map: Afficher POI sur carte
    A2->>Map: Afficher POI sur carte

    Hub->>A2: Push notification
    A2-->>U2: 🔔 "Jean propose arrêt restaurant dans 2km"
```

### Cas d'Usage 6 : Gestion Permissions

```mermaid
sequenceDiagram
    participant L as Leader
    participant A as App
    participant API as API

    L->>A: Voir participants
    A->>A: Afficher liste avec permissions

    L->>A: Sélectionne "Marie"
    A->>L: Permissions actuelles:<br/>✅ Ajouter waypoints<br/>✅ Envoyer messages<br/>❌ Modifier route

    L->>A: Activer "Modifier route"
    A->>API: PUT /api/convoys/{id}/participants/{userId}/permissions
    API->>API: Update permissions flags
    API-->>A: Permissions mises à jour

    A-->>L: "Permissions de Marie modifiées"
```

---

## Sécurité et RGPD

### Authentification Passwordless

#### Magic Link Flow

**Avantages**
- Pas de mot de passe à retenir/oublier
- Pas de hash à stocker
- Plus sécurisé (token à usage unique)
- UX moderne et simple

**Implémentation**

```csharp
public class MagicLinkToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    [Required]
    public string Token { get; set; } // GUID ou random string

    public DateTime ExpiresAt { get; set; } // 15 minutes
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; }
}
```

**Sécurité**
- Token : GUID aléatoire cryptographiquement sûr
- Expiration : 15 minutes
- Usage unique (marqué `IsUsed` après validation)
- Rate limiting : max 3 demandes par email par heure
- HTTPS obligatoire

#### 2FA Téléphone (optionnel mais recommandé)

**Flow**
1. Premier login : magic link validé
2. Demande numéro de téléphone
3. Envoi OTP (6 chiffres) par SMS
4. Validation OTP → profil activé

**Après activation**
- 2FA optionnelle (paramètres)
- Réactivation possible à tout moment

### JWT Tokens

**Structure**

```json
// Access Token (durée: 1 heure)
{
  "sub": "user-id",
  "email": "user@example.com",
  "name": "Jean Dupont",
  "exp": 1705324800,
  "iat": 1705323900
}

// Refresh Token (durée: 30 jours)
{
  "sub": "user-id",
  "type": "refresh",
  "exp": 1707916800
}
```

**Stockage**
- Frontend : Secure Storage MAUI (chiffré)
- Backend : Redis avec TTL

### Protection des Données (RGPD)

#### Principes Appliqués

1. **Minimisation des données**
   - Email, nom, prénom, téléphone (optionnel)
   - Position GPS uniquement durant trips actifs
   - Historique limité dans le temps

2. **Droit à l'oubli**
   - Suppression de compte
   - Suppression de trips spécifiques
   - Anonymisation des données après suppression
   - Export complet avant suppression

3. **Portabilité**
   - Export JSON complet
   - Export GPX/KML par trip
   - API standardisée

4. **Consentement**
   - Acceptation CGU explicite
   - Opt-in notifications
   - Opt-in conservation historique

5. **Sécurité**
   - HTTPS/TLS obligatoire
   - Tokens sécurisés (JWT)
   - Chiffrement données sensibles
   - Rate limiting

#### Données Collectées

| Donnée | Finalité | Durée Conservation | Supprimable |
|--------|----------|-------------------|-------------|
| Email | Identification, magic links | Tant que compte actif | Oui |
| Nom, prénom | Affichage profil | Tant que compte actif | Oui |
| Téléphone | 2FA, notifications | Tant que compte actif | Oui |
| Position GPS temps réel | Suivi convoy | Durée du trip | Auto-supprimé |
| Historique positions | Statistiques, replay | Configurable (30-365j) | Oui |
| Messages chat | Communication | 7 jours ou fin trip | Auto-supprimé |
| Statistiques trips | Historique | Configurable | Oui |

#### API RGPD

```http
# Export complet des données
GET /api/users/me/export
Response: JSON avec toutes les données

# Suppression d'un trip
DELETE /api/trips/{tripId}

# Suppression du compte
DELETE /api/users/me
```

### Sécurité Technique

#### Backend

- **Validation** : FluentValidation sur toutes les requêtes
- **Rate limiting** : 100 req/min par IP, 50 req/min par user
- **CORS** : Origines autorisées configurées
- **Headers** : HSTS, X-Content-Type-Options, X-Frame-Options
- **Secrets** : Variables d'environnement, Azure Key Vault
- **Logs** : Pas de données sensibles (email/position masqués)

#### Frontend

- **Secure Storage** : Chiffrement natif (iOS Keychain, Android Keystore)
- **Certificate Pinning** : Validation certificat serveur
- **Code Obfuscation** : Dotfuscator en production
- **Validation client** : Pré-validation avant envoi API

---

## Technologies et Stack

### Décisions Architecturales

#### Pourquoi .NET 10 MAUI ?

**Avantages**
- Multiplateforme : un code pour Android/iOS/Windows
- Performance native (compilation AOT)
- Accès direct aux APIs plateforme
- Écosystème .NET riche
- Hot Reload ultra rapide
- Support Microsoft long terme

#### Pourquoi OpenStreetMap (Mapsui) ?

**Avantages**
- **Gratuit** et open source
- **Pas de limites** d'utilisation
- Données mondiales complètes
- Personnalisable à 100%
- Pas de dépendance Google/Apple
- Tiles cachables en offline

**Alternatives considérées**
- Google Maps : Payant, quotas, vendor lock-in
- Apple Maps : iOS uniquement
- Mapbox : Payant après quota

#### Pourquoi PostgreSQL ?

**Avantages**
- Open source et gratuit
- Support JSON/JSONB natif
- PostGIS pour données géospatiales
- Performance éprouvée
- Transactions ACID
- Scaling vertical et horizontal

#### Pourquoi SignalR ?

**Avantages**
- Intégration native .NET 10
- Gestion auto reconnexions
- Fallback (WebSocket → SSE → Long Polling)
- Groupes pour trips
- Typage fort C#
- Scale-out avec Redis backplane

#### Pourquoi Redis ?

**Avantages**
- Ultra rapide (in-memory)
- Expiration auto (TTL)
- Pub/Sub pour SignalR
- Structures de données riches
- Scaling horizontal

**Usage**
- Positions GPS récentes (TTL 5min)
- Cache queries fréquentes
- Rate limiting
- SignalR backplane (scale-out)

### Configuration Environnements

#### Développement

```yaml
Frontend:
  - .NET 10 SDK
  - Émulateur Android 14+
  - Simulateur iOS 17+
  - API locale : https://localhost:7001

Backend:
  - .NET 10 SDK
  - PostgreSQL 16 (Docker)
  - Redis 7 (Docker)
  - API ASP.NET Core 10

Outils:
  - Visual Studio 2022 17.12+ / Rider 2024.3+
  - Docker Desktop
  - Postman / Insomnia
  - pgAdmin 4
```

#### Production

```yaml
Frontend:
  - Build Release AOT
  - Obfuscation activée
  - API : https://api.synctrip.app

Backend:
  - Azure App Service / AWS ECS
  - PostgreSQL managed
  - Redis managed
  - HTTPS + HSTS
  - Rate limiting actif
  - Monitoring Application Insights

CI/CD:
  - GitHub Actions
  - Tests auto (>80% coverage)
  - Déploiement automatique
```

---

## Bonnes Pratiques

### Génération Code Convoy

```csharp
public class ConvoyCodeGenerator
{
    private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int CODE_LENGTH = 6;

    public static string GenerateCode()
    {
        var random = new Random();
        return new string(Enumerable.Range(0, CODE_LENGTH)
            .Select(_ => CHARS[random.Next(CHARS.Length)])
            .ToArray());
    }

    // Vérification unicité
    public async Task<string> GenerateUniqueCodeAsync()
    {
        string code;
        do
        {
            code = GenerateCode();
        } while (await _context.Convoys.AnyAsync(c => c.Code == code));

        return code;
    }
}

// Exemples de codes générés : aB3xK9, Zp7mQ2, kL9nR4
// Probabilité collision : 1 / 56,800,235,584 ≈ 0.0000000176%
```

### Architecture MVVM

```csharp
public partial class ConvoyMapViewModel : BaseViewModel
{
    private readonly IConvoyService _convoyService;
    private readonly ILocationService _locationService;
    private readonly ISignalRService _signalRService;

    [ObservableProperty]
    private Convoy _currentConvoy;

    [ObservableProperty]
    private Trip _activeTrip;

    [ObservableProperty]
    private ObservableCollection<ParticipantPin> _participantPins;

    [RelayCommand]
    private async Task StartTrackingAsync()
    {
        if (ActiveTrip?.Status != TripStatus.Active) return;

        await _locationService.StartTrackingAsync(async (location) =>
        {
            await _signalRService.SendLocationUpdateAsync(new LocationUpdateDto
            {
                TripId = ActiveTrip.Id,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Speed = location.Speed ?? 0,
                Heading = location.Course ?? 0
            });
        });
    }
}
```

### Repository Pattern

```csharp
public interface IConvoyRepository : IRepository<Convoy>
{
    Task<Convoy> GetByCodeAsync(string code);
    Task<Convoy> GetWithActiveTrip Async(Guid convoyId);
    Task<IEnumerable<Convoy>> GetUserConvoysAsync(Guid userId, bool includeArchived = false);
}

public class ConvoyRepository : Repository<Convoy>, IConvoyRepository
{
    public async Task<Convoy> GetWithActiveTripAsync(Guid convoyId)
    {
        return await _context.Convoys
            .Include(c => c.Participants).ThenInclude(p => p.User)
            .Include(c => c.Trips.Where(t => t.Status == TripStatus.Active))
                .ThenInclude(t => t.Waypoints)
            .FirstOrDefaultAsync(c => c.Id == convoyId);
    }
}
```

### Nettoyage Automatique (Background Service)

```csharp
public class ConvoyCleanupService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Archiver convoys sans participants depuis 30 jours
            var emptyConvoys = await _context.Convoys
                .Where(c => c.Status == ConvoyStatus.Active)
                .Where(c => !c.Participants.Any())
                .Where(c => c.CreatedAt < DateTime.UtcNow.AddDays(-30))
                .ToListAsync();

            foreach (var convoy in emptyConvoys)
            {
                convoy.Status = ConvoyStatus.Archived;
                convoy.ArchivedAt = DateTime.UtcNow;
            }

            // Supprimer convoys archivés depuis 1 an
            var oldArchivedConvoys = await _context.Convoys
                .Where(c => c.Status == ConvoyStatus.Archived)
                .Where(c => c.ArchivedAt < DateTime.UtcNow.AddYears(-1))
                .ToListAsync();

            _context.Convoys.RemoveRange(oldArchivedConvoys);

            await _context.SaveChangesAsync();

            // Exécuter toutes les 24h
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
```

---

## Roadmap

### Phase 1 : MVP (3-4 mois)

**Objectifs**
- Application fonctionnelle pour cas de base
- 2-5 véhicules
- Fonctionnalités essentielles

**Features**
- ✅ Authentification magic link + 2FA téléphone
- ✅ Création convoy avec premier trip
- ✅ Rejoindre convoy (code/lien)
- ✅ Suivi GPS temps réel
- ✅ Carte OpenStreetMap avec participants
- ✅ Ajout destination obligatoire
- ✅ Ajout points d'arrêt
- ✅ Chat de groupe basique
- ✅ Notifications push
- ✅ Mode offline (cache local)
- ✅ Terminer trip et créer nouveau trip

**Technique**
- Backend ASP.NET Core 10
- Frontend MAUI .NET 10 (Android + iOS)
- PostgreSQL + Redis
- SignalR temps réel
- CI/CD basique

### Phase 2 : Amélioration (2-3 mois)

**Objectifs**
- Scalabilité 10+ véhicules
- UX améliorée
- Permissions avancées

**Features**
- ✅ Système permissions granulaires
- ✅ Gestion rôles (Leader/Member)
- ✅ Messages rapides prédéfinis
- ✅ Statuts véhicule
- ✅ ETA et distances calculées
- ✅ Historique complet avec statistiques
- ✅ Export GPX/KML
- ✅ Mode pause trip
- ✅ Réutilisation groupes

**Technique**
- Optimisation GPS (batching)
- Tests E2E
- Monitoring avancé
- Performance tuning

### Phase 3 : Avancé (3-4 mois)

**Objectifs**
- Différenciation produit
- Engagement utilisateur

**Features**
- ✅ Partage photos aux waypoints
- ✅ Alertes routières collaboratives
- ✅ Intégration météo
- ✅ Suggestions POI automatiques (IA)
- ✅ Modes convoy (Route, Moto, 4x4)
- ✅ Planification trajet multi-étapes
- ✅ Replay animé des trajets
- ✅ Statistiques agrégées convoy

**Technique**
- ML.NET pour suggestions
- APIs tierces (météo, traffic)
- Version Windows app

### Phase 4 : Enterprise (6+ mois)

**Objectifs**
- Grands groupes et flottes
- Monétisation

**Features**
- ✅ Support 50+ véhicules
- ✅ Gestion de flotte professionnelle
- ✅ Rapports personnalisés
- ✅ API publique
- ✅ White-label
- ✅ SLA et support premium

**Technique**
- Kubernetes
- Multi-région
- CDN cartes
- Analytics avancés

---

## Annexes

### Glossaire

| Terme | Définition |
|-------|------------|
| **Convoy** | Groupe persistant de personnes avec code permanent |
| **Trip** | Instance de voyage avec destination obligatoire |
| **Leader** | Créateur/administrateur d'un convoy |
| **Participant** | Membre d'un convoy |
| **Waypoint** | Point d'intérêt (destination, arrêt, alerte) |
| **Magic Link** | Lien unique d'authentification sans mot de passe |
| **2FA** | Two-Factor Authentication (téléphone) |
| **OTP** | One-Time Password (code à usage unique) |

### Ressources

- [.NET MAUI Docs](https://learn.microsoft.com/dotnet/maui/)
- [ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
- [SignalR](https://learn.microsoft.com/aspnet/core/signalr/)
- [Mapsui](https://mapsui.com/)
- [OpenStreetMap](https://www.openstreetmap.org/)

---

**Version** : 2.0
**Date** : 2025-11-16
**Statut** : Spécifications validées
