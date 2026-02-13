# SyncTrip - Documentation Architecture

**Version** : 1.4
**Date** : 13 Février 2026

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
│   ├── SyncTrip.Shared/            # DTOs partagés (API ↔ App)
│   ├── SyncTrip.Application/       # Use Cases (MediatR + Validation)
│   ├── SyncTrip.Infrastructure/    # Implémentations (EF Core, Services)
│   ├── SyncTrip.API/               # API REST + SignalR Hubs
│   ├── SyncTrip.App/               # UI partagée AvaloniaUI (Views, VMs, Services)
│   ├── SyncTrip.App.Desktop/       # Head Desktop (Win/Mac/Linux)
│   ├── SyncTrip.App.Android/       # Head Android (stub)
│   ├── SyncTrip.App.iOS/           # Head iOS (stub)
│   └── SyncTrip.App.Browser/       # Head WASM (stub)
├── tests/
│   ├── SyncTrip.Core.Tests/
│   └── SyncTrip.Application.Tests/
├── .project-tracking/              # Fichiers de suivi du projet
└── DOCUMENTATION.md                # Spécifications fonctionnelles
```

---

## Principes Architecturaux

### 1. Clean Architecture (Onion)

```
┌─────────────────────────────────────┐
│         SyncTrip.App             │ ← Presentation Layer
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
- **Entities** : User, MagicLinkToken, Vehicle, Brand, UserLicense, Convoy, ConvoyMember, Trip, TripWaypoint, StopProposal, Vote, Message
- **Interfaces** : IUserRepository, IMagicLinkTokenRepository, IVehicleRepository, IBrandRepository, IConvoyRepository, ITripRepository, IStopProposalRepository, IMessageRepository, IAuthService, IEmailService
- **Enums** : LicenseType, VehicleType, ConvoyRole, TripStatus, RouteProfile, WaypointType, StopType, ProposalStatus

**Règles** :
- Aucune dépendance externe (pas de EF Core, pas de HttpClient)
- Logique métier dans les entités (ex: validation âge > 14 ans)
- Interfaces seulement, implémentations dans Infrastructure

---

### SyncTrip.Shared (.NET 10)

**Responsabilité** : DTOs partagés entre l'API et l'application Mobile.

**Contenu** :
- **DTOs** : Tous les DTOs Request/Response organisés par domaine (Auth, Users, Vehicles, Brands, Convoys, Trips, Voting, Chat)
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
- **Commands** : Actions qui modifient l'état (CreateConvoy, SendMagicLink, ProposeStop, CastVote, SendMessage)
- **Queries** : Récupération de données (GetUserProfile, GetConvoyDetails, GetActiveProposal, GetConvoyMessages)
- **Handlers** : Implémentation des Commands/Queries (MediatR)
- **Validators** : Validation des inputs (FluentValidation)
- **Services (interfaces)** : Abstractions pour services cross-layer (ITripNotificationService, IConvoyNotificationService)

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
- **Repositories** : UserRepository, MagicLinkTokenRepository, VehicleRepository, BrandRepository, ConvoyRepository, TripRepository, StopProposalRepository, MessageRepository

**Dépendances** :
- Entity Framework Core (PostgreSQL via Npgsql)
- StackExchange.Redis
- System.IdentityModel.Tokens.Jwt

---

### SyncTrip.API (ASP.NET Core 10)

**Responsabilité** : Exposition des endpoints REST et SignalR.

**Contenu** :
- **Controllers** : AuthController, UsersController, VehiclesController, BrandsController, ConvoysController, TripsController, VotingController, MessagesController
- **Hubs** : TripHub (positions GPS, votes temps réel), ConvoyHub (chat temps réel)
- **Services** : TripNotificationService (implémente ITripNotificationService via IHubContext<TripHub>), ConvoyNotificationService (implémente IConvoyNotificationService via IHubContext<ConvoyHub>)
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
- `POST/GET /api/convoys/{id}/messages` : Chat convoi
- `/hubs/trip` : SignalR Hub (positions, votes)
- `/hubs/convoy` : SignalR Hub (chat)

**Authentification** : JWT Bearer Token (query string pour SignalR)

---

### SyncTrip.App (AvaloniaUI)

**Responsabilité** : UI partagée multiplateforme.

**Plateformes supportées** (via platform heads) :
- ✅ Windows 10/11 (SyncTrip.App.Desktop — actif)
- ✅ macOS (SyncTrip.App.Desktop — actif)
- ✅ Linux (SyncTrip.App.Desktop — actif)
- 📋 iOS (SyncTrip.App.iOS — stub)
- 📋 Android (SyncTrip.App.Android — stub)
- 📋 WebAssembly (SyncTrip.App.Browser — stub)

**Architecture** : MVVM (Model-View-ViewModel) + ViewLocator convention

**Structure** :
```
SyncTrip.App/                          # net10.0, UI partagée
├── App.axaml / App.axaml.cs           # DI complet, ViewLocator, FluentTheme, converters
├── MainWindow.axaml                   # ContentControl bindé au NavigationService.CurrentViewModel
├── MainView.axaml                     # TabControl (Profil, Garage, Convois)
├── MainViewModel.cs                   # Contient les 3 VMs d'onglets
├── Features/                          # Organisé par feature (vertical slices)
│   ├── Authentication/
│   │   ├── Views/                     (MagicLinkView, RegistrationView)
│   │   └── ViewModels/                (MagicLinkViewModel, RegistrationViewModel)
│   ├── Profile/
│   │   ├── Views/                     (ProfileView)
│   │   └── ViewModels/                (ProfileViewModel + HasLicenseB/A/C/D)
│   ├── Garage/
│   │   ├── Views/                     (GarageView, AddVehicleView)
│   │   └── ViewModels/                (GarageViewModel, AddVehicleViewModel)
│   ├── Convoy/
│   │   ├── Views/                     (ConvoyLobbyView, CreateConvoyView, JoinConvoyView, ConvoyDetailView)
│   │   └── ViewModels/                (ConvoyLobbyViewModel, CreateConvoyViewModel, JoinConvoyViewModel, ConvoyDetailViewModel)
│   ├── Trip/
│   │   ├── Views/                     (CockpitView — carte Mapsui.Avalonia)
│   │   └── ViewModels/                (CockpitViewModel — GPS + SignalR + DispatcherTimer)
│   ├── Voting/
│   │   ├── Views/                     (VotingView — proposition, vote OUI/NON, countdown)
│   │   └── ViewModels/                (VotingViewModel — SignalR realtime + DispatcherTimer)
│   └── Chat/
│       ├── Views/                     (ChatView — messages temps réel, envoi, historique)
│       └── ViewModels/                (ChatViewModel — ConvoySignalR + pagination curseur)
├── Core/
│   ├── Platform/                      # Abstractions plateforme
│   │   ├── INavigationService         (NavigateToAsync, GoBackAsync)
│   │   ├── IDialogService             (ConfirmAsync, AlertAsync)
│   │   ├── ISecureStorageService      (GetAsync, SetAsync, Remove)
│   │   └── ILocationService           (GetCurrentLocationAsync → LocationResult?)
│   ├── Services/                      # 11 services métier (interfaces + implémentations)
│   │   ├── IApiService / ApiService
│   │   ├── IAuthenticationService / AuthenticationService  (injecte ISecureStorageService)
│   │   ├── IUserService / IVehicleService / IBrandService
│   │   ├── IConvoyService / ConvoyService
│   │   ├── ITripService / TripService
│   │   ├── ISignalRService / SignalRService        (TripHub: GPS + votes)
│   │   ├── IVotingService / VotingService           (REST: propositions, votes)
│   │   ├── IChatService / ChatService               (REST: messages, historique)
│   │   └── IConvoySignalRService / ConvoySignalRService  (ConvoyHub: chat realtime)
│   ├── Http/                          (AuthorizationMessageHandler — injecte ISecureStorageService)
│   └── Converters/                    (9 convertisseurs Avalonia.Data.Converters.IValueConverter)
├── Navigation/
│   ├── ViewLocator.cs                 # Convention : XxxViewModel → XxxView (assembly reflection)
│   ├── NavigationService.cs           # Stack-based, routes registry, Initialize() reflection
│   ├── DialogService.cs               # Fenêtres de dialogue Avalonia
│   ├── DesktopSecureStorageService.cs # JSON file dans %APPDATA%/SyncTrip/
│   └── DesktopLocationService.cs      # Stub (retourne null sur desktop)
└── Themes/
    └── Colors.axaml                   # Palette (Primary #512BD4, Success, Error, Warning, Gray)

SyncTrip.App.Desktop/                  # WinExe, Avalonia.Desktop 11.3.1
└── Program.cs                         # AppBuilder.Configure<App>.UsePlatformDetect.StartWithClassicDesktopLifetime
```

**Navigation** : INavigationService custom + ViewLocator (pas ReactiveUI)
- Stack-based : push/pop de ViewModels
- Routes enregistrées dans `App.axaml.cs` : login, registration, main, addvehicle, createconvoy, joinconvoy, convoydetail, cockpit, voting, chat
- `Initialize(params)` via reflection remplace `[QueryProperty]` de MAUI
- MainWindow : `<ContentControl Content="{Binding CurrentViewModel}" />` bindé au NavigationService

**État** : CommunityToolkit.Mvvm (ObservableObject, RelayCommand, [ObservableProperty])

**Services — Lifetimes DI** :
- **Singleton** : ISecureStorageService, ILocationService, IDialogService, INavigationService, AuthenticationService, UserService, VehicleService, BrandService, ConvoyService, TripService, SignalRService, VotingService, ChatService, ConvoySignalRService
- **Transient** : ViewModels (MagicLinkViewModel, RegistrationViewModel, VotingViewModel, ChatViewModel, etc.), AuthorizationMessageHandler

**Dépendances clés** :
- Avalonia 11.3.1 + Avalonia.Themes.Fluent (framework UI cross-platform)
- Mapsui.Avalonia 5.0.0 (carte OpenStreetMap)
- CommunityToolkit.Mvvm 8.4.0 (MVVM source generators)
- Microsoft.AspNetCore.SignalR.Client 10.0.0
- Microsoft.Extensions.DependencyInjection 10.0.0
- Microsoft.Extensions.Http 10.0.0

**CockpitView (Mapsui.Avalonia)** :
- `OpenStreetMap.CreateTileLayer()` pour les tuiles
- `WritableLayer` pour user + membres (PointFeature + SymbolStyle)
- `SphericalMercator.FromLonLat()` → tuple `(x, y)` → `new MPoint(x, y)`
- `map.Navigator.CenterOnAndZoomTo()` pour centrage initial (Paris par défaut)
- Timer géolocalisation via `DispatcherTimer` (5s) → `ILocationService` + `SendLocationAsync()`
- `PositionsUpdated` event → code-behind met à jour les features sur la WritableLayer

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
- **Scoped** : DbContext, Repositories, AuthService, EmailService, TripNotificationService, ConvoyNotificationService
- **Singleton** : ProposalResolutionService (BackgroundService, crée son propre scope via IServiceScopeFactory)
- **Transient** : Validators (enregistrés via AddValidatorsFromAssembly)

---

## Gestion des Données

### Base de Données : PostgreSQL

**Connexion** :
- Développement (Docker) : `localhost:5433` (DB: synctrip, User: postgres)
- Conteneur API → DB : `Host=postgres;Database=synctrip` (réseau Docker interne)
- Production : Configuration via variables d'environnement

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

### ConvoyHub (`/hubs/convoy`)

**Méthodes client → serveur** :
- `JoinConvoy(Guid convoyId)` : Rejoindre le groupe d'un convoi
- `LeaveConvoy(Guid convoyId)` : Quitter le groupe d'un convoi

**Events serveur → client** (via IHubContext, pas de méthode hub) :
- `ReceiveMessage(MessageDto)` : Nouveau message de chat

**Groupes** : `convoy-{convoyId}` — tous les membres connectés au convoi

**Architecture notifications chat** :
- `IConvoyNotificationService` (interface dans Application)
- `ConvoyNotificationService` (implémentation dans API, utilise `IHubContext<ConvoyHub>`)
- Même pattern que ITripNotificationService

**Authentification** : JWT Bearer dans query string
```
wss://api.synctrip.com/hubs/convoy?access_token=<JWT>
```

**Note** : Les messages sont persistés en DB (contrairement aux positions GPS qui sont éphémères)

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
- [AvaloniaUI](https://docs.avaloniaui.net/)
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
11. **Messages persistés** : Chat stocké en DB, historique consultable avec pagination curseur (before + pageSize)
12. **Messages max 500 chars** : Validation dans l'entité Message et FluentValidation

### Points d'Attention
- Gestion propre des déconnexions SignalR
- Validation côté client ET serveur (FluentValidation + domain validation)
- Messages d'erreur clairs et en français
- Logs structurés avec ILogger<T>

---

**Dernière mise à jour** : 13 Février 2026
