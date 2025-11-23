# SyncTrip - Documentation Architecture

**Version** : 1.0
**Date** : 23 Novembre 2025

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
- **Entities** : User, Convoy, Trip, Vehicle, StopProposal, Vote, etc.
- **Interfaces** : IUserRepository, IConvoyRepository, IAuthService, etc.
- **Value Objects** : JoinCode, Coordinates, etc.
- **Enums** : LicenseType, TripStatus, VoteChoice, etc.

**Règles** :
- Aucune dépendance externe (pas de EF Core, pas de HttpClient)
- Logique métier dans les entités (ex: validation âge > 14 ans)
- Interfaces seulement, implémentations dans Infrastructure

---

### SyncTrip.Shared (.NET 10)

**Responsabilité** : DTOs partagés entre l'API et l'application Mobile.

**Contenu** :
- **DTOs** : Tous les DTOs (Request, Response)
- **Enums partagés** : Réutilisation des enums de Core
- **Contrats SignalR** : Interfaces des Hubs

**Avantage** :
- Évite la duplication de code entre API et Mobile
- Contrat clair entre backend et frontend

---

### SyncTrip.Application (.NET 10)

**Responsabilité** : Orchestration des use cases métier.

**Contenu** :
- **Commands** : Actions qui modifient l'état (CreateConvoy, SendMagicLink)
- **Queries** : Récupération de données (GetUserProfile, GetConvoyDetails)
- **Handlers** : Implémentation des Commands/Queries (MediatR)
- **Validators** : Validation des inputs (FluentValidation)
- **Mappings** : AutoMapper pour Entity ↔ DTO

**Pattern utilisé** : CQRS (Command Query Responsibility Segregation)

**Dépendances** :
- MediatR
- FluentValidation
- AutoMapper
- SyncTrip.Core (interfaces seulement)

---

### SyncTrip.Infrastructure (.NET 10)

**Responsabilité** : Implémentation technique des interfaces du Core.

**Contenu** :
- **Persistence** : DbContext (EF Core), Repositories, Migrations
- **Services** : AuthService, EmailService, GeocodingService, etc.
- **External APIs** : Intégrations tierces
- **Caching** : Redis (StackExchange.Redis)

**Dépendances** :
- Entity Framework Core (PostgreSQL)
- StackExchange.Redis
- MailKit (pour emails)
- Npgsql

---

### SyncTrip.API (ASP.NET Core 10)

**Responsabilité** : Exposition des endpoints REST et SignalR.

**Contenu** :
- **Controllers** : Endpoints REST
- **Hubs** : SignalR (ConvoyHub, TripHub)
- **Middleware** : Exception handling, JWT validation
- **Configuration** : DI, CORS, Authentication

**Endpoints principaux** :
- `/auth/*` : Authentification
- `/users/*` : Gestion profil
- `/vehicles/*` : Garage
- `/convoys/*` : Convois
- `/trips/*` : Voyages
- `/voting/*` : Système de vote
- `/hub/convoy` : SignalR Convoy
- `/hub/trip` : SignalR Trip

**Authentification** : JWT Bearer Token

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
- **Scoped** : DbContext, Repositories (API)
- **Singleton** : Redis, EmailService, SignalR connections
- **Transient** : Validators, Mappers

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

### ConvoyHub
- Gestion du chat
- Notifications membres (join/leave)

### TripHub
- Mise à jour position GPS
- Propositions d'arrêt
- Votes temps réel

**Authentification** : JWT Bearer dans query string
```
wss://api.synctrip.com/hub/trip?access_token=<JWT>
```

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
1. **Âge minimum** : > 14 ans (validation partout)
2. **Véhicule obligatoire** : Pour rejoindre un convoi
3. **Vote implicite** : Le proposant vote OUI automatiquement
4. **Règle du silence** : Timeout → Acceptation par défaut
5. **Foreground GPS** : Pas de tracking silencieux

### Points d'Attention
- Gestion propre des déconnexions SignalR
- Validation côté client ET serveur
- Messages d'erreur clairs et en français
- Logs structurés (Serilog)

---

**Dernière mise à jour** : 23 Novembre 2025
