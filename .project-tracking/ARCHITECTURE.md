# SyncTrip - Documentation Architecture

**Version** : 1.2
**Date** : 12 Février 2026

---

## Vue d'ensemble

SyncTrip utilise une architecture **Clean Architecture** avec une approche **Domain-Driven Design (DDD)**.
L'application est découpée en 6 projets suivant le principe de séparation des responsabilités.

---

## Structure de la Solution

```
SyncTrip/
├── src/
│   ├── SyncTrip.Core/              # Domain Layer (Entités, Interfaces)
│   ├── SyncTrip.Shared/            # DTOs partagés (API ↔ Mobile)
│   ├── SyncTrip.Application/       # Use Cases (MediatR + Validation)
│   ├── SyncTrip.Infrastructure/    # Implémentations (EF Core, Services)
│   ├── SyncTrip.API/               # API REST + SignalR Hubs
│   └── SyncTrip.Mobile/            # Application MAUI (iOS, Android, Windows)
├── tests/
│   ├── SyncTrip.Core.Tests/
│   ├── SyncTrip.Application.Tests/
│   ├── SyncTrip.Infrastructure.Tests/
│   └── SyncTrip.Mobile.Tests/
├── .project-tracking/              # Fichiers de suivi du projet
└── DOCUMENTATION.md                # Spécifications fonctionnelles
```

---

## Principes Architecturaux

### 1. Clean Architecture (Onion)

```
┌─────────────────────────────────────┐
│         SyncTrip.Mobile             │ ← Presentation Layer
│         SyncTrip.API                │
├─────────────────────────────────────┤
│      SyncTrip.Application           │ ← Application Layer (Use Cases)
├─────────────────────────────────────┤
│      SyncTrip.Infrastructure        │ ← Infrastructure Layer
├─────────────────────────────────────┤
│         SyncTrip.Core               │ ← Domain Layer (Centre)
│         SyncTrip.Shared             │
└─────────────────────────────────────┘
```

**Règles de dépendance** :
- ✅ Infrastructure → Application → Core
- ✅ API → Application → Core
- ✅ Mobile → Application → Core
- ❌ Core ne dépend de RIEN (sauf .NET BCL)
- ❌ Application ne dépend pas d'Infrastructure

---

## Description des Projets

### SyncTrip.Core (.NET 10)

**Responsabilité** : Contient la logique métier pure et les règles du domaine.

**Contenu** :
- **Entities** : User, MagicLinkToken, Vehicle, Brand, UserLicense, Convoy, ConvoyMember, Trip, TripWaypoint, StopProposal, Vote
- **Interfaces** : IUserRepository, IMagicLinkTokenRepository, IVehicleRepository, IBrandRepository, IConvoyRepository, ITripRepository, IStopProposalRepository, IAuthService, IEmailService
- **Enums** : LicenseType, VehicleType, ConvoyRole, TripStatus, RouteProfile, WaypointType, StopType, ProposalStatus

**Règles** :
- Aucune dépendance externe (pas de EF Core, pas de HttpClient)
- Logique métier dans les entités (ex: validation âge > 14 ans)
- Interfaces seulement, implémentations dans Infrastructure

---

### SyncTrip.Shared (.NET 10)

**Responsabilité** : DTOs partagés entre l'API et l'application Mobile.

**Contenu** :
- **DTOs** : Tous les DTOs Request/Response organisés par domaine (Auth, Users, Vehicles, Brands, Convoys, Trips, Voting)
- **Enums** : Enums en `int` (Shared ne référence PAS Core — l'Application fait le cast)

**Avantage** :
- Évite la duplication de code entre API et Mobile
- Contrat clair entre backend et frontend

**Règle importante** :
- Shared ne dépend PAS de Core — les enums sont représentés en `int` dans les DTOs
- L'Application est responsable du cast `int` → enum Core

---

### SyncTrip.Application (.NET 10)

**Responsabilité** : Orchestration des use cases métier.

**Contenu** :
- **Commands** : Actions qui modifient l'état (CreateConvoy, SendMagicLink, ProposeStop, CastVote)
- **Queries** : Récupération de données (GetUserProfile, GetConvoyDetails, GetActiveProposal)
- **Handlers** : Implémentation des Commands/Queries (MediatR)
- **Validators** : Validation des inputs (FluentValidation)
- **Services (interfaces)** : Abstractions pour services cross-layer (ITripNotificationService)

**Pattern utilisé** : CQRS (Command Query Responsibility Segregation)

**Dépendances** :
- MediatR
- FluentValidation
- SyncTrip.Core (interfaces seulement)
- SyncTrip.Shared (DTOs)

**Note** : Le mapping Entity → DTO est fait manuellement dans les handlers (pas d'AutoMapper).

---

### SyncTrip.Infrastructure (.NET 10)

**Responsabilité** : Implémentation technique des interfaces du Core.

**Contenu** :
- **Persistence** : ApplicationDbContext (EF Core), Repositories, Migrations, Configurations, Seed Data
- **Services** : AuthService, EmailService/DevelopmentEmailService, ProposalResolutionService (BackgroundService)
- **Repositories** : UserRepository, MagicLinkTokenRepository, VehicleRepository, BrandRepository, ConvoyRepository, TripRepository, StopProposalRepository

**Dépendances** :
- Entity Framework Core (PostgreSQL via Npgsql)
- StackExchange.Redis
- System.IdentityModel.Tokens.Jwt

---

### SyncTrip.API (ASP.NET Core 10)

**Responsabilité** : Exposition des endpoints REST et SignalR.

**Contenu** :
- **Controllers** : AuthController, UsersController, VehiclesController, BrandsController, ConvoysController, TripsController, VotingController
- **Hubs** : TripHub (positions GPS, votes temps réel)
- **Services** : TripNotificationService (implémente ITripNotificationService via IHubContext<TripHub>)
- **Middleware** : Global exception handling, JWT validation, Rate Limiting
- **Configuration** : DI, CORS, Authentication, SignalR, Scalar (OpenAPI)

**Endpoints principaux** :
- `POST/GET /api/auth/*` : Authentification Magic Link
- `GET/PUT /api/users/me` : Profil utilisateur
- `CRUD /api/vehicles/*` : Garage véhicules
- `GET /api/brands` : Marques véhicules
- `CRUD /api/convoys/*` : Gestion convois
- `CRUD /api/convoys/{id}/trips/*` : Voyages GPS
- `CRUD /api/convoys/{id}/trips/{id}/proposals/*` : Système de vote
- `/hubs/trip` : SignalR Hub (positions, votes)

**Authentification** : JWT Bearer Token (query string pour SignalR)

---

### SyncTrip.Mobile (.NET MAUI 10)

**Responsabilité** : Application mobile multiplateforme.

**Plateformes supportées** :
- ✅ Android (API 21+)
- ✅ iOS (14+)
- ✅ Windows 10/11
- ✅ MacCatalyst

**Architecture** : MVVM (Model-View-ViewModel)

**Structure** :
```
SyncTrip.Mobile/
├── Features/              # Organisé par feature (vertical slices)
│   ├── Authentication/
│   │   ├── Views/
│   │   └── ViewModels/
│   ├── Profile/
│   ├── Garage/
│   ├── Convoy/
│   ├── Trip/
│   └── Chat/
├── Core/
│   ├── Services/          # Services métier (API, SignalR, Location)
│   ├── Helpers/
│   └── Converters/
├── Models/                # ViewModels locaux
└── Resources/
```

**Navigation** : Shell Navigation (AppShell.xaml)

**État** : CommunityToolkit.Mvvm (ObservableObject, RelayCommand)

**Dépendances clés** :
- Microsoft.AspNetCore.SignalR.Client
- Mapsui (cartographie)
- CommunityToolkit.Mvvm

---

## Patterns & Pratiques

### CQRS (Command Query Responsibility Segregation)

**Commands** : Modifient l'état, ne retournent rien (ou un ID)
```csharp
public record CreateConvoyCommand(Guid UserId, bool IsPrivate) : IRequest<Guid>;
```

**Queries** : Lisent les données, ne modifient rien
```csharp
public record GetConvoyDetailsQuery(string JoinCode) : IRequest<ConvoyDto>;
```

---

### Repository Pattern

Abstraction de l'accès aux données.

**Interface** (Core) :
```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}
```

**Implémentation** (Infrastructure) :
```csharp
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    // ...
}
```

---

### Dependency Injection

Tous les services sont enregistrés via DI.

**Lifetimes** :
- **Scoped** : DbContext, Repositories, AuthService, EmailService, TripNotificationService
- **Singleton** : ProposalResolutionService (BackgroundService, crée son propre scope via IServiceScopeFactory)
- **Transient** : Validators (enregistrés via AddValidatorsFromAssembly)

---

## Gestion des Données

### Base de Données : PostgreSQL

**Connexion** :
- Développement : localhost:5432
- Production : Configuration via appsettings.json

**Migrations** :
```bash
dotnet ef migrations add MigrationName --project SyncTrip.Infrastructure --startup-project SyncTrip.API
dotnet ef database update --project SyncTrip.Infrastructure --startup-project SyncTrip.API
```

---

### Cache : Redis

**Utilisation** :
- Stockage des tokens Magic Link (TTL 10 minutes)
- Cache des données fréquentes (Brands)
- Session SignalR (si scaling multi-serveur)

---

## Communication Temps Réel : SignalR

### TripHub (`/hubs/trip`)

**Méthodes client → serveur** :
- `JoinTrip(Guid tripId)` : Rejoindre le groupe d'un voyage
- `LeaveTrip(Guid tripId)` : Quitter le groupe d'un voyage
- `SendLocationUpdate(Guid tripId, double lat, double lon)` : Envoyer position GPS
- `SendRouteUpdate(Guid tripId, string geoJson)` : Envoyer mise à jour route

**Events serveur → client** (via IHubContext, pas de méthode hub) :
- `ReceiveLocationUpdate` : Position GPS d'un membre
- `ReceiveRouteUpdate` : Mise à jour route d'un membre
- `StopProposed(StopProposalDto)` : Nouvelle proposition d'arrêt
- `VoteUpdate({ ProposalId, YesCount, NoCount })` : Vote enregistré
- `ProposalResolved(StopProposalDto)` : Proposition résolue

**Groupes** : `trip-{tripId}` — tous les membres connectés au voyage

**Architecture notifications vote** :
- `ITripNotificationService` (interface dans Application)
- `TripNotificationService` (implémentation dans API, utilise `IHubContext<TripHub>`)
- Évite les dépendances circulaires Application ↔ API

**Authentification** : JWT Bearer dans query string
```
wss://api.synctrip.com/hubs/trip?access_token=<JWT>
```

### ConvoyHub (Feature 6 — à faire)
- Gestion du chat
- Notifications membres (join/leave)

---

## Sécurité

### Authentification
- **Passwordless** : Magic Link via email
- **JWT** : Token signé (HS256 ou RS256)
- **Refresh Tokens** : Non (pour simplifier v1)

### Autorisation
- **Claims** : UserId dans JWT
- **Rôles** : Leader vs Membre (vérification dans handlers)

### Données Sensibles
- **Pas de mots de passe** stockés
- **Emails** : Index unique, non exposés publiquement
- **Tokens** : Hachés dans Redis

---

## GPS & Privacy

### Règles de Confidentialité
1. GPS actif **uniquement si app au premier plan**
2. Service foreground avec notification (Android/iOS)
3. Arrêt automatique du tracking si app en background
4. Avatar "gris" si utilisateur déconnecté/inactif

### Fréquence de mise à jour
- **En mouvement** : Toutes les 5 secondes
- **À l'arrêt** : Toutes les 30 secondes
- **Seuil de mouvement** : 10 mètres

---

## Tests

### Types de Tests

**Tests Unitaires** :
- Entités (Core)
- Handlers (Application)
- Services (Infrastructure)
- ViewModels (Mobile)

**Tests d'Intégration** :
- API endpoints
- SignalR hubs
- Repositories (avec DB en mémoire)

**Outils** :
- xUnit
- FluentAssertions
- Moq
- Testcontainers (PostgreSQL pour tests d'intégration)

---

## Conventions de Code

### Nommage
- **Entities** : PascalCase (User, Convoy)
- **Interfaces** : IPascalCase (IUserRepository)
- **DTOs** : PascalCase + suffixe (UserDto, CreateConvoyRequest)
- **Variables privées** : _camelCase
- **Propriétés** : PascalCase
- **Méthodes** : PascalCase + Async si asynchrone

### Organisation des Fichiers
- **1 classe = 1 fichier**
- Nom du fichier = Nom de la classe

### Async/Await
- Toujours utiliser `async/await` pour I/O
- Passer `CancellationToken` partout

---

## Références Techniques

**Documentation .NET** :
- [Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures#clean-architecture)
- [MAUI](https://learn.microsoft.com/en-us/dotnet/maui/)
- [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/)

**Librairies** :
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Mapsui](https://mapsui.com/)

---

## Notes Importantes

### Règles Métier Critiques
1. **Âge minimum** : > 14 ans (validation dans entité User)
2. **Véhicule obligatoire** : Pour rejoindre un convoi
3. **Vote implicite** : Le proposant vote OUI automatiquement (auto-vote dans ProposeStopCommandHandler)
4. **Règle du silence** : Si majorité absolue de NON → Rejeté, sinon → Accepté (silence = consentement)
5. **Un seul vote actif** : Une seule proposition Pending par voyage à la fois
6. **Résolution anticipée** : Si tous les membres ont voté, résolution immédiate sans attendre le timer
7. **Timer 30s** : ProposalResolutionService poll toutes les 5s, résout les propositions expirées
8. **Waypoint automatique** : Sur acceptation, un TripWaypoint de type Stopover est créé automatiquement
9. **Foreground GPS** : Pas de tracking silencieux
10. **Positions éphémères** : GPS relayé via SignalR, PAS stocké en DB

### Points d'Attention
- Gestion propre des déconnexions SignalR
- Validation côté client ET serveur (FluentValidation + domain validation)
- Messages d'erreur clairs et en français
- Logs structurés avec ILogger<T>

---

**Dernière mise à jour** : 12 Février 2026
